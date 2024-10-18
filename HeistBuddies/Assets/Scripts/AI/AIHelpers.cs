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
}
