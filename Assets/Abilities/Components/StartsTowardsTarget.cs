using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartsTowardsTarget : DefineStartLocation {

    public float distance = 0f;
    public bool addWeaponRange = false;

    public override void setLocation(Vector3 startLocation, Vector3 targetLocation)
    {
        float newDistance = distance;
        if (addWeaponRange)
        {
            newDistance += PlayerFinder.getPlayer().GetComponent<WeaponInfoHolder>().weaponRange;
        }
        transform.position = transform.position + Vector3.Normalize(targetLocation - transform.position) * newDistance;
    }

    public override bool maintainDirectionFromCastPoint()
    {
        return true;
    }
}
