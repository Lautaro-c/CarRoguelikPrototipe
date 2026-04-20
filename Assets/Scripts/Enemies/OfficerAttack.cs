using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficerAttack : EnemyAttack
{
    [SerializeField] private Transform spawnPos;
    [SerializeField] private ProjectilePool projectilePool;
    [SerializeField] private float damage;
    [SerializeField] private float projectileForce;
    private Transform player;

    private void Start()
    {
        player = GameManager.Instance.GetPlayerTransform();
    }
    public override float Attack(float speed)
    {
        Vector3 direction = (player.position - spawnPos.position).normalized;
        GameObject proj = projectilePool.GetProjectile(spawnPos.position, transform.rotation);
        proj.GetComponent<Projectile>().Init(projectilePool, damage);
        proj.GetComponent<Rigidbody>().AddForce(direction * projectileForce, ForceMode.Impulse);
        return 0f;
    }
}
