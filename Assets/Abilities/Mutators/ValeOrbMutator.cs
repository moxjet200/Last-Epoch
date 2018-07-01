using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValeOrbMutator : AbilityMutator
{
    // mostly copied from fireball
    public int extraProjectiles = 0;

    public bool addExplosion = false;

    [Range(0f, 1f)]
    public float chanceToCreateExplosionOnHit = 0f;

    [Range(0f, 1f)]
    public float igniteChance = 0f;

    public int targetsToPierce = 0;

    public bool additionalIgnite = false;
    public bool reduceBaseDamageBy80Percent = false;
    public float increasedCastSpeed = 0f;
    public bool fireInSequence = false;

    public float chanceForDoubleDamage = 0f;
    
    public float increasedDamage = 0f;

    public float moreDamageAgainstPoisoned = 0f;
    public float moreDamageAgainstBleeding = 0f;

    public bool homing = false;

    public bool freeWhenOutOfMana = false;

    BaseMana mana = null;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.valeOrb);
        base.Awake();
        mana = GetComponent<BaseMana>();
    }

    public override float mutateUseSpeed(float useSpeed)
    {
        return base.mutateUseSpeed(useSpeed) * (1 + increasedCastSpeed);
    }

    public override int mutateDelayedCasts(int defaultCasts)
    {
        if (extraProjectiles > 0 && fireInSequence)
        {
            return base.mutateDelayedCasts(defaultCasts) + extraProjectiles;
        }
        else
        {
            return base.mutateDelayedCasts(defaultCasts);
        }
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

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        // add an explosion
        if (addExplosion)
        {
            abilityObject.AddComponent<CreateAbilityObjectOnDeath>().abilityToInstantiate = AbilityIDList.getAbility(AbilityID.fireballAoe);
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
        if (additionalIgnite)
        {
            ApplyStatusOnEnemyHit component = abilityObject.AddComponent<ApplyStatusOnEnemyHit>();
            component.statusEffect = StatusEffectList.getEffect(StatusEffectID.Ignite);
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
                for (int i = 0; i < holder.baseDamageStats.damage.Count; i++)
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

        if (moreDamageAgainstPoisoned != 0)
        {
            // create the conditional
            DamageConditionalEffect conditionalEffect = new DamageConditionalEffect();
            HasStatusEffectConditional conditional = new HasStatusEffectConditional();
            conditional.statusEffect = StatusEffectID.Poison;
            conditionalEffect.conditional = conditional;
            conditionalEffect.effect = new DamageEffectMoreDamage(moreDamageAgainstPoisoned);
            // add the conditional to all damage stats holders
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.baseDamageStats.conditionalEffects.Add(conditionalEffect);
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

        if (homing)
        {
            abilityObject.AddComponent<HomingMovement>();
        }


        return abilityObject;
    }
}
