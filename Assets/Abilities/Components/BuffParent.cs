using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BuffParent : MonoBehaviour {
    
    public float lifeRegen = 0f;
    public float wardRegen = 0f;
    public float speed = 0f;
    public float percentageSpeed = 0f;
    public float reducedStunDuration = 0f;
    public float manaCostDivider = 0f;
    public float ward = 0f;
    public float wardPercentage = 0f;
    public float increasedSize = 0f;
    public List<TaggedStatsHolder.TaggableStat> taggedStats = new List<TaggedStatsHolder.TaggableStat>(); 

    ProtectionClass parentProtection;
	BaseHealth parentHealth;
    BaseMana parentMana;
    BaseStats parentBaseStats;
    SpeedManager parentSpeedManager;
    Stunned parentStunnedState;
    bool initialised = false;

    GameObject parent = null;

    float speedGivenFromPercentageIncrease = 0f;

    bool buffsRemoved = false;

    void Update () {
        if (!initialised) { initialise(); }
        
    }

    void Awake()
    {
        initialised = false;
        if (transform.parent)
        {
            parent = transform.parent.gameObject;
        }
    }

    void initialise()
    {
        initialised = true;
        // make sure there is a parent
        if (transform.parent == null) { return; }
        if (parent == null) { parent = transform.parent.gameObject; }
        if (parent == null) { Debug.LogError("BuffParent component has no parent"); return; }
        // find the parent's protection class
        parentProtection = parent.gameObject.GetComponent<ProtectionClass>();

        if (parentProtection)
        {
            if (ward != 0) { parentProtection.GainWard(ward); }
            if (wardPercentage != 0 && parent.GetComponent<BaseHealth>())
            {
                parentProtection.GainWard((wardPercentage * parent.GetComponent<BaseHealth>().maxHealth));
            }
        }

        if (increasedSize != 0)
        {
            SizeManager sizeManager = parent.GetComponent<SizeManager>();
            if (sizeManager)
            {
                sizeManager.increaseSize(increasedSize);
            }
        }
		
        // find the parent's base health
        if (lifeRegen != 0)
        {
            parentHealth = parent.GetComponent<BaseHealth>();
			parentHealth.addedHealthRegenPerSecond += lifeRegen;
        }
        // find the parent's base mana
        if (manaCostDivider != 0)
        {
            parentMana = parent.GetComponent<BaseMana>();
            parentMana.addedManaCostDivider += manaCostDivider;
        }
        // find the parent's demo ward regen
        if (wardRegen != 0)
        {
			parentProtection.wardRegen += wardRegen;
        }
        // find the parent's base stats
        if (speed != 0)
        {
            parentBaseStats = parent.GetComponent<BaseStats>();
            if (parentBaseStats != null)
            {
				parentBaseStats.ChangeStatModifier (Tags.Properties.Movespeed, speed,  BaseStats.ModType.ADDED);
            }
        }
        // find the parent's speed manager
        if (percentageSpeed != 0)
        {
            if (!parentBaseStats) { parentBaseStats = parent.GetComponent<BaseStats>(); }
            parentSpeedManager = parent.GetComponent<SpeedManager>();
            if (parentSpeedManager != null && parentBaseStats != null)
            {
				parentBaseStats.ChangeStatModifier(Tags.Properties.Movespeed, percentageSpeed, BaseStats.ModType.INCREASED);
            }
        }
        if (reducedStunDuration != 0)
        {
            parentStunnedState = parent.gameObject.GetComponent<Stunned>();
            if (parentStunnedState != null)
            {
                parentStunnedState.baseStunDuration -= reducedStunDuration;
            }
        }
        // apply taggedStats
        parentBaseStats = parent.GetComponent<BaseStats>();
        if (parentBaseStats)
        {
            foreach (TaggedStatsHolder.TaggableStat stat in taggedStats)
            {
				parentBaseStats.ChangeStatModifier(stat.property, stat.addedValue, BaseStats.ModType.ADDED, stat.tagList);
				parentBaseStats.ChangeStatModifier(stat.property, stat.increasedValue, BaseStats.ModType.INCREASED, stat.tagList);
                foreach (float value in stat.moreValues)
                {
                    parentBaseStats.ChangeStatModifier(stat.property, value, BaseStats.ModType.MORE, stat.tagList);
                }
                foreach (float value in stat.quotientValues)
                {
                    parentBaseStats.ChangeStatModifier(stat.property, value, BaseStats.ModType.QUOTIENT, stat.tagList);
                }
            }
            parentBaseStats.UpdateStats();
        }

        // update protection totals
        if (parentProtection) { parentProtection.UpdateProtectionTotals(); }
        // subscribe to a death event to remove the buffs
        if (GetComponent<SelfDestroyer>()) { GetComponent<SelfDestroyer>().deathEvent += removeBuffs; }
    }

    void OnDestroy()
    {
        removeBuffs();
    }

    void removeBuffs()
    {
        if (buffsRemoved) { return; }
        
        if (!initialised) { return; }

        buffsRemoved = true;

        if (parentProtection)
        {
			parentProtection.wardRegen -= wardRegen;
        }
        if (increasedSize != 0 && parent)
        {
            SizeManager sizeManager = parent.GetComponent<SizeManager>();
            if (sizeManager)
            {
                sizeManager.increaseSize(-increasedSize);
            }
        }
        if (parentHealth && lifeRegen != 0)
        {
			parentHealth.addedHealthRegenPerSecond -= lifeRegen;
        }
        if (parentMana && manaCostDivider != 0)
        {
            parentMana.addedManaCostDivider -= manaCostDivider;
        }
        if (parentBaseStats != null)
        {
			parentBaseStats.ChangeStatModifier (Tags.Properties.Movespeed, -speed,  BaseStats.ModType.ADDED);
            if (parentSpeedManager != null)
            {
				parentBaseStats.ChangeStatModifier(Tags.Properties.Movespeed, -percentageSpeed, BaseStats.ModType.INCREASED);
            }
        }
        if (parentStunnedState != null)
        {
            parentStunnedState.baseStunDuration += reducedStunDuration;
        }
        // remove taggedStats
        if (parentBaseStats)
        {
            foreach (TaggedStatsHolder.TaggableStat stat in taggedStats)
            {
				parentBaseStats.ChangeStatModifier(stat.property, -stat.addedValue, BaseStats.ModType.ADDED, stat.tagList);
				parentBaseStats.ChangeStatModifier(stat.property, -stat.increasedValue, BaseStats.ModType.INCREASED, stat.tagList);
                foreach (float value in stat.moreValues)
                {
                    parentBaseStats.ChangeStatModifier(stat.property, value, BaseStats.ModType.QUOTIENT, stat.tagList);
                }
                foreach (float value in stat.quotientValues)
                {
                    parentBaseStats.ChangeStatModifier(stat.property, value, BaseStats.ModType.MORE, stat.tagList);
                }
            }
            parentBaseStats.UpdateStats();
        }
    }
}
