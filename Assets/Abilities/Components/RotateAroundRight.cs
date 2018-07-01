using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundRight : MonoBehaviour {

    public float degreesPerSecond = 0f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(transform.position,transform.right, degreesPerSecond * Time.deltaTime);
    }
}
