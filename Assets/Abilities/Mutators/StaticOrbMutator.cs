using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticOrbMutator : AbilityMutator
{
    public float doubleCastChance = 0f;
    public float increasedExplosionDamage = 0f;
    public float explosionChanceToShock = 0f;
    public bool explodesAtTarget = false;
    public float explosionWardGainedOnKill = 0f;
    public float explosionWardGainedOnKillChance = 0f;
    public float manaOnHitChance = 0f;
    public float manaOnHit = 0f;
    public float chanceToAttachSparkCharge = 0f;
    public float increasedProjectileDamage = 0f;
    public bool removeExplosion = false;
    public bool freeWhenOutOfMana = false;
    public bool removePull = false;
    public float increasedSpeed = 0f;
    public float lightningAegisChance = 0f;
    public float knockBackOnCastChance = 0f;
    public float chargedGroundAtEndChance = 0f;
    public float shockChance = 0f;

    ProtectionClass protectionClass = null;
    BaseMana mana = null;
    UsingAbility usingAbility = null;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.staticOrb);
        base.Awake();
        AbilityEventListener eventListener = GetComponent<AbilityEventListener>();
        if (eventListener)
        {
            eventListener.onKillEvent += OnKill;
            eventListener.onHitEvent += OnHit;
        }
        protectionClass = GetComponent<ProtectionClass>();
        mana = GetComponent<BaseMana>();
        usingAbility = GetComponent<UsingAbility>();
    }

    public override float getIncreasedManaCost()
    {
        if (freeWhenOutOfMana && mana && mana.currentMana <= 0)
        {
            return -100f;
        }
        else
        {
            return increasedManaCost;
        }
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

    public void OnKill(Ability _ability, GameObject target)
    {
        if (protectionClass && explosionWardGainedOnKillChance > 0 && _ability == AbilityIDList.getAbility(AbilityID.lightningExplosion))
        {
            float rand = Random.Range(0f, 1f);
            if (rand < explosionWardGainedOnKillChance)
            {
                protectionClass.currentWard += explosionWardGainedOnKill;
            }
        }
    }

    public void OnHit(Ability _ability, GameObject target)
    {
        if (mana && manaOnHitChance > 0 && _ability == ability)
        {
            float rand = Random.Range(0f, 1f);
            if (rand < manaOnHitChance)
            {
                mana.gainMana(manaOnHit);
            }
        }
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        // check if the explosion needs to be removed
        if (removeExplosion)
        {
            CreateAbilityObjectOnDeath[] components = abilityObject.GetComponents<CreateAbilityObjectOnDeath>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].abilityToInstantiate == AbilityIDList.getAbility(AbilityID.lightningExplosion))
                {
                    Destroy(components[i]);
                    components[i].deactivate();
                }
            }
        }
        // explosion only stuff
        else
        {
            if (increasedExplosionDamage != 0 || explosionChanceToShock != 0)
            {
                LightningExplosionMutator mut = abilityObject.AddComponent<LightningExplosionMutator>();
                mut.increasedDamage = increasedExplosionDamage;
                mut.chanceToShock = explosionChanceToShock;
            }

            if (explodesAtTarget)
            {
                abilityObject.GetComponent<DestroyAfterDuration>().duration = 1.5f;
                if (!abilityObject.GetComponent<LocationDetector>()) { abilityObject.AddComponent<LocationDetector>(); }
                DestroyAfterDurationAfterReachingTargetLocation component = abilityObject.AddComponent<DestroyAfterDurationAfterReachingTargetLocation>();
                component.duration = 0f;
            }

            if (chargedGroundAtEndChance > 0)
            {
                float rand = Random.Range(0f, 1f);
                if (rand < chargedGroundAtEndChance)
                {
                    CreateAbilityObjectOnDeath component = abilityObject.AddComponent<CreateAbilityObjectOnDeath>();
                    component.abilityToInstantiate = AbilityIDList.getAbility(AbilityID.chargedGround);
                }
            }
        }

        if (chanceToAttachSparkCharge > 0)
        {
            ChanceToCreateAbilityObjectOnNewEnemyHit newComponent = abilityObject.AddComponent<ChanceToCreateAbilityObjectOnNewEnemyHit>();
            newComponent.spawnAtHit = true;
            newComponent.chance = chanceToAttachSparkCharge;
            newComponent.abilityToInstantiate = AbilityIDList.getAbility(AbilityID.sparkCharge);
        }

        if (increasedProjectileDamage != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(increasedProjectileDamage);
            }
        }

        if (removePull)
        {
            PullComponent[] pullComponents = abilityObject.GetComponents<PullComponent>();
            for (int i = 0; i < pullComponents.Length; i++)
            {
                Destroy(pullComponents[i]);
            }
        }

        if (shockChance > 0)
        {
            ChanceToApplyStatusOnEnemyHit chanceTo = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            chanceTo.chance = shockChance;
            chanceTo.statusEffect = StatusEffectList.getEffect(StatusEffectID.Shock);
        }

        if (increasedSpeed > 0)
        {
            AbilityMover mover = abilityObject.GetComponent<AbilityMover>();
            if (mover) { mover.speed *= (1 + increasedSpeed); }
        }

        if (!usingAbility) { usingAbility = GetComponent<UsingAbility>(); }

        // casting stuff
        if (lightningAegisChance > 0 && usingAbility)
        {
            if (lightningAegisChance >= Random.Range(0f, 1f))
            {
                usingAbility.UseAbility(AbilityIDList.getAbility(AbilityID.lightningAegis), transform.position, false, false);
            }
        }
            
        if (knockBackOnCastChance > 0 && usingAbility)
        {
            if (knockBackOnCastChance >= Random.Range(0f, 1f))
            {
                usingAbility.UseAbility(AbilityIDList.getAbility(AbilityID.knockBack), transform.position, false, false);
            }
        }


        return abilityObject;
    }


}
