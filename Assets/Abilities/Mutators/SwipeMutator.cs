using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeMutator : AbilityMutator {
    
    public float addedCritMultiplier = 0f;
    public float addedCritChance = 0f;
    public float chanceToPoison = 0f;
    public float cullPercent = 0f;
    public bool travels = false;
    public float addedSpeed = 0f;

    public float increasedDamage = 0f;

    public float addedFireDamage = 0f;
    public float addedPhysicalDamage = 0f;
    public float chanceToIgnite = 0f;
    public float chanceToShredArmour = 0f;

    public float clawTotemOnKillChance = 0f;

    public float chanceOfExtraProjectiles = 0f;
    public bool slows = false;

    public float spiritWolvesOnFirstHitChance = 0f;
    public float increasedDamageOnFirstHit = 0f;
    public float increasedAttackSpeedOnFirstHit = 0f;
    public float increasedCritChanceOnFirstHit = 0f;
    public float addedCritMultiOnFirstHit = 0f;

    public float increasedAttackSpeed = 0f;
    public float increasedDuration = 0f;

    [Header("random movement variables")]
    public bool movesRandomly = false;
    public float increasedRandomisation = 0f;
    public float directionChangeInterval = 0.1f;
    public float boundingAngle = 180f;

    StatBuffs statBuffs = null;

    UsingAbility usingAbility = null;

    // true when the next hit with this ability is the first hit since it was used
    bool firstHit = false;
    Vector3 lastTargetPosition = Vector3.zero;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.swipe);
        AbilityEventListener listener = GetComponent<AbilityEventListener>();
        if (!listener) { listener = gameObject.AddComponent<AbilityEventListener>(); }
        listener.onKillEvent += SummonOnKill;
        listener.onHitEvent += OnHit;
        base.Awake();
        statBuffs = GetComponent<StatBuffs>();
        if (statBuffs == null) { statBuffs = gameObject.AddComponent<StatBuffs>(); }
    }

    public override float mutateUseSpeed(float useSpeed)
    {
        return useSpeed * (1f + increasedAttackSpeed);
    }

    public void SummonOnKill(Ability _ability, GameObject target)
    {
        if (clawTotemOnKillChance > 0 && (_ability == ability))
        {
            float rand = Random.Range(0f, 1f);
            if (rand < clawTotemOnKillChance)
            {
                if (!usingAbility) { usingAbility = GetComponent<UsingAbility>(); }
                if (usingAbility)
                {
                    usingAbility.UseAbility(AbilityIDList.getAbility(AbilityID.summonClawTotem), target.transform.position, false, false);
                }
            }
        }
    }

    public override float mutateStopRange()
    {
        if (travels)
        {
            return base.mutateStopRange() + 1 + addedSpeed * 0.17f;
        }
        else
        {
            return base.mutateStopRange();
        }
    }

    public void OnHit(Ability _ability, GameObject target)
    {
        if (firstHit)
        {
            firstHit = false;
            // buffs
            if (increasedDamageOnFirstHit != 0)
            {
                statBuffs.addBuff(4f, Tags.Properties.Damage, 0, increasedDamageOnFirstHit, null, null);
            }
            if (increasedAttackSpeedOnFirstHit != 0)
            {
                statBuffs.addBuff(4f, Tags.Properties.AttackSpeed, 0, increasedAttackSpeedOnFirstHit, null, null, null, "Swipe attack speed");
                statBuffs.addBuff(4f, Tags.Properties.CastSpeed, 0, increasedAttackSpeedOnFirstHit, null, null, null, "Swipe cast speed");
            }
            if (increasedCritChanceOnFirstHit != 0)
            {
                statBuffs.addBuff(4f, Tags.Properties.CriticalChance, 0, increasedCritChanceOnFirstHit, null, null, null, "Swipe crit chance");
            }
            if (addedCritMultiOnFirstHit != 0)
            {
                statBuffs.addBuff(4f, Tags.Properties.CriticalMultiplier, addedCritMultiOnFirstHit, 0, null, null, null, "Swipe crit multi");
            }
            // wolf
            float random = Random.Range(0f, 1f);
            if (!usingAbility) { usingAbility = GetComponent<UsingAbility>(); }
            if (random < spiritWolvesOnFirstHitChance)
            {
                usingAbility.UseAbility(AbilityIDList.getAbility(AbilityID.spiritWolves), lastTargetPosition, false, false);
            }
        }
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        firstHit = true;
        lastTargetPosition = targetLocation;

        if (movesRandomly)
        {
            RandomiseDirection component = abilityObject.AddComponent<RandomiseDirection>();
            component.maximumAngleChange = 180f * (1 + increasedRandomisation);
            component.timeToRandomise = RandomiseDirection.TimeToRandomise.DirectionMode;
            component.directionChangeInterval = directionChangeInterval;
            component.boundingAngle = boundingAngle;
        }

        if (chanceToPoison > 0)
        {
            ChanceToApplyStatusOnEnemyHit newComponent = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            newComponent.statusEffect = StatusEffectList.getEffect(StatusEffectID.Poison);
            newComponent.chance = chanceToPoison;
        }

        if (chanceToIgnite > 0)
        {
            ChanceToApplyStatusOnEnemyHit chanceTo = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            chanceTo.chance = chanceToIgnite;
            chanceTo.statusEffect = StatusEffectList.getEffect(StatusEffectID.Ignite);
        }
        
        if (chanceToShredArmour > 0)
        {
            ChanceToApplyStatusOnEnemyHit chanceTo = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            chanceTo.chance = chanceToShredArmour;
            chanceTo.statusEffect = StatusEffectList.getEffect(StatusEffectID.ArmourShred);
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

        if (cullPercent > 0)
        {
            foreach (DamageStatsHolder damage in abilityObject.GetComponents<DamageStatsHolder>())
            {
                damage.baseDamageStats.cullPercent += cullPercent;
            }
        }

        if (increasedDamage != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(increasedDamage);
                //holder.addBaseDamage(DamageType.PHYSICAL, holder.getBaseDamage(DamageType.PHYSICAL) * increasedDamage);
            }
        }

        if (addedFireDamage > 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.addBaseDamage(DamageType.FIRE, addedFireDamage);
            }
            // change the vfx
            abilityObject.GetComponent<CreateOnDeath>().objectsToCreateOnDeath.RemoveAt(0);
            abilityObject.GetComponent<CreateOnDeath>().add(PrefabList.getPrefab("animalSwipeVFXOrange"));
        }

        if (addedPhysicalDamage > 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.addBaseDamage(DamageType.PHYSICAL, addedPhysicalDamage);
            }
        }

        if (slows)
        {
            AttachAttachableOnEnemyHit newComponent = abilityObject.AddComponent<AttachAttachableOnEnemyHit>();
            newComponent.attachable = AbilityIDList.getAbility(AbilityID.slow).abilityPrefab;
        }

        if (travels)
        {
            // enable the vfx on the ability object itself
            foreach (Transform child in abilityObject.transform)
            {
                if (child.name == "SwipeVFX" && addedFireDamage <= 0)
                {
                    child.gameObject.SetActive(true);
                }
                if (child.name == "SwipeVFXOrange" && addedFireDamage > 0)
                {
                    child.gameObject.SetActive(true);
                }
            }
            // disable creating the vfx on death
            abilityObject.GetComponent<CreateOnDeath>().objectsToCreateOnDeath.RemoveAt(0);
            // set duration
            abilityObject.GetComponent<DestroyAfterDuration>().duration = 0.18f * (1 + increasedDuration);
            // set speed
            abilityObject.GetComponent<AbilityMover>().speed = 12f + addedSpeed;
            // remove start towards target
            abilityObject.GetComponent<StartsTowardsTarget>().distance = 0f;

            // add extra projectiles
            if (chanceOfExtraProjectiles > 0)
            {
                float rand2 = Random.Range(0f, 1f);
                if (rand2 < chanceOfExtraProjectiles)
                {
                    ExtraProjectiles extraProjectiles = abilityObject.AddComponent<ExtraProjectiles>();
                    extraProjectiles.angle = 180f;
                    extraProjectiles.randomAngles = true;
                }
            }
        }


        return abilityObject;
    }
}
