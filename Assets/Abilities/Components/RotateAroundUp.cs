using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundUp : MonoBehaviour {

    public float degreesPerSecond = 0f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(transform.position,transform.up, degreesPerSecond * Time.deltaTime);
    }
}
