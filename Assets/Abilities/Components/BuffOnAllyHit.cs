using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HitDetector))]
[RequireComponent(typeof(AbilityObjectConstructor))]
public class BuffOnAllyHit : MonoBehaviour
{
    public List<Buff> simpleBuffs = new List<Buff>();
    public List<TaggedBuff> taggedBuffs = new List<TaggedBuff>();

    public bool canApplyToSameAllyAgain = false;
    public bool onlyApplyToCreatorsMinions = false;
    public bool onlyApplyToMinionFromAbility = false;
    public Ability requiredAbility = null;

    CreationReferences myReferences = null;

    // I'm working Jerle!
    void Start()
    {
        // subscribe to the new enemy hit event
        GetComponent<HitDetector>().newAllyHitEvent += Apply;
        // if it can hit the same ally again also subscribe to the ally hit again event
        if (canApplyToSameAllyAgain) { GetComponent<HitDetector>().allyHitAgainEvent += Apply; }

        // only get references if needed
        if (onlyApplyToCreatorsMinions)
        {
            myReferences = GetComponent<CreationReferences>();
        }
    }

    public void Apply(GameObject go)
    {
        // check for minion statuses
        if (onlyApplyToCreatorsMinions)
        {
            // check for creator
            CreationReferences refences = go.GetComponent<CreationReferences>();
            if (!refences) { return; }
            if (refences.creator != myReferences.creator)
            {
                return;
            }

            // check for ability
            if (onlyApplyToMinionFromAbility)
            {
                if (requiredAbility == null) { return; }
                if (refences.thisAbility != requiredAbility) { return; }
            }
        }
        // also check for ability if the creator does not need to be checked
        else if (onlyApplyToMinionFromAbility)
        {
            if (requiredAbility == null) { return; }
            CreationReferences refences = go.GetComponent<CreationReferences>();
            if (!refences) { return; }
            if (refences.thisAbility != requiredAbility) { return; }
        }

        StatBuffs buffs = go.GetComponent<StatBuffs>();
        if (buffs == null) { buffs = go.AddComponent<StatBuffs>(); }

        foreach (Buff buff in simpleBuffs)
        {
            buffs.addBuff(buff.remainingDuration, buff.stat.property, buff.stat.addedValue, buff.stat.increasedValue, buff.stat.moreValues, buff.stat.quotientValues, null, buff.name);
        }
        foreach (TaggedBuff buff in taggedBuffs)
        {
            buffs.addBuff(buff.remainingDuration, buff.stat.property, buff.stat.addedValue, buff.stat.increasedValue, buff.stat.moreValues, buff.stat.quotientValues, buff.stat.tagList, buff.name);
        }
    }

    public void addBuffToList(Tags.Properties property, float addedValue, float increasedValue, List<float> moreValues, List<float> quotientValues, float duration, List<Tags.AbilityTags> tags = null, string _name = "")
    {
        if (tags == null)
        {
            TaggedStatsHolder.Stat buff = new TaggedStatsHolder.Stat(property);
            buff.addedValue = addedValue;
            buff.increasedValue = increasedValue;
            if (moreValues != null)
            {
                buff.moreValues.AddRange(moreValues);
            }
            if (quotientValues != null)
            {
                buff.quotientValues.AddRange(quotientValues);
            }
            simpleBuffs.Add(new Buff(buff, duration, _name));
        }
        else
        {
            TaggedStatsHolder.TaggableStat buff = new TaggedStatsHolder.TaggableStat(property, tags);
            buff.addedValue = addedValue;
            buff.increasedValue = increasedValue;
            if (moreValues != null)
            {
                buff.moreValues.AddRange(moreValues);
            }
            if (quotientValues != null)
            {
                buff.quotientValues.AddRange(quotientValues);
            }
            taggedBuffs.Add(new TaggedBuff(buff, duration, _name));
        }
    }


}
