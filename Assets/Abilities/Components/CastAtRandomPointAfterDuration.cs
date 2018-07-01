using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AbilityObjectConstructor))]
public class CastAtRandomPointAfterDuration : MonoBehaviour {

    public Ability ability = null;
    public float duration = 1f;
    public bool limitCasts = false;
    public int remainingCasts = 0;
    public float radius = 0f;
    public bool castsAtStart = false;
    [HideInInspector]
    public float age = 0f;
    public bool castAllAtStart = false;
    public Vector3 offset = Vector3.zero;

    public bool restrictToNavmeshAccessiblePoints = false;

    // Use this for initialization
    void Start()
    {
        if (castAllAtStart && limitCasts)
        {
            int i = 0;
            // hard cap at 100 just in case
            while (remainingCasts > 0 && i <100)
            {
                i++;
                castAbility();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!limitCasts || (remainingCasts > 0))
        {
            if (castsAtStart)
            {
                castAbility();
                castsAtStart = false;
            }

            if (age >= duration)
            {
                castAbility();
                // reset the age
                age -= duration;
            }
        }
        // increment the age
        age += Time.deltaTime;
    }

    public void castAbility()
    {
        // cast the ability
        Vector3 pointInCircle = transform.position + getRandomPositionInCircle();
        Vector3 pointOnGround = new Vector3(pointInCircle.x, getY(pointInCircle), pointInCircle.z);
        // make sure the point is navmesh accessible
        if (restrictToNavmeshAccessiblePoints)
        {
            int i = 0;
            bool success = false;
            while (i < 50 && !success)
            {
                i++;
                if (NavMeshChecker.accessibleFrom(transform.position, pointOnGround))
                {
                    success = true;
                }
                if (!success)
                {
                    pointInCircle = transform.position + getRandomPositionInCircle();
                    pointOnGround = new Vector3(pointInCircle.x, getY(pointInCircle), pointInCircle.z);
                }
            }
        }
        GetComponent<AbilityObjectConstructor>().constructAbilityObject(ability, pointOnGround + offset, pointOnGround + offset);
        // reduce the number of available casts
        if (limitCasts) { remainingCasts--; }
    }

    // gets a random point inside a circle of the radius defined in the radius variable
    public Vector3 getRandomPositionInCircle()
    {
        // Old code that is biased towards the centre of the circle
        /*
        float angle = Random.Range(0f, 2 * Mathf.PI);
        float rand = Random.Range(0f, radius * 2);
        float distance = (rand > 1) ? 2 - rand : rand;
        return new Vector3(distance * Mathf.Cos(angle), 0, distance * Mathf.Sin(angle));
        */

        float angle = Random.Range(0f, 2 * Mathf.PI);
        float distance = Mathf.Sqrt(Random.Range(0f, 1f)) * radius;
        return new Vector3(distance * Mathf.Cos(angle), 0, distance * Mathf.Sin(angle));
    }

    // gets the Y coordinate of the ground at a position
    public float getY(Vector3 position)
    {
        // the maximum height above the ground to look for a surface
        float height = 10f;
        // raycast down from this height
        RaycastHit hit;
        LayerMask mask = 1 << LayerMask.NameToLayer("Default");
        if (Physics.Raycast(new Vector3(position.x, position.y + height, position.z), Vector3.down, out hit, Mathf.Infinity, mask))
        {
            return hit.point.y + 1.2f;
        }
        // if nothing was hit, just return the y value of the point
        return position.y;
    }
}
