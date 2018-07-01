using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StretchToLocationCreatedFrom : MonoBehaviour {

    public float defaultLength = 1f;
    float baseLocalScaleZ = 1f;
    CreationReferences references = null;
    Vector3 locationCreatedFrom;
    bool locationFound = false;
    public Vector3 offset;

	// Use this for initialization
	void Start () {
        baseLocalScaleZ = transform.localScale.z;
        references = GetComponentInParent<CreationReferences>();
        if (references)
        {
            locationCreatedFrom = references.locationCreatedFrom;
            locationFound = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (locationFound)
        {
            Vector3 aimPos = locationCreatedFrom + offset;
            transform.LookAt(aimPos);
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, baseLocalScaleZ * (Vector3.Magnitude(transform.position - aimPos))/defaultLength);
        }
	}
}
