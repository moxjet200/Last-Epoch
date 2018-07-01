using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationDetector : MonoBehaviour {

    [HideInInspector]
    public Vector3 startLocation;
    [HideInInspector]
    public bool reachedTargetLocation = false;


    // setting the target location sets the exactTargetLocation, getting it returns the exact target location + the offset
    [HideInInspector]
    public Vector3 targetLocation
    {
        get { return exactTargetLocation + targetLocationOffset; }
        set
        {
            exactTargetLocation = value;
        }
    }
    private Vector3 exactTargetLocation = new Vector3(0, 0, 0);
    public Vector3 targetLocationOffset;

    public delegate void ReachedTargetLocationAction();
    public event ReachedTargetLocationAction reachedTargetLocationEvent;

    // Use this for initialization
    void Awake () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // if something is listening for the reachedTargetLocationEvent then check for it
		if (!reachedTargetLocation && reachedTargetLocationEvent != null)
        {
            // if near the target location and further from the start location than the target location is
            if (Vector3.Distance(transform.position, targetLocation) < 10.5f && Vector3.Distance(transform.position, startLocation) >= Vector3.Distance(targetLocation, startLocation))
            {
                reachedTargetLocation = true;
                reachedTargetLocationEvent.Invoke();
            }
        }
	}

    public static void copy(LocationDetector to, LocationDetector from)
    {
        to.startLocation = from.startLocation;
        to.targetLocationOffset = from.targetLocationOffset;
        to.targetLocation = from.targetLocation - from.targetLocationOffset;
    }
}
