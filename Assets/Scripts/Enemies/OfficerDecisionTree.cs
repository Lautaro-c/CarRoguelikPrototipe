using UnityEngine;

public class OfficerDecisionTree : DecisionTree
{
    public override DecisionNode CreateTree()
    {
        DecisionNode canAttack;
        DecisionNode mustFlee;

        ActionNode patrol = new ActionNode(enemy => enemy.SetMode(EnemyController.Mode.Patrol));
        ActionNode pursue = new ActionNode(enemy => enemy.SetMode(EnemyController.Mode.Pursue));
        ActionNode attack = new ActionNode(enemy => enemy.SetMode(EnemyController.Mode.Attack));
        ActionNode evade = new ActionNode(enemy => enemy.SetMode(EnemyController.Mode.Flee));

        mustFlee = new QuestionNode(context => context.los.CanFlee(context.self, context.player), evade, attack);
        canAttack = new QuestionNode(context => context.los.CanAttack(context.self, context.player), mustFlee, pursue);
        return new QuestionNode(context => context.los.CanBeSeen(context.self, context.player), canAttack, patrol);
    }
}