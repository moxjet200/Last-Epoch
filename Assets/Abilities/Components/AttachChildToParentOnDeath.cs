using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SelfDestroyer))]
public class AttachChildToParentOnDeath : MonoBehaviour {

    public GameObject childToAdd = null;

	// Use this for initialization
	void Start () {
        GetComponent<SelfDestroyer>().deathEvent += attachChild;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void attachChild()
    {
        GameObject child = Instantiate(childToAdd);
        child.transform.parent = transform.parent;
        child.transform.position = transform.parent.position;
    }
}
