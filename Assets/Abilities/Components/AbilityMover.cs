using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityMover : MonoBehaviour
{

    [HideInInspector]
    public Vector3 positionDelta;

    public float speed;

    public bool fixedY = false;

    public bool staysOnGround = false;
    public float yOffSetWhenStayingOnGround = 0f;

    // Use this for initialization
    void Start()
    {
        if (fixedY && positionDelta != null)
        {
            positionDelta.y = 0;
            SetDirection(positionDelta);
        }
    }

    public void SetDirection(Vector3 direction, bool changeFacingDirection = true)
    {
        if (fixedY) { positionDelta = Vector3.Normalize(new Vector3(direction.x, 0, direction.z)); }
        else { positionDelta = Vector3.Normalize(direction); }
        if (changeFacingDirection)
        {
            transform.LookAt(transform.position + positionDelta);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += positionDelta * speed * Time.deltaTime;
        if (staysOnGround) { transform.position = new Vector3(transform.position.x, getY(transform.position), transform.position.z); }
    }

    // gets the Y coordinate of the ground at a position
    public float getY(Vector3 position)
    {
        // the maximum height above the ground to look for a surface
        float height = 5f;
        // raycast down from this height
        RaycastHit hit;
        LayerMask mask = 1 << LayerMask.NameToLayer("Default");
        if (Physics.Raycast(new Vector3(position.x, position.y + height, position.z), Vector3.down, out hit, Mathf.Infinity, mask))
        {
            return hit.point.y + yOffSetWhenStayingOnGround;
        }
        // if nothing was hit, just return the y value of the point
        return position.y;
    }
}

// Test Test
