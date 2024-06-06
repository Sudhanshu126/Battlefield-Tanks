using System;
using UnityEngine;

public class RespawningCoin : BaseCoin
{
    public event Action<RespawningCoin> onCollected;
    public override int Collect()
    {
        if (IsClient)
        {
            ShowSprite(false);
            //return 0;
        }

        if (alreadyCollected) { return 0; }

        alreadyCollected = true;
        onCollected?.Invoke(this);
        return coinValue;
    }

    public void ResetCoin(Vector2 newPosition)
    {
        transform.position = newPosition;
        alreadyCollected = false;
        ShowSprite(true);
    }
}
