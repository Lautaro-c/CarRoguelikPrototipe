using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdManager : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private Bird agentPrefab;
    [SerializeField] private int agentCount = 20;
    [SerializeField] private Vector3 spawnExtents = new Vector3(10f, 5f, 10f);

    [Header("Movement")]
    [SerializeField] private float minSpeed = 4f;
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float maxForce = 10f;

    [Header("Neighbors")]
    [SerializeField] private float neighborRadius = 4f;
    [SerializeField] private float separationRadius = 2f;

    [Header("Weights")]
    [SerializeField] private float separationWeight = 2f;
    [SerializeField] private float alignmentWeight = 1f;
    [SerializeField] private float cohesionWeight = 1f;
    [SerializeField] private float targetWeight = 0.6f;
    [SerializeField] private float boundsWeight = 1.5f;

    [Header("Optional Target")]
    [SerializeField] private Transform globalTarget;

    private readonly List<Bird> agents = new List<Bird>();

    public List<Bird> Agents => agents;
    public float MinSpeed => minSpeed;
    public float MaxSpeed => maxSpeed;
    public float MaxForce => maxForce;
    public float NeighborRadius => neighborRadius;
    public float SeparationRadius => separationRadius;
    public float SeparationWeight => separationWeight;
    public float AlignmentWeight => alignmentWeight;
    public float CohesionWeight => cohesionWeight;
    public float TargetWeight => targetWeight;
    public float BoundsWeight => boundsWeight;
    public Transform GlobalTarget => globalTarget;
    public Vector3 BoundsCenter => transform.position;
    public Vector3 BoundsExtents => spawnExtents;

    private void Start()
    {
        SpawnAgents();
    }

    private void SpawnAgents()
    {
        for (int i = 0; i < agentCount; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-spawnExtents.x, spawnExtents.x),
                Random.Range(-spawnExtents.y, spawnExtents.y),
                Random.Range(-spawnExtents.z, spawnExtents.z)
            );

            Vector3 spawnPosition = transform.position + randomOffset;
            Quaternion spawnRotation = Random.rotation;

            Bird newAgent = Instantiate(agentPrefab, spawnPosition, spawnRotation, transform);
            newAgent.Initialize(this);
            agents.Add(newAgent);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, spawnExtents * 2f);
    }
}
