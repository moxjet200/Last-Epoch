using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonSkeletonWarriorMutator : AbilityMutator
{


    public float additionalDuration = 0f;

    public int additionalSkeletons = 0;

    public bool limitDuration = false;

    public List<TaggedStatsHolder.TaggableStat> statList = new List<TaggedStatsHolder.TaggableStat>();

    public List<TaggedStatsHolder.TaggableStat> tempStats = new List<TaggedStatsHolder.TaggableStat>();

    public bool usesDeathSlash = false;
    public float increasedDeathSlashCooldownRecovery = 0f;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.summonSkeletonWarrior);
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
        summoner.limit = 3 + additionalSkeletons;
        
        // change limit type
        if (limitDuration)
        {
            summoner.limitNumber = false;
            summoner.limitDuration = true;
        }

        // change the adapter
        SkeletonAdapter adapter = abilityObject.AddComponent<SkeletonAdapter>();
        adapter.tempStats = tempStats;

        // add extra abilities
        if (usesDeathSlash)
        {
            adapter.extraAbilities.Add(new EntityAdapter.ExtraAbility(AbilityIDList.getAbility(AbilityID.deathSlash), 1, 0.2f * (1 + increasedDeathSlashCooldownRecovery), 0f, 1.2f, 1.6f, 11.8f, false));
        }

        return abilityObject;
    }
}
