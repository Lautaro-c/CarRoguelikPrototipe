using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private List<Node> nodes = new List<Node>();
    [SerializeField] private int desiredTraps = 10;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        int countToActivate = Mathf.Min(desiredTraps, nodes.Count);
        List<Node> tempList = new List<Node>(nodes);

        for (int i = 0; i < countToActivate; i++)
        {
            int randomIndex = Random.Range(0, tempList.Count);
            Node node = tempList[randomIndex];
            node.SetTrap(true);
            tempList.RemoveAt(randomIndex);
        }
    }

    public Transform GetPlayerTransform()
    {
        return playerTransform;
    }

    public Rigidbody GetPlayerRB()
    {
        return playerRb;
    }

    public void Retry()
    {
        SceneManager.LoadScene(1);
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
