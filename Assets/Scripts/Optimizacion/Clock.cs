using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour
{
    public static Clock Instance { get; private set; }
    [Header("Time")]
    [SerializeField] private float maxTime = 100f;
    private float currentTime;

    [Header("UI")]
    [Tooltip("Image con Type = Filled y Fill Method = Radial")]
    [SerializeField] private Image timeFillImage;

    [Header("Death")]
    [SerializeField] private GameObject deathImage;

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
        if (currentTime <= 0f) currentTime = maxTime;
        UpdateUIInstant();
        if (deathImage != null) deathImage.SetActive(false);
        IsDead = false;
    }

    private void Update()
    {
        CountTime();
    }

    public void CountTime()
    {
        if (IsDead) return;
        currentTime -= Time.deltaTime;
        currentTime = Mathf.Clamp(currentTime, 0f, maxTime);
        UpdateUIInstant();

        if (currentTime <= 0f && !IsDead)
        {
            OnDeath();
        }
    }

    private void UpdateUIInstant()
    {
        if (timeFillImage != null)
            timeFillImage.fillAmount = currentTime / maxTime;
    }

    private void OnDeath()
    {
        IsDead = true;
        if (deathImage != null) deathImage.SetActive(true);
    }
}