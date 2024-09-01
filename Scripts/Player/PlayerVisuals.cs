using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVisuals : NetworkBehaviour
{
    public static PlayerVisuals LocalInstance { get; private set; }

    public NetworkVariable<FixedString32Bytes> playerNickname = new NetworkVariable<FixedString32Bytes>();

    [Header("References")]
    [SerializeField] private Image healthBar, reloadProgressBar;
    [SerializeField] private GameObject reloadIcon;
    [SerializeField] private Animator animator;
    [SerializeField] private TMP_Text playerNameText;

    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            playerNickname.Value = HostSingleton.Instance.GameManager.NetworkServer.GetSharedUserData(OwnerClientId).nickName;
        }

        if(!IsOwner) { return; }

        LocalInstance = this;
        playerNameText.text = LocalDataHandler.Instance.PlayerData.nickName;
    }

    private void Start()
    {
        OnNicknameChanged(string.Empty, playerNickname.Value);
        playerNickname.OnValueChanged += OnNicknameChanged;
    }

    private void OnNicknameChanged(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
        playerNameText.text = newValue.ToString();
    }

    public override void OnDestroy()
    {
        playerNickname.OnValueChanged -= OnNicknameChanged;
    }

    [ServerRpc]
    public void UpdateHealthUIServerRpc(float value)
    {
        UpdateHealthUIClientRpc(value);
    }

    [ClientRpc]
    private void UpdateHealthUIClientRpc(float value)
    {
        healthBar.fillAmount = value;
        if (value > 0.5f)
        {
            healthBar.color = Color.green;
        }
        else if (value > 0.25f)
        {
            healthBar.color = Color.yellow;
        }
        else
        {
            healthBar.color = Color.red;
        }
    }

    public void PlayFireAnimation()
    {
        animator.Play("PrimaryFire");
    }

    public void SetReloadProgressBarValue(float value)
    {
        reloadProgressBar.fillAmount = value;
        if(value == 1)
        {
            ShowReloadBar(false);
        }
        else
        {
            ShowReloadBar(true);
        }
    }

    private void ShowReloadBar(bool show)
    {
        reloadIcon.SetActive(show);
    }
}
