using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuryLeapAoeMutator : AbilityMutator
{
    public float increasedDamage = 0f;
    public float increasedRadius = 0f;
    public float bleedChance = 0f;
    public float increasedStunChance = 0f;
    public float addedCritMultiplier = 0f;

    public float moreDamageAgainstFullHealth = 0f;

    AbilityObjectConstructor aoc = null;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.furyLeapAoe);
        base.Awake();
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        if (addedCritMultiplier != 0)
        {
            foreach (DamageStatsHolder damage in abilityObject.GetComponents<DamageStatsHolder>())
            {
                damage.baseDamageStats.critMultiplier += addedCritMultiplier;
            }
        }

        if (increasedDamage != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(increasedDamage);
            }
        }

        if (increasedRadius > 0)
        {
            foreach (CreateOnDeath cod in abilityObject.GetComponents<CreateOnDeath>())
            {
                cod.increasedRadius = increasedRadius;
            }
            foreach (CapsuleCollider col in abilityObject.GetComponents<CapsuleCollider>())
            {
                col.height *= (1 + increasedRadius);
                col.radius *= (1 + increasedRadius);
            }
        }

        if (bleedChance > 0)
        {
            ChanceToApplyStatusOnEnemyHit newComponent = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            newComponent.statusEffect = StatusEffectList.getEffect(StatusEffectID.Bleed);
            newComponent.chance = bleedChance;
        }

        if (increasedStunChance != 0)
        {
            foreach (DamageStatsHolder damage in abilityObject.GetComponents<DamageStatsHolder>())
            {
                damage.baseDamageStats.increasedStunChance += increasedStunChance;
            }
        }

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

        return abilityObject;
    }
}
