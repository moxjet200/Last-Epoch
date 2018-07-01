using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RipBloodMutator : AbilityMutator
{
    public float splatter_increasedRadius = 0f;
    public float splatter_increasedDamage = 0f;
    public float splatter_increasedStunChance = 0f;
    public float splatter_chanceToPoison = 0f;
    public float splatter_armourReductionChance = 0f;
    public float splatter_armourReduction = 0f;
    public bool splatter_armourReductionStacks = false;
    public float splatter_increasedArmourDebuffDuration = 0f;
    public float splatter_increasedDamagePerMinion = 0f;
    public bool splatter_reducesDarkProtectionInstead = false;

    public List<TaggedStatsHolder.TaggableStat> splatter_minionBuffs = new List<TaggedStatsHolder.TaggableStat>();
    
    public float splatterChance = 0f;
    public float increasedDamage = 0f;
    public float increasedStunChance = 0f;
    public float chanceToBleed = 0f;
    public float chanceToPoison = 0f;
    public float increasedDamagePerMinion = 0f;
    public float increasedCastSpeed = 0f;

    public float addedHealthGained = 0f;
    public float increasedHealthGained = 0f;
    public float increasedHealthGainedPerAttunement = 0f;
    public float manaGained = 0f;
    public bool convertHealthToWard = false;

    public bool freeWhenOutOfMana = false;
    public bool targetsAlliesInstead = false;
    public bool necrotic = false;
    public bool recastOnKill = false;

    public List<TaggedStatsHolder.TaggableStat> onHitBuffs = new List<TaggedStatsHolder.TaggableStat>();

    public List<float> moreDamageInstances = new List<float>();

    StatBuffs statBuffs = null;
    SummonTracker tracker = null;
    AbilityObjectConstructor aoc = null;
    BaseMana mana = null;
    bool recast = false;
    bool waitedForRecast = false;
    Vector3 recastPoint = Vector3.zero;
    BaseStats baseStats = null;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.ripBlood);
        AbilityEventListener listener = Comp<AbilityEventListener>.GetOrAdd(gameObject);
        listener.onHitEvent += OnHit;
        listener.onKillEvent += OnKill;
        statBuffs = Comp<StatBuffs>.GetOrAdd(gameObject);
        tracker = Comp<SummonTracker>.GetOrAdd(gameObject);
        aoc = Comp<AbilityObjectConstructor>.GetOrAdd(gameObject);
        mana = GetComponent<BaseMana>();
        baseStats = GetComponent<BaseStats>();
        base.Awake();
    }

    public void OnHit(Ability _ability, GameObject target)
    {
        foreach (TaggedStatsHolder.TaggableStat stat in onHitBuffs)
        {
            statBuffs.addBuff(4f, stat.property, stat.addedValue, stat.increasedValue, stat.moreValues, stat.quotientValues, stat.tagList);
        }
    }

    public void OnKill(Ability _ability, GameObject target)
    {
        if (_ability == ability && recastOnKill && (!mana || mana.currentMana >=0) && target)
        {
            recast = true;
            recastPoint = target.transform.position;
        }
    }

    public override float getIncreasedManaCost()
    {
        if (freeWhenOutOfMana && mana && mana.currentMana <=0) { return -100f; }

        return base.getIncreasedManaCost();
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
        if (splatterChance > 0 && (!mana || mana.currentMana > 0) && (splatterChance >= 1 || splatterChance > (Random.Range(0f, 1f)))){

            // splatter trigger
            CreateAbilityObjectOnDeath component = abilityObject.AddComponent<CreateAbilityObjectOnDeath>();
            component.abilityToInstantiate = Ability.getAbility(AbilityID.bloodSplatter);

            // splatter mutator
            BloodSplatterMutator mutator = abilityObject.AddComponent<BloodSplatterMutator>();
            mutator.increasedRadius = splatter_increasedRadius;
            mutator.increasedDamage = splatter_increasedDamage;
            mutator.chanceToPoison = splatter_chanceToPoison;
            mutator.armourReductionChance = splatter_armourReductionChance;
            mutator.armourReduction = splatter_armourReduction;
            mutator.armourReductionStacks = splatter_armourReductionStacks;
            mutator.increasedArmourDebuffDuration = splatter_increasedArmourDebuffDuration;
            mutator.increasedDamagePerMinion = splatter_increasedDamagePerMinion;
            mutator.minionBuffs = splatter_minionBuffs;
            mutator.reducesDarkProtectionInstead = splatter_reducesDarkProtectionInstead;
            mutator.necrotic = necrotic;
        }
        

        if (necrotic)
        {
            // replace vfx
            CreateOnDeath cod = abilityObject.GetComponent<CreateOnDeath>();
            if (cod && cod.objectsToCreateOnDeath != null && cod.objectsToCreateOnDeath.Count > 0)
            {
                cod.objectsToCreateOnDeath[0] = new CreateOnDeath.GameObjectHolder(PrefabList.getPrefab("NecroticRipBloodOnDeathVFX"));
            }

            // convert damage
            foreach (DamageStatsHolder damage in abilityObject.GetComponents<DamageStatsHolder>())
            {
                damage.convertAllDamageOfType(DamageType.PHYSICAL, DamageType.NECROTIC);
            }
        }

        if (targetsAlliesInstead)
        {
            // change the damage component to hit allies (only hits minions by default)
            DamageEnemyOnHit damageComponent = abilityObject.GetComponent<DamageEnemyOnHit>();
            DamageAllyOnHit component = abilityObject.AddComponent<DamageAllyOnHit>();
            component.baseDamageStats = damageComponent.baseDamageStats;
            damageComponent.deactivate();
            Destroy(damageComponent);

            // make sure it still creates a blood orb
            CreateResourceReturnAbilityObjectOnEnemyHit component2 = abilityObject.GetComponent<CreateResourceReturnAbilityObjectOnEnemyHit>();
            component2.hitsAlliesInstead = true;

            // change the targetting
            MoveToNearestEnemyOnCreation moveComponent = abilityObject.GetComponent<MoveToNearestEnemyOnCreation>();
            if (moveComponent) { moveComponent.moveToAllyInstead = true; }
        }


        if (chanceToPoison > 0)
        {
            ChanceToApplyStatusOnEnemyHit newComponent = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            newComponent.statusEffect = StatusEffectList.getEffect(StatusEffectID.Poison);
            newComponent.chance = chanceToPoison;
        }

        if (chanceToBleed > 0)
        {
            ChanceToApplyStatusOnEnemyHit newComponent = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            newComponent.statusEffect = StatusEffectList.getEffect(StatusEffectID.Bleed);
            newComponent.chance = chanceToBleed;
        }

        if (addedHealthGained != 0 || increasedHealthGained != 0 || manaGained != 0 || convertHealthToWard || necrotic)
        {
            CreateResourceReturnAbilityObjectOnEnemyHit component = abilityObject.GetComponent<CreateResourceReturnAbilityObjectOnEnemyHit>();

            // check if this behaviour needs to be removed
            if (increasedHealthGained < -1 && manaGained <= 0)
            {
                component.deactivated = true;
            }
            // if it does not then change its values
            else
            {
                component.health += addedHealthGained;
                component.health *= (1 + increasedHealthGained);
                if (increasedHealthGainedPerAttunement != 0)
                {
                    component.health *= (1 + baseStats.GetStatValue(Tags.Properties.Attunement) * increasedHealthGainedPerAttunement);
                }
                component.mana += manaGained;
                if (convertHealthToWard)
                {
                    component.ward = component.health;
                    component.health = 0;
                }
                if (necrotic)
                {
                    component.abilityObject = Ability.getAbility(AbilityID.necroticReturn);
                }
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


        if (realIncreasedDamage != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(realIncreasedDamage);
            }
        }

        if (moreDamageInstances != null && moreDamageInstances.Count > 0)
        {
            float moreDamage = 1f;
            foreach (float instance in moreDamageInstances)
            {
                moreDamage *= 1 + instance;
            }

            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(moreDamage - 1);
            }
        }

        return abilityObject;
    }


}