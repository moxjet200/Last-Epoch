using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldRushEndMutator : AbilityMutator
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
    
    public float delayIncreasedDamage = 0f;
    public float delayIncreasedRadius = 0f;
    public float delayTimeRotChance = 0f;
    public float delayIncreasesDamageTaken = 0f;
    public float delayIncreasesDoTDamageTaken = 0f;
    public float delayIncreasedStunChance = 0f;
    public float delayAddedCritMultiplier = 0f;
    public float delayAddedCritChance = 0f;

    public float addedVoidDamage = 0f;

    AbilityObjectConstructor aoc = null;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.shieldRushEnd);
        base.Awake();
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

        if (timeRotChance > 0)
        {
            ChanceToApplyStatusOnEnemyHit newComponent = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            newComponent.statusEffect = StatusEffectList.getEffect(StatusEffectID.TimeRot);
            newComponent.chance = timeRotChance;
        }

        if (increasesDamageTaken > 0 || increasesDoTDamageTaken > 0)
        {
            DebuffOnEnemyHit doeh = abilityObject.AddComponent<DebuffOnEnemyHit>();
            if (increasesDamageTaken > 0)
            {
                doeh.addDebuffToList(Tags.Properties.DamageTaken, 0f, -increasesDamageTaken, new List<float>(), new List<float>(), 4f);
            }
            if (increasesDoTDamageTaken > 0)
            {
                List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                tagList.Add(Tags.AbilityTags.DoT);
                doeh.addDebuffToList(Tags.Properties.DamageTaken, 0f, -increasesDoTDamageTaken, new List<float>(), new List<float>(), 4f, tagList);
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

        if (addedCritMultiplier != 0)
        {
            foreach (DamageStatsHolder damage in abilityObject.GetComponents<DamageStatsHolder>())
            {
                damage.baseDamageStats.critMultiplier += addedCritMultiplier;
            }
        }

        if (leaveDelayed)
        {
            CreateAbilityObjectOnDeath component = abilityObject.AddComponent<CreateAbilityObjectOnDeath>();
            component.abilityToInstantiate = Ability.getAbility(AbilityID.delayedShieldRushEnd);

            DelayedShieldRushEndMutator mutator = abilityObject.AddComponent<DelayedShieldRushEndMutator>();
            mutator.increasedDamage = increasedDamage + delayIncreasedDamage;
            mutator.increasedRadius = increasedRadius + delayIncreasedRadius;
            mutator.timeRotChance = timeRotChance + delayTimeRotChance;
            mutator.increasesDamageTaken = increasesDamageTaken + delayIncreasesDamageTaken;
            mutator.increasesDoTDamageTaken = increasesDoTDamageTaken + delayIncreasesDoTDamageTaken;
            mutator.increasedStunChance = increasedStunChance + delayIncreasedStunChance;
            mutator.addedCritMultiplier = addedCritMultiplier + delayAddedCritMultiplier;
            mutator.addedCritChance = addedCritChance + delayAddedCritChance;
            mutator.leaveDelayed = false;
            mutator.increasedDelayLength = increasedDelayLength;
            mutator.addedVoidDamage = addedVoidDamage;
        }

        return abilityObject;
    }
}
