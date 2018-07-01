using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedShieldRushEndMutator : AbilityMutator
{
    public float increasedDamage = 0f;
    public float increasedRadius = 0f;
    public float timeRotChance = 0f;
    public float increasesDamageTaken = 0f;
    public float increasesDoTDamageTaken = 0f;
    public float increasedStunChance = 0f;
    public float addedCritMultiplier = 0f;
    public float addedCritChance = 0f;
    public bool leaveDelayed = false;
    public float increasedDelayLength = 0f;
    public float addedVoidDamage = 0f;

    AbilityObjectConstructor aoc = null;


    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.delayedShieldRushEnd);
        base.Awake();
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        ShieldRushEndMutator mutator = abilityObject.AddComponent<ShieldRushEndMutator>();
        mutator.increasedDamage = increasedDamage;
        mutator.increasedRadius = increasedRadius;
        mutator.timeRotChance = timeRotChance;
        mutator.increasesDamageTaken = increasesDamageTaken;
        mutator.increasesDoTDamageTaken = increasesDoTDamageTaken;
        mutator.increasedStunChance = increasedStunChance;
        mutator.addedCritMultiplier = addedCritMultiplier;
        mutator.addedCritChance = addedCritChance;
        mutator.leaveDelayed = leaveDelayed;
        mutator.addedVoidDamage = addedVoidDamage;

        if (increasedDelayLength != 0)
        {
            DestroyAfterDuration component = abilityObject.GetComponent<DestroyAfterDuration>();
            if (component)
            {
                component.duration *= (1 + increasedDelayLength);
            }
        }


        if (increasedRadius > 0)
        {
            foreach (CreateOnDeath cod in abilityObject.GetComponents<CreateOnDeath>())
            {
                cod.increasedRadius = increasedRadius;
                cod.increasedHeight = increasedRadius;
            }
        }

        return abilityObject;
    }
}
