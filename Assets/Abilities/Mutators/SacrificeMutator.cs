using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacrificeMutator : AbilityMutator
{
    public float nova_increasedSpeed = 0f;
    public bool nova_pierces = false;
    public float nova_increasedDamage = 0f;
    public float nova_increasedStunChance = 0f;
    public float nova_bleedChance = 0f;
    public float nova_addedCritChance = 0f;
    public float nova_addedCritMultiplier = 0f;
    public float nova_moreDamageAgainstBleeding = 0f;

    public float boneNovaChance = 0f;

    public float moreDamageAgainstBleeding = 0f;
    public float increasedDotDamageOnCast = 0f;
    public float increasedDamage = 0f;
    public float increasedStunChance = 0f;
    public float increasedDamagePerMinion = 0f;

    public float increasedArea = 0f;
    public float increasedAreaWith3OrMoreMinions = 0f;

    public float increasedCastSpeed = 0f;
    public float chanceToIgnite = 0f;
    public float addedFireDamage = 0f;

    public float bloodWraithChance = 0f;
    public List<TaggedStatsHolder.TaggableStat> bloodWraithStats = new List<TaggedStatsHolder.TaggableStat>();
    public float increasedBloodWraithSize = 0f;

    public float increasedDamageIfDetonatedMinionHasMoreHealth = 0f;
    public float increasedDamageWithOneMinion = 0f;

    public bool chainsBetweenMinions = false;

    Vector3 boneNovaOffset = Vector3.zero;

    StatBuffs statBuffs = null;
    SummonTracker tracker = null;
    AbilityObjectConstructor aoc = null;
    BaseMana mana = null;
    bool recast = false;
    bool waitedForRecast = false;
    Vector3 recastPoint = Vector3.zero;
    BaseStats baseStats = null;

    List<Tags.AbilityTags> dotTag = new List<Tags.AbilityTags>();

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.sacrifice);
        dotTag.Add(Tags.AbilityTags.DoT);
        AbilityEventListener listener = Comp<AbilityEventListener>.GetOrAdd(gameObject);
        listener.onKillEvent += OnKill;
        BaseHealth health = GetComponent<BaseHealth>();
        if (health)
        {
            statBuffs = Comp<StatBuffs>.GetOrAdd(gameObject);
            tracker = Comp<SummonTracker>.GetOrAdd(gameObject);
        }
        aoc = Comp<AbilityObjectConstructor>.GetOrAdd(gameObject);
        mana = GetComponent<BaseMana>();
        baseStats = GetComponent<BaseStats>();
        boneNovaOffset = new Vector3(0f, 1.2f, 0f);
        base.Awake();
    }

    public void OnKill(Ability _ability, GameObject target)
    {
        
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

        // DoT on cast buff
        if (statBuffs)
        {
            statBuffs.addBuff(4f, Tags.Properties.Damage, 0, increasedDotDamageOnCast, null, null, dotTag, "sacrifice dot on cast bug");
        }

        if (boneNovaChance > 0 && (!mana || mana.currentMana > 0) && (boneNovaChance >= 1 || boneNovaChance > (Random.Range(0f, 1f))))
        {

            // nova trigger
            CreateAbilityObjectOnDeath component = abilityObject.AddComponent<CreateAbilityObjectOnDeath>();
            component.abilityToInstantiate = Ability.getAbility(AbilityID.boneNova);
            component.failsIfFailedAbility = true;
            component.offset = boneNovaOffset;

            // nova mutator
            BoneNovaMutator mutator = abilityObject.AddComponent<BoneNovaMutator>();
            mutator.increasedSpeed = nova_increasedSpeed;
            mutator.pierces = nova_pierces;
            mutator.increasedDamage = nova_increasedDamage;
            mutator.increasedStunChance = nova_increasedStunChance;
            mutator.bleedChance = nova_bleedChance;
            mutator.addedCritChance = nova_addedCritChance;
            mutator.addedCritMultiplier = nova_addedCritMultiplier;
            mutator.cone = false;
            mutator.dontAttach = true;
            mutator.dontMoveToTarget = true;
            mutator.randomAngles = false;
            mutator.noVFX = true;
            mutator.moreDamageAgainstBleeding = nova_moreDamageAgainstBleeding;
        }

        if (bloodWraithChance > 0 && (bloodWraithChance >= 1 || bloodWraithChance > Random.Range(0f, 1f)))
        {
            // wraith trigger
            CreateAbilityObjectOnDeath component = abilityObject.AddComponent<CreateAbilityObjectOnDeath>();
            component.abilityToInstantiate = Ability.getAbility(AbilityID.summonBloodWraith);
            component.failsIfFailedAbility = true;

            // wraith mutator
            BloodWraithMutator mutator = abilityObject.AddComponent<BloodWraithMutator>();
            mutator.statList.AddRange(bloodWraithStats);
            mutator.increasedSize = increasedBloodWraithSize;
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

        // increase radius
        float totalIncreasedArea = increasedArea;
        if (increasedArea != 0 || increasedAreaWith3OrMoreMinions != 0)
        {
            // calculate total increased area
            if (increasedAreaWith3OrMoreMinions != 0 && tracker && tracker.summons != null && tracker.summons.Count >= 3)
            {
                totalIncreasedArea += increasedAreaWith3OrMoreMinions;
            }

            // calculate increased radius
            float increasedRadius = Mathf.Sqrt(totalIncreasedArea + 1) - 1; ;

            // apply increased radius
            if (increasedRadius != 0)
            {
                foreach (CreateOnDeath cod in abilityObject.GetComponents<CreateOnDeath>())
                {
                    cod.increasedRadius = increasedRadius;
                    cod.increasedHeight = increasedRadius;
                }
                foreach (SphereCollider col in abilityObject.GetComponents<SphereCollider>())
                {
                    col.radius *= (1 + increasedRadius);
                }
                foreach (CapsuleCollider col in abilityObject.GetComponents<CapsuleCollider>())
                {
                    col.radius *= (1 + increasedRadius);
                    col.height *= (1 + increasedRadius);
                }
            }
        }

        

        if (chanceToIgnite > 0)
        {
            ChanceToApplyStatusOnEnemyHit newComponent = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            newComponent.statusEffect = StatusEffectList.getEffect(StatusEffectID.Ignite);
            newComponent.chance = chanceToIgnite;
        }

        if (addedFireDamage > 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.addBaseDamage(DamageType.FIRE, addedFireDamage);
            }
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

        // increased damage if the minion has more health
        if (increasedDamageIfDetonatedMinionHasMoreHealth != 0 && tracker)
        {
            // get the likely target
            Summoned targetMinion = tracker.getNearestMinion(targetLocation);
            if (targetMinion)
            {
                // check if it has more health
                BaseHealth minionHealth = targetMinion.getBaseHealth();
                if (minionHealth && baseStats && baseStats.myHealth && minionHealth.currentHealth > baseStats.myHealth.currentHealth)
                {
                    realIncreasedDamage += increasedDamageIfDetonatedMinionHasMoreHealth;
                }
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

        // increase damage
        if (realIncreasedDamage != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(realIncreasedDamage);
            }
        }

        // chaining (mutator needs to be updated)
        if (chainsBetweenMinions)
        {
            CreateAbilityObjectOnNewAllyHit component = abilityObject.AddComponent<CreateAbilityObjectOnNewAllyHit>();
            component.abilityToInstantiate = Ability.getAbility(AbilityID.sacrifice);
            component.onlyHitCreatorMinions = true;
            component.aimTowardsHit = true;

            SacrificeMutator mutator = abilityObject.AddComponent<SacrificeMutator>();
            mutator.nova_increasedSpeed = nova_increasedSpeed;
            mutator.nova_pierces = nova_pierces;
            mutator.nova_increasedDamage = nova_increasedDamage;
            mutator.nova_increasedStunChance = nova_increasedStunChance;
            mutator.nova_bleedChance = nova_bleedChance;
            mutator.nova_addedCritChance = nova_addedCritChance;
            mutator.nova_addedCritMultiplier = nova_addedCritMultiplier;
            mutator.nova_moreDamageAgainstBleeding = nova_moreDamageAgainstBleeding;

            mutator.boneNovaChance = boneNovaChance;

            mutator.moreDamageAgainstBleeding = moreDamageAgainstBleeding;

            mutator.increasedStunChance = increasedStunChance;
            
            mutator.chanceToIgnite = chanceToIgnite;
            mutator.addedFireDamage = addedFireDamage;

            mutator.bloodWraithChance = bloodWraithChance;
            mutator.bloodWraithStats = new List<TaggedStatsHolder.TaggableStat>();
            mutator.bloodWraithStats.AddRange(bloodWraithStats);
            mutator.increasedBloodWraithSize = increasedBloodWraithSize;

            mutator.chainsBetweenMinions = true;

            // snap shot the damage increase and area increase
            mutator.increasedDamage = realIncreasedDamage;
            mutator.increasedArea = totalIncreasedArea;
        }

        return abilityObject;
    }


}