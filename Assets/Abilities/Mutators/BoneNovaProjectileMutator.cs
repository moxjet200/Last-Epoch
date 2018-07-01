using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneNovaProjectileMutator : AbilityMutator
{
    public float increasedSpeed = 0f;
    public bool pierces = false;
    public float increasedDamage = 0f;
    public float increasedStunChance = 0f;
    public float bleedChance = 0f;
    public float addedCritChance = 0f;
    public float addedCritMultiplier = 0f;
    public bool cone = false;
    public bool randomAngles = false;
    public float moreDamageAgainstBleeding = 0f;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.boneNovaProjectile);
        base.Awake();
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        if (cone || randomAngles)
        {
            ExtraProjectiles component = abilityObject.GetComponent<ExtraProjectiles>();
            if (randomAngles) { component.randomAngles = true; }
            if (cone) { component.angle = 38f; }
        }

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

        if (bleedChance > 0)
        {
            ChanceToApplyStatusOnEnemyHit chanceTo = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            chanceTo.chance = bleedChance;
            chanceTo.statusEffect = StatusEffectList.getEffect(StatusEffectID.Bleed);
        }

        if (addedCritChance != 0)
        {
            foreach (DamageStatsHolder damage in abilityObject.GetComponents<DamageStatsHolder>())
            {
                damage.baseDamageStats.critChance += addedCritChance;
            }
        }

        if (addedCritMultiplier != 0)
        {
            foreach (DamageStatsHolder damage in abilityObject.GetComponents<DamageStatsHolder>())
            {
                damage.baseDamageStats.critMultiplier += addedCritMultiplier;
            }
        }

        if (moreDamageAgainstBleeding != 0)
        {
            // create the conditional
            DamageConditionalEffect conditionalEffect = new DamageConditionalEffect();
            HasStatusEffectConditional conditional = new HasStatusEffectConditional();
            conditional.statusEffect = StatusEffectID.Bleed;
            conditionalEffect.conditional = conditional;
            conditionalEffect.effect = new DamageEffectMoreDamage(moreDamageAgainstBleeding);
            // add the conditional to all damage stats holders
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.baseDamageStats.conditionalEffects.Add(conditionalEffect);
            }
        }

        return abilityObject;
    }


}