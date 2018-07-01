using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonSprigganMutator : AbilityMutator
{
    // non mutator
    public float vineCooldownRecovery = 0f;
    public float rootsCooldownRecovery = 0f;
    float baseVineCooldown = 5f;
    float baseRootsCooldown = 15f;

    // aura
    public List<TaggedStatsHolder.TaggableStat> extraLeafShieldStats = new List<TaggedStatsHolder.TaggableStat>();

    // vale orb
    public int orbExtraProjectiles = 0;
    public int orbTargetsToPierce = 0;
    public float orbIncreasedCastSpeed = 0f;
    public bool orbFireInSequence = false;
    public float orbChanceForDoubleDamage = 0f;
    public float orbIncreasedDamage = 0f;
    public float orbMoreDamageAgainstPoisoned = 0f;
    public float orbMoreDamageAgainstBleeding = 0f;

    // entangling roots
    public bool castsEntanglingRoots = false;
    public float rootsIncreasedDamage = 0f;
    public float rootsIncreasedRadius = 0f;
    public int rootsAddedPatches = 0;
    public bool rootsPatchesInLine = false;
    public float rootsVineOnKillChance = 0f;
    public float rootsIncreasedDuration = 0f;
    public float rootsHealingNovaChance = 0f;

    // vines
    public bool summonsVines = false;
    public int extraVines = 0;
    public float increasedVineCooldown = 0f;


    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.summonSpriggan);
        base.Awake();
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        // get a reference to the child's SummonEntityOnDeath component, this the component that makes the wolf
        SummonEntityOnDeath summoner = abilityObject.GetComponent<SummonEntityOnDeath>();

        SprigganAdapter adapter = abilityObject.AddComponent<SprigganAdapter>();

        if (castsEntanglingRoots)
        {
            adapter.extraAbilities.Add(new EntityAdapter.ExtraAbility(AbilityIDList.getAbility(AbilityID.entanglingRoots), 1f, (1f / baseRootsCooldown) * (1 + rootsCooldownRecovery), 0f, 5f, 6f, 11f, false));
        }

        if (summonsVines)
        {
            adapter.extraAbilities.Add(new EntityAdapter.ExtraAbility(AbilityIDList.getAbility(AbilityID.summonVineAtTarget), 1f, (1f / baseVineCooldown) * (1 + vineCooldownRecovery) / (1 + increasedVineCooldown), 0f, 9f, 10f, 11f, false));
        }


        adapter.extraLeafShieldStats = new List<TaggedStatsHolder.TaggableStat>();
        adapter.extraLeafShieldStats.AddRange(extraLeafShieldStats);

        // vale orb
        adapter.orbExtraProjectiles = orbExtraProjectiles;
        adapter.orbTargetsToPierce = orbTargetsToPierce;
        adapter.orbIncreasedCastSpeed = orbIncreasedCastSpeed;
        adapter.orbFireInSequence = orbFireInSequence;
        adapter.orbChanceForDoubleDamage = orbChanceForDoubleDamage;
        adapter.orbIncreasedDamage = orbIncreasedDamage;
        adapter.orbMoreDamageAgainstPoisoned = orbMoreDamageAgainstPoisoned;
        adapter.orbMoreDamageAgainstBleeding = orbMoreDamageAgainstBleeding;

        // entangling roots
        adapter.castsEntanglingRoots = castsEntanglingRoots;
        adapter.rootsIncreasedDamage = rootsIncreasedDamage;
        adapter.rootsIncreasedRadius = rootsIncreasedRadius;
        adapter.rootsAddedPatches = rootsAddedPatches;
        adapter.rootsPatchesInLine = rootsPatchesInLine;
        adapter.rootsVineOnKillChance = rootsVineOnKillChance;
        adapter.rootsIncreasedDuration = rootsIncreasedDuration;
        adapter.rootsHealingNovaChance = rootsHealingNovaChance;

        // vines
        adapter.summonsVines = summonsVines;
        adapter.extraVines = extraVines;

        return abilityObject;
    }



}
