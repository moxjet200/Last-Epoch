using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AbilityMover))]
[RequireComponent(typeof(LocationDetector))]
public class RandomiseDirection : MonoBehaviour {

    public enum TimeToRandomise
    {
        Start, EveryFrame, DirectionMode
    }


    public float maximumAngleChange = 0f;
    public TimeToRandomise timeToRandomise = TimeToRandomise.Start;
    [Header("direction mode only")]
    public float directionChangeInterval = 0.1f;
    public float boundingAngle = 180f;

    float currentAngleChange = 0f;
    float index = 0f;
    Vector3 currentDirection = Vector3.zero;

    AbilityMover mover = null;
    LocationDetector locationDetector = null;

    // Use this for initialization
    void Start () {
        // get references
        mover = GetComponent<AbilityMover>();
        locationDetector = GetComponent<LocationDetector>();
        // set the current direction to the start direction
        currentDirection = locationDetector.targetLocation - locationDetector.startLocation;
        // randomise at start
        if (timeToRandomise == TimeToRandomise.Start)
        {
            randomiseDirection(maximumAngleChange);
        }
        // prepare to randomise initialy if using direction mode
        index = directionChangeInterval;
    }
	
	// Update is called once per frame
	void Update () {
        // randomise every frame
        if (timeToRandomise == TimeToRandomise.EveryFrame)
        {
            randomiseDirection(maximumAngleChange * Time.deltaTime);
        }
        // direction mode stuff
        else if (timeToRandomise == TimeToRandomise.DirectionMode)
        {
            // change the direction every directionChangeInterval seconds
            if (index <= directionChangeInterval)
            {
                changeDirection();
                index -= directionChangeInterval;
            }
            index += Time.deltaTime;
            // change the angle every frame
            Vector3 direction = Quaternion.Euler(0, currentAngleChange, 0) * currentDirection;
            mover.SetDirection(direction);
        }
	}

    public void randomiseDirection(float maxAngleChange)
    {
        float angleChange = Random.Range(-maxAngleChange, maxAngleChange);
        Vector3 direction = Quaternion.Euler(0, angleChange, 0) * currentDirection;
        mover.SetDirection(direction);
    }

    public void changeDirection()
    {
        float angleChange = Random.Range(-maximumAngleChange * directionChangeInterval, maximumAngleChange * directionChangeInterval);
        currentAngleChange += angleChange;
        currentAngleChange = Mathf.Clamp(currentAngleChange, -boundingAngle, boundingAngle);
    }

}
