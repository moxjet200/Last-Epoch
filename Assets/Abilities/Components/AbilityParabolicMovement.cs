using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LocationDetector))]
public class AbilityParabolicMovement : MonoBehaviour
{

    public float maximumHeight = 5f;
    public float duration = 1f;
    float proportionDone = 0f;
    LocationDetector locationDetector = null;

    // Use this for initialization
    void Start()
    {
        locationDetector = GetComponent<LocationDetector>();
    }

    void Update()
    {
        if (proportionDone < 1)
        {
            // increment the proportion travelled
            float proportionToTravel = Time.deltaTime / duration;
            proportionDone += proportionToTravel;
            // set the position
            Vector3 pointOnLine = (locationDetector.startLocation + (locationDetector.targetLocation - locationDetector.startLocation) * proportionDone);
            float additionalHeight = maximumHeight * (1 - Mathf.Pow(2 * proportionDone - 1, 2));
            transform.position = new Vector3(pointOnLine.x, pointOnLine.y + additionalHeight, pointOnLine.z);
        }
    }
}
