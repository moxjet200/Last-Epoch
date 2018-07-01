using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErasingStrikeMutator : AbilityMutator
{
    // hit stats
    public float increasedDamage = 0f;
    public float increasedRadius = 0f;
    public float timeRotChance = 0f;
    public float increasedStunChance = 0f;
    public float addedCritMultiplier = 0f;
    public float addedCritChance = 0f;
    public float addedVoidDamage = 0f;
    public float moreDamageAgainstFullHealth = 0f;
    public float moreDamageAgainstDamaged = 0f;
    public float cullPercent = 0f;

    public float voidRift_increasedDamage = 0f;
    public float voidRift_increasedRadius = 0f;
    public float voidRift_timeRotChance = 0f;
    public float voidRift_increasesDamageTaken = 0f;
    public float voidRift_increasesDoTDamageTaken = 0f;
    public float voidRift_increasedStunChance = 0f;
    public float voidRift_moreDamageAgainstStunned = 0f;

    // on kill stats
    public float manaGainedOnKill = 0f;
    public float percentLifeGainedOnKill = 0f;
    public float movementSpeedOnKill = 0f;
    public float stationaryVoidBeamChance = 0f;

    public bool voidBeamsOnkill = false;
    public int reducedKillsRequiredForVoidBeams = 0;

    // other stats
    public bool noAttackSpeedScaling = false;
    public bool voidChargeOnCrit = false;


    BaseMana mana = null;
    BaseHealth health = null;
    StatBuffs statBuffs = null;
    UsingAbility ua = null;
    BaseStats baseStats = null;

    int killsSinceUse = 0;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.erasingStrike);
        base.Awake();
        mana = GetComponent<BaseMana>();
        health = GetComponent<BaseHealth>();
        baseStats = GetComponent<BaseStats>();
        AbilityEventListener ael = AbilityEventListener.GetOrAdd(gameObject);
        statBuffs = Comp<StatBuffs>.GetOrAdd(gameObject);
        ua = GetComponent<UsingAbility>();
        
        ael.onHitEvent += OnKill;
        ael.onCritEvent += OnCrit;
    }

    public void OnCrit(Ability _ability, GameObject target)
    {
        if (_ability == Ability.getAbility(AbilityID.erasingStrikeHit) && voidChargeOnCrit)
        {
            if (!ua) { ua = GetComponent<UsingAbility>(); }
            ua.UseAbility(Ability.getAbility(AbilityID.voidEssence), transform.position, false, false);
        }
    }

    public void OnKill(Ability _ability, GameObject target)
    {
        if (_ability && _ability == Ability.getAbility(AbilityID.erasingStrikeHit))
        {
            // void beams
            killsSinceUse++;
            if (killsSinceUse >= 5 && ua && voidBeamsOnkill)
            {
                killsSinceUse = -10000;
                ua.UseAbility(Ability.getAbility(AbilityID.playerVoidBeams), transform.position + transform.forward, false, false);
            }
            // stationary void beam
            if (!ua) { ua = GetComponent<UsingAbility>(); }
            if (stationaryVoidBeamChance != 0 && ua)
            {
                if ((Random.Range(0f, 1f) < stationaryVoidBeamChance) && target)
                {
                    ua.UseAbility(Ability.getAbility(AbilityID.stationaryVoidBeam), target.transform.position, false, false);
                }
            }
            // mana and health
            if (mana && manaGainedOnKill != 0) { mana.currentMana += manaGainedOnKill; if (mana.currentMana > mana.maxMana) { mana.currentMana = mana.maxMana; } }
            if (health && percentLifeGainedOnKill != 0 && target) {
                BaseHealth targetHealth = target.GetComponent<BaseHealth>();
                if (targetHealth)
                {
                    health.Heal(targetHealth.maxHealth * percentLifeGainedOnKill);
                }
            }
            // move speed
            if (movementSpeedOnKill != 0)
            {
                TaggedStatsHolder.Stat stat = new TaggedStatsHolder.Stat(Tags.Properties.Movespeed);
                stat.increasedValue = movementSpeedOnKill;
                Buff buff = new Buff(stat, 4f);
                statBuffs.addBuff(buff);
            }
        }
    }

    public override float mutateUseSpeed(float useSpeed)
    {
        if (noAttackSpeedScaling)
        {
            float divider = baseStats.GetStatValue(Tags.Properties.AttackSpeed, ability.useTags);
            if (divider > 0)
            {
                return base.mutateUseSpeed(useSpeed) / divider;
            }
        }
        return base.mutateUseSpeed(useSpeed);
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        killsSinceUse = 0;

        ErasingStrikeHitMutator mut = abilityObject.AddComponent<ErasingStrikeHitMutator>();
        mut.increasedDamage = increasedDamage;
        mut.increasedRadius = increasedRadius;
        mut.timeRotChance = timeRotChance;
        mut.increasedStunChance = increasedStunChance;
        mut.addedCritMultiplier = addedCritMultiplier;
        mut.addedCritChance = addedCritChance;
        mut.addedVoidDamage = addedVoidDamage;
        mut.moreDamageAgainstFullHealth = moreDamageAgainstFullHealth;
        mut.moreDamageAgainstDamaged = moreDamageAgainstDamaged;
        mut.cullPercent = cullPercent;

        mut.voidRift_increasedDamage = voidRift_increasedDamage;
        mut.voidRift_increasedRadius = voidRift_increasedRadius;
        mut.voidRift_timeRotChance = voidRift_timeRotChance;
        mut.voidRift_increasesDamageTaken = voidRift_increasesDamageTaken;
        mut.voidRift_increasesDoTDamageTaken = voidRift_increasesDoTDamageTaken;
        mut.voidRift_increasedStunChance = voidRift_increasedStunChance;
        mut.voidRift_moreDamageAgainstStunned = voidRift_moreDamageAgainstStunned;

        if (increasedRadius != 0)
        {
            foreach (CreateOnDeath cod in abilityObject.GetComponents<CreateOnDeath>())
            {
                cod.increasedRadius = increasedRadius;
                cod.increasedHeight = increasedRadius;
            }
        }

        return abilityObject;
    }
}
