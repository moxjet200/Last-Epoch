using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsCreator : MonoBehaviour
{
    GameObject creator = null;
    public Vector3 displacement = Vector3.zero;
    public float speed = 0f;
    public float acceleration = 0f;
    public bool snapWhenClose = true; 
    //public float maxDuration = 0f;
    //float index = 0f;
    bool snappedThisFrame = false;

    public void Start()
    {
        CreationReferences references = GetComponent<CreationReferences>();
        if (references)
        {
            creator = references.creator;
        }
    }

    public void Update()
    {
        if (creator)
        {
            snappedThisFrame = false;

            // if there is a max duration just move the creator if it has been reached
            //if (maxDuration > 0)
            //{
            //    if (index > maxDuration)
            //    {
            //        transform.position = creator.transform.position + displacement;
            //        snappedThisFrame = true;
            //    }
            //    index += Time.deltaTime;
            //}

            // if it should snap when close and is close then snap
            if (snapWhenClose && !snappedThisFrame)
            {
                if (Maths.distanceLessThan(transform.position, creator.transform.position, speed * Time.deltaTime))
                {
                    transform.position = creator.transform.position + displacement;
                    snappedThisFrame = true;
                }
            }

            // if it has not spanned to the target this frame then move as normal
            if (!snappedThisFrame) {
                Vector3 position = transform.position;
                transform.position = position + Vector3.Normalize(creator.transform.position + displacement - position) * speed * Time.deltaTime;
                speed += acceleration * Time.deltaTime;
            }
        }
    }


}
