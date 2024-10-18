using UnityEngine;
using UnityEngine.AI;

public static class AIHelpers
{
    public static Vector3 GetRandomSearchPosition(float searchRadius, Transform transform)
    {
        Vector3 randomDirection = Random.insideUnitSphere * searchRadius;
        randomDirection += transform.position;
        NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, searchRadius, 1);
        return hit.position;
    }

    public static bool CheckLineOfSight(Vector3 targetPosition, Transform transform, LayerMask layer)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, layer))
        {
            return true;
        }

        return false;
    }
}
