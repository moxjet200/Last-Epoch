using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlatPullComponent : PullComponent
{

    public float pullDistance = 0.05f;

    public override void Pull(GameObject enemy, float scale)
    {
        if (canPull(enemy))
        {
            // if the enemy is closer than pull distance then just set the position
            if (Vector3.Distance(enemy.transform.position, transform.position) <= pullDistance * scale)
            {
                enemy.transform.position = transform.position;
            }
            else
            {
                // pull the enemy by reducing the distance by a flat ammount
                enemy.transform.position = transform.position + Vector3.Normalize(enemy.transform.position - transform.position) * pullDistance * scale;
            }
            // if the enemy has a push detector then update it
            if (enemy.GetComponent<PlayerPushDetector>()) { enemy.GetComponent<PlayerPushDetector>().pushed(); }
        }
    }
}
