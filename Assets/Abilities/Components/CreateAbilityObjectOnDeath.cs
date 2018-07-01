using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SelfDestroyer))]
[RequireComponent(typeof(AbilityObjectConstructor))]
public class CreateAbilityObjectOnDeath : MonoBehaviour {

    public enum AimingMethod
    {
        TargetDirection, TravelDirection, Random
    }

    public Ability abilityToInstantiate;
    public AimingMethod aimingMethod = AimingMethod.Random;
    public bool createAtTarget = false;
    public bool createAtStartLocation = false;
    public bool createAtCastLocation = false;
    public Vector3 offset = new Vector3(0,0,0);
    public bool failsIfFailedAbility = false;

	void Awake () {
        GetComponent<SelfDestroyer>().deathEvent += createAbilityObject;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void createAbilityObject()
    {
        if (failsIfFailedAbility)
        {
            SelfDestroyer destroyer = GetComponent<SelfDestroyer>();
            if (destroyer && destroyer.failedAbility) { return; }
        }

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
        else if (createAtCastLocation)
        {
            CreationReferences references = GetComponent<CreationReferences>();
            if (references)
            {
                startPos = references.locationCreatedFrom;
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
        SelfDestroyer sd = GetComponent<SelfDestroyer>();
        if (sd)
        {
            sd.deathEvent -= createAbilityObject;
        }
    }
}
