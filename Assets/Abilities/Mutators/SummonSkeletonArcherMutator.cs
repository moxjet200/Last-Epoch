using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonSkeletonArcherMutator : AbilityMutator
{


    public float additionalDuration = 0f;

    public int additionalSkeletons = 0;

    public bool limitDuration = false;

    public List<TaggedStatsHolder.TaggableStat> statList = new List<TaggedStatsHolder.TaggableStat>();

    public List<TaggedStatsHolder.TaggableStat> tempStats = new List<TaggedStatsHolder.TaggableStat>();

    public bool usesPoisonArrow = false;
    public float increasedPoisonArrowCooldownSpeed = 0f;
    public float increasedPoisonArrowCooldown = 0f;

    public bool poisonArrowPierces = false;
    public bool poisonArrowInaccurate = false;
    public float poisonArrowIncreasedDamage = 0f;
    public int poisonArrowDelayedAttacks = 0;

    public bool usesMultishot = false;
    public float increasedMultishotCooldownSpeed = 0f;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.summonSkeletonArcher);
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

        // change the limit type
        if (limitDuration)
        {
            summoner.limitNumber = false;
            summoner.limitDuration = true;
        }

        SkeletonArcherAdapter adapter = abilityObject.AddComponent<SkeletonArcherAdapter>();
        adapter.tempStats.AddRange(tempStats);
        adapter.pierces = poisonArrowPierces;
        adapter.inaccurate = poisonArrowInaccurate;
        adapter.increasedDamage = poisonArrowIncreasedDamage;
        adapter.delayedAttacks = poisonArrowDelayedAttacks;
        adapter.usesPoisonArrow = usesPoisonArrow;

        // add extra abilities
        if (usesMultishot)
        {
            adapter.extraAbilities.Add(new EntityAdapter.ExtraAbility(AbilityIDList.getAbility(AbilityID.multishot), 1, 0.17f * (1 + increasedMultishotCooldownSpeed), 0f, 8f, 9f, 11.8f, false));
        }

        // avoid dividing by 0
        if (increasedPoisonArrowCooldown == -1) { increasedPoisonArrowCooldown = -0.9f; }

        // add extra abilities
        if (usesPoisonArrow)
        {
            adapter.extraAbilities.Add(new EntityAdapter.ExtraAbility(AbilityIDList.getAbility(AbilityID.poisonArrow), 1, 0.25f * (1 + increasedPoisonArrowCooldownSpeed) / (1 + increasedPoisonArrowCooldown), 0f, 8f, 9f, 11.8f, false));
        }

        return abilityObject;
    }
}
