using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DecisionNode
{
    public abstract void Evaluate(EnemyController enemy, EnemyContext context);
}
