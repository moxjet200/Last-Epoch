using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnablesChildrenAfterDelay : MonoBehaviour {

    public float delay = 0f;
    float age = 0f;

	// Update is called once per frame
	void Update () {
		if (age >= delay)
        {
            foreach(Transform child in transform)
            {
                child.gameObject.SetActive(true);
                child.transform.parent = null;
            }
            Destroy(gameObject);
        }
        else
        {
            age += Time.deltaTime;
        }
	}
}
