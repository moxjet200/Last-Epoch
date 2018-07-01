using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PercentagePullComponent : PullComponent
{

    public float distanceMultiplier = 0.95f;

    public bool limitDistanceToMax = false;

    public float maxDistance = 3f;

    public override void Pull(GameObject enemy, float scale)
    {
        if (canPull(enemy))
        {
            // estabilish the displacement
            Vector3 displacement = - (enemy.transform.position - transform.position) * (1 - distanceMultiplier) * scale;
            if (limitDistanceToMax && displacement.magnitude > maxDistance * scale) { displacement = Vector3.Normalize(displacement) * maxDistance * scale; }
            // pull the enemy by multiplying the enemy's distance to this object by the distance multiplier
            enemy.transform.position = enemy.transform.position + displacement;
            // if the enemy has a push detector then update it
            if (enemy.GetComponent<PlayerPushDetector>()) { enemy.GetComponent<PlayerPushDetector>().pushed(); }
        }
    }
}
