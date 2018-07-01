using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// does not move the object itself, but is detected by the ability object constructor
public abstract class DefineStartLocation : MonoBehaviour {

    public bool active = true;

    public abstract void setLocation(Vector3 startLocation, Vector3 targetLocation);

    public virtual bool maintainDirectionFromCastPoint()
    {
        return false;
    }

}
