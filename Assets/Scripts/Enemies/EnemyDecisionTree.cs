using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class EnemyDecisionTree : DecisionTree
{
    public override DecisionNode CreateTree()
    {
        DecisionNode rootNode;
        DecisionNode canAttack;
        ActionNode wander = new ActionNode(enemy => enemy.SetMode(EnemyController.Mode.Wander));
        ActionNode pursue = new ActionNode(enemy => enemy.SetMode(EnemyController.Mode.Pursue));
        ActionNode attack = new ActionNode(enemy => enemy.SetMode(EnemyController.Mode.Attack));
        canAttack = new QuestionNode(context => context.los.CanAttack(context.self, context.player), attack, pursue);
        return rootNode = new QuestionNode(context => context.los.CanBeSeen(context.self, context.player), canAttack, wander);
    }
}
