using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AlignmentManager))]
[RequireComponent(typeof(AbilityObjectConstructor))]
public class RepeatedlyCastAtNearestEnemyWithinRadius : MonoBehaviour {

    Alignment alignment;
    float index = 0;

    public Ability abilityToCast;

    public float radius = 0f;

    public float castInterval = 0f;

    // used for getting the nearest enemy in range
    List<GameObject> enemies = new List<GameObject>();
    List<float> distances = new List<float>();

	// Use this for initialization
	void Start () {
        alignment = GetComponent<AlignmentManager>().alignment;
	}
	
	// Update is called once per frame
	void Update () {
        // only apply damage every damageInterval seconds
        index += Time.deltaTime;
        if (index > castInterval)
        {
            index -= castInterval;
            GameObject nearestEnemy = getNearestEnemyInRange();
            // clear lists used
            enemies.Clear();
            distances.Clear();
            // cast at the enemy
            if (nearestEnemy) { GetComponent<AbilityObjectConstructor>().constructAbilityObject(abilityToCast, transform.position, nearestEnemy.transform.position); }
            
        }
	}

    GameObject getNearestEnemyInRange()
    {
        // clear any prexisting entries in the lists
        enemies.Clear();
        distances.Clear();
        // find enemies
        float distance = 0f;
        foreach (BaseHealth health in BaseHealth.all)
        {
            // check that it's an enemy and it's not dying
            if (!alignment.isSameOrFriend(health.GetComponent<AlignmentManager>().alignment) &&
                (!health.GetComponent<StateController>() || (health.GetComponent<StateController>().currentState != health.GetComponent<StateController>().dying)))
            {
                distance = Vector3.Distance(transform.position, health.transform.position);
                // check whether the enemy's centre if within the radius
                if (distance <= radius)
                {
                    // add the enemy to the list
                    enemies.Add(health.gameObject);
                    distances.Add(distance);
                }
                // if the centre is not within the radius, and no enemy in range has been found yet check if the edge of the damage sphere is within a collider of the enemy
                else if (enemies.Count == 0)
                {
                    // find the point on the radius closest to the enemy
                    Vector3 point = transform.position + Vector3.Normalize(health.transform.position - transform.position) * radius;
                    // check if it's in any of their colliders
                    bool hit = false;
                    foreach (Collider collider in health.GetComponents<Collider>())
                    {
                        if (!hit && collider.bounds.Contains(point))
                        {
                            // add the enemy to the list
                            enemies.Add(health.gameObject);
                            distances.Add(distance);
                        }
                    }
                }
            }
        }
        // get the nearest enemy
        if (enemies.Count == 0) { return null; }
        else
        {
            GameObject nearestEnemy = enemies[0];
            float nearestDistance = distances[0];
            for (int i = 0; i< enemies.Count && i<distances.Count; i++)
            {
                if (distances[i]< nearestDistance)
                {
                    nearestEnemy = enemies[i];
                    nearestDistance = distances[i];
                }
            }
            return nearestEnemy;
        }
    }
}
