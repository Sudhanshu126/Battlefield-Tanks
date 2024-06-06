using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Properties and Settings")]
    [SerializeField] private float projectileSpeed;

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(gameObject);
    }

    public void StartProjectile(float range)
    {
        float lifeTime = range / projectileSpeed;
        Destroy(gameObject, lifeTime);
        rb.velocity = transform.up *  projectileSpeed;
    }
}
