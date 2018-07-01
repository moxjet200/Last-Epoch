using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonSprigganSkillTree : SkillTree
{

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.summonSpriggan);
    }

    public override void updateMutator()
    {
        GameObject player = PlayerFinder.getPlayer();

        SummonSprigganMutator mutator = player.GetComponent<SummonSprigganMutator>();
        float vineCooldownRecovery = 0f;
        float increasedVineCooldown = 0f;
          float rootsCooldownRecovery = 0f;

        // aura
          List<TaggedStatsHolder.TaggableStat> extraLeafShieldStats = new List<TaggedStatsHolder.TaggableStat>();

        // vale orb
          int orbExtraProjectiles = 0;
          int orbTargetsToPierce = 0;
          float orbIncreasedCastSpeed = 0f;
          bool orbFireInSequence = false;
          float orbChanceForDoubleDamage = 0f;
          float orbIncreasedDamage = 0f;
          float orbMoreDamageAgainstPoisoned = 0f;
          float orbMoreDamageAgainstBleeding = 0f;

        // entangling roots
          bool castsEntanglingRoots = false;
          float rootsIncreasedDamage = 0f;
          float rootsIncreasedRadius = 0f;
          int rootsAddedPatches = 0;
          bool rootsPatchesInLine = false;
          float rootsVineOnKillChance = 0f;
          float rootsIncreasedDuration = 0f;
          float rootsHealingNovaChance = 0f;

        // vines
        bool summonsVines = false;
        int extraVines = 0;

    List<TaggedStatsHolder.TaggableStat> statList = new List<TaggedStatsHolder.TaggableStat>();
        SkillTreeNode skillTreeNode;
        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            skillTreeNode = node.GetComponent<SkillTreeNode>();
            if (node.name == "Spriggan Tree Summons Vines")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    summonsVines = true;
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.CastSpeed, null);
                    newStat.increasedValue = skillTreeNode.pointsAllocated * -0.1f;
                    statList.Add(newStat);
                }
            }
            if (node.name == "Spriggan Tree Vines Cooldown")
            {
                vineCooldownRecovery += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
            }
            if (node.name == "Spriggan Tree Three Vines")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    increasedVineCooldown += node.GetComponent<SkillTreeNode>().pointsAllocated * 1f;
                    extraVines += node.GetComponent<SkillTreeNode>().pointsAllocated * 1;
                }
            }
            if (node.name == "Spriggan Tree Vine Health")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                    tagList.Add(Tags.AbilityTags.Minion);
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Health, tagList);
                    newStat.increasedValue = skillTreeNode.pointsAllocated * 0.3f;
                    statList.Add(newStat);
                }
            }
            if (node.name == "Spriggan Tree Vine Damage")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                    tagList.Add(Tags.AbilityTags.Minion);
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, tagList);
                    newStat.increasedValue = skillTreeNode.pointsAllocated * 0.2f;
                    statList.Add(newStat);
                }
            }
            if (node.name == "Spriggan Tree Aura Regen")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.HealthRegen, null);
                    newStat.increasedValue = skillTreeNode.pointsAllocated * 0.25f;
                    extraLeafShieldStats.Add(newStat);
                }
            }
            if (node.name == "Spriggan Tree Aura Damage")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, null);
                    newStat.increasedValue = skillTreeNode.pointsAllocated * 0.1f;
                    extraLeafShieldStats.Add(newStat);
                }
            }
            if (node.name == "Spriggan Tree Aura Life On Kill")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                    tagList.Add(Tags.AbilityTags.Kill);
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.LifeGain, null);
                    newStat.addedValue = skillTreeNode.pointsAllocated * 5f;
                    extraLeafShieldStats.Add(newStat);
                }
            }
            if (node.name == "Spriggan Tree Health")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Health, null);
                    newStat.increasedValue = skillTreeNode.pointsAllocated * 0.15f;
                    statList.Add(newStat);
                }
            }
            if (node.name == "Spriggan Tree Armour And Protections")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Armour, null);
                    newStat.addedValue = skillTreeNode.pointsAllocated * 50f;
                    statList.Add(newStat);
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.ElementalProtection, null);
                    newStat2.addedValue = skillTreeNode.pointsAllocated * 50f;
                    statList.Add(newStat2);
                }
            }
            if (node.name == "Spriggan Tree Uses Entangling Roots")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    castsEntanglingRoots = true;
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.CastSpeed, null);
                    newStat.increasedValue = skillTreeNode.pointsAllocated * -0.1f;
                    statList.Add(newStat);
                }
            }
            if (node.name == "Spriggan Tree Entangling Roots Damage")
            {
                rootsIncreasedDamage = skillTreeNode.pointsAllocated * 0.25f;
            }
            if (node.name == "Spriggan Tree Entangling Roots Patches")
            {
                rootsAddedPatches += node.GetComponent<SkillTreeNode>().pointsAllocated;
                rootsIncreasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * -0.2f;
            }
            if (node.name == "Spriggan Tree Entangling Roots Line")
            {
                rootsPatchesInLine = (node.GetComponent<SkillTreeNode>().pointsAllocated > 0);
            }
            if (node.name == "Spriggan Tree Entangling Roots Duration")
            {
                rootsIncreasedDuration += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Spriggan Tree Entangling Roots Healing Nova")
            {
                rootsHealingNovaChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.5f;
            }
            if (node.name == "Spriggan Tree Entangling Roots Cooldown")
            {
                rootsCooldownRecovery += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.35f;
                rootsIncreasedDuration += node.GetComponent<SkillTreeNode>().pointsAllocated * -0.15f;
            }
            if (node.name == "Spriggan Tree Spell Damage")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                    tagList.Add(Tags.AbilityTags.Spell);
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, null);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * 0.12f);
                    statList.Add(newStat);
                }
            }
            if (node.name == "Spriggan Tree Orb Projectiles")
            {
                orbExtraProjectiles += node.GetComponent<SkillTreeNode>().pointsAllocated * 2;
                orbIncreasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * -0.2f;
            }
            if (node.name == "Spriggan Tree Orb Sequence")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    orbFireInSequence = true;
                    orbIncreasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * -0.2f;
                }
            }
            if (node.name == "Spriggan Tree Orb VS Bleeding")
            {
                orbMoreDamageAgainstBleeding += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.4f;
            }
            if (node.name == "Spriggan Tree Orb VS Poison")
            {
                orbMoreDamageAgainstPoisoned += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.4f;
            }
        }

        // update the mutator
        mutator.vineCooldownRecovery = vineCooldownRecovery;
        mutator.rootsCooldownRecovery = rootsCooldownRecovery;

        // aura
        mutator.extraLeafShieldStats = extraLeafShieldStats;

        // vale orb
        mutator.orbExtraProjectiles = orbExtraProjectiles;
        mutator.orbTargetsToPierce = orbTargetsToPierce;
        mutator.orbIncreasedCastSpeed = orbIncreasedCastSpeed;
        mutator.orbFireInSequence = orbFireInSequence;
        mutator.orbChanceForDoubleDamage = orbChanceForDoubleDamage;
        mutator.orbIncreasedDamage = orbIncreasedDamage;
        mutator.orbMoreDamageAgainstPoisoned = orbMoreDamageAgainstPoisoned;
        mutator.orbMoreDamageAgainstBleeding = orbMoreDamageAgainstBleeding;

        // entangling roots
        mutator.castsEntanglingRoots = castsEntanglingRoots;
        mutator.rootsIncreasedDamage = rootsIncreasedDamage;
        mutator.rootsIncreasedRadius = rootsIncreasedRadius;
        mutator.rootsAddedPatches = rootsAddedPatches;
        mutator.rootsPatchesInLine = rootsPatchesInLine;
        mutator.rootsVineOnKillChance = rootsVineOnKillChance;
        mutator.rootsIncreasedDuration = rootsIncreasedDuration;
        mutator.rootsHealingNovaChance = rootsHealingNovaChance;

        // vines
        mutator.summonsVines = summonsVines;
        mutator.extraVines = extraVines;
        mutator.increasedVineCooldown = increasedVineCooldown;

        // update existing spriggans
        if (player.GetComponent<SummonTracker>())
        {

            // get a list of existing bears
            List<SummonChangeTracker> spriggans = new List<SummonChangeTracker>();
            foreach (Summoned summon in player.GetComponent<SummonTracker>().summons)
            {
                if (summon.GetComponent<CreationReferences>() && summon.GetComponent<CreationReferences>().thisAbility == ability && summon.GetComponent<SummonChangeTracker>())
                {
                    spriggans.Add(summon.GetComponent<SummonChangeTracker>());
                }
            }

            // update the bears
            foreach (SummonChangeTracker spriggan in spriggans)
            {
                spriggan.changeStats(statList);
            }
        }

    }
}