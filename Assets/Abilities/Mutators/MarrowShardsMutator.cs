using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarrowShardsMutator : AbilityMutator
{
    public float nova_increasedSpeed = 0f;
    public float nova_increasedDamage = 0f;
    public float nova_increasedStunChance = 0f;
    public float nova_bleedChance = 0f;
    public float nova_addedCritChance = 0f;
    public float nova_addedCritMultiplier = 0f;
    public float nova_moreDamageAgainstBleeding = 0f;

    public float doubleCastChance = 0f;
    public float physLeechOnCast = 0f;
    public float increasedDuration = 0f;

    public float addedCritChance = 0f;
    public float addedCritMultiplier = 0f;
    public float percentCurrentHealthLostOnCrit = 0f;
    public float critChanceOnCast = 0f;

    public bool createsSplintersAtEnd = false;
    public bool endsAtTargetPoint = false;
    public float chanceToShredArmour = 0f;
    public float chanceToBleed = 0f;

    public bool damagesMinions = false;
    public bool doesntPierce = false;

    public float healthGainedOnMinionKill = 0f;
    public float physLeechOnKill = 0f;
    public float increasedHealthCost = 0f;

    public float moreDamageAgainstBleeding = 0f;
    public float increasedDamage = 0f;
    public float increasedStunChance = 0f;
    public float increasedDamagePerMinion = 0f;
    public float increasedDamageFromMinionDrain = 0f;

    public float returnHealthChance = 0f;
    public float healthReturned = 0f;
    public float manaReturnChance = 0f;
    public float manaReturned = 0f;

    public float increasedCastSpeed = 0f;
    public float increasedDamageWithOneMinion = 0f;
    public float moreDamage = 0f;
    public float critChanceOnKill = 0f;

    GameObject spiritEscapePrefab = null;
    Vector3 spiritEscapeOffset = Vector3.zero;

    Vector3 boneNovaOffset = Vector3.zero;

    StatBuffs statBuffs = null;
    SummonTracker tracker = null;
    AbilityObjectConstructor aoc = null;
    BaseMana mana = null;
    bool recast = false;
    bool waitedForRecast = false;
    Vector3 recastPoint = Vector3.zero;
    BaseStats baseStats = null;
    BaseHealth health = null;

    List<Tags.AbilityTags> physTag = new List<Tags.AbilityTags>();

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.marrowShards);
        physTag.Add(Tags.AbilityTags.Physical);
        AbilityEventListener listener = Comp<AbilityEventListener>.GetOrAdd(gameObject);
        listener.onKillEvent += OnKill;
        listener.onCritEvent += OnCrit;
        health = GetComponent<BaseHealth>();
        if (health)
        {
            statBuffs = Comp<StatBuffs>.GetOrAdd(gameObject);
            tracker = Comp<SummonTracker>.GetOrAdd(gameObject);
        }
        aoc = Comp<AbilityObjectConstructor>.GetOrAdd(gameObject);
        mana = GetComponent<BaseMana>();
        baseStats = GetComponent<BaseStats>();
        boneNovaOffset = new Vector3(0f, 1.2f, 0f);
        spiritEscapeOffset = new Vector3(0f, 1.1f, 0f);
        spiritEscapePrefab = PrefabList.getPrefab("spiritEscape");
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


    public void OnKill(Ability _ability, GameObject target)
    {
        // health on minion kill
        CreationReferences refs = target.GetComponent<CreationReferences>();
        if (refs && refs.creator == gameObject && health)
        {
            health.Heal(healthGainedOnMinionKill);
        }

        // leech on kill
        if (statBuffs)
        {
            statBuffs.addBuff(4f, Tags.Properties.PercentLifeLeech, 0, physLeechOnKill, null, null, physTag, "marrow shards phys leech on kill buff");
            statBuffs.addBuff(4f, Tags.Properties.CriticalChance, 0, critChanceOnKill, null, null, physTag, "marrow shards crit chance on kill buff");
        }
    }

    public void OnCrit(Ability _ability, GameObject target)
    {
        if (percentCurrentHealthLostOnCrit != 0 && health)
        {
            health.HealthDamage(health.currentHealth * percentCurrentHealthLostOnCrit);
        }
    }

    public override float mutateUseSpeed(float useSpeed)
    {
        return base.mutateUseSpeed(useSpeed) * (1f + increasedCastSpeed);
    }

    public override void Update()
    {
        base.Update();
        if (recast)
        {
            if (waitedForRecast)
            {
                aoc.constructAbilityObject(ability, transform.position, recastPoint);
                recast = false;
                waitedForRecast = false;
            }
            else
            {
                waitedForRecast = true;
            }
        }
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {

        // Leech on cast buff
        if (statBuffs)
        {
            statBuffs.addBuff(4f, Tags.Properties.PercentLifeLeech, 0, physLeechOnCast, null, null, physTag, "marrow shards phys leech on cast buff");
            if (tracker && tracker.numberOfMinions() == 2)
            {
                statBuffs.addBuff(2f, Tags.Properties.CriticalChance, 0, critChanceOnCast, null, null, physTag, "marrow shards phys leech on cast buff");
            }
        }

        if (createsSplintersAtEnd)
        {

            // nova trigger(s)
            CreateAbilityObjectOnDeath component = abilityObject.AddComponent<CreateAbilityObjectOnDeath>();
            component.abilityToInstantiate = Ability.getAbility(AbilityID.boneNova);
            component.failsIfFailedAbility = true;
            component.offset = boneNovaOffset;
            component.aimingMethod = CreateAbilityObjectOnDeath.AimingMethod.TravelDirection;

            // nova mutator
            BoneNovaMutator mutator = abilityObject.AddComponent<BoneNovaMutator>();
            mutator.increasedSpeed = nova_increasedSpeed;
            mutator.pierces = true;
            mutator.increasedDamage = nova_increasedDamage;
            mutator.increasedStunChance = nova_increasedStunChance;
            mutator.bleedChance = nova_bleedChance;
            mutator.addedCritChance = nova_addedCritChance;
            mutator.addedCritMultiplier = nova_addedCritMultiplier;
            mutator.cone = true;
            mutator.randomAngles = true;
            mutator.noVFX = true;
            mutator.dontAttach = true;
            mutator.dontMoveToTarget = true;
            mutator.moreDamageAgainstBleeding = nova_moreDamageAgainstBleeding;
        }

        if (returnHealthChance != 0 && returnHealthChance > (Random.Range(0f, 1f)))
        {
            CreateResourceReturnAbilityObjectOnEnemyHit component = abilityObject.AddComponent<CreateResourceReturnAbilityObjectOnEnemyHit>();
            component.abilityObject = Ability.getAbility(AbilityID.bloodReturn);
            component.health = healthReturned;
            if (manaReturnChance != 0 && manaReturnChance > (Random.Range(0f, 1f)))
            {
                component.mana = manaReturned;
            }
        }

        if (increasedHealthCost != 0)
        {
            DamageCreatorOnCreation component = abilityObject.GetComponent<DamageCreatorOnCreation>();
            if (component)
            {
                component.flatDamage *= (1 + increasedHealthCost);
                component.percentCurrentHealthTaken *= (1 + increasedHealthCost);
            }
        }

        if (doesntPierce)
        {
            Comp<Pierce>.GetOrAdd(abilityObject).objectsToPierce = 0;
            abilityObject.AddComponent<DestroyOnFailingToPierceEnemy>();
        }

        if (endsAtTargetPoint)
        {
            if (!abilityObject.GetComponent<LocationDetector>()) { abilityObject.AddComponent<LocationDetector>(); }
            DestroyAfterDurationAfterReachingTargetLocation component = abilityObject.AddComponent<DestroyAfterDurationAfterReachingTargetLocation>();
            component.duration = 0f;
        }

        if (increasedDuration != 0)
        {
            DestroyAfterDuration dad = abilityObject.GetComponent<DestroyAfterDuration>();
            dad.duration *= 1 + increasedDuration;
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

        if (chanceToBleed > 0)
        {
            ChanceToApplyStatusOnEnemyHit newComponent = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            newComponent.statusEffect = StatusEffectList.getEffect(StatusEffectID.Bleed);
            newComponent.chance = chanceToBleed;
        }

        if (chanceToShredArmour > 0)
        {
            ChanceToApplyStatusOnEnemyHit newComponent = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            newComponent.statusEffect = StatusEffectList.getEffect(StatusEffectID.ArmourShred);
            newComponent.chance = chanceToShredArmour;
        }

        if (increasedStunChance != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.baseDamageStats.increasedStunChance += increasedStunChance;
            }
        }

        // increase damage based on the number of minions
        float realIncreasedDamage = increasedDamage;
        if (increasedDamagePerMinion != 0)
        {
            if (tracker && tracker.summons != null)
            {
                realIncreasedDamage += increasedDamagePerMinion * tracker.summons.Count;
            }
        }
        
        // more damage if you only have one minion
        if (increasedDamageWithOneMinion != 0)
        {
            if (tracker && tracker.numberOfMinions() == 1)
            {
                realIncreasedDamage += increasedDamageWithOneMinion;
            }
        }

        if (increasedDamageFromMinionDrain != 0 && tracker && tracker.summons != null && tracker.summons.Count > 0)
        {
            // choose a minion to drain
            BaseHealth minionHealth = tracker.summons[Random.Range(0, tracker.summons.Count - 1)].GetComponent<BaseHealth>();

            if (minionHealth)
            {
                // gain extra damage
                realIncreasedDamage += increasedDamageFromMinionDrain;
                // create a death vfx
                Instantiate(spiritEscapePrefab).transform.position = minionHealth.transform.position + spiritEscapeOffset;
                // kill the minion
                minionHealth.HealthDamage(minionHealth.currentHealth + 1);
            }
        }

        // increase damage
        if (realIncreasedDamage != 0 || moreDamage != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(realIncreasedDamage);
                if (moreDamage != 0)
                {
                    holder.increaseAllDamage(moreDamage);
                }
            }
        }

        if (damagesMinions)
        {
            DamageEnemyOnHit deoh = abilityObject.GetComponent<DamageEnemyOnHit>();
            if (deoh) {
                DamageCreatorMinionOnHit component = abilityObject.AddComponent<DamageCreatorMinionOnHit>();
                component.baseDamageStats = deoh.baseDamageStats;

                if (doesntPierce)
                {
                    abilityObject.GetComponent<DestroyOnFailingToPierceEnemy>().alsoDestroyOnFailingToPierceCreatorMinion();
                }
            }
        }

        return abilityObject;
    }


}