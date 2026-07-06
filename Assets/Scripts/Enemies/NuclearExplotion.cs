using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NuclearExplotion : MonoBehaviour
{
    [SerializeField] private GameObject father;

    private void Start()
    {
        StartCoroutine(SelfDestruct());
    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(father);
        Destroy(this.gameObject);
    }
}
