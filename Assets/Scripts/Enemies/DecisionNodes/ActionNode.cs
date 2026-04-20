using System;

public class ActionNode : DecisionNode
{
    private Action<EnemyController> action;

    public ActionNode(Action<EnemyController> action)
    {
        this.action = action;
    }

    public override void Evaluate(EnemyController enemy, EnemyContext context)
    {
        action(enemy);
    }
}
