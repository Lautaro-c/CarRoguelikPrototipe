using UnityEngine;
using static UnityEditor.PlayerSettings;

public class EnemyController : MonoBehaviour
{
    public enum Mode
    {
        Pursue,
        Wander,
        Attack,
        AfterAttack,
        Flee,
        Dead
    }
    [SerializeField] private Transform player;
    [SerializeField] private float speed;
    [SerializeField] private float slowRadious = 5f;
    [SerializeField] private float maxPredictionTime = 10;
    [SerializeField] private float maxAngleChange = 90f;
    [SerializeField] private int rotationSpeed = 50;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float damage;
    [SerializeField] private bool canCrash;
    [SerializeField] private EnemyAnimator enemyAnimator;
    private DecisionTree decisionTree;
    private float timeSinceLastAttack;
    private Mode mode;
    private Rigidbody enemyRb;
    [SerializeField] private float wanderChangeInterval = 1.5f;
    private DecisionNode tree;
    private EnemyContext context;
    private LineOfSight los;
    private Vector3 wanderDirection;
    private float wanderTimer;
    private EnemyAttack enemyAttack;
    private bool isDead;
    private void Awake()
    {
        isDead = false;
        enemyRb = GetComponent<Rigidbody>();
        wanderDirection = transform.forward;
        los = GetComponent<LineOfSight>();
        decisionTree = GetComponent<DecisionTree>();
        wanderTimer = 0f;
    }

    private void Start()
    {
        tree = decisionTree.CreateTree();
        player = GameManager.Instance.GetPlayerTransform();
        playerRb = GameManager.Instance.GetPlayerRB();
        enemyAttack = GetComponent<EnemyAttack>();
        context = new EnemyContext { self = transform, player = player, los = los };
        timeSinceLastAttack = attackCooldown;
    }

    private void FixedUpdate()
    {
        if (timeSinceLastAttack >= attackCooldown && !isDead)
        {
            tree.Evaluate(this, context);
        }else
        {
            timeSinceLastAttack += Time.deltaTime;
        }
        Vector3 dir = Vector3.zero;
        float movementSpeed = 0;
        switch (mode)
        {
            case Mode.Pursue:
                dir = SteeringBehaviour.Pursue(this.transform, player, playerRb, maxPredictionTime, slowRadious);
                movementSpeed = speed;
                if(enemyAnimator != null)
                {
                    enemyAnimator.PlayRunningAnamiation();
                }
                break;
            case Mode.Wander:
                wanderTimer -= Time.deltaTime;
                if (wanderTimer <= 0f)
                {
                    wanderDirection = SteeringBehaviour.Wander(wanderDirection, maxAngleChange);
                    wanderTimer = wanderChangeInterval;
                }
                dir = wanderDirection;
                movementSpeed = speed / 2;
                if (enemyAnimator != null)
                {
                    enemyAnimator.PlayWalkingAnamiation();
                }
                break;
            case Mode.Attack:
                dir = SteeringBehaviour.Seek(this.transform, player.position);
                movementSpeed = enemyAttack.Attack(speed);
                if (enemyAnimator != null)
                {
                    enemyAnimator.PlayAttackAnamiation();
                }
                if (!canCrash)
                {
                    this.mode = Mode.AfterAttack;
                    timeSinceLastAttack = 0;
                }
                break;
            case Mode.AfterAttack:
                break;
            case Mode.Flee:
                dir = SteeringBehaviour.Flee(this.transform, player.position);
                movementSpeed = speed * 2;
                if (enemyAnimator != null)
                {
                    enemyAnimator.PlayRunningAnamiation();
                }
                break;
            case Mode.Dead:
                movementSpeed = 0;
                break;
        }
        Move(dir, movementSpeed);
    }
    private void Move(Vector3 dir, float movementSpeed)
    {
        enemyRb.velocity = dir * movementSpeed;
        if (dir != Vector3.zero)
        {
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed);
        }
    }

    public void SetMode(Mode mode)
    {
        this.mode = mode;
    }

    private void OnDeath()
    {
        mode = Mode.Dead;
        isDead = true;
        enemyRb.velocity = Vector3.zero;
        if (enemyAnimator != null)
        {
            enemyAnimator.PlayDeathAnamiation();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if(canCrash)
            {
                timeSinceLastAttack = 0;
                HealthManager.Instance.ReceiveDamage(damage);
                this.mode = Mode.AfterAttack;
            }
            else
            {
                OnDeath();
            }
        }
    }
}