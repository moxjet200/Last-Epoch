using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodWraithMutator : AbilityMutator
{
    public float increasedSize = 0f;

    public List<TaggedStatsHolder.TaggableStat> statList = new List<TaggedStatsHolder.TaggableStat>();
    

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.summonBloodWraith);
        base.Awake();
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        // get a reference to the child's SummonEntityOnDeath component, this the component that makes the wolf
        SummonEntityOnDeath summoner = abilityObject.GetComponent<SummonEntityOnDeath>();

        // add additional stats
        summoner.statList.AddRange(statList);

        // change the adapter
        BloodWraithAdapter adapter = abilityObject.AddComponent<BloodWraithAdapter>();
        adapter.increasedSize = increasedSize;

        return abilityObject;
    }
}
