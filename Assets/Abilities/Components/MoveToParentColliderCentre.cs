using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToParentColliderCentre : MonoBehaviour {

    bool moved = false;

	// Use this for initialization
	void Start () {
		
	}
	
    void Awake()
    {
        if (!moved)
        {
            if (transform.parent != null)
            {
                moved = true;
                if (GetComponentInParent<BoxCollider>())
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + GetComponentInParent<BoxCollider>().center.y, transform.position.z);
                }
                else if (GetComponentInParent<CapsuleCollider>())
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + GetComponentInParent<CapsuleCollider>().center.y, transform.position.z);
                }
                else if (GetComponentInParent<SphereCollider>())
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + GetComponentInParent<SphereCollider>().center.y, transform.position.z);
                }
            }
        }
    }

	// Update is called once per frame
	void Update () {
		if (!moved)
        {
            if (transform.parent != null)
            {
                moved = true;
                if (GetComponentInParent<BoxCollider>())
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + GetComponentInParent<BoxCollider>().center.y, transform.position.z);
                }
                else if (GetComponentInParent<CapsuleCollider>())
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + GetComponentInParent<CapsuleCollider>().center.y, transform.position.z);
                }
                else if (GetComponentInParent<SphereCollider>())
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + GetComponentInParent<SphereCollider>().center.y, transform.position.z);
                }
            }
        }
	}
}
