using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// stretches to a start point defined in the parent's Location Detector
public class StretchToCreator : MonoBehaviour {

    public float defaultLength = 1f;
    float baseLocalScaleZ = 1f;
    CreationReferences references = null;
    GameObject creator;
    public Vector3 offset;

    public bool stretchToCastPointInstead = false;

    SizeManager sm = null;

    // Use this for initialization
    void Start () {
        baseLocalScaleZ = transform.localScale.z;
        references = GetComponentInParent<CreationReferences>();
        if (references)
        {
            creator = references.creator; if (stretchToCastPointInstead)
            {
                sm = creator.GetComponent<SizeManager>();
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (creator)
        {
            if (stretchToCastPointInstead)
            {
                if (sm && sm.castPoints != null && sm.castPoints.Count > 0)
                {
                    Vector3 aimPos = sm.castPoints[0].transform.position + offset;
                    transform.LookAt(aimPos);
                    transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, baseLocalScaleZ * (Vector3.Magnitude(transform.position - aimPos)) / defaultLength);
                }
            }
            else
            {
                Vector3 aimPos = creator.transform.position + offset;
                transform.LookAt(aimPos);
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, baseLocalScaleZ * (Vector3.Magnitude(transform.position - aimPos)) / defaultLength);
            }
        }
	}
}
