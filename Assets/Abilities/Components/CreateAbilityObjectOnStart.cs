using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AbilityObjectConstructor))]
public class CreateAbilityObjectOnStart : MonoBehaviour
{

    public enum AimingMethod
    {
        TargetDirection, TravelDirection, Random
    }

    public Ability abilityToInstantiate;
    public AimingMethod aimingMethod = AimingMethod.Random;
    public bool createAtTarget = false;
    public bool createAtStartLocation = false;
    public Vector3 offset = new Vector3(0, 0, 0);
    public bool active = true;

    public void Start()
    {
        if (!active) { return; }
        //if the aiming method requires a component that is not present do not use it
        if (aimingMethod == AimingMethod.TargetDirection && !GetComponent<LocationDetector>()) { aimingMethod = AimingMethod.Random; }
        if (aimingMethod == AimingMethod.TravelDirection && !GetComponent<AbilityMover>()) { aimingMethod = AimingMethod.Random; }

        Vector3 startPos = transform.position;
        if (createAtTarget)
        {
            LocationDetector ld = GetComponent<LocationDetector>();
            if (ld)
            {
                startPos = ld.targetLocation;
            }
        }
        else if (createAtStartLocation)
        {
            LocationDetector ld = GetComponent<LocationDetector>();
            if (ld)
            {
                startPos = ld.startLocation;
            }
        }


        startPos += offset;

        // create the ability object depending on the aiming method
        if (aimingMethod == AimingMethod.Random)
        {
            // create a random aim point
            Vector3 aimPoint = new Vector3(startPos.x + Random.Range(-5f, 5f), startPos.y, startPos.z + Random.Range(-5f, 5f));
            // create the ability object
            GetComponent<AbilityObjectConstructor>().constructAbilityObject(abilityToInstantiate, startPos, aimPoint);
        }
        else if (aimingMethod == AimingMethod.TargetDirection)
        {
            // create the ability object
            GetComponent<AbilityObjectConstructor>().constructAbilityObject(abilityToInstantiate, startPos, GetComponent<LocationDetector>().targetLocation);
        }
        else if (aimingMethod == AimingMethod.TravelDirection)
        {
            // create the ability object
            GetComponent<AbilityObjectConstructor>().constructAbilityObject(abilityToInstantiate, startPos, transform.position + GetComponent<AbilityMover>().positionDelta);
        }
    }

    public void deactivate()
    {
        active = false;
    }
}
