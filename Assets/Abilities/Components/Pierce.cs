using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DestroyOnFailingToPierceEnemy))]
public class Pierce : MonoBehaviour {

    public int objectsToPierce = 0;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual bool canPierce(GameObject gameObject)
    {
        if (objectsToPierce > 0) { return true; }
        else { return false; }
    }

    public virtual void pierce(GameObject gameObject)
    {
        objectsToPierce--;
    }

}
