using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public static class SteeringBehaviour
{
    public static Vector3 Seek(Transform self, Vector3 target)
    {
        Vector3 dir = target - self.position;
        dir.y = 0;
        return dir.normalized;
    }
    public static Vector3 Flee(Transform self, Vector3 target)
    {
        Vector3 dir = self.position - target;
        dir.y = 0;
        return dir.normalized;
    }

    public static Vector3 Arrive(Transform self, Vector3 target, float slowRadious)
    {
        Vector3 dir = target - self.position;
        dir.y = 0;
        float distance = dir.magnitude;
        if (distance < 0.001f)
        {
            return Vector3.zero;
        }
        float speedFactor = Mathf.Clamp01(distance / slowRadious);
        return dir.normalized * speedFactor;
    }

    public static Vector3 Pursue(Transform self, Transform target, Rigidbody targetRb, float maxPredictionTime, float slowRadious)
    {
        Vector3 futurePos = CalculateFuture(self, target, targetRb, maxPredictionTime, slowRadious);
        return Seek(self, futurePos);
    }

    public static Vector3 Evade(Transform self, Transform target, Rigidbody targetRb, float maxPredictionTime, float slowRadious)
    {
        Vector3 futurePos = CalculateFuture(self, target, targetRb, maxPredictionTime, slowRadious);
        return Flee(self, futurePos);
    }

    public static Vector3 Wander(Vector3 currentDirection, float maxAngleChange)
    {
        float randomAngle = UnityEngine.Random.Range(-maxAngleChange, maxAngleChange);
        Quaternion rotation = Quaternion.Euler(0f, randomAngle, 0f);
        Vector3 newDirection = rotation * currentDirection;
        newDirection.y = 0f;
        return newDirection.normalized;
    }

    public static Vector3 CalculateFuture(Transform self, Transform target, Rigidbody targetRb, float maxPredictionTime, float slowRadious)
    {
        Vector3 targetVelocity = Vector3.zero;
        targetVelocity = targetRb.velocity;
        Vector3 toTarget = target.position - self.position;
        toTarget.y = 0;
        float distance = toTarget.magnitude;
        float predictionTime = Mathf.Clamp(distance / slowRadious, 0f, maxPredictionTime);
        return target.position + targetRb.velocity * predictionTime;
    }
}
