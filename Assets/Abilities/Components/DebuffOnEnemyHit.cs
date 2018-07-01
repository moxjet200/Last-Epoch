using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HitDetector))]
[RequireComponent(typeof(AbilityObjectConstructor))]
public class DebuffOnEnemyHit : MonoBehaviour
{
    public List<Buff> simpleDebuff = new List<Buff>();
    public List<TaggedBuff> taggedDebuffs = new List<TaggedBuff>();

    public bool canApplyToSameEnemyAgain = false;


    // Use this for initialization
    void Start()
    {

        // subscribe to the new enemy hit event
        GetComponent<HitDetector>().newEnemyHitEvent += Apply;
        // if it can damage the same enemy again also subscribe to the enemy hit again event
        if (canApplyToSameEnemyAgain) { GetComponent<HitDetector>().enemyHitAgainEvent += Apply; }
    }

    public void Apply(GameObject go)
    {
        StatBuffs buffs = go.GetComponent<StatBuffs>();
        if (buffs == null) { buffs = go.AddComponent<StatBuffs>(); }

        foreach (Buff debuff in simpleDebuff)
        {
            buffs.addBuff(debuff.remainingDuration, debuff.stat.property, -debuff.stat.addedValue, -debuff.stat.increasedValue, debuff.stat.quotientValues, debuff.stat.moreValues, null, debuff.name);
        }
        foreach (TaggedBuff debuff in taggedDebuffs)
        {
            buffs.addBuff(debuff.remainingDuration, debuff.stat.property, -debuff.stat.addedValue, -debuff.stat.increasedValue, debuff.stat.quotientValues, debuff.stat.moreValues, debuff.stat.tagList, debuff.name);
        }
    }

    public void addDebuffToList(Tags.Properties property, float addedValue, float increasedValue, List<float> moreValues, List<float> quotientValues, float duration, List<Tags.AbilityTags> tags = null, string _name = "")
    {
        if (tags == null)
        {
            TaggedStatsHolder.Stat debuff = new TaggedStatsHolder.Stat(property);
            debuff.addedValue = addedValue;
            debuff.increasedValue = increasedValue;
            if (moreValues != null)
            {
                debuff.moreValues.AddRange(moreValues);
            }
            if (quotientValues != null)
            {
                debuff.quotientValues.AddRange(quotientValues);
            }
            simpleDebuff.Add(new Buff(debuff, duration, _name));
        }
        else
        {
            TaggedStatsHolder.TaggableStat debuff = new TaggedStatsHolder.TaggableStat(property, tags);
            debuff.addedValue = addedValue;
            debuff.increasedValue = increasedValue;
            if (moreValues != null)
            {
                debuff.moreValues.AddRange(moreValues);
            }
            if (quotientValues != null)
            {
                debuff.quotientValues.AddRange(quotientValues);
            }
            taggedDebuffs.Add(new TaggedBuff(debuff, duration, _name));
        }
    }


}