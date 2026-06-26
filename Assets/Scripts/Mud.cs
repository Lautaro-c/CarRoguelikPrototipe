using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mud : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        CarController carController = other.GetComponent<CarController>();
        if (carController != null)
        {
            carController.OnMud(true);
        }
        EnemyController enemyController = other.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.OnMud(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CarController carController = other.GetComponent<CarController>();
        if (carController != null)
        {
            carController.OnMud(false);
        }
        EnemyController enemyController = other.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.OnMud(false);
        }
    }
}
