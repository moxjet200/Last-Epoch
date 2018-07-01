using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AbilityObjectConstructor))]
public class CastInLine : MonoBehaviour
{

    public Ability ability = null;
    public float distancePerCast = 1f;
    public int casts = 0;
    public Vector3 targetPoint = Vector3.zero;

    // Use this for initialization
    void Start()
    {
        AbilityObjectConstructor constructor = GetComponent<AbilityObjectConstructor>();
        if (constructor)
        {
            for (int i = 0; i < casts; i++)
            {
                Vector3 point = transform.position + Vector3.Normalize(targetPoint - transform.position) * distancePerCast * (i + 1);
                Vector3 pointOnGround = new Vector3(point.x, getY(point), point.z);
                constructor.constructAbilityObject(ability, pointOnGround, transform.position + Vector3.Normalize(targetPoint - transform.position) * distancePerCast * (i + 3));
            }
        }
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
