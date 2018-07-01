using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// must be able to listen for hits and destroy itself
[RequireComponent(typeof(HitDetector))]
[RequireComponent(typeof(ReturnToCasterAfterDuration))]
public class ReturnOnInanimateCollision : MonoBehaviour
{
    ReturnToCasterAfterDuration returner = null;
    HitDetector detector = null;

    // Use this for initialization
    void Start()
    {
        returner = GetComponent<ReturnToCasterAfterDuration>();
        // subscribe to the inanimate hit event
        detector = GetComponent<HitDetector>();
        if (detector) { detector.inanimateHitEvent += startReturn; }
    }

    public void startReturn(GameObject hitObject)
    {
        if (returner)
        {
            returner.returning = true;
        }
    }

    void OnDestroy()
    {
        if (detector) { detector.inanimateHitEvent -= startReturn; }
    }
}
