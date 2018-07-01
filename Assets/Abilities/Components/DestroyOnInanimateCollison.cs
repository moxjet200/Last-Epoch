using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// must be able to listen for hits and destroy itself
[RequireComponent(typeof(HitDetector))]
[RequireComponent(typeof(SelfDestroyer))]
public class DestroyOnInanimateCollison : MonoBehaviour {

	// Use this for initialization
	void Start () {
        // subscribe to the inanimate hit event
        GetComponent<HitDetector>().inanimateHitEvent += destroySelf;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void destroySelf(GameObject hitObject)
    {
        GetComponent<SelfDestroyer>().die();
    }
}
