using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVisuals : NetworkBehaviour
{
    public static PlayerVisuals LocalInstance { get; private set; }

    [Header("References")]
    [SerializeField] private Image healthBar, reloadProgressBar;
    [SerializeField] private GameObject reloadIcon;
    [SerializeField] private Animator animator;

    public override void OnNetworkSpawn()
    {
        if(!IsOwner) { return; }

        LocalInstance = this;
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
