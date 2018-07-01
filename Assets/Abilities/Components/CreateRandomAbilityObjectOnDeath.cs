using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SelfDestroyer))]
[RequireComponent(typeof(AbilityObjectConstructor))]
public class CreateRandomAbilityObjectOnDeath : MonoBehaviour
{

    public enum AimingMethod
    {
        TargetDirection, TravelDirection, Random
    }

    public List<Ability> possibleAbilities = new List<Ability>();

    public List<float> weights = new List<float>();

    [HideInInspector]
    public Ability abilityToInstantiate = null;
    public AimingMethod aimingMethod = AimingMethod.Random;
    public bool createAtTarget = false;
    public Vector3 offset = new Vector3(0, 0, 0);

    void Start()
    {
        // discard or add extra weights
        while (weights.Count > possibleAbilities.Count)
        {
            weights.RemoveAt(weights.Count - 1);
        }
        while (weights.Count < possibleAbilities.Count)
        {
            weights.Add(1);
        }
        // find total weight
        float totalWeight = 0f;
        for (int i = 0; i< weights.Count; i++)
        {
            totalWeight += weights[i];
        }
        // get random number
        float rand = Random.Range(0f, totalWeight);
        // check number against weights to pick skill
        float checkWeight = 0f;
        for (int i = 0; i < weights.Count; i++)
        {
            if (abilityToInstantiate == null)
            {
                checkWeight += weights[i];
                if (rand < checkWeight)
                {
                    abilityToInstantiate = possibleAbilities[i];
                }
            }
        }

        // subscribe to event
        GetComponent<SelfDestroyer>().deathEvent += createAbilityObject;
    }

    public void createAbilityObject()
    {
        if (abilityToInstantiate == null) { return; }

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
