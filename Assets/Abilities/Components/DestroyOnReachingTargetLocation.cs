using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SelfDestroyer))]
[RequireComponent(typeof(LocationDetector))]
public class DestroyOnReachingTargetLocation : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        GetComponent<LocationDetector>().reachedTargetLocationEvent += GetComponent<SelfDestroyer>().die;
	}
}
