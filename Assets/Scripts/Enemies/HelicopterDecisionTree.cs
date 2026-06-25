using UnityEngine;

public class HelicopterDecisionTree : DecisionTree
{
    public override DecisionNode CreateTree()
    {
        DecisionNode canAttack;

        ActionNode patrol = new ActionNode(enemy => enemy.SetMode(EnemyController.Mode.Patrol));
        ActionNode pursue = new ActionNode(enemy => enemy.SetMode(EnemyController.Mode.Pursue));
        ActionNode attack = new ActionNode(enemy => enemy.SetMode(EnemyController.Mode.Attack));

        canAttack = new QuestionNode(context => context.los.CanAttack(context.self, context.player), attack, pursue);
        return new QuestionNode(context => context.los.CanBeSeen(context.self, context.player), canAttack, patrol);
    }
}