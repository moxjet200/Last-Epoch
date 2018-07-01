using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnStart : MonoBehaviour {

    public bool active = true;

	// Use this for initialization
	void Update () {
		if (active) { gameObject.SetActive(false); }
	}
}
