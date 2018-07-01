using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroyer : MonoBehaviour {

    public delegate void DeathAction();
    public event DeathAction deathEvent;

    public bool failedAbility = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void die()
    {
        // invoke the death event
        if (deathEvent != null) { deathEvent.Invoke(); }
        // destroy this game object
        Destroy(gameObject);
    }

}
