using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LocationDetector))]
public class HomingMovement : MonoBehaviour {

    LocationDetector locationDetector = null;
    Transform target = null;
    GameObject targetGo = null;
    AbilityMover abilityMover = null;
    public float stopHomingDistance = 1f;
    public float maxDistanceFromTarget = 8f;
    Vector3 targetPosition;
    Vector3 newDirection;
    public float homingSpeed = 10f;
    public bool changeFacingDirection = true;

    // Use this for initialization
    void Start () {
        abilityMover = GetComponent<AbilityMover>();

        locationDetector = GetComponent<LocationDetector>();
        AlignmentManager alignmentManager = GetComponent<AlignmentManager>();
        if (locationDetector && alignmentManager)
        {
            Alignment alignment = alignmentManager.alignment;
            if (alignment) {

                List<Transform> possibleTargets = new List<Transform>();
                foreach (BaseHealth health in BaseHealth.all)
                {
                    if (health.alignmentManager && alignment.foes.Contains(health.alignmentManager.alignment) && health.currentHealth > 0)
                    {
                        possibleTargets.Add(health.transform);
                    }
                }

                Vector3 position = locationDetector.targetLocation;
                targetPosition = position;
                float distance = 0f;
                float targetDistance = 0f;

                foreach (Transform possibleTarget in possibleTargets)
                {
                    distance = Vector3.Distance(position, possibleTarget.position);
                    targetDistance = Vector3.Distance(position, targetPosition);
                    if ((target == null || distance < targetDistance) && distance < maxDistanceFromTarget)
                    {
                        targetPosition = possibleTarget.position;
                        target = possibleTarget;
                    }
                }

                if (target) { targetGo = target.gameObject; }
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		if (target && abilityMover && !Dying.isDying(targetGo))
        {
            targetPosition = target.position;
            newDirection = Vector3.Normalize(new Vector3(target.position.x, target.position.y + 1.2f, target.position.z) - transform.position);
            if (Time.deltaTime * homingSpeed > 1)
            {
                abilityMover.SetDirection(newDirection, changeFacingDirection);
            }
            else {
                abilityMover.SetDirection(newDirection* Time.deltaTime * homingSpeed + abilityMover.positionDelta * (1 - Time.deltaTime * homingSpeed), changeFacingDirection);
            }
        }
	}



}
