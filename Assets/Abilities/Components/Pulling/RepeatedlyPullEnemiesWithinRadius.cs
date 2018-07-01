using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RepeatedlyPullEnemiesWithinRadius : PercentagePullComponent
{

    Alignment alignment;
    float index = 0;

    public float radius = 0f;

    public float interval = 0f;

    // Use this for initialization
    void Start()
    {
        alignment = GetComponent<AlignmentManager>().alignment;
    }

    // Update is called once per frame
    void Update()
    {
        // only apply damage every "interval" seconds
        index += Time.deltaTime;
        if (index > interval)
        {
            // find enemies
            foreach (BaseHealth health in BaseHealth.all)
            {
                // check that it's an enemy and it's not dying
                if (!alignment.isSameOrFriend(health.GetComponent<AlignmentManager>().alignment) &&
                    (!health.GetComponent<StateController>() || (health.GetComponent<StateController>().currentState != health.GetComponent<StateController>().dying)))
                {
                    // check whether the enemy's centre is within the radius
                    if (Vector3.Distance(transform.position, health.transform.position) <= radius)
                    {
                        // pull the enemy
                        if (interval != 0)
                        {
                            Pull(health.gameObject, index / interval);
                        }
                        else
                        {
                            Pull(health.gameObject, Time.deltaTime);
                        }
                    }
                    // if the centre is not within the radius, check if the edge of the damage sphere is within a collider of the enemy
                    else
                    {
                        // find the point on the radius closest to the enemy
                        Vector3 point = transform.position + Vector3.Normalize(health.transform.position - transform.position) * radius;
                        // check if it's in any of their colliders
                        bool hit = false;
                        foreach (Collider collider in health.GetComponents<Collider>())
                        {
                            if (!hit && collider.bounds.Contains(point))
                            {
                                // pull the enemy
                                if (interval != 0)
                                {
                                    Pull(health.gameObject, index / interval);
                                }
                                else
                                {
                                    Pull(health.gameObject, Time.deltaTime);
                                }
                            }
                        }
                    }
                }
            }
            index = 0;
        }
    }


}
