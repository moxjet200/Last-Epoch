using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneNovaMutator : AbilityMutator
{
    public float increasedSpeed = 0f;
    public bool pierces = false;
    public float increasedDamage = 0f;
    public float increasedStunChance = 0f;
    public float bleedChance = 0f;
    public float addedCritChance = 0f;
    public float addedCritMultiplier = 0f;
    public bool cone = false;
    public bool dontAttach = false;
    public bool dontMoveToTarget = false;
    public bool randomAngles = false;
    public float moreDamageAgainstBleeding = 0f;
    public bool noVFX = false;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.boneNova);
        base.Awake();
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        if (dontAttach)
        {
            AttachToNearestAllyOnCreation component = abilityObject.GetComponent<AttachToNearestAllyOnCreation>();
            component.runOnCreation = false;
        }

        if (dontMoveToTarget)
        {
            StartsAtTarget component = abilityObject.GetComponent<StartsAtTarget>();
            component.active = false;
        }

        if (noVFX)
        {
            CreateOnDeath cod = abilityObject.GetComponent<CreateOnDeath>();
            if (cod)
            {
                cod.objectsToCreateOnDeath.Clear();
            }
        }

        BoneNovaProjectileMutator mutator = abilityObject.AddComponent<BoneNovaProjectileMutator>();
        mutator.increasedSpeed = increasedSpeed;
        mutator.pierces = pierces;
        mutator.increasedDamage = increasedDamage;
        mutator.increasedStunChance = increasedStunChance;
        mutator.bleedChance = bleedChance;
        mutator.addedCritChance = addedCritChance;
        mutator.addedCritMultiplier = addedCritMultiplier;
        mutator.cone = cone;
        mutator.randomAngles = randomAngles;
        mutator.moreDamageAgainstBleeding = moreDamageAgainstBleeding;

        return abilityObject;
    }


}