using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasAssault : EnemyAttack
{
    public override float Attack(float speed)
    {
        return speed * 2;
    }
}
