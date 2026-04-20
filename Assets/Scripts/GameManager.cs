using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Rigidbody playerRb;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public Transform GetPlayerTransform()
    {
        return playerTransform;
    }

    public Rigidbody GetPlayerRB()
    {
        return playerRb;
    }

}
