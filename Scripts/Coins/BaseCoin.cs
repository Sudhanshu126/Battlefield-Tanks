using Unity.Netcode;
using UnityEngine;

public abstract class BaseCoin : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    protected int coinValue = 10;
    protected bool alreadyCollected;

    public abstract int Collect();

    public void SetCoinValue(int value)
    {
        coinValue = value;
    }

    protected void ShowSprite(bool show)
    {
        spriteRenderer.enabled = show;
    }
}
