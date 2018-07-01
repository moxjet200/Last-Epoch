using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorShrapnelMutator : AbilityMutator
{
    public float increasedSpeed = 0f;
    public bool pierces = false;
    public float increasedDamage = 0f;
    public float increasedStunChance = 0f;
    public List<float> moreDamageInstances = new List<float>();

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.meteorShrapnel);
        base.Awake();
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        if (increasedSpeed != 0)
        {
            AbilityMover component = abilityObject.GetComponent<AbilityMover>();
            if (component)
            {
                component.speed *= 1 + increasedSpeed;
            }
        }

        if (increasedStunChance != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.baseDamageStats.increasedStunChance += increasedStunChance;
            }
        }

        if (increasedDamage != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(increasedDamage);
            }
        }

        if (moreDamageInstances != null && moreDamageInstances.Count > 0)
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