using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempestHitMutator : AbilityMutator
{

    public float addedCritMultiplier = 0f;
    public float addedCritChance = 0f;
    
    public float increasedDamage = 0f;
    public float chanceToGainWardOnHit = 0f;
    public float increasedWardGained = 0f;
    public float timeRotChance = 0f;
    public float moreDamageAgainstFullHealth = 0f;
    public float chanceToAttachSparkCharge = 0f;
    public float increasedRadius = 0f;
    public float igniteChance = 0f;
    public float moreDamageAgainstTimeRotting = 0f;

    public float addedVoidDamage = 0f;

    ProtectionClass protectionClass = null;
    StatBuffs statBuffs = null;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.tempestHit);
        base.Awake();
    }

    public override List<Tags.AbilityTags> getUseTags()
    {
        List<Tags.AbilityTags> list = base.getUseTags();
        if (addedVoidDamage > 0)
        {
            list.Add(Tags.AbilityTags.Void);
        }
        return list;
    }

    GameObject instantiateIceNova(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        Destroy(abilityObject);
        return Instantiate(AbilityIDList.getAbility(AbilityID.iceNova).abilityPrefab, location, Quaternion.Euler(targetLocation - location));
    }

    GameObject instantiateFireNova(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        Destroy(abilityObject);
        return Instantiate(AbilityIDList.getAbility(AbilityID.fireNova).abilityPrefab, location, Quaternion.Euler(targetLocation - location));
    }

    GameObject instantiateLightningNova(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        Destroy(abilityObject);
        return Instantiate(AbilityIDList.getAbility(AbilityID.lightningNova).abilityPrefab, location, Quaternion.Euler(targetLocation - location));
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {

        if (addedVoidDamage > 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.addBaseDamage(DamageType.VOID, addedVoidDamage);
            }
        }

        if (timeRotChance > 0)
        {
            ChanceToApplyStatusOnEnemyHit newComponent = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            newComponent.statusEffect = StatusEffectList.getEffect(StatusEffectID.TimeRot);
            newComponent.chance = timeRotChance;
        }

        if (igniteChance > 0)
        {
            ChanceToApplyStatusOnEnemyHit newComponent = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            newComponent.statusEffect = StatusEffectList.getEffect(StatusEffectID.Ignite);
            newComponent.chance = igniteChance;
        }

        if (addedCritChance != 0)
        {
            foreach (DamageStatsHolder damage in abilityObject.GetComponents<DamageStatsHolder>())
            {
                damage.baseDamageStats.critChance += addedCritChance;
            }
        }

        if (chanceToAttachSparkCharge > 0)
        {
            ChanceToCreateAbilityObjectOnNewEnemyHit newComponent = abilityObject.AddComponent<ChanceToCreateAbilityObjectOnNewEnemyHit>();
            newComponent.spawnAtHit = true;
            newComponent.chance = chanceToAttachSparkCharge;
            newComponent.abilityToInstantiate = AbilityIDList.getAbility(AbilityID.sparkCharge);
        }

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


        if (moreDamageAgainstTimeRotting != 0)
        {
            // create the conditional
            DamageConditionalEffect conditionalEffect = new DamageConditionalEffect();
            HasStatusEffectConditional conditional = new HasStatusEffectConditional();
            conditional.statusEffect = StatusEffectID.TimeRot;
            conditionalEffect.conditional = conditional;
            conditionalEffect.effect = new DamageEffectMoreDamage(moreDamageAgainstTimeRotting);
            // add the conditional to all damage stats holders
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.baseDamageStats.conditionalEffects.Add(conditionalEffect);
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

        return abilityObject;
    }
}
