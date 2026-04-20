using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    [SerializeField] private int dis;
    [SerializeField] private int fleeDis;
    [SerializeField] private int attackDis;
    [SerializeField] private int angle;
    [SerializeField] private LayerMask layerMask;

    public bool CanBeSeen(Transform self, Transform target)
    {
        return isInRange(self, target) && isInRange(self, target) && hasLineOfSight(self, target);
    }

    public bool CanAttack(Transform self, Transform target)
    {
        return CanBeSeen(self, target) && isInAttackRange(self, target);
    }

    public bool CanFlee(Transform self, Transform target)
    {
        return Vector3.Distance(self.position, target.position) <= fleeDis;
    }

    public bool isInRange(Transform self, Transform target)
    {
        return Vector3.Distance(self.position, target.position) <= dis;
    }

    public bool isInAngle(Transform self, Transform target)
    {
        Vector3 dir = (target.position - self.position).normalized;
        return Vector3.Angle(self.forward, dir) <= angle / 2;
    }

    public bool hasLineOfSight(Transform self, Transform target)
    {
        Vector3 dir = target.position - self.position;
        return !Physics.Raycast(self.position, dir, dir.magnitude, layerMask);
    }

    public bool isInAttackRange(Transform self, Transform target)
    {
        return Vector3.Distance(self.position, target.position) <= attackDis;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dis);
        Vector3 leftBoundary = Quaternion.AngleAxis(-angle / 2f, Vector3.up) * transform.forward;
        Vector3 rightBoundary = Quaternion.AngleAxis(angle / 2f, Vector3.up) * transform.forward;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * dis);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * dis);
    }
}

