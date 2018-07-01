using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorAoEMutator : AbilityMutator
{

    public float moreDamageAgainstFullHealth = 0f;
    public float shrapnelChance = 0f;
    public float increasedShrapnelSpeed = 0f;
    public float increasedStunChance = 0f;
    public List<float> moreDamageInstances = new List<float>();


    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.meteorAoe);
        base.Awake();
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        if (moreDamageAgainstFullHealth != 0)
        {
            // create the conditional
            DamageConditionalEffect conditionalEffect = new DamageConditionalEffect();
            conditionalEffect.conditional = new FullHealthConditional();
            conditionalEffect.effect = new DamageEffectMoreDamage(moreDamageAgainstFullHealth);
            // add the conditional to all damage stats holders
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.baseDamageStats.conditionalEffects.Add(conditionalEffect);
            }
        }

        if (increasedStunChance != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.baseDamageStats.increasedStunChance += increasedStunChance;
            }
        }

        if (moreDamageInstances!=null && moreDamageInstances.Count > 0)
        {
            float moreDamage = 1f;
            foreach (float instance in moreDamageInstances)
            {
                moreDamage *= 1 + instance;
            }

            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(moreDamage - 1);
            }
        }

        return abilityObject;
    }
}