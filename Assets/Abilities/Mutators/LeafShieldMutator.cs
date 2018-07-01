using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafShieldMutator : AbilityMutator
{
    public List<TaggedStatsHolder.TaggableStat> extraStats = new List<TaggedStatsHolder.TaggableStat>();

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.leafShield);
        base.Awake();
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        if (extraStats != null && extraStats.Count > 0)
        {
            BuffParent component = abilityObject.GetComponent<BuffParent>();
            if (component)
            {
                component.taggedStats.AddRange(extraStats);
            }
        }

        return abilityObject;
    }
}
