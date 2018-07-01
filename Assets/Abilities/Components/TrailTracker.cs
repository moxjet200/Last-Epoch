using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailTracker : MonoBehaviour
{
    public float trailDuration = 2f;
    public int trailPrecision = 5;

    public List<Vector3> trailPoints = new List<Vector3>();
    public List<float> trailPointAges = new List<float>();

    float recordInterval = 0f;
    float age = 0f;

    void Start()
    {
        recordInterval = trailDuration / ((float)trailPrecision * 3);
    }

    void Update()
    {
        // increment the ages
        for (int i =0; i < trailPointAges.Count; i++)
        {
            trailPointAges[i] += Time.deltaTime;
        }

        // remove points that are much too old
        while (trailPointAges.Count > 0 && trailPointAges[0] > trailDuration + 1)
        {
            trailPointAges.RemoveAt(0);
            trailPoints.RemoveAt(0);
        }

        // add a new point
        age += Time.deltaTime;
        if (age > recordInterval)
        {
            age -= recordInterval;
            trailPoints.Add(transform.position);
            trailPointAges.Add(0);
        }
    }

    public List<Vector3> getTrailPoints()
    {
        // the points to return
        List<Vector3> returnPoints = new List<Vector3>();

        // gather trailPrecision number of points
        float precision = trailPrecision;
        float requiredAge = trailDuration/precision;
        for (int i = 0; i < trailPoints.Count && i < trailPointAges.Count; i++)
        {
            if (requiredAge <= trailPointAges[i])
            {
                returnPoints.Add(trailPoints[i]);
                requiredAge += trailDuration / precision;
            }
        }

        // return them
        return returnPoints;
    }
    
}

