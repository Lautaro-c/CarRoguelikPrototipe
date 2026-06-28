using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum Mode
    {
        Pursue,
        Wander,
        Patrol,
        Attack,
        AfterAttack,
        Flee,
        Dead,
    }

    [Header("Referencias")]
    [SerializeField] private Transform player;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private EnemyAnimator enemyAnimator;

    [Header("Movimiento")]
    [SerializeField] private float speed;
    [SerializeField] private float slowRadious = 5f;
    [SerializeField] private float maxPredictionTime = 10f;
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

    private DecisionTree decisionTree;
    private DecisionNode tree;
    private EnemyContext context;
    private LineOfSight los;
    private EnemyAttack enemyAttack;
    private Rigidbody enemyRb;

    private float timeSinceLastAttack;
    [SerializeField] private Mode mode;
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
        decisionTree = GetComponent<DecisionTree>();
        enemyAttack = GetComponent<EnemyAttack>();
        mudSpeedReduction = 1f;
    }

    private void Start()
    {
        tree = decisionTree.CreateTree();

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
            tree.Evaluate(this, context);
        }
        else
        {
            timeSinceLastAttack += Time.deltaTime;
        }

        Vector3 dir = Vector3.zero;
        float movementSpeed = 0f;

        switch (mode)
        {
            case Mode.Pursue:
                dir = SteeringBehaviour.Pursue(transform, player, playerRb, maxPredictionTime, slowRadious);
                movementSpeed = speed;

                if (enemyAnimator != null)
                {
                    enemyAnimator.PlayRunningAnamiation();
                }

                break;
            case Mode.Patrol:
                dir = GetPatrolDirection();
                movementSpeed = speed * patrolSpeedMultiplier;

                if (enemyAnimator != null)
                {
                    enemyAnimator.PlayWalkingAnamiation();
                }

                break;

            case Mode.Attack:
                dir = SteeringBehaviour.Seek(transform, player.position);

                if (enemyAttack != null)
                {
                    transform.LookAt(player); 
                    movementSpeed = enemyAttack.Attack(speed);
                }

                if (enemyAnimator != null)
                {
                    enemyAnimator.PlayAttackAnamiation();
                }

                if (!canCrash)
                {
                    mode = Mode.AfterAttack;
                    timeSinceLastAttack = 0f;
                }

                break;

            case Mode.AfterAttack:
                movementSpeed = 0f;
                break;

            case Mode.Flee:
                dir = SteeringBehaviour.Flee(transform, player.position);
                movementSpeed = speed * 2f;

                if (enemyAnimator != null)
                {
                    enemyAnimator.PlayRunningAnamiation();
                }

                break;

            case Mode.Dead:
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

        List<Node> nodePath = ThetaStar.Run(
            start,
            node => node == goal,
            node => node.neightbourds,
            GetCost,
            node => Vector3.Distance(node.transform.position, goal.transform.position),
            HasNodesLineOfSight,
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

    private bool HasNodesLineOfSight(Node node1, Node node2)
    {
        if (node1 == null || node2 == null)
        {
            return false;
        }
        return NodesCanBeSeen(node1.transform.position, node2.transform.position, nodeSearchRadius);
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

    public void SetMode(Mode newMode)
    {
        if (mode == newMode)
        {
            return;
        }

        mode = newMode;

        if (mode == Mode.Patrol || mode == Mode.Wander)
        {
            BuildPathToCurrentPatrolNode();
        }
    }

    public void OnDeath()
    {
        mode = Mode.Dead;
        isDead = true;
        if(enemyRb != null)
        {
            enemyRb.velocity = Vector3.zero;
        }

        if (enemyAnimator != null)
        {
            enemyAnimator.PlayDeathAnamiation();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (canCrash)
            {
                timeSinceLastAttack = 0f;
                HealthManager.Instance.ReceiveDamage(damage);
                mode = Mode.AfterAttack;
            }
            else
            {
                OnDeath();
            }
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
}