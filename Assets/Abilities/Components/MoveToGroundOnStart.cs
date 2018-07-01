using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToGroundOnStart : MonoBehaviour
{

    void Start()
    {
        transform.position = new Vector3(transform.position.x, getY(transform.position), transform.position.z);
    }

    // gets the Y coordinate of the ground at a position
    public float getY(Vector3 position)
    {
        // the maximum height above the ground to look for a surface
        float height = 10f;
        // raycast down from this height
        RaycastHit hit;
        LayerMask mask = 1 << LayerMask.NameToLayer("Default");
        if (Physics.Raycast(new Vector3(position.x, position.y + height, position.z), Vector3.down, out hit, Mathf.Infinity, mask))
        {
            return hit.point.y + 1.2f;
        }
        // if nothing was hit, just return the y value of the point
        return position.y;
    }
}
