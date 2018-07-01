using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballMutator : AbilityMutator {

    public int extraProjectiles = 0;

    public bool addExplosion = false;

    [Range(0f,1f)]
    public float chanceToCreateExplosionOnHit = 0f;

    [Range(0f, 1f)]
    public float igniteChance = 0f;

    public int targetsToPierce = 0;
    
    public bool reduceBaseDamageBy80Percent = false;
    public float increasedCastSpeed = 0f;
    public bool fireInSequence = false;

    public float chanceForDoubleDamage = 0f;

    public float fireAddedAsLightning = 0f;
    public float increasedDamage = 0f;

    public float moreDamageAgainstIgnited = 0f;
    public float moreDamageAgainstChilled = 0f;

    public bool homing = false;

    public bool freeWhenOutOfMana = false;
    public bool alwaysFree = false;

    public float increasedSpeed = 0f;
    public float increasedDuration = 0f;

    public bool inaccuracy = false;
    public bool channelled = false;

    // true on the channelled fireball object
    public bool channelledFireballObject = false;

    BaseMana mana = null;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.fireball);
        base.Awake();
        mana = GetComponent<BaseMana>();
    }

    public override float mutateUseSpeed(float useSpeed)
    {
        return base.mutateUseSpeed(useSpeed) * (1 + increasedCastSpeed);
    }

    public override int mutateDelayedCasts(int defaultCasts)
    {
        if (extraProjectiles > 0 && fireInSequence && !channelled) {
            return base.mutateDelayedCasts(defaultCasts) + extraProjectiles;
        }
        else
        {
            return base.mutateDelayedCasts(defaultCasts);
        }
    }

    public override float getIncreasedManaCost()
    {
        if (alwaysFree)
        {
            return -100f;
        }
        if (freeWhenOutOfMana && mana && mana.currentMana <= 0)
        {
            return -100f;
        }
        else
        {
            return increasedManaCost;
        }
    }

    public override AbilityAnimation getAbilityAnimation()
    {
        if (channelled) { return AbilityAnimation.Channel; }
        return base.getAbilityAnimation();
    }

    public override bool isChanneled()
    {
        return channelled;
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        // return a channelled fireball object if this is channelled
        if (channelled)
        {
            Destroy(abilityObject);
            abilityObject = Instantiate(Ability.getAbility(AbilityID.channelledFireball).abilityPrefab, location, Quaternion.Euler(targetLocation - location));

            // change the mutator on the new abilityObject
            FireballMutator mut = Comp<FireballMutator>.GetOrAdd(abilityObject);
            mut.extraProjectiles = extraProjectiles;
            mut.addExplosion = addExplosion;
            mut.chanceToCreateExplosionOnHit = chanceToCreateExplosionOnHit;
            mut.igniteChance = igniteChance;
            mut.targetsToPierce = targetsToPierce;
            mut.reduceBaseDamageBy80Percent = reduceBaseDamageBy80Percent;
            mut.increasedCastSpeed = increasedCastSpeed;
            mut.chanceForDoubleDamage = chanceForDoubleDamage;
            mut.fireAddedAsLightning = fireAddedAsLightning;
            mut.increasedDamage = increasedDamage;
            mut.moreDamageAgainstIgnited = moreDamageAgainstIgnited;
            mut.moreDamageAgainstChilled = moreDamageAgainstChilled;
            mut.homing = homing;
            mut.freeWhenOutOfMana = freeWhenOutOfMana;
            mut.alwaysFree = alwaysFree;
            mut.increasedSpeed = increasedSpeed;
            mut.increasedDuration = increasedDuration;
            mut.inaccuracy = inaccuracy;

            // some are always false
            mut.channelled = false;
            mut.fireInSequence = false;

            // this will be the channelled fireball object
            mut.channelledFireballObject = true;

            // return the new ability object and do not change it further
            return abilityObject;
        }

        // disable shared hit detector for channelling
        if (channelledFireballObject)
        {
            foreach(HitDetector detector in abilityObject.GetComponents<HitDetector>())
            {
                detector.cannotHaveSharedhitDetector = true;
            }
        }

        // add an explosion
        if (addExplosion)
        {
            abilityObject.AddComponent<CreateAbilityObjectOnDeath>().abilityToInstantiate = AbilityIDList.getAbility(AbilityID.fireballAoe);
        }

        // increase speed
        if (increasedSpeed != 0)
        {
            abilityObject.GetComponent<AbilityMover>().speed *= (1 + increasedSpeed);
        }

        // increase duration
        if (increasedDuration != 0)
        {
            abilityObject.GetComponent<DestroyAfterDuration>().duration *= (1 + increasedDuration);
        }

        // slightly randomise target location
        if (inaccuracy)
        {
            targetLocation += new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * 0.25f * Vector3.Distance(location, targetLocation);
        }

        // add extra projectiles
        if (extraProjectiles != 0 && !fireInSequence)
        {
            ExtraProjectiles extraProjectilesObject = abilityObject.GetComponent<ExtraProjectiles>();
            if (extraProjectilesObject == null) { extraProjectilesObject = abilityObject.AddComponent<ExtraProjectiles>(); extraProjectilesObject.numberOfExtraProjectiles = 0; }
            extraProjectilesObject.numberOfExtraProjectiles += extraProjectiles;
        }

        // add pierce change
        if (targetsToPierce > 0)
        {
            Pierce pierce = abilityObject.GetComponent<Pierce>();
            if (pierce == null) { pierce = abilityObject.AddComponent<Pierce>(); pierce.objectsToPierce = 0; }
            pierce.objectsToPierce += targetsToPierce;
        }

        // add chance to create explosion on hit
        if (chanceToCreateExplosionOnHit > 0)
        {
            ChanceToCreateAbilityObjectOnNewEnemyHit ctcaooneh = abilityObject.AddComponent<ChanceToCreateAbilityObjectOnNewEnemyHit>();
            ctcaooneh.chance = chanceToCreateExplosionOnHit;
            ctcaooneh.spawnAtHit = true;
            ctcaooneh.abilityToInstantiate = AbilityIDList.getAbility(AbilityID.fireballAoe);
        }

        // add chance to ignite
        if (igniteChance > 0)
        {
            ChanceToApplyStatusOnEnemyHit chanceTo = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            chanceTo.chance = igniteChance;
            chanceTo.statusEffect = StatusEffectList.getEffect(StatusEffectID.Ignite);
        }

        if (increasedDamage != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(increasedDamage);
                //holder.addBaseDamage(DamageType.FIRE, holder.getBaseDamage(DamageType.FIRE) * increasedDamage);
            }
        }

        // reduce hit damage
        if (reduceBaseDamageBy80Percent)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                for (int i = 0; i< holder.baseDamageStats.damage.Count; i++)
                {
                    holder.baseDamageStats.damage[i] = new DamageStatsHolder.DamageTypesAndValues(holder.baseDamageStats.damage[i].damageType, holder.baseDamageStats.damage[i].value * 0.2f);
                }
            }
        }

        if (chanceForDoubleDamage > 0)
        {
            float rand = Random.Range(0f, 1f);
            if (rand < chanceForDoubleDamage)
            {
                foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
                {
                    for (int i = 0; i < holder.baseDamageStats.damage.Count; i++)
                    {
                        holder.addBaseDamage(holder.baseDamageStats.damage[i].damageType, holder.getBaseDamage(holder.baseDamageStats.damage[i].damageType));
                    }
                }
            }
        }

        if (fireAddedAsLightning > 0)
        {
            abilityObject.GetComponentInChildren<DisableOnStart>().active = false;
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.addBaseDamage(DamageType.LIGHTNING, holder.getBaseDamage(DamageType.FIRE) * fireAddedAsLightning);
            }
        }

        if (moreDamageAgainstIgnited != 0)
        {
            // create the conditional
            DamageConditionalEffect conditionalEffect = new DamageConditionalEffect();
            HasStatusEffectConditional conditional = new HasStatusEffectConditional();
            conditional.statusEffect = StatusEffectID.Ignite;
            conditionalEffect.conditional = conditional;
            conditionalEffect.effect = new DamageEffectMoreDamage(moreDamageAgainstIgnited);
            // add the conditional to all damage stats holders
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.baseDamageStats.conditionalEffects.Add(conditionalEffect);
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

        if (homing)
        {
            abilityObject.AddComponent<HomingMovement>();
        }


        return abilityObject;
    }
}
