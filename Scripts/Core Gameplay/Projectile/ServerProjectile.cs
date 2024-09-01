using UnityEngine;

public class ServerProjectile : ProjectileBase
{
    private int damage;

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out IDamagable damagable))
        {
            damagable.TakeDamage(damage);
        }

        base.OnTriggerEnter2D(other);
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }
}
