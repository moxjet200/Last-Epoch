using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonBearSkillTree : SkillTree
{

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.summonBear);
    }

    public override void updateMutator()
    {
        GameObject player = PlayerFinder.getPlayer();

        SummonBearMutator mutator = player.GetComponent<SummonBearMutator>();
        bool thornBear = false;
         bool Retaliates = false;
         float thornPoisonChance = 0f;
         float thornBurstBleedChance = 0f;
         float thornBurstAddedSpeed = 0f;
         float clawTotemOnKillChance = 0f;
         float ThornAttackChanceForDoubleDamage = 0f;
         float ThornAttackChanceToShredArmour = 0f;
         bool usesSwipe = false;
         float increasedSwipeCooldownRecovery = 0f;
        int thornAttackEnemiesToPierce = 0;
        float thornCooldown = 0f;
        float percentReducedRetaliationThreshold = 0f;

        List<TaggedStatsHolder.TaggableStat> statList = new List<TaggedStatsHolder.TaggableStat>();
        SkillTreeNode skillTreeNode;
        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            skillTreeNode = node.GetComponent<SkillTreeNode>();
            if (node.name == "Summon Bear Skill Tree Thorn Bear")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    thornBear = true;
                    percentReducedRetaliationThreshold = 0.6f;
                }
            }
            if (node.name == "Summon Bear Skill Tree Thorn Attack Double Damage")
            {
                ThornAttackChanceForDoubleDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }

            if (node.name == "Summon Bear Skill Tree Thorn Attack Pierce")
            {

                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.CastSpeed, null);
                    newStat.increasedValue = skillTreeNode.pointsAllocated  * -0.1f;
                    statList.Add(newStat);
                    thornAttackEnemiesToPierce += 10000;
                }
            }
            if (node.name == "Summon Bear Skill Tree Thorn Attack Armour Shred")
            {
                ThornAttackChanceToShredArmour += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Summon Bear Skill Tree Cast Speed")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.CastSpeed, null);
                    newStat.increasedValue = skillTreeNode.pointsAllocated * 0.08f;
                    statList.Add(newStat);
                }
            }
            if (node.name == "Summon Bear Skill Tree Retaliates")
            {
                Retaliates = (node.GetComponent<SkillTreeNode>().pointsAllocated > 0);
            }
            if (node.name == "Summon Bear Skill Tree Thorn Poison")
            {
                thornPoisonChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Summon Bear Skill Tree Thorn Attack Cooldown")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    thornCooldown += 4;
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Health, null);
                    newStat.moreValues.Add(0.08f);
                    statList.Add(newStat);
                }
            }
            if (node.name == "Summon Bear Skill Tree Thorn Burst Bleed")
            {
                thornBurstBleedChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Summon Bear Skill Tree Thorn Burst Speed")
            {
                thornBurstAddedSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 2.4f;
            }
            if (node.name == "Summon Bear Skill Tree Claw Totem")
            {
                clawTotemOnKillChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Summon Bear Skill Tree Swipe")
            {
                usesSwipe = (node.GetComponent<SkillTreeNode>().pointsAllocated > 0);
            }
            if (node.name == "Summon Bear Skill Tree Swipe Cooldown")
            {
                increasedSwipeCooldownRecovery += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.4f;
            }
            if (node.name == "Summon Bear Skill Tree Melee Damage And Stun")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                    tagList.Add(Tags.AbilityTags.Melee);
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, tagList);
                    newStat.increasedValue = skillTreeNode.pointsAllocated * 0.15f;
                    statList.Add(newStat);
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.IncreasedStunChance, tagList);
                    newStat2.addedValue = skillTreeNode.pointsAllocated * 0.15f;
                    statList.Add(newStat2);
                }
            }
            if (node.name == "Summon Bear Skill Tree Added Health And Tenacity")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Health, null);
                    newStat.addedValue = skillTreeNode.pointsAllocated * 100;
                    statList.Add(newStat);
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.Tenacity, null);
                    newStat2.addedValue = skillTreeNode.pointsAllocated * 100;
                    statList.Add(newStat2);
                }
            }
            if (node.name == "Summon Bear Skill Tree Damage And Bleed Chance")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.BleedChance, null);
                    newStat.addedValue = skillTreeNode.pointsAllocated * 0.1f;
                    statList.Add(newStat);
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, null);
                    newStat2.increasedValue = skillTreeNode.pointsAllocated * 0.15f;
                    statList.Add(newStat2);
                }
            }
            if (node.name == "Summon Bear Skill Tree Armour")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Armour, null);
                    newStat.addedValue = skillTreeNode.pointsAllocated * 150f;
                    statList.Add(newStat);
                }
            }
            if (node.name == "Summon Bear Skill Tree Crit Chance")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.CriticalChance, null);
                    newStat.increasedValue = skillTreeNode.pointsAllocated * 0.4f;
                    statList.Add(newStat);
                }
            }
            if (node.name == "Summon Bear Skill Tree Crit Damage")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.CriticalMultiplier, null);
                    newStat.increasedValue = skillTreeNode.pointsAllocated * 0.3f;
                    statList.Add(newStat);
                }
            }
        }

        // update the mutator
        mutator.thornBear = thornBear;
        mutator.Retaliates = Retaliates;
        mutator.thornPoisonChance = thornPoisonChance;
        mutator.thornBurstBleedChance = thornBurstBleedChance;
        mutator.clawTotemOnKillChance = clawTotemOnKillChance;
        mutator.thornBurstAddedSpeed = thornBurstAddedSpeed;
        mutator.ThornAttackChanceForDoubleDamage = ThornAttackChanceForDoubleDamage;
        mutator.ThornAttackChanceToShredArmour = ThornAttackChanceToShredArmour;
        mutator.usesSwipe = usesSwipe;
        mutator.increasedSwipeCooldownRecovery = increasedSwipeCooldownRecovery;
        mutator.thornAttackEnemiesToPierce = thornAttackEnemiesToPierce;
        mutator.thornCooldown = thornCooldown;
        mutator.percentReducedRetaliationThreshold = percentReducedRetaliationThreshold;

        // update existing bears
        if (player.GetComponent<SummonTracker>())
        {

            // get a list of existing bears
            List<SummonChangeTracker> bears = new List<SummonChangeTracker>();
            foreach (Summoned summon in player.GetComponent<SummonTracker>().summons)
            {
                if (summon.GetComponent<CreationReferences>() && summon.GetComponent<CreationReferences>().thisAbility == ability && summon.GetComponent<SummonChangeTracker>())
                {
                    bears.Add(summon.GetComponent<SummonChangeTracker>());
                }
            }

            // update the bears
            foreach (SummonChangeTracker bear in bears)
            {
                bear.changeStats(statList);
            }
        }

    }
}