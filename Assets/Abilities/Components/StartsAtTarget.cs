using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartsAtTarget : DefineStartLocation {

    public bool facesAwayFromStart = false;

    public override void setLocation(Vector3 startLocation, Vector3 targetLocation)
    {
        if (active)
        {
            if (facesAwayFromStart) { transform.LookAt(targetLocation); }
            transform.position = targetLocation;
        }
    }
}
