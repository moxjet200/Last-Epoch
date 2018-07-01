using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAdapter : EntityAdapter
{
    public List<TaggedStatsHolder.TaggableStat> tempStats = new List<TaggedStatsHolder.TaggableStat>();

    public override GameObject adapt(GameObject entity)
    {
        // apply temporary stat buffs
        StatBuffs statBuffs = Comp<StatBuffs>.GetOrAdd(entity);
        TaggedStatsHolder.TaggableStat stat = null;
        TaggedBuff buff;
        for (int i = 0; i < tempStats.Count; i++)
        {
            stat = new TaggedStatsHolder.TaggableStat(tempStats[i]);
            buff = new TaggedBuff(stat, 15f);
            statBuffs.addTaggedBuff(buff);
        }

        return base.adapt(entity);
    }



}