using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldRushMutator : AbilityMutator
{

    public List<TaggedStatsHolder.TaggableStat> statsWhileTravelling = new List<TaggedStatsHolder.TaggableStat>();

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

    public float increasedTravelDamage = 0f;
    public float increasedTravelStunChance = 0f;

    public bool forwardVoidBeam = false;
    public bool backwardsVoidBeam = false;

    public bool noShieldRequirement = false;
    public bool returnToStart = false;
    public bool restoreMana = false;

    public float percentCurrentHealthLostOnCast = 0f;

    public float addedVoidDamage = 0f;
    public float additionalAoEAddedVoidDamage = 0f;
    public float addedVoidReducedByAttackSpeed = 0f;

    public float moreTravelDamageAgainstFullHealth = 0f;

    AbilityObjectConstructor aoc = null;
    BaseHealth health = null;
    BaseStats baseStats = null;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.shieldRush);
        health = GetComponent<BaseHealth>();
        baseStats = GetComponent<BaseStats>();
        base.Awake();
    }

    public override bool mutateRequiresShield()
    {
        if (noShieldRequirement) { return false; }
        return base.mutateRequiresShield();
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        float newAddedVoidDamage = addedVoidDamage;

        if (addedVoidReducedByAttackSpeed != 0)
        {
            float voidDamageToAdd = addedVoidReducedByAttackSpeed;
            if (baseStats)
            {
                List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                tagList.Add(Tags.AbilityTags.Melee);
                float increasedAttackSpeed = baseStats.GetStatValue(Tags.Properties.AttackSpeed, tagList) - 1;
                voidDamageToAdd *= (1 - increasedAttackSpeed);
            }
            if (voidDamageToAdd > 0) { newAddedVoidDamage += voidDamageToAdd; }
        }


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
        mutator.increasedDelayLength = increasedDelayLength;

        mutator.delayIncreasedDamage = delayIncreasedDamage;
        mutator.delayIncreasedRadius = delayIncreasedRadius;
        mutator.delayTimeRotChance = delayTimeRotChance;
        mutator.delayIncreasesDamageTaken = delayIncreasesDamageTaken;
        mutator.delayIncreasesDoTDamageTaken = delayIncreasesDoTDamageTaken;
        mutator.delayIncreasedStunChance = delayIncreasedStunChance;
        mutator.delayAddedCritMultiplier = delayAddedCritMultiplier;
        mutator.delayAddedCritChance = delayAddedCritChance;

        mutator.addedVoidDamage = newAddedVoidDamage + additionalAoEAddedVoidDamage;

        if (newAddedVoidDamage > 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.addBaseDamage(DamageType.VOID, addedVoidDamage);
            }
        }

        if (moreTravelDamageAgainstFullHealth != 0)
        {
            // create the conditional
            DamageConditionalEffect conditionalEffect = new DamageConditionalEffect();
            conditionalEffect.conditional = new FullHealthConditional();
            conditionalEffect.effect = new DamageEffectMoreDamage(moreTravelDamageAgainstFullHealth);
            // add the conditional to all damage stats holders
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.baseDamageStats.conditionalEffects.Add(conditionalEffect);
            }
        }

        if (forwardVoidBeam)
        {
            CreateAbilityObjectOnStart component = abilityObject.AddComponent<CreateAbilityObjectOnStart>();
            component.abilityToInstantiate = Ability.getAbility(AbilityID.forwardVoidBeam);
            component.aimingMethod = CreateAbilityObjectOnStart.AimingMethod.TargetDirection;
            component.createAtStartLocation = true;
        }

        if (backwardsVoidBeam)
        {
            CreateAbilityObjectOnDeath component = abilityObject.AddComponent<CreateAbilityObjectOnDeath>();
            component.abilityToInstantiate = Ability.getAbility(AbilityID.backwardsVoidBeam);
            component.aimingMethod = CreateAbilityObjectOnDeath.AimingMethod.TravelDirection;
        }

        if (returnToStart)
        {
            ReturnCasterToOlderPosition component = abilityObject.AddComponent<ReturnCasterToOlderPosition>();
            component.increasePositionAgeWithAge = true;
            component.positionAge = 0f;
            component.restoreMana = restoreMana;
            component.additionalAgeForManaRestoration = 0.35f;
            component.whenToMoveCaster = ReturnCasterToOlderPosition.StartOrEnd.End;
        }

        if (percentCurrentHealthLostOnCast != 0 && health)
        {
            health.HealthDamage(health.currentHealth * percentCurrentHealthLostOnCast);
        }

        if (increasedTravelDamage != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(increasedTravelDamage);
            }
        }

        if (increasedTravelStunChance != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.baseDamageStats.increasedStunChance += increasedStunChance;
            }
        }

        if (statsWhileTravelling != null && statsWhileTravelling.Count > 0)
        {
            BuffParent bp = abilityObject.GetComponent<BuffParent>();
            if (!bp) { bp = abilityObject.AddComponent<BuffParent>(); }

            List<TaggedStatsHolder.TaggableStat> stats = new List<TaggedStatsHolder.TaggableStat>();

            foreach (TaggedStatsHolder.TaggableStat stat in statsWhileTravelling)
            {
                TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(stat);
                stats.Add(newStat);
            }

            bp.taggedStats.AddRange(stats);
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
