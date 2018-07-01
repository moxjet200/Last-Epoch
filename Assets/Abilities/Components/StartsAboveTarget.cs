using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartsAboveTarget : DefineStartLocation {

    public float startHeight = 4;

    public override void setLocation(Vector3 startLocation, Vector3 targetLocation)
    {
        transform.position = new Vector3(targetLocation.x, targetLocation.y + startHeight, targetLocation.z);
    }
}
