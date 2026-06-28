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
    [SerializeField] private List<GameObject> Exits = new List<GameObject>();
    [SerializeField] private List<GameObject> ExitBlock = new List<GameObject>();
    [SerializeField] private BirdManager birdManager;
    [SerializeField] private CarController carController;
    [SerializeField] private List<EnemyController> enemyControllers = new List<EnemyController>();
    public CarController CarController => carController;
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
        List<Node> tempListMud = new List<Node>(nodes);
        List<GameObject> tempListExit = new List<GameObject>(Exits);
        List<GameObject> tempListExitBlocks = new List<GameObject>(ExitBlock);

        for (int i = 0; i < countToActivate; i++)
        {
            int randomIndex = Random.Range(0, tempListMud.Count);
            Node node = tempListMud[randomIndex];
            node.SetTrap(true);
            tempListMud.RemoveAt(randomIndex);
        }
        for (int i = 0; i < Exits.Count - 1; i++)
        {
            int randomIndex = Random.Range(0, tempListExit.Count);
            GameObject exit = tempListExit[randomIndex];
            GameObject exitBlock = tempListExitBlocks[randomIndex];
            exit.SetActive(false);
            exitBlock.SetActive(true);
            tempListExitBlocks.RemoveAt(randomIndex);
            tempListExit.RemoveAt(randomIndex);
        }
        birdManager.ReceiveExit(tempListExit[0].transform);
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

    public void GameFinished()
    {
        for (int i = 0; i < enemyControllers.Count; i++)
        {
            enemyControllers[i].OnDeath();
        }
        carController.CantMove();  
    }
}
