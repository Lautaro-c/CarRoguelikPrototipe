using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private GameObject VictoryImage;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI pointsText;
    private float timeSpend;
    private int score;
    private float health;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            timeSpend = Time.timeSinceLevelLoad;
            timeText.text = timeSpend.ToString("F2") + "s";
            health = HealthManager.Instance.GetHealth();
            healthText.text = health.ToString() + "/" + HealthManager.Instance.GetMaxHealth().ToString();
            score = (Mathf.Max(0, (int)(1000 - timeSpend * 10)) + 1) * ((int)(health * 10) + 1);
            pointsText.text = score.ToString();
            VictoryImage.SetActive(true);
        }
    }
}

