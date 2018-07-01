using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AbilityMover))]
[RequireComponent(typeof(LocationDetector))]
public class StopAtTargetLocation : MonoBehaviour {

    AbilityMover mover = null;
    LocationDetector detector = null;
    Vector3 previousPosition = new Vector3(0, 0, 0);
    bool arrivedAtTargetLocation = false;

    // Use this for initialization
    void Start () {
        mover = GetComponent<AbilityMover>();
        detector = GetComponent<LocationDetector>();
        previousPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (!arrivedAtTargetLocation)
        {
            float distToPrevious = Vector3.Magnitude(transform.position - previousPosition);
            float distToDestination = Vector3.Magnitude(detector.targetLocation - transform.position);
            float distFromPreviousToDestination = Vector3.Magnitude(detector.targetLocation - previousPosition);

            if (distToDestination < distToPrevious && distFromPreviousToDestination < distToPrevious)
            {
                transform.position = detector.targetLocation;
                mover.speed = 0;
                arrivedAtTargetLocation = true;
            }

            previousPosition = transform.position;
        }
	}
}
