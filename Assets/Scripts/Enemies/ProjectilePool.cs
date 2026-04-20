using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int poolSize = 10;

    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject proj = Instantiate(projectilePrefab);
            proj.SetActive(false);
            pool.Enqueue(proj);
        }
    }

    public GameObject GetProjectile(Vector3 position, Quaternion rotation)
    {
        if (pool.Count > 0)
        {
            GameObject proj = pool.Dequeue();
            proj.transform.position = position;
            proj.transform.rotation = rotation * Quaternion.Euler(45f, 0f, 90f);
            Rigidbody rb = proj.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            proj.SetActive(true);
            return proj;
        }
        else
        {
            GameObject proj = Instantiate(projectilePrefab, position, rotation);
            return proj;
        }
    }

    public void ReturnProjectile(GameObject proj)
    {
        proj.SetActive(false);
        pool.Enqueue(proj);
    }
}