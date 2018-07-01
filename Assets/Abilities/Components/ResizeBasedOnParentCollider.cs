using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeBasedOnParentCollider : MonoBehaviour {

    bool resized = false;

    GameObject parent = null;

    // if true it will keep the x,y and z scales equal to each other
    public bool maintainShape = false;

    // Use this for initialization
    void Start () {
	}
	
    void Awake()
    {
        if (transform.parent) { parent = transform.parent.gameObject; }
    }

	// Update is called once per frame
	void Update () {
		if (!resized)
        {
            if (parent == null && transform.parent) { parent = transform.parent.gameObject; }
            if (parent != null)
            {
                transform.localScale = new Vector3(1, 1, 1);
                resized = true;
                // if the shape should be maintained then look at the max or min of the dimensions of the colliders
                if (maintainShape)
                {
                    if (parent.GetComponent<BoxCollider>())
                    {
                        transform.localScale *= Mathf.Max(parent.GetComponent<BoxCollider>().size.x, parent.GetComponent<BoxCollider>().size.y, parent.GetComponent<BoxCollider>().size.z);
                    }
                    else if (parent.GetComponent<CapsuleCollider>())
                    {
                        transform.localScale *= Mathf.Max(parent.GetComponent<CapsuleCollider>().radius*2, parent.GetComponent<CapsuleCollider>().height);
                    }
                    else if (parent.GetComponent<SphereCollider>())
                    {
                        transform.localScale = new Vector3(parent.GetComponent<SphereCollider>().radius * 2, parent.GetComponent<SphereCollider>().radius * 2, parent.GetComponent<SphereCollider>().radius * 2);
                    }
                }
                // otherwise scale each dimension individually
                else
                {
                    if (parent.GetComponent<BoxCollider>())
                    {
                        transform.localScale = parent.GetComponent<BoxCollider>().size;
                    }
                    else if (parent.GetComponent<CapsuleCollider>())
                    {
                        CapsuleCollider cap = parent.GetComponent<CapsuleCollider>();
                        if (cap.direction == 0)
                        {
                            transform.localScale = new Vector3(parent.GetComponent<CapsuleCollider>().height,
                                parent.GetComponent<CapsuleCollider>().radius * 2, parent.GetComponent<CapsuleCollider>().radius * 2);
                        }
                        else if (cap.direction == 1)
                        {
                            transform.localScale = new Vector3(parent.GetComponent<CapsuleCollider>().radius * 2,
                                parent.GetComponent<CapsuleCollider>().height, parent.GetComponent<CapsuleCollider>().radius * 2);
                        }
                        else
                        {
                            transform.localScale = new Vector3(parent.GetComponent<CapsuleCollider>().radius * 2,
                                parent.GetComponent<CapsuleCollider>().radius * 2, parent.GetComponent<CapsuleCollider>().height);
                        }
                    }
                    else if (parent.GetComponent<SphereCollider>())
                    {
                        transform.localScale = new Vector3(parent.GetComponent<SphereCollider>().radius * 2, parent.GetComponent<SphereCollider>().radius * 2, parent.GetComponent<SphereCollider>().radius * 2);
                    }
                }
            }
        }
	}
}
