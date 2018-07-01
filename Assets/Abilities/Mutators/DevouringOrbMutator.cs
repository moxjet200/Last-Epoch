using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevouringOrbMutator : AbilityMutator
{
    public bool dealsDamage = false;
    public float increasedDamage = 0f;
    public float movementSpeedOnCast = 0f;
    public bool orbitsCaster = false;
    public float increasedOrbitDistance = 0f;
    public float increasedDuration = 0f;
    public bool castsAbyssalOrb = false;
    public float increasedAbyssalOrbFrequency = 0f;

    public bool voidEruptionOnDeath = false;
    public float increasedCastSpeed = 0f;

    // void rift stats
    public float voidRift_increasedDamage = 0f;
    public float voidRift_increasedRadius = 0f;
    public float voidRift_timeRotChance = 0f;
    public float voidRift_increasesDamageTaken = 0f;
    public float voidRift_increasesDoTDamageTaken = 0f;
    public float voidRift_increasedStunChance = 0f;
    public float voidRift_moreDamageAgainstStunned = 0f;
    public float voidRift_igniteChance = 0f;
    public float voidRift_damageAgainstIgnited = 0f;
    public float voidRift_damageAgainstTimeRotting = 0f;
    public float voidRift_increasedDamageGrowth = 0f;
    public float voidRift_increasedAreaGrowth = 0f;
    public bool voidRift_noGrowth = false;

    AbilityObjectConstructor aoc = null;
    StatBuffs statBuffs = null;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.devouringOrb);
        statBuffs = Comp<StatBuffs>.GetOrAdd(gameObject);
        base.Awake();
    }

    public override float mutateUseSpeed(float useSpeed)
    {
        return base.mutateUseSpeed(useSpeed) * (1 + increasedCastSpeed);
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
        voidRiftMutator.igniteChance = voidRift_igniteChance;
        voidRiftMutator.moreDamageAgainstIgnited = voidRift_damageAgainstIgnited;
        voidRiftMutator.moreDamageAgainstTimeRotting = voidRift_damageAgainstTimeRotting;
        if (voidRift_noGrowth)
        {
            voidRiftMutator.areaGainOnNearbyDeath = 0f;
            voidRiftMutator.damageGainOnNearbyDeath = 0f;
        }
        else
        {
            voidRiftMutator.areaGainOnNearbyDeath *= (1 + voidRift_increasedAreaGrowth);
            voidRiftMutator.damageGainOnNearbyDeath *= (1 + voidRift_increasedDamageGrowth);
        }

        if (dealsDamage)
        {
            SphereCollider col = abilityObject.AddComponent<SphereCollider>();
            col.radius = 0.3f;
            col.isTrigger = true;
        }


        if (increasedDamage != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(increasedDamage);
            }
        }

        if (movementSpeedOnCast != 0)
        {
            TaggedStatsHolder.Stat stat = new TaggedStatsHolder.Stat(Tags.Properties.Movespeed);
            stat.increasedValue = movementSpeedOnCast;
            Buff buff = new Buff(stat, 4f);
            statBuffs.addBuff(buff);
        }
        
        if (orbitsCaster)
        {
            SpiralMovement movement = abilityObject.AddComponent<SpiralMovement>();
            movement.outwardDistance = 2f + increasedOrbitDistance;
            movement.angleChangedPerSecond = 180f;
            movement.constantVelocity = SpiralMovement.ConstantType.AngularVelocity;
            movement.centreOnCaster = true;
            movement.offsetFromTransform = new Vector3(0, 1, 0);
            movement.outwardSpeed = 0f;
            movement.randomStartAngle = true;
        }

        if (castsAbyssalOrb)
        {
            CastAtRandomPointAfterDuration component = abilityObject.AddComponent<CastAtRandomPointAfterDuration>();
            component.ability = Ability.getAbility(AbilityID.abyssalOrb);
            component.radius = 5f;
            component.limitCasts = false;
            component.duration = 3f / (1f + increasedAbyssalOrbFrequency);
        }

        if (voidEruptionOnDeath)
        {
            CreateAbilityObjectOnDeath component = abilityObject.AddComponent<CreateAbilityObjectOnDeath>();
            component.abilityToInstantiate = Ability.getAbility(AbilityID.voidEruption);
            component.aimingMethod = CreateAbilityObjectOnDeath.AimingMethod.Random;
        }

        if (increasedDuration != 0)
        {
            DestroyAfterDuration dad = abilityObject.GetComponent<DestroyAfterDuration>();
            if (dad)
            {
                dad.duration *= (1 + increasedDuration);
            }
        }


        return abilityObject;
    }
}
