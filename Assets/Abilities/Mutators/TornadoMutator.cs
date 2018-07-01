using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoMutator : AbilityMutator {

    public float additionalDuration = 0f;
    public float pullMultiplier = 1f;
    public float increasedBasePhysicalDamage = 0f;
    public bool castsLightning = false;
    public float lightningInterval = 2f;
    public float lightningRange = 0f;
    public bool fireTornado = false;
    public bool stationary = false;
    public bool attaches = false;
    public float doubleCastChance = 0f;

    public bool leavesStormOrbs = false;
    public float increasedStormOrbFrequency = 0f;
    public bool ignitesInAoe = false;
    public float increasedIgniteFrequency = 0f;
    public float increasedCastSpeed = 0f;

    public float movementSpeedOnCast = 0f;
    public float attackAndCastSpeedOnCast = 0f;
    public float manaRegenOnCast = 0f;
    public float increasedBuffDuration = 0f;

    StatBuffs statBuffs = null;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.tornado);
        statBuffs = Comp<StatBuffs>.GetOrAdd(gameObject);
        base.Awake();
    }

    public override int mutateDelayedCasts(int defaultCasts)
    {
        int extraCasts = 0;
        if (doubleCastChance > 0)
        {
            float rand = Random.Range(0f, 1f);
            if (rand < doubleCastChance)
            {
                extraCasts = 1;
            }
        }
        return base.mutateDelayedCasts(defaultCasts) + extraCasts;
    }

    public override float mutateUseSpeed(float useSpeed)
    {
        return useSpeed * (1 + increasedCastSpeed);
    }

    public override List<Tags.AbilityTags> getUseTags()
    {
        List<Tags.AbilityTags> list = base.getUseTags();
        if (fireTornado)
        {
            if (list.Contains(Tags.AbilityTags.Physical)) { list.Remove(Tags.AbilityTags.Physical); }
            list.Add(Tags.AbilityTags.Fire);
        }
        if (castsLightning)
        {
            list.Add(Tags.AbilityTags.Lightning);
        }
        return list;
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        if (additionalDuration > 0)
        {
            DestroyAfterDuration durationObject = abilityObject.GetComponent<DestroyAfterDuration>();
            if (durationObject != null) { durationObject.duration += additionalDuration; }
        }

        // increase the strength of the pull
        if (pullMultiplier != 1)
        {
            RepeatedlyPullEnemiesWithinRadius pull = abilityObject.GetComponent<RepeatedlyPullEnemiesWithinRadius>();
            if (pull) { pull.distanceMultiplier = 1 - (1 - pull.distanceMultiplier) * pullMultiplier; }
        }

        if (increasedBasePhysicalDamage > 0)
        {
            foreach (DamageStatsHolder damage in abilityObject.GetComponents<DamageStatsHolder>())
            {
                damage.addBaseDamage(DamageType.PHYSICAL, damage.getBaseDamage(DamageType.PHYSICAL) * increasedBasePhysicalDamage);
            }
        }

        if (fireTornado)
        {
            // change the vfx
            foreach (Transform t in abilityObject.transform)
            {
                if (t.name == "TornadoVFX")
                {
                    t.gameObject.SetActive(false);
                }
                if (t.name == "Tornado_Fire")
                {
                    t.gameObject.SetActive(true);
                }
            }
            // change the damage to fire
            foreach (DamageStatsHolder damage in abilityObject.GetComponents<DamageStatsHolder>())
            {
                damage.addBaseDamage(DamageType.FIRE, damage.getBaseDamage(DamageType.PHYSICAL));
                damage.addBaseDamage(DamageType.PHYSICAL, -damage.getBaseDamage(DamageType.PHYSICAL));
            }
        }

        if (castsLightning)
        {
            RepeatedlyCastAtNearestEnemyWithinRadius cast = abilityObject.AddComponent<RepeatedlyCastAtNearestEnemyWithinRadius>();
            cast.abilityToCast = AbilityIDList.getAbility(AbilityID.lesserLightning);
            cast.castInterval = lightningInterval;
            cast.radius = lightningRange;
        }

        if (stationary)
        {
            abilityObject.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = 0f;
            Destroy(abilityObject.GetComponent<RandomNavmeshMovement>());
        }

        if (attaches)
        {
            Destroy(abilityObject.GetComponent<UnityEngine.AI.NavMeshAgent>());
            Destroy(abilityObject.GetComponent<RandomNavmeshMovement>());
            AttachToCreatorOnCreation component = abilityObject.AddComponent<AttachToCreatorOnCreation>();
            component.replaceExistingBuff = false;
            component.displacement = new Vector3(0, 1f, 0);
        }

        if (leavesStormOrbs)
        {
            CastAtRandomPointAfterDuration cast = abilityObject.AddComponent<CastAtRandomPointAfterDuration>();
            cast.ability = Ability.getAbility(AbilityID.delayedStormOrb);
            cast.duration = 1f / (1 + increasedStormOrbFrequency);
            cast.radius = 0.5f;
            cast.limitCasts = false;
        }

        if (ignitesInAoe)
        {
            CastAfterDuration cad = abilityObject.AddComponent<CastAfterDuration>();
            cad.ability = Ability.getAbility(AbilityID.invisibleIgniteNova);
            cad.interval = 1f / (1 + increasedIgniteFrequency);
            cad.limitCasts = false;
        }

        if (movementSpeedOnCast != 0 || manaRegenOnCast != 0 || attackAndCastSpeedOnCast != 0)
        {
            float buffDuration = 2f * (1 + increasedBuffDuration);
            if (movementSpeedOnCast != 0)
            {
                statBuffs.addBuff(buffDuration, Tags.Properties.Movespeed, 0, movementSpeedOnCast, null, null, null, "tornadoMovementSpeed");
            }
            if (manaRegenOnCast != 0)
            {
                statBuffs.addBuff(buffDuration, Tags.Properties.ManaRegen, 0, manaRegenOnCast, null, null, null, "tornadoManaRegen");
            }
            if (attackAndCastSpeedOnCast != 0)
            {
                statBuffs.addBuff(buffDuration, Tags.Properties.AttackSpeed, 0, attackAndCastSpeedOnCast, null, null, null, "tornadoAttackSpeed");
                statBuffs.addBuff(buffDuration, Tags.Properties.CastSpeed, 0, attackAndCastSpeedOnCast, null, null, null, "tornadoCastSpeed");
            }
        }

        return abilityObject;
    }
}
