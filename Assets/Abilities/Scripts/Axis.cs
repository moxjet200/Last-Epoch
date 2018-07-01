using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axis : MonoBehaviour {

    public SkillTreeNode axisNode = null;
    public float pointsToReachEnd = 30f;
    public Transform startPoint = null;
    public Transform endPoint = null;

    public UIBar bar = null;

    // Use this for initialization
    void Start () {
        if (bar)
        {
            bar.maxFill = 1;
        }
    }
	
	// Update is called once per frame
	void Update () {
        float ratio = (axisNode.pointsAllocated / pointsToReachEnd);
        if (ratio > 1) { ratio = 1; }
        transform.position = startPoint.position + ratio * (endPoint.position - startPoint.position);

        if (bar)
        {
            bar.currentFill = 0.089f + (1f - 0.089f) * (ratio);
        }
	}
    
}
