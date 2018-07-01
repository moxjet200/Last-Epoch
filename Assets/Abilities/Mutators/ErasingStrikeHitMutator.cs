using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErasingStrikeHitMutator : AbilityMutator
{
    public float increasedDamage = 0f;
    public float increasedRadius = 0f;
    public float timeRotChance = 0f;
    public float increasedStunChance = 0f;
    public float addedCritMultiplier = 0f;
    public float addedCritChance = 0f;

    public float addedVoidDamage = 0f;

    public float moreDamageAgainstFullHealth = 0f;
    public float moreDamageAgainstDamaged = 0f;

    public float cullPercent = 0f;


    // void rift stats
    public float voidRift_increasedDamage = 0f;
    public float voidRift_increasedRadius = 0f;
    public float voidRift_timeRotChance = 0f;
    public float voidRift_increasesDamageTaken = 0f;
    public float voidRift_increasesDoTDamageTaken = 0f;
    public float voidRift_increasedStunChance = 0f;
    public float voidRift_moreDamageAgainstStunned = 0f;

    AbilityObjectConstructor aoc = null;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.erasingStrikeHit);
        base.Awake();
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        VoidRiftMutator voidRiftMutator = Comp<VoidRiftMutator>.GetOrAdd(abilityObject);
        voidRiftMutator.increasedDamage = voidRift_increasedDamage;
        voidRiftMutator.increasedRadius = voidRift_increasedRadius;
        voidRiftMutator.timeRotChance = voidRift_timeRotChance;
        voidRiftMutator.increasesDamageTaken = voidRift_increasesDamageTaken;
        voidRiftMutator.increasesDoTDamageTaken = voidRift_increasesDoTDamageTaken;
        voidRiftMutator.increasedStunChance = voidRift_increasedStunChance;
        voidRiftMutator.moreDamageAgainstStunned = voidRift_moreDamageAgainstStunned;

        if (addedVoidDamage > 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.addBaseDamage(DamageType.VOID, addedVoidDamage);
            }
        }

        if (increasedDamage != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(increasedDamage);
            }
        }

        if (increasedRadius != 0)
        {
            foreach (CreateOnDeath cod in abilityObject.GetComponents<CreateOnDeath>())
            {
                cod.increasedRadius = increasedRadius;
                cod.increasedHeight = increasedRadius;
            }
            foreach (CapsuleCollider col in abilityObject.GetComponents<CapsuleCollider>())
            {
                col.height *= (1 + increasedRadius);
                col.radius *= (1 + increasedRadius);
            }
        }

        if (timeRotChance > 0)
        {
            ChanceToApplyStatusOnEnemyHit newComponent = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            newComponent.statusEffect = StatusEffectList.getEffect(StatusEffectID.TimeRot);
            newComponent.chance = timeRotChance;
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

        if (addedCritMultiplier != 0)
        {
            foreach (DamageStatsHolder damage in abilityObject.GetComponents<DamageStatsHolder>())
            {
                damage.baseDamageStats.critMultiplier += addedCritMultiplier;
            }
        }

        if (moreDamageAgainstFullHealth != 0)
        {
            // create the conditional
            DamageConditionalEffect conditionalEffect = new DamageConditionalEffect();
            FullHealthConditional conditional = new FullHealthConditional();
            conditionalEffect.conditional = conditional;
            conditionalEffect.effect = new DamageEffectMoreDamage(moreDamageAgainstFullHealth);
            // add the conditional to all damage stats holders
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.baseDamageStats.conditionalEffects.Add(conditionalEffect);
            }
        }

        if (moreDamageAgainstDamaged != 0)
        {
            // create the conditional
            DamageConditionalEffect conditionalEffect = new DamageConditionalEffect();
            DamagedConditional conditional = new DamagedConditional();
            conditionalEffect.conditional = conditional;
            conditionalEffect.effect = new DamageEffectMoreDamage(moreDamageAgainstDamaged);
            // add the conditional to all damage stats holders
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.baseDamageStats.conditionalEffects.Add(conditionalEffect);
            }
        }

        if (cullPercent > 0)
        {
            foreach (DamageStatsHolder damage in abilityObject.GetComponents<DamageStatsHolder>())
            {
                damage.baseDamageStats.cullPercent += cullPercent;
            }
        }


        return abilityObject;
    }
}
