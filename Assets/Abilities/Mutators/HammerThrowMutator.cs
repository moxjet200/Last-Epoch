using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerThrowMutator : AbilityMutator
{

    public int extraProjectiles = 0;
    public float armourShredChance = 0f;

    public bool noPierce = false;
    public float increasedAttackSpeed = 0f;
    public float chanceForDoubleDamage = 0f;
    public float increasedDamage = 0f;
    public float moreDamageAgainstStunned = 0f;

    public bool freeWhenOutOfMana = false;
    public bool centreOnCaster = false;
    public bool spiralMovement = false;

    public float healthGainOnHit = 0f;
    public float manaGainOnHit = 0f;

    public bool aoeVoidDamage = false;
    public float increasedAoEBaseDamage = 0f;

    public float increasedProjectileSpeed = 0f;
    public bool projectileNova = false;

    public int chains = 0;

    public float increasedStunChance = 0f;
    public float moreDamage = 0f;
    public bool noReturn = false;
    public bool canDamageSameEnemyAgain = false;
    public bool hasChained = false;

    BaseMana mana = null;
    BaseHealth health = null;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.hammerThrow);
        base.Awake();
        mana = GetComponent<BaseMana>();
        health = GetComponent<BaseHealth>();
        AbilityEventListener ael = AbilityEventListener.GetOrAdd(gameObject);
        ael.onHitEvent += OnHit;
    }
    

    public void OnHit(Ability _ability, GameObject target)
    {
        if (_ability && _ability == ability)
        {
            if (health && healthGainOnHit != 0)
            {
                health.Heal(healthGainOnHit);
            }
            if (mana && manaGainOnHit != 0)
            {
                mana.currentMana += manaGainOnHit;
                if (mana.currentMana > mana.maxMana) { mana.currentMana = mana.maxMana; }
            }
        }
    }

    public override float mutateUseSpeed(float useSpeed)
    {
        if (freeWhenOutOfMana && mana && mana.currentMana <= 0)
        {
            return base.mutateUseSpeed(useSpeed) * (1 + increasedAttackSpeed) * 0.75f;
        }
        return base.mutateUseSpeed(useSpeed) * (1 + increasedAttackSpeed);
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
        if (extraProjectiles > 0 && !spiralMovement)
        {
            foreach (DamageEnemyOnHit component in abilityObject.GetComponents<DamageEnemyOnHit>())
            {
                component.canDamageSameEnemyAgain = false;
            }
        }

        if (chains > 0)
        {
            ChainOnHit chain = abilityObject.AddComponent<ChainOnHit>();
            chain.chainsRemaining = chains;
            chain.abilityToChain = ability;
            chain.range = 8f;
            chain.destroyAfterChainAttempt = true;
            chain.cannotHitSame = true;
            chain.offset = new Vector3(0f, 1.2f, 0f);


        }

        if (chains > 0 || hasChained)
        {
            // add a copy of this mutator to the ability object, but remove the chains (because it will chain anyway), the increased damage to first enemy hit, and the on cast stuff
            HammerThrowMutator newMutator = Comp<HammerThrowMutator>.GetOrAdd(abilityObject);
            newMutator.chains = 0;
            newMutator.increasedDamage = increasedDamage;
            newMutator.extraProjectiles = 0;
            newMutator.armourShredChance = 0f;

            newMutator.noPierce = noPierce;
            newMutator.chanceForDoubleDamage = chanceForDoubleDamage;
            newMutator.increasedDamage = increasedDamage;
            newMutator.moreDamageAgainstStunned = moreDamageAgainstStunned;

            newMutator.spiralMovement = false;

            newMutator.aoeVoidDamage = aoeVoidDamage;
            newMutator.increasedAoEBaseDamage = increasedAoEBaseDamage;

            newMutator.increasedProjectileSpeed = increasedProjectileSpeed;
            newMutator.increasedStunChance = increasedStunChance;
            newMutator.moreDamage = moreDamage;
            newMutator.noReturn = true;
            newMutator.canDamageSameEnemyAgain = false;
            newMutator.hasChained = true;
        }

        if (noReturn)
        {
            DestroyAfterDuration dad = abilityObject.GetComponent<DestroyAfterDuration>();
            ReturnToCasterAfterDuration component = abilityObject.GetComponent<ReturnToCasterAfterDuration>();
            component.duration = 10000f;
            if (spiralMovement) { dad.duration = 6f; }
            else { dad.duration = 2.5f; }
            if (!hasChained)
            {
                abilityObject.AddComponent<DestroyOnInanimateCollison>();
                ReturnOnInanimateCollision ret = abilityObject.GetComponent<ReturnOnInanimateCollision>();
                if (ret) { Destroy(ret); }
            }
        }

        // aoe void damage
        if (aoeVoidDamage)
        {
            foreach (Transform child in abilityObject.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == "aura")
                {
                    child.gameObject.SetActive(true);
                }
            }
            RepeatedlyDamageEnemiesWithinRadius repeatDamage = abilityObject.GetComponent<RepeatedlyDamageEnemiesWithinRadius>();
            if (repeatDamage == null) { repeatDamage = abilityObject.AddComponent<RepeatedlyDamageEnemiesWithinRadius>(); }
            if (repeatDamage.baseDamageStats.damage == null) { repeatDamage.baseDamageStats.damage = new List<DamageStatsHolder.DamageTypesAndValues>(); }
            repeatDamage.addBaseDamage(DamageType.VOID, 6f * (1 + increasedAoEBaseDamage));
            repeatDamage.damageInterval = 0.5f;
            repeatDamage.radius = 1.5f;
            repeatDamage.baseDamageStats.addedDamageScaling = 0.2f;
            repeatDamage.tags.Add(Tags.AbilityTags.AoE);
            repeatDamage.tags.Add(Tags.AbilityTags.Throwing);
            repeatDamage.tags.Add(Tags.AbilityTags.DoT);
        }

        // add extra projectiles
        if (extraProjectiles != 0)
        {
            ExtraProjectiles extraProjectilesObject = abilityObject.GetComponent<ExtraProjectiles>();
            if (extraProjectilesObject == null) { extraProjectilesObject = abilityObject.AddComponent<ExtraProjectiles>(); extraProjectilesObject.numberOfExtraProjectiles = 0; }
            extraProjectilesObject.numberOfExtraProjectiles += extraProjectiles;
            if (projectileNova) {
                extraProjectilesObject.angle = 144f;
                if (extraProjectiles >= 6) { extraProjectilesObject.angle = 160; }
            }
        }

        // remove pierce
        if (noPierce && chains <= 0 && !hasChained)
        {
            Pierce pierce = abilityObject.GetComponent<Pierce>();
            if (pierce == null) { pierce = abilityObject.AddComponent<Pierce>(); pierce.objectsToPierce = 0; }
            if (chains <= 0 && !hasChained)
            {
                abilityObject.AddComponent<DestroyOnFailingToPierceEnemy>();
            }
        }

        // add chance to shred armour
        if (armourShredChance > 0)
        {
            ChanceToApplyStatusOnEnemyHit chanceTo = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            chanceTo.chance = armourShredChance;
            chanceTo.statusEffect = StatusEffectList.getEffect(StatusEffectID.ArmourShred);
        }

        if (increasedStunChance != 0)
        {
            foreach (DamageStatsHolder damage in abilityObject.GetComponents<DamageStatsHolder>())
            {
                damage.baseDamageStats.increasedStunChance += increasedStunChance;
            }
        }

        if (increasedDamage != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(increasedDamage);
            }
        }

        if (moreDamage != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(moreDamage);
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

        if (moreDamageAgainstStunned != 0)
        {
            // create the conditional
            DamageConditionalEffect conditionalEffect = new DamageConditionalEffect();
            StunnedConditional conditional = new StunnedConditional();
            conditionalEffect.conditional = conditional;
            conditionalEffect.effect = new DamageEffectMoreDamage(moreDamageAgainstStunned);
            // add the conditional to all damage stats holders
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.baseDamageStats.conditionalEffects.Add(conditionalEffect);
            }
        }

        if (increasedProjectileSpeed != 0)
        {
            AbilityMover mover = abilityObject.GetComponent<AbilityMover>();
            mover.speed *= (1 + increasedProjectileSpeed);
        }

        if (spiralMovement)
        {
            SpiralMovement spira = abilityObject.AddComponent<SpiralMovement>();
            spira.constantVelocity = SpiralMovement.ConstantType.BothAreMaxima;
            spira.tangentialVelocity = 5.3f * (1 + increasedProjectileSpeed);
            spira.angleChangedPerSecond = 157f * (1 + increasedProjectileSpeed);
            spira.outwardSpeed = 1.15f;
            spira.outwardDistance = 0.6f;

            AbilityMover mover = abilityObject.GetComponent<AbilityMover>();
            mover.speed = 0;

            if (centreOnCaster)
            {
                spira.centreOnCaster = true;
                spira.offsetFromTransform = new Vector3(0f, 1.2f, 0f);
            }

            if (extraProjectiles > 0)
            {
                spira.randomStartAngle = true;
            }
        }

        

        return abilityObject;
    }
}
