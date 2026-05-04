using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance { get; private set; }
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float health = -1f;

    [Header("UI")]
    [Tooltip("Image con Type = Filled y Fill Method = Horizontal")]
    [SerializeField] private Image healthFillImage;

    [Header("Death")]
    [SerializeField] private GameObject deathImage;

    //Optimizacion
    [Header("Heart")]
    [SerializeField] private Transform heart;
    [SerializeField] private Animator animator;

    private bool IsDead = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (health <= 0f) health = maxHealth;
        UpdateUIInstant();
        if (deathImage != null) deathImage.SetActive(false);
        IsDead = false;
    }

    public void ReceiveDamage(float damage)
    {
        if (IsDead) return;
        health -= Mathf.Max(0f, damage);
        health = Mathf.Clamp(health, 0f, maxHealth);
        animator.SetTrigger("Hit");
        UpdateUIInstant();

        if (health <= 0f && !IsDead)
        {
            OnDeath();
        }
    }

    private void UpdateUIInstant()
    {
        if (healthFillImage != null)
            //Hola Lauti. El profe de optimización dice que con hacer el calculo de "health/maxHealth" está bien. No hace falta el Clamp porque lo hace solo el fillAmount.
            healthFillImage.fillAmount = Mathf.Clamp01(health / maxHealth);
    }

    private void OnDeath()
    {
        IsDead = true;
        if (deathImage != null) deathImage.SetActive(true);
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }
}