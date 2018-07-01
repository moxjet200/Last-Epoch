using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AbilityMover))]
public class RandomiseSpeed : MonoBehaviour {

    public int stepDelay = 0;
    public float maxReduction = 0f;
    public float maxIncrease = 0f;
    int index = 0;
    bool changed = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (!changed)
        {
            index++;
            if (index > stepDelay)
            {
                changed = true;
                GetComponent<AbilityMover>().speed += Random.Range(-maxReduction, maxIncrease);
            }
        }
	}
}
