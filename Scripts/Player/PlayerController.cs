using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour, IDamagable
{
    //Public static members
    public static PlayerController localInstance { get; private set; }

    //Public variables
    public event Action<PlayerController> onDie;

    //Network variables
    [Header("Network Variables")]
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>();
    public NetworkVariable<int> coinsCollected = new NetworkVariable<int>();

    //Private visible variables
    [Header("References")]
    [SerializeField] private Rigidbody2D rigidbody2d;
    [SerializeField] private Transform trackTransform, turretTransform, projectileSpawnPoint;
    [SerializeField] private GameObject clientProjectilePrefab, serverProjectilePrefab, muzzleFlash;
    [SerializeField] private Collider2D playerCollider;

    [Header("Properties and Settings")]
    [SerializeField] private float turnRate;
    [SerializeField] private float minimumTurnThreshold, muzzleFlashDuration, singleAmmoReloadTime;

    [Header("Player Data")]
    [SerializeField] private int damage;
    [SerializeField] private int maxAmmoCount;
    [SerializeField] private float moveSpeed, fireRate, range;

    //Private properties
    [field: SerializeField] public int MaxHealth { get; private set; }

    //Private hidden variables
    private InputReader inputReader;
    private bool isTurning, isFireRateReset, isDead, isFiring;
    private float lastFireTimer, lastMuzzleFlashTimer, lastAmmoReloadTimer;
    private int currentAmmoCount;

    /* ---------- Inbuilt methods ---------- */

    //Called when object spawned on network
    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            currentHealth.Value = MaxHealth;
        }

        if (IsOwner)
        {
            inputReader = InputReader.Instance;
            localInstance = this;
            isFireRateReset = true;
            currentAmmoCount = maxAmmoCount;

            lastFireTimer = 1/fireRate;
            lastMuzzleFlashTimer = muzzleFlashDuration;
            lastAmmoReloadTimer = singleAmmoReloadTime;

            inputReader.PrimaryFirePerformedEvent += PrimaryFire;
            currentHealth.OnValueChanged += OnCurrentHealthChanged;
        }
    }

    //Called when object despawned on network
    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            inputReader.PrimaryFirePerformedEvent -= PrimaryFire;
            currentHealth.OnValueChanged -= OnCurrentHealthChanged;
        }
    }

    //Called once per frame
    private void Update()
    {
        if(!IsOwner) { return; }

        HandleRotation();
        isFiring = inputReader.isPrimaryFiring;
        if (isFiring)
        {
            isFiring = true;
            PrimaryFire();
        }

        //Muzzle flash timer
        if(lastMuzzleFlashTimer > 0f)
        {
            if (StartCountDown(ref lastMuzzleFlashTimer))
            {
                DisableMuzzleFlashServerRpc();
            }
        }

        //Fire rate timer
        if(lastFireTimer > 0f)
        {
            if(StartCountDown(ref lastFireTimer))
            {
                isFireRateReset = true;
            }
        }

        //Ammo reload time
        if (!isFiring && lastAmmoReloadTimer > 0 && currentAmmoCount < maxAmmoCount)
        {
            if(StartCountDown(ref lastAmmoReloadTimer))
            {
                ReloadOneAmmo();
            }
        }
    }

    //Called on regular intervals
    private void FixedUpdate()
    {
        if(!IsOwner) { return; }

        HandleMovement();
    }

    //Called after all other update methods
    private void LateUpdate()
    {
        if(!IsOwner) { return; }

        HandleAim();
    }

    //Called when collided with a trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Checks if collided with a coin
        if (collision.TryGetComponent(out BaseCoin coin))
        {
            int coinValue = coin.Collect();

            if (IsServer)
            {
                coinsCollected.Value += coinValue;
            }
        }
    }

    //Called when current health changes
    private void OnCurrentHealthChanged(int previousValue, int newValue)
    {
        float healthRatio = (float)currentHealth.Value / MaxHealth;
        PlayerVisuals.LocalInstance.UpdateHealthUIServerRpc(healthRatio);
    }

    /* ---------- Player controlling code ---------- */

    //Calculate the rotation related calculations
    private void HandleRotation()
    {
        Vector2 currentMovementInput = inputReader.GetMovementVectorNormalized();
        Debug.DrawRay(transform.position, currentMovementInput * 5f, Color.red);

        //Final rotation angle
        float targetAngle = Mathf.Atan2(currentMovementInput.y, currentMovementInput.x) * Mathf.Rad2Deg - 90f;
        //Delta rotation angle for per frame rotation
        float rotationAngle = Mathf.MoveTowardsAngle(trackTransform.eulerAngles.z, targetAngle, turnRate * Time.deltaTime);

        //Difference between current rotation and final rotation
        float turnThreshold = Mathf.Abs(trackTransform.eulerAngles.z - targetAngle);
        //Adjust turn threshold angle to be under 360 deg 
        if (turnThreshold >= 360f)
        {
            turnThreshold -= 360f;
        }

        if (currentMovementInput != Vector2.zero && turnThreshold > minimumTurnThreshold)
        {
            isTurning = true;
            //Finally rotate player
            RotatePlayer(rotationAngle);
        }
        else
        {
            isTurning = false;
        }
    }

    //Calculate movement related calculations
    private void HandleMovement()
    {
        Vector2 moveDirection = inputReader.GetMovementVectorNormalized();

        if (isTurning)
        {
            rigidbody2d.velocity = trackTransform.up * moveSpeed;
        }
        else
        {
            rigidbody2d.velocity = moveDirection * moveSpeed;
        }
    }

    //Calculate player aiming direction
    private void HandleAim()
    {
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(inputReader.GetMousePosition());
        Vector2 aimDirection = (mouseWorldPosition -  (Vector2)transform.position).normalized;
        Debug.DrawRay(turretTransform.position, aimDirection * 5f, Color.yellow);

        turretTransform.up = aimDirection;
    }

    //Rotate player
    private void RotatePlayer(float rotationAngle)
    {
        trackTransform.rotation = Quaternion.AngleAxis(rotationAngle, trackTransform.forward);
    }

    //Primary firing method
    private void PrimaryFire()
    {
        if (!isFireRateReset || currentAmmoCount <= 0 ) { return; }

        isFireRateReset = false;
        lastFireTimer = 1/fireRate;

        PlayerVisuals.LocalInstance.PlayFireAnimation();
        SpawnProjectileServerRpc();
        currentAmmoCount--;
        SetReloadBarProgress();
        SpawnDummyProjectile();
    }

    /* ---------- Utility and Behaviour methods ---------- */

    //Countdown timer
    private bool StartCountDown(ref float timer)
    {
        bool timerEnded = false;
        timer -= Time.deltaTime;
        if(timer < 0)
        {
            timer = 0;
            timerEnded = true;
        }

        return timerEnded;
    }

    //Reloads one ammo
    private void ReloadOneAmmo()
    {
        lastAmmoReloadTimer = singleAmmoReloadTime;
        currentAmmoCount++;

        SetReloadBarProgress();
    }

    private void SetReloadBarProgress()
    {
        float reloadBarValue = (float)currentAmmoCount / maxAmmoCount;
        PlayerVisuals.LocalInstance.SetReloadProgressBarValue(reloadBarValue);
    }

    //Spawn a dummy projectile on firing
    private void SpawnDummyProjectile()
    {
        Quaternion projectileRotation = turretTransform.rotation;
        GameObject dummyProjectile = Instantiate(clientProjectilePrefab, projectileSpawnPoint.position, projectileRotation);

        Physics2D.IgnoreCollision(playerCollider, dummyProjectile.GetComponent<Collider2D>());

        if (dummyProjectile.TryGetComponent(out ClientProjectile projectileScript))
        {
            projectileScript.StartProjectile(range);
        }

        muzzleFlash.SetActive(true);
        lastMuzzleFlashTimer = muzzleFlashDuration;
    }

    //Restore player health
    public void RestoreHealth(int healValue)
    {
        if (isDead) { return; }

        ModifyHealth(healValue);
    }

    //Take damage
    public void TakeDamage(int damageValue)
    {
        if (isDead) { return; }

        ModifyHealth(-damageValue);
    }

    //Kill player
    public void KillDamagable()
    {
        isDead = true;
        onDie?.Invoke(this);
    }

    //Modify changes to the health
    private void ModifyHealth(int value)
    {
        int newHealth = currentHealth.Value + value;
        currentHealth.Value = Mathf.Clamp(newHealth, 0, MaxHealth);

        if (currentHealth.Value == 0)
        {
            KillDamagable();
        }
    }

    /* ---------- Remote Procedure Calls (RPCs) ---------- */

    //Disable muzzle flash via server
    [ServerRpc]
    private void DisableMuzzleFlashServerRpc()
    {
        lastMuzzleFlashTimer = 0f;
        DisableMuzzleFlashClientRpc();
    }

    //Disable muzzle flash on clients
    [ClientRpc]
    private void DisableMuzzleFlashClientRpc()
    {
        muzzleFlash.SetActive(false);
    }

    //Spawn server projectile via server
    [ServerRpc]
    private void SpawnProjectileServerRpc()
    {
        Quaternion projectileRotation = turretTransform.rotation;
        GameObject serverProjectile = Instantiate(serverProjectilePrefab, projectileSpawnPoint.position, projectileRotation);

        Physics2D.IgnoreCollision(playerCollider, serverProjectile.GetComponent<Collider2D>());

        if (serverProjectile.TryGetComponent(out ServerProjectile projectileScript))
        {
            projectileScript.SetDamage(damage);
            projectileScript.StartProjectile(range);
        }

        SpawnDummyProjectileClientRpc();
    }

    //Spawn dummy projectile on clients
    [ClientRpc]
    private void SpawnDummyProjectileClientRpc()
    {
        if(IsOwner) { return; }

        SpawnDummyProjectile();
    }
}
