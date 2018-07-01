using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// stretches to a start point defined in the parent's Location Detector
public class StretchToStart : MonoBehaviour {

    public float defaultLength = 1f;
    float baseLocalScaleZ = 1f;
    LocationDetector detector = null;
    public Vector3 startLocation;

	// Use this for initialization
	void Start () {
        baseLocalScaleZ = transform.localScale.z;
        detector = GetComponentInParent<LocationDetector>();
        if (detector)
        {
            startLocation = detector.startLocation;
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (detector)
        {
            transform.LookAt(startLocation);
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, baseLocalScaleZ * (Vector3.Magnitude(transform.position - startLocation))/defaultLength);
        }
	}
}
