using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotation : MonoBehaviour {

    public float degreesPerSecond = 0f;

    public float yrot = 0f;

    public bool randomiseStartingRotation = false;

	// Use this for initialization
	void Start () {
        if (randomiseStartingRotation) { yrot = Random.Range(0f,360f); }
        else { yrot = transform.rotation.y; }
	}
	
	// Update is called once per frame
	void Update () {
        if (degreesPerSecond != 0)
        {
            yrot += degreesPerSecond * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, yrot, 0);
        }
	}

    void LateUpdate()
    {
        if (degreesPerSecond != 0)
        {
            transform.rotation = Quaternion.Euler(0, yrot, 0);
        }
    }

}
