using UnityEngine;

public class ClientProjectile : ProjectileBase
{
    [SerializeField] private GameObject destroyEffect;

    private void OnDestroy()
    {
        Instantiate(destroyEffect, transform.position, Quaternion.identity);
    }
}
