using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifeTime = 2f;
    private ProjectilePool pool;
    private float damage;

    public void Init(ProjectilePool poolRef, float damage)
    {
        pool = poolRef;
        this.damage = damage;
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    private void ReturnToPool()
    {
        if (pool != null)
        {
            pool.ReturnProjectile(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            HealthManager.Instance.ReceiveDamage(damage);
            ReturnToPool();
        }
    }
}
