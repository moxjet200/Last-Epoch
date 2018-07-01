using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AbilityMover))]
public class DefineStartDirection : MonoBehaviour {

    public Vector3 startDirection;

    public void setDirection()
    {
        GetComponent<AbilityMover>().positionDelta = Vector3.Normalize(startDirection);
    }

}
