using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonWolfSkillTree : SkillTree {

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.summonWolf);
    }

    public override void updateMutator()
    {
        GameObject player = PlayerFinder.getPlayer();

        SummonWolfMutator mutator = player.GetComponent<SummonWolfMutator>();
        float additionalDuration = 0f;
        int wolfLimit = 1;
        bool limittedWolfDuration = false;
        bool dontFollow = false;
        float addedManaCostDivider = 0f;
        float increasedCooldownRecoverySpeed = 0f;
        float addedCharges = 0;
        bool leaps = false;
        float increasedLeapCooldownRecovery = 0f;
        bool retaliatesWithLightning = false;

    List<TaggedStatsHolder.TaggableStat> statList = new List<TaggedStatsHolder.TaggableStat>();
        SkillTreeNode skillTreeNode;
        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            skillTreeNode = node.GetComponent<SkillTreeNode>();
            if (node.name == "Summon Wolf Skill Tree Additional Duration")
            {
                additionalDuration += node.GetComponent<SkillTreeNode>().pointsAllocated * 4;
            }
            if (node.name == "Summon Wolf Skill Tree Added Health")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Health, null);
                    newStat.addedValue = skillTreeNode.pointsAllocated * 70;
                    statList.Add(newStat);
                }
            }
            if (node.name == "Summon Wolf Skill Tree Added Speed")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Movespeed, null);
                    newStat.increasedValue = skillTreeNode.pointsAllocated * 0.1f;
                    statList.Add(newStat);
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.AttackSpeed, null);
                    newStat2.increasedValue = skillTreeNode.pointsAllocated * 0.1f;
                    statList.Add(newStat2);
                }
            }
            if (node.name == "Summon Wolf Skill Tree Added Health Regen")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.HealthRegen, null);
                    newStat.increasedValue = skillTreeNode.pointsAllocated * 0.75f;
                    statList.Add(newStat);
                }
            }
            if (node.name == "Summon Wolf Skill Tree Wolf Limit")
            {
                limittedWolfDuration = (node.GetComponent<SkillTreeNode>().pointsAllocated > 0);
            }
            if (node.name == "Summon Wolf Skill Tree Additional Wolves")
            {
                wolfLimit += node.GetComponent<SkillTreeNode>().pointsAllocated * 1;
                addedCharges += node.GetComponent<SkillTreeNode>().pointsAllocated * 1;
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, null);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * -0.2f);
                    statList.Add(newStat);
                }
            }
            if (node.name == "Summon Wolf Skill Tree Dont Follow")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    dontFollow = true;
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Health, null);
                    newStat.addedValue = skillTreeNode.pointsAllocated * 300;
                    statList.Add(newStat);
                }
            }
            if (node.name == "Summon Wolf Skill Tree Mana")
            {
                addedManaCostDivider += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Summon Wolf Skill Tree Cooldown")
            {
                increasedCooldownRecoverySpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
            }
            if (node.name == "Summon Wolf Skill Tree Damage")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, null);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * 0.2f);
                    statList.Add(newStat);
                }
            }
            if (node.name == "Summon Wolf Skill Tree Damage And Cooldown")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, null);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * 0.6f);
                    statList.Add(newStat);
                    increasedCooldownRecoverySpeed -= 0.3f;
                }
            }
            if (node.name == "Summon Wolf Skill Tree Health And Cooldown")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Health, null);
                    newStat.addedValue = skillTreeNode.pointsAllocated * 300;
                    statList.Add(newStat);
                    increasedCooldownRecoverySpeed -= 0.3f;
                }
            }
            if (node.name == "Summon Wolf Skill Tree Bleed Chance")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.BleedChance, null);
                    newStat.addedValue = skillTreeNode.pointsAllocated * 0.17f;
                    statList.Add(newStat);
                }
            }
            if (node.name == "Summon Wolf Skill Tree DoT Damage")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                    tagList.Add(Tags.AbilityTags.DoT);
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, tagList);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * 0.5f);
                    statList.Add(newStat);
                }
            }

            if (node.name == "Summon Wolf Skill Tree Dodge Rating")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.DodgeRating, null);
                    newStat.addedValue = skillTreeNode.pointsAllocated * 70f;
                    statList.Add(newStat);
                }
            }

            if (node.name == "Summon Wolf Skill Tree Melee Damage")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                    tagList.Add(Tags.AbilityTags.Melee);
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, tagList);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * 0.25f);
                    statList.Add(newStat);
                }
            }

            if (node.name == "Summon Wolf Skill Tree Leaps")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    leaps = true;
                }
            }

            if (node.name == "Summon Wolf Skill Tree Leap Cooldown")
            {
                increasedLeapCooldownRecovery += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.5f;
            }

            if (node.name == "Summon Wolf Skill Tree Lightning Retaliation")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    retaliatesWithLightning = true;
                    List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                    tagList.Add(Tags.AbilityTags.Lightning);
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.DamageTaken, tagList);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * 0.15f);
                    statList.Add(newStat);
                }
            }

            if (node.name == "Summon Wolf Skill Tree Lightning Damage")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                    tagList.Add(Tags.AbilityTags.Lightning);
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, tagList);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * 0.3f);
                    statList.Add(newStat);
                }
            }

            if (node.name == "Summon Wolf Skill Tree Melee Lightning Damage")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                    tagList.Add(Tags.AbilityTags.Lightning);
                    tagList.Add(Tags.AbilityTags.Melee);
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, tagList);
                    newStat.addedValue = skillTreeNode.pointsAllocated * 5f;
                    statList.Add(newStat);
                }
            }
        }

        // update the mutator
        mutator.additionalDuration = additionalDuration;
        mutator.wolfLimit = wolfLimit;
        mutator.limittedWolfDuration = limittedWolfDuration;
        mutator.dontFollow = dontFollow;
        mutator.addedManaCostDivider = addedManaCostDivider;
        mutator.addedChargeRegen = ability.chargesGainedPerSecond * increasedCooldownRecoverySpeed;
        mutator.addedCharges = addedCharges;
        mutator.statList.Clear();
        mutator.statList.AddRange(statList);
        mutator.leaps = leaps;
        mutator.increasedLeapCooldownRecovery = increasedLeapCooldownRecovery;
        mutator.retaliatesWithLightning = retaliatesWithLightning;

        // update existing wolves
        if (player.GetComponent<SummonTracker>()) {

            // get a list of existing wolves
            List<SummonChangeTracker> wolves = new List<SummonChangeTracker>();
            foreach (Summoned summon in player.GetComponent<SummonTracker>().summons)
            {
                if (summon.GetComponent<CreationReferences>() && summon.GetComponent<CreationReferences>().thisAbility == ability && summon.GetComponent<SummonChangeTracker>())
                {
                    wolves.Add(summon.GetComponent<SummonChangeTracker>());
                }
            }

            // update the wolves
            foreach (SummonChangeTracker wolf in wolves)
            {
                wolf.changeStats(statList);
                wolf.changeFollowsCreator(!dontFollow, player);
                wolf.changeLimitDuration(limittedWolfDuration, ability.abilityPrefab.GetComponent<SummonEntityOnDeath>().duration + additionalDuration);
            }
        }

    }
}
