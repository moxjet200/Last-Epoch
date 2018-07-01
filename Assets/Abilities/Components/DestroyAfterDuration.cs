using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SelfDestroyer))]
public class DestroyAfterDuration : MonoBehaviour {
    
    public float duration = 1f;
    //[HideInInspector]
    public float age = 0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (age>= duration) { GetComponent<SelfDestroyer>().die(); }
        age += Time.deltaTime;
	}
}
