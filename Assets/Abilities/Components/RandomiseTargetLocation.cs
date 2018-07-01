using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LocationDetector))]
public class RandomiseTargetLocation : MonoBehaviour {

    public float radius = 0f;
    public float yOffset = 1.2f;


	// Use this for initialization
	void Start () {
        LocationDetector detector = GetComponent<LocationDetector>();
        Vector3 newTargetLocation = detector.targetLocation;
        newTargetLocation += getRandomPositionInCircle();
        newTargetLocation = new Vector3(newTargetLocation.x, getY(newTargetLocation), newTargetLocation.z);
        detector.targetLocation = newTargetLocation;
	}


    // gets a random point inside a circle of the radius defined in the radius variable
    public Vector3 getRandomPositionInCircle()
    {
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
            return hit.point.y + yOffset;
        }
        // if nothing was hit, just return the y value of the point
        return position.y;
    }

}
