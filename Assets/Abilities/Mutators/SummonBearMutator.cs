using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonBearMutator : AbilityMutator
{

    public List<TaggedStatsHolder.TaggableStat> statList = new List<TaggedStatsHolder.TaggableStat>();

    public bool thornBear = false;

    public bool Retaliates = false;
    public float thornPoisonChance = 0f;
    public float thornBurstBleedChance = 0f;
    public float thornBurstAddedSpeed = 0f;

    public float clawTotemOnKillChance = 0f;

    public float ThornAttackChanceForDoubleDamage = 0f;
    public float ThornAttackChanceToShredArmour = 0f;
    public int thornAttackEnemiesToPierce = 0;
    public float thornCooldown = 0f;
    public float percentReducedRetaliationThreshold = 0f;

    public bool usesSwipe = false;
    public float increasedSwipeCooldownRecovery = 0f;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.summonBear);
        base.Awake();
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        // get a reference to the child's SummonEntityOnDeath component, this the component that makes the wolf
        SummonEntityOnDeath summoner = abilityObject.GetComponent<SummonEntityOnDeath>();

        BearAdapter adapter = abilityObject.AddComponent<BearAdapter>();

        // give the wolf weak leap if necessary
        if (thornBear)
        {
            adapter.thornBear = true;

            // Add ability
            if (thornCooldown > 0)
            {
                adapter.extraAbilities.Add(new EntityAdapter.ExtraAbility(AbilityIDList.getAbility(AbilityID.thornTotemAttack), 1f, 1f/thornCooldown, 2f, 7f, 8f, 11f, false));
            }
            else
            {
                adapter.extraAbilities.Add(new EntityAdapter.ExtraAbility(AbilityIDList.getAbility(AbilityID.thornTotemAttack), 0, 0, 0, 7f, 8f, 11f, false));
            }
            adapter.ThornAttackChanceToShredArmour = ThornAttackChanceToShredArmour;
            adapter.ThornAttackChanceForDoubleDamage = ThornAttackChanceForDoubleDamage;
            adapter.thornAttackEnemiesToPierce = thornAttackEnemiesToPierce;
            
        }

        if (usesSwipe)
        {
            // Add ability
            adapter.extraAbilities.Add(new EntityAdapter.ExtraAbility(AbilityIDList.getAbility(AbilityID.swipe), 1, 0.2f * (1 + increasedSwipeCooldownRecovery), 0, 1f, 1.3f, 4f, false));
            adapter.swipes = usesSwipe;
        }

        if (Retaliates)
        {
            adapter.retaliates = true;
            adapter.retaliators.Add(new EntityAdapter.Retaliator(AbilityIDList.getAbility(AbilityID.thornBurst), Mathf.RoundToInt(75f * (1f - percentReducedRetaliationThreshold)), Mathf.RoundToInt(75f * (1f - percentReducedRetaliationThreshold) * 0.75f)));
            adapter.thornBurstAddedSpeed = thornBurstAddedSpeed;
            adapter.thornBurstBleedChance = thornBurstBleedChance;
        }
        
        adapter.thornPoisonChance = thornPoisonChance;
        adapter.clawTotemOnKillChance = clawTotemOnKillChance;

        return abilityObject;
    }



}
