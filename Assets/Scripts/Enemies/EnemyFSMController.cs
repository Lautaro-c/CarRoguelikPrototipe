using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyFSMController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform player;
    [SerializeField] private Rigidbody playerRb;


    [Header("Movimiento")]
    [SerializeField] private float speed;
    [SerializeField] private int rotationSpeed = 50;

    [Header("Ataque")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float damage;
    [SerializeField] private bool canCrash;

    [Header("Patrol con ThetaStar")]
    [SerializeField] private List<Node> patrolNodes = new List<Node>();
    [SerializeField] private float patrolSpeedMultiplier = 0.5f;
    [SerializeField] private float patrolPointReachedDistance = 0.35f;
    [SerializeField] private float nodeSearchRadius;
    [SerializeField] private LayerMask nodeMask;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private int thetaWatchDog = 1000;
    [SerializeField] private float explotionRadius = 10;
    [SerializeField] private GameObject explotion;

    private FSMClasses fsm;
    private EnemyContext context;
    private LineOfSight los;
    private EnemyAttack enemyAttack;
    private Rigidbody enemyRb;

    private float timeSinceLastAttack;
    [SerializeField] private string mode;
    private bool isDead;

    private int patrolTargetIndex;
    private List<Vector3> currentPath = new List<Vector3>();
    private int currentPathIndex;
    private float mudSpeedReduction;

    private void Awake()
    {
        isDead = false;

        enemyRb = GetComponent<Rigidbody>();
        los = GetComponent<LineOfSight>();
        enemyAttack = GetComponent<EnemyAttack>();
        mudSpeedReduction = 1f;
    }

    private void Start()
    {
        fsm = this.GetComponent<FSMClasses>();
        player = GameManager.Instance.GetPlayerTransform();
        playerRb = GameManager.Instance.GetPlayerRB();

        context = new EnemyContext
        {
            self = transform,
            player = player,
            los = los
        };

        timeSinceLastAttack = attackCooldown;
        nodeSearchRadius = los.Dis;
    }

    private void FixedUpdate()
    {
        if (timeSinceLastAttack >= attackCooldown && !isDead)
        {
            fsm.UpdateState(context.los, this.transform, player);
            mode = fsm.GetClassName();
        }
        else
        {
            timeSinceLastAttack += Time.deltaTime;
        }

        Vector3 dir = Vector3.zero;
        float movementSpeed = 0f;

        switch (mode)
        {
            case "Patrol":
                dir = GetPatrolDirection();
                movementSpeed = speed * patrolSpeedMultiplier;

                break;
            case "Flee":
                dir = SteeringBehaviour.Flee(transform, player.position);
                movementSpeed = speed * 2f;

                break;

            case "Explode":
                ExplodeEnemy();
                movementSpeed = 0f;
                break;
            case "Dead":
                movementSpeed = 0f;
                break;
        }

        Move(dir, movementSpeed * mudSpeedReduction);
    }

    private Vector3 GetPatrolDirection()
    {
        if (patrolNodes == null || patrolNodes.Count == 0)
        {
            return Vector3.zero;
        }

        if (currentPath == null || currentPath.Count == 0 || currentPathIndex >= currentPath.Count)
        {
            BuildPathToCurrentPatrolNode();
        }

        if (currentPath == null || currentPath.Count == 0)
        {
            return Vector3.zero;
        }

        Vector3 point = currentPath[currentPathIndex];
        point.y = transform.position.y;

        Vector3 dir = point - transform.position;

        if (dir.magnitude <= patrolPointReachedDistance)
        {
            currentPathIndex++;

            if (currentPathIndex >= currentPath.Count)
            {
                patrolTargetIndex++;

                if (patrolTargetIndex >= patrolNodes.Count)
                {
                    patrolTargetIndex = 0;
                }

                BuildPathToCurrentPatrolNode();

                if (currentPath == null || currentPath.Count == 0)
                {
                    return Vector3.zero;
                }
            }

            point = currentPath[currentPathIndex];
            point.y = transform.position.y;
            dir = point - transform.position;
        }

        dir.y = 0f;
        return dir.normalized;
    }

    private void BuildPathToCurrentPatrolNode()
    {
        currentPath.Clear();
        currentPathIndex = 0;

        Node goal = patrolNodes[patrolTargetIndex];

        if (goal == null)
        {
            return;
        }

        Node start = GetClosestVisibleNode(transform.position);

        if (start == null)
        {
            return;
        }

        List<Node> nodePath = AStar.Run(
            start,
            node => node == goal,
            node => node.neightbourds,
            GetCost,
            node => Vector3.Distance(node.transform.position, goal.transform.position),
            thetaWatchDog
        );

        for (int i = 0; i < nodePath.Count; i++)
        {
            currentPath.Add(nodePath[i].transform.position);
        }
    }

    private Node GetClosestVisibleNode(Vector3 position)
    {
        Node closest = null;
        float closestDistance = Mathf.Infinity;

        float searchRadius = nodeSearchRadius;
        float maxRadius = 200;
        float step = nodeSearchRadius;

        while (closest == null && searchRadius <= maxRadius)
        {
            Collider[] colliders = Physics.OverlapSphere(position, searchRadius, nodeMask);
            for (int i = 0; i < colliders.Length; i++)
            {
                Node node = colliders[i].GetComponent<Node>();
                if (node == null)
                {
                    continue;
                }

                float distance = Vector3.Distance(position, node.transform.position);

                if (distance >= closestDistance)
                {
                    continue;
                }
                if (!NodesCanBeSeen(position, node.transform.position, searchRadius))
                {
                    continue;
                }

                closestDistance = distance;
                closest = node;
            }

            // si no encontró nada, expandimos el radio
            if (closest == null)
            {
                searchRadius += step;
            }
        }

        return closest;
    }


    private float GetCost(Node node1, Node node2)
    {
        float distanceCost = Vector3.Distance(node1.transform.position, node2.transform.position);
        float trapCost = node2 != null ? node2.TrapCost : 0f;

        return distanceCost + trapCost;
    }

    private bool NodesCanBeSeen(Vector3 from, Vector3 to, float lookingDistance)
    {
        Vector3 direction = to - from;
        float distance = direction.magnitude;

        if (distance <= 0.01f)
        {
            return true;
        }
        if (distance > lookingDistance)
        {
            return false;
        }

        return !Physics.Raycast(from, direction.normalized, distance, obstacleMask);
    }

    private void Move(Vector3 dir, float movementSpeed)
    {
        enemyRb.velocity = dir * movementSpeed;

        if (dir != Vector3.zero)
        {
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, angle, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void OnDeath()
    {
        mode = "Dead";
        isDead = true;
        if (enemyRb != null)
        {
            enemyRb.velocity = Vector3.zero;
        }
        explotion.SetActive(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnDeath();
        }
    }

    public void OnMud(bool onMud)
    {
        if (onMud)
        {
            mudSpeedReduction = 0.1f;
        }
        else
        {
            mudSpeedReduction = 1f;
        }
    }

    private void ExplodeEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explotionRadius);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].CompareTag("Player"))
            {
                HealthManager.Instance.ReceiveDamage(damage);
                OnDeath();
            }
        }
    }
}
