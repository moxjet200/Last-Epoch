using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprigganAdapter : EntityAdapter
{
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


    public override GameObject adapt(GameObject entity)
    {
        // aura
        if (extraLeafShieldStats != null && extraLeafShieldStats.Count > 0)
        {
            LeafShieldMutator mutator = entity.AddComponent<LeafShieldMutator>();
            mutator.extraStats.AddRange(extraLeafShieldStats);
        }

        // roots
        if (castsEntanglingRoots)
        {
            EntanglingRootsMutator mutator = entity.AddComponent<EntanglingRootsMutator>();
            mutator.increasedDamage = rootsIncreasedDamage;
            mutator.increasedRadius = rootsIncreasedRadius;
            mutator.addedPatches = rootsAddedPatches;
            mutator.patchesInLine = rootsPatchesInLine;
            mutator.vineOnKillChance = rootsVineOnKillChance;
            mutator.increasedDuration = rootsIncreasedDuration;
            mutator.healingNovaChance = rootsHealingNovaChance;
        }

        // vale orb mutator
        ValeOrbMutator orbMutator = entity.AddComponent<ValeOrbMutator>();
        orbMutator.extraProjectiles = orbExtraProjectiles;
        orbMutator.targetsToPierce = orbTargetsToPierce;
        orbMutator.increasedCastSpeed = orbIncreasedCastSpeed;
        orbMutator.fireInSequence = orbFireInSequence;
        orbMutator.chanceForDoubleDamage = orbChanceForDoubleDamage;
        orbMutator.increasedDamage = orbIncreasedDamage;
        orbMutator.moreDamageAgainstPoisoned = orbMoreDamageAgainstPoisoned;
        orbMutator.moreDamageAgainstBleeding = orbMoreDamageAgainstBleeding;

        // vines
        if (summonsVines)
        {
            SummonVineAtTargetMutator mutator = entity.AddComponent<SummonVineAtTargetMutator>();
            mutator.extraVines = extraVines;
        }

        return base.adapt(entity);
    }



}