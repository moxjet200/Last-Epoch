using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornTotemMutator : AbilityMutator
{

    public List<TaggedStatsHolder.TaggableStat> statList = new List<TaggedStatsHolder.TaggableStat>();

    public float poisonCloudChance = 0f;
    public float additionalDuration = 0f;
    public int totemLimit = 2;
    public int extraProjectiles = 0;
    public bool homing = false;
    public float chanceForDoubleDamage = 0f;
    public float chanceToShredArmour = 0f;
    public float chanceToPoison = 0f;
    public float reducedSpread = 0f;
    public float increasedThornTotemAttackDamage = 0f;
    public int targetsToPierce = 0;
    public float increasedSpeed = 0f;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.summonThornTotem);
        base.Awake();
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        // get a reference to the child's SummonEntityOnDeath component, this the component that makes the wolf
        SummonEntityOnDeath summoner = abilityObject.GetComponent<SummonEntityOnDeath>();

        ThornTotemAdapter adapter = abilityObject.AddComponent<ThornTotemAdapter>();

        // add a poison cloud on death if necessary
        if (poisonCloudChance > 0)
        {
            adapter.poisonCloudChance = poisonCloudChance;
        }

        // adapter variables
        adapter.extraProjectiles = extraProjectiles;
        adapter.homing = homing;
        adapter.chanceForDoubleDamage = chanceForDoubleDamage;
        adapter.chanceToShredArmour = chanceToShredArmour;
        adapter.chanceToPoison = chanceToPoison;
        adapter.reducedSpread = reducedSpread;
        adapter.increasedThornTotemAttackDamage = increasedThornTotemAttackDamage;
        adapter.targetsToPierce = targetsToPierce;
        adapter.increasedSpeed = increasedSpeed;


        // add additional duration
        summoner.duration += additionalDuration;

        // set the maximum number of totems
        summoner.limit = totemLimit;

        return abilityObject;
    }



}
