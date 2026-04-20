using System;

public class WeightedRandomActionNode : DecisionNode
{
    private (float weight, Action<EnemyController> action)[] options;

    public WeightedRandomActionNode((float weight, Action<EnemyController> action)[] options)
    {
        this.options = options;
    }

    public override void Evaluate(EnemyController enemy, EnemyContext context)
    {
        float totalWeight = 0f;

        foreach (var option in options)
        {
            totalWeight += option.weight;
        }

        float randomValue = UnityEngine.Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        foreach (var option in options)
        {
            currentWeight += option.weight;

            if (randomValue <= currentWeight)
            {
                option.action(enemy);
                return;
            }
        }
    }
}
