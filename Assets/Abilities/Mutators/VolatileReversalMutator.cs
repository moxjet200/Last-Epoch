using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolatileReversalMutator : AbilityMutator
{
    // start nova
    public float increasedDamage = 0f;
    public float increasedRadius = 0f;
    public float timeRotChance = 0f;
    public float increasesDamageTaken = 0f;
    public float increasesDoTDamageTaken = 0f;
    public float increasedStunChance = 0f;

    public bool voidRiftAtStart = false;
    public bool voidRiftAtEnd = false;

    public List<TaggedStatsHolder.TaggableStat> statsWhileOnCooldown = new List<TaggedStatsHolder.TaggableStat>();
    public List<TaggedStatsHolder.TaggableStat> statOnUse = new List<TaggedStatsHolder.TaggableStat>();

    public float percentCooldownRecoveredOnKill = 0f;
    public float additionalSecondsBack = 0f;

    public bool noHealthRestoration = false;
    public bool noManaRestoration = false;

    public bool healsOrDamagesAtRandom = false;
    public float healOrDamagePercent = 0f;

    AbilityObjectConstructor aoc = null;
    BaseHealth health = null;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.volatileReversal);
        AbilityEventListener ael = AbilityEventListener.GetOrAdd(gameObject);
        ael.onKillEvent += OnKill;
        chargeManager = GetComponent<ChargeManager>();
        health = GetComponent<BaseHealth>();
        base.Awake();
    }

    public void OnKill(Ability _ability, GameObject target)
    {
        if (percentCooldownRecoveredOnKill > 0 && chargeManager)
        {
            chargeManager.recoverPercentageCooldown(ability, percentCooldownRecoveredOnKill);
        }
    }

    public void Start()
    {
        aoc = GetComponent<AbilityObjectConstructor>();
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {

        if (voidRiftAtStart)
        {
            CreateAbilityObjectOnStart component = abilityObject.AddComponent<CreateAbilityObjectOnStart>();
            component.abilityToInstantiate = Ability.getAbility(AbilityID.voidRift);
            component.createAtStartLocation = true;
        }

        if (voidRiftAtEnd)
        {
            CreateAbilityObjectOnDeath component = abilityObject.AddComponent<CreateAbilityObjectOnDeath>();
            component.abilityToInstantiate = Ability.getAbility(AbilityID.voidRift);
        }

        if (voidRiftAtStart || voidRiftAtEnd)
        {
            VoidRiftMutator mut = abilityObject.AddComponent<VoidRiftMutator>();
            mut.increasedDamage = increasedDamage;
            mut.increasedRadius = increasedRadius;
            mut.increasedStunChance = increasedStunChance;
            mut.increasesDoTDamageTaken = increasesDoTDamageTaken;
            mut.timeRotChance = timeRotChance;
            mut.increasesDamageTaken = increasesDamageTaken;
        }

        // apply stats on cooldown
        float cooldown = getCooldown();
        StatBuffs buffs = GetComponent<StatBuffs>();
        if (!buffs) { buffs = gameObject.AddComponent<StatBuffs>(); }
        foreach (TaggedStatsHolder.TaggableStat stat in statsWhileOnCooldown)
        {
            TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(stat);
            buffs.addTaggedBuff(new TaggedBuff(newStat, cooldown));
        }

        // apply stats on use
        float duration = 2 + additionalSecondsBack;
        foreach (TaggedStatsHolder.TaggableStat stat in statOnUse)
        {
            TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(stat);
            buffs.addTaggedBuff(new TaggedBuff(newStat, duration));
        }
        
        if (noHealthRestoration || noManaRestoration)
        {
            ReturnCasterToOlderPosition component = abilityObject.GetComponent<ReturnCasterToOlderPosition>();
            if (noHealthRestoration) { component.restoreHealth = false; }
            if (noManaRestoration) { component.restoreMana = false; }
        }

        if (healsOrDamagesAtRandom && health)
        {
            float rand = Random.Range(0f, 1f);
            if (rand < 0.8f)
            {
                health.Heal((health.maxHealth - health.currentHealth) * healOrDamagePercent);
            }
            else
            {
                health.HealthDamage(health.currentHealth * healOrDamagePercent);
            }
        }

        return abilityObject;
    }

}
