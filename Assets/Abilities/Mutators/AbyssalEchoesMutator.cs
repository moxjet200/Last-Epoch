using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AbyssalEchoesMutator : AbilityMutator
{
    public bool noChain = false;
    public bool pullsOnHit = false;
    public float increasedPullStrength = 0f;
    public float increasedRadius = 0f;
    public float speedDebuff = 0f;
    public float armourDebuff = 0f;
    public float increasedDebuffDuration = 0f;
    public float increasedDebuffStrength = 0f;
    public float dotTakenDebuff = 0f;
    public bool dotTakenDebuffStacks = false;

    public int additionalCasts = 0;
    public float increasedDelay = 0f;
    public bool stacks = false;

    public List<TaggedStatsHolder.Stat> stackingStatsOnKill = new List<TaggedStatsHolder.Stat>();

    public float healthRegenOnKill = 0f;
    public float manaRegenOnKill = 0f;

    public float increasedNonStackingBuffDuration = 0f;
    public float movementSpeedOnKill = 0f;
    public float attackAndCastSpeedOnKill = 0f;

    public float increasedCastSpeed = 0f;

    public float increasedDamage = 0f;

    StatBuffs statBuffs = null;
    AbilityObjectConstructor aoc = null;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.abyssalEchoes);
        base.Awake();
        AbilityEventListener ael = GetComponent<AbilityEventListener>();
        if (ael)
        {
            ael.onKillEvent += onKill;
        }
        aoc = GetComponent<AbilityObjectConstructor>();
        statBuffs = Comp<StatBuffs>.GetOrAdd(gameObject);
        if (statBuffs == null) { statBuffs = gameObject.AddComponent<StatBuffs>(); }
    }

    public void onKill(Ability _ability, GameObject target)
    {
        if (_ability == ability)
        {
            float buffDuration = 4f * (1 + increasedNonStackingBuffDuration);
            if (healthRegenOnKill != 0)
            {
                statBuffs.addBuff(buffDuration, Tags.Properties.HealthRegen, 0, healthRegenOnKill, null, null, null, "abyssalEchoesOnKillHealthRegen");
            }
            if (manaRegenOnKill != 0)
            {
                statBuffs.addBuff(buffDuration, Tags.Properties.ManaRegen, 0, manaRegenOnKill, null, null, null, "abyssalEchoesOnKillManaRegen");
            }
            if (movementSpeedOnKill != 0)
            {
                statBuffs.addBuff(buffDuration, Tags.Properties.Movespeed, 0, movementSpeedOnKill, null, null, null, "abyssalEchoesOnKillMoveSpeed");
            }
            if (attackAndCastSpeedOnKill != 0)
            {
                statBuffs.addBuff(buffDuration, Tags.Properties.AttackSpeed, 0, attackAndCastSpeedOnKill, null, null, null, "abyssalEchoesOnKillAttackSpeed");
                statBuffs.addBuff(buffDuration, Tags.Properties.CastSpeed, 0, attackAndCastSpeedOnKill, null, null, null, "abyssalEchoesOnKillCastSpeed");
            }
            foreach(TaggedStatsHolder.Stat stat in stackingStatsOnKill) {
                Buff buff = new Buff();
                buff.stat = new TaggedStatsHolder.Stat(stat);
                buff.remainingDuration = 2f;
                statBuffs.addBuff(buff);
            }
        }
    }

    public override float mutateUseSpeed(float useSpeed)
    {
        return useSpeed * (1 + increasedCastSpeed);
    }

    public override int mutateDelayedCasts(int defaultCasts)
    {
        return base.mutateDelayedCasts(defaultCasts) + additionalCasts;
    }

    public override float mutateDelatedCastDuration(float defaultDuration)
    {
        return ((float)additionalCasts) * (1 + increasedDelay);
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        if (noChain)
        {
            CreateAbilityObjectOnNewEnemtHit component = abilityObject.GetComponent<CreateAbilityObjectOnNewEnemtHit>();
            if (component)
            {
                component.active = false;
            }
        }

        if (pullsOnHit)
        {
            DestroyAfterDuration dad = abilityObject.GetComponent<DestroyAfterDuration>();
            if (dad)
            {
                dad.duration = 0.1f;
            }
            RepeatedlyPullEnemiesWithinRadius component = abilityObject.AddComponent<RepeatedlyPullEnemiesWithinRadius>();
            component.distanceMultiplier = 0.92f / (1 + increasedPullStrength);
            component.limitDistanceToMax = false;
            component.interval = 0.02f;
            component.radius = 4f * (1 + increasedRadius);
        }

        if (increasedRadius != 0)
        {
            foreach (CreateOnDeath cod in abilityObject.GetComponents<CreateOnDeath>())
            {
                cod.increasedRadius = increasedRadius;
                cod.increasedHeight = increasedRadius;
            }
            foreach (CapsuleCollider col in abilityObject.GetComponents<CapsuleCollider>())
            {
                col.height *= (1 + increasedRadius);
                col.radius *= (1 + increasedRadius);
            }
        }

        if (increasedDamage != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(increasedDamage);
            }
        }

        if (stacks)
        {
            ApplyStatusOnEnemyHit component = abilityObject.GetComponent<ApplyStatusOnEnemyHit>();
            component.statusEffect = StatusEffectList.getEffect(StatusEffectID.StackingAbyssalDecay);
        }

        if (speedDebuff != 0 || armourDebuff != 0  || dotTakenDebuff != 0)
        {
            DebuffOnEnemyHit component = Comp<DebuffOnEnemyHit>.GetOrAdd(abilityObject);
            if (speedDebuff != 0)
            {
                List<float> speedDebuffList = new List<float>();
                speedDebuffList.Add(speedDebuff * (1 + increasedDebuffStrength));
                component.addDebuffToList(Tags.Properties.AttackSpeed, 0f, 0f, speedDebuffList, null, 4f * (1 + increasedDebuffDuration), null, "AbyssalEchoesDebuffAttackSpeed");
                component.addDebuffToList(Tags.Properties.CastSpeed, 0f, 0f, speedDebuffList, null, 4f * (1 + increasedDebuffDuration), null, "AbyssalEchoesDebuffCastSpeed");
                component.addDebuffToList(Tags.Properties.Movespeed, 0f, 0f, speedDebuffList, null, 4f * (1 + increasedDebuffDuration), null, "AbyssalEchoesDebuffMoveSpeed");
            }
            if (armourDebuff != 0)
            {
                component.addDebuffToList(Tags.Properties.Movespeed, armourDebuff * (1 + increasedDebuffStrength), 0f, null, null, 4f * (1 + increasedDebuffDuration), null, "AbyssalEchoesDebuffArmour");
            }
            if (dotTakenDebuff != 0)
            {
                if (dotTakenDebuffStacks)
                {
                    component.addDebuffToList(Tags.Properties.DamageTaken, 0f, -dotTakenDebuff, null, null, 4f);
                }
                else
                {
                    component.addDebuffToList(Tags.Properties.DamageTaken, 0f, -dotTakenDebuff, null, null, 4f, null, "AbyssalEchoesDebuffDoTTaken");
                }
            }

        }

        return abilityObject;
    }
}
