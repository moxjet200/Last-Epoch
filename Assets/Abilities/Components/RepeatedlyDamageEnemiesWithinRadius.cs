using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AlignmentManager))]
public class RepeatedlyDamageEnemiesWithinRadius : DamageStatsHolder {

    Alignment alignment;
    float index = 0;

    public float radius = 0f;

    public float damageInterval = 0f;

    public float damageIncreasePerInterval = 0f;

	// Use this for initialization
	void Start () {
        alignment = GetComponent<AlignmentManager>().alignment;
	}
	
	// Update is called once per frame
	void Update () {
        // only apply damage every damageInterval seconds
        index += Time.deltaTime;
        if (index > damageInterval)
        {
            index -= damageInterval;
            // find enemies
            foreach (BaseHealth health in BaseHealth.all)
            {
                // check that it's an enemy and it's not dying
                if (!alignment.isSameOrFriend(health.GetComponent<AlignmentManager>().alignment) && 
                    (!health.GetComponent<StateController>() || (health.GetComponent<StateController>().currentState != health.GetComponent<StateController>().dying)))
                {
                    // check whether the enemy's centre if within the radius
                    if (Vector3.Distance(transform.position, health.transform.position) <= radius)
                    {
                        // damage the enemy
                        applyDamage(health.gameObject);
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
                                // damage the enemy
                                applyDamage(health.gameObject);
                            }
                        }
                    }
                }
            }
            // increase damage if necessary
            if (damageIncreasePerInterval != 0)
            {
                damageStats.increaseDamage(damageIncreasePerInterval);
            }
        }
	}
}
