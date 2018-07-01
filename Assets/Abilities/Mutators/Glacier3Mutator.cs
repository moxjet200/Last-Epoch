using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glacier3Mutator : AbilityMutator
{

    [Range(0f, 1f)]
    public float chanceToCreateIceVortex = 0f;

    [Range(0f, 1f)]
    public float chanceForSuperIceVortex = 0f;

    [Range(0f, 1f)]
    public float chillChance = 0f;

    public float increasedDamage = 0f;
    public float moreDamageAgainstChilled = 0f;

    public float increasedStunChance = 0f;
    public float addedCritChance = 0f;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.glacier3);
        base.Awake();
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        // add chance to create ice vortex
        if (chanceToCreateIceVortex > 0)
        {
            float rand = Random.Range(0f, 1f);
            if (rand < chanceToCreateIceVortex)
            {
                float rand2 = Random.Range(0f, 1f);
                if (rand2 < chanceForSuperIceVortex)
                {
                    abilityObject.AddComponent<CreateAbilityObjectOnDeath>().abilityToInstantiate = AbilityIDList.getAbility(AbilityID.superIceVortex);
                }
                else
                {
                    abilityObject.AddComponent<CreateAbilityObjectOnDeath>().abilityToInstantiate = AbilityIDList.getAbility(AbilityID.iceVortex);
                }
            }
        }

        // add chance to chill
        if (chillChance > 0)
        {
            ChanceToApplyStatusOnEnemyHit chanceTo = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            chanceTo.chance = chillChance;
            chanceTo.statusEffect = StatusEffectList.getEffect(StatusEffectID.Chill);
        }

        if (increasedDamage != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(increasedDamage);
                //holder.addBaseDamage(DamageType.FIRE, holder.getBaseDamage(DamageType.FIRE) * increasedDamage);
            }
        }

        if (moreDamageAgainstChilled != 0)
        {
            // create the conditional
            DamageConditionalEffect conditionalEffect = new DamageConditionalEffect();
            HasStatusEffectConditional conditional = new HasStatusEffectConditional();
            conditional.statusEffect = StatusEffectID.Chill;
            conditionalEffect.conditional = conditional;
            conditionalEffect.effect = new DamageEffectMoreDamage(moreDamageAgainstChilled);
            // add the conditional to all damage stats holders
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.baseDamageStats.conditionalEffects.Add(conditionalEffect);
            }
        }

        if (increasedStunChance != 0)
        {
            foreach (DamageStatsHolder damage in abilityObject.GetComponents<DamageStatsHolder>())
            {
                damage.baseDamageStats.increasedStunChance += increasedStunChance;
            }
        }

        if (addedCritChance != 0)
        {
            foreach (DamageStatsHolder damage in abilityObject.GetComponents<DamageStatsHolder>())
            {
                damage.baseDamageStats.critChance += addedCritChance;
            }
        }


        return abilityObject;
    }
}