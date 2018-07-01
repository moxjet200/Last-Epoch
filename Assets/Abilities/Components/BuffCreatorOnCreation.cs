using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffCreatorOnCreation : OnCreation {

    public List<Buff> simpleBuffs = new List<Buff>();
    public List<TaggedBuff> taggedBuffs = new List<TaggedBuff>();

    // Use this for initialization
    public override void onCreation () {
        CreationReferences references = GetComponent<CreationReferences>();
        if (references.creator) {
            Apply(references.creator);
        }
	}


    public void Apply(GameObject go)
    {
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
            simpleBuffs.Add(new Buff(debuff, duration, _name));
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
            taggedBuffs.Add(new TaggedBuff(debuff, duration, _name));
        }
    }

}
