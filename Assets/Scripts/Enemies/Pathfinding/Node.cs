using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> neightbourds;//<- Esto es lo unico que importa
    //Si utilizan este codigo con los raycast en el start/update/realtime son un punto menos por raycast.
    public bool hasTrap;
    [SerializeField] private float trapCost = 100f;
    [SerializeField] private GameObject mudModel;
    public float TrapCost => hasTrap ? trapCost : 0f;

    public void SetTrap(bool hasTrap)
    {
        this.hasTrap = hasTrap;
        if (hasTrap)
        {
            mudModel.SetActive(true);
        }
    }
}
