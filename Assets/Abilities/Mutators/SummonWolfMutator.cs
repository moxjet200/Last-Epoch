using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonWolfMutator : AbilityMutator {

    
    public float additionalDuration = 0f;

    public int wolfLimit = 1;

    public bool dontFollow = false;

    public bool limittedWolfDuration = false;

    public List<TaggedStatsHolder.TaggableStat> statList = new List<TaggedStatsHolder.TaggableStat>();

    public bool leaps = false;
    public float increasedLeapCooldownRecovery = 0f;

    public bool retaliatesWithLightning = false;

    public int extraWolvesFromItems = 0;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.summonWolf);
        base.Awake();
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        // get a reference to the child's SummonEntityOnDeath component, this the component that makes the wolf
        SummonEntityOnDeath summoner = abilityObject.GetComponent<SummonEntityOnDeath>();
        
        // add additional duration
        summoner.duration += additionalDuration;

        // add additional stats
        summoner.statList.AddRange(statList);
        summoner.limit = wolfLimit + extraWolvesFromItems;

        // there are a limitted number of wolves then apply the wolf limit
        if (limittedWolfDuration)
        {
            summoner.limitNumber = false;
            summoner.limitDuration = true;
        }

        // if the wolves don't follow, then stop them following
        if (dontFollow)
        {
            summoner.followsCreator = false;
        }

        WolfAdapter adapter = abilityObject.AddComponent<WolfAdapter>();
        adapter.retaliatesWithLightning = retaliatesWithLightning;

        // give the wolf weak leap if necessary
        if (leaps)
        {
            adapter.extraAbilities.Add(new EntityAdapter.ExtraAbility(AbilityIDList.getAbility(AbilityID.fastWeakLeap), 0,
                -AbilityIDList.getAbility(AbilityID.fastWeakLeap).chargesGainedPerSecond + 0.1f * (1 + increasedLeapCooldownRecovery), 5f, 11f, 11.6f, 11.8f, false));
        }

        // give the wolf lightning retaliation if necessary
        if (retaliatesWithLightning)
        {
            adapter.retaliators.Add(new EntityAdapter.Retaliator(AbilityIDList.getAbility(AbilityID.lightning), 50, 0));
        }

        return abilityObject;
    }
}
