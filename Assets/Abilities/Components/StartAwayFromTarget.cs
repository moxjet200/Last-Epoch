using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAwayFromTarget : DefineStartLocation
{
    public enum MeasuredFrom
    {
        Target, Start
    }


    public bool facesAwayFromStart = false;

    public MeasuredFrom measuredFrom = MeasuredFrom.Start;

    public float distance = 0f;

    public override void setLocation(Vector3 startLocation, Vector3 targetLocation)
    {
        if (active)
        {
            if (facesAwayFromStart) { transform.LookAt(targetLocation); }
            if (measuredFrom == MeasuredFrom.Start)
            {
                transform.position = transform.position - Vector3.Normalize(targetLocation - transform.position) * distance;
            }
            else if (measuredFrom == MeasuredFrom.Target)
            {
                transform.position = targetLocation - Vector3.Normalize(targetLocation - transform.position) * distance;
            }
        }
    }

}
