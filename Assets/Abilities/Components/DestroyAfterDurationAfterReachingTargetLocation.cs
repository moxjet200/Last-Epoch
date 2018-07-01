using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SelfDestroyer))]
[RequireComponent(typeof(LocationDetector))]
public class DestroyAfterDurationAfterReachingTargetLocation : MonoBehaviour {

    public float duration = 0.5f;

    // Use this for initialization
    void Awake()
    {
        GetComponent<LocationDetector>().reachedTargetLocationEvent += beginDestruction;
    }

    public void beginDestruction()
    {
        DestroyAfterDuration destroyAfterDuration = GetComponent<DestroyAfterDuration>();
        if (destroyAfterDuration == null) {
            destroyAfterDuration = gameObject.AddComponent<DestroyAfterDuration>();
            destroyAfterDuration.duration = duration;
        }
        if (destroyAfterDuration.duration > duration) { destroyAfterDuration.duration = duration; }
    }
}
