using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornTotemTree : SkillTree
{

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.summonThornTotem);
    }

    public override void updateMutator()
    {
        GameObject player = PlayerFinder.getPlayer();

        ThornTotemMutator mutator = player.GetComponent<ThornTotemMutator>();
        float additionalDuration = 0f;
        int totemLimit = 2;
        float addedManaCostDivider = 0f;
        float increasedCooldownRecoverySpeed = 0f;
        float addedCharges = 0;
        float poisonCloudChance = 0f;
        int extraProjectiles = 0;
         bool homing = false;
         float chanceForDoubleDamage = 0f;
         float chanceToShredArmour = 0f;
         float chanceToPoison = 0f;
         float reducedSpread = 0f;
         float increasedThornTotemAttackDamage = 0f;
         int targetsToPierce = 0;
        float increasedManaCost = 0f;
        float increasedSpeed = 0f;

    List<TaggedStatsHolder.TaggableStat> statList = new List<TaggedStatsHolder.TaggableStat>();
        SkillTreeNode skillTreeNode;
        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            skillTreeNode = node.GetComponent<SkillTreeNode>();

            if (node.name == "Thorn Totem Tree More Totems")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    totemLimit += skillTreeNode.pointsAllocated * 1;
                    addedCharges += skillTreeNode.pointsAllocated * 1;
                    increasedManaCost += skillTreeNode.pointsAllocated * 0.4f;
                    additionalDuration += skillTreeNode.pointsAllocated * 3;
                }
            }
            if (node.name == "Thorn Totem Tree Cooldown")
            {
                increasedCooldownRecoverySpeed += skillTreeNode.pointsAllocated * 0.3f;
            }
            if (node.name == "Thorn Totem Tree Mana Efficiency")
            {
                addedManaCostDivider += skillTreeNode.pointsAllocated * 0.35f;
            }
            if (node.name == "Thorn Totem Tree Poison Cloud")
            {
                poisonCloudChance += skillTreeNode.pointsAllocated * 0.25f;
            }
            if (node.name == "Thorn Totem Tree Poison Chance")
            {
                chanceToPoison += skillTreeNode.pointsAllocated * 0.143f;
            }
            if (node.name == "Thorn Totem Tree Extra Projectiles")
            {
                extraProjectiles += skillTreeNode.pointsAllocated * 1;
            }
            if (node.name == "Thorn Totem Tree Homing")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    homing = true;
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.CastSpeed, null);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * -0.25f);
                    statList.Add(newStat);
                }
            }
            if (node.name == "Thorn Totem Tree Damage vs Cast Speed")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, null);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * 0.18f);
                    statList.Add(newStat);
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.CastSpeed, null);
                    newStat2.moreValues.Add(skillTreeNode.pointsAllocated * -0.05f);
                    statList.Add(newStat2);
                }
            }
            if (node.name == "Thorn Totem Tree Armour Shred Chance")
            {
                chanceForDoubleDamage += skillTreeNode.pointsAllocated * 0.143f;
            }
            if (node.name == "Thorn Totem Tree Reduced Spread")
            {
                reducedSpread += skillTreeNode.pointsAllocated * 40f;
            }
            if (node.name == "Thorn Totem Tree Physical Damage vs Distance And Speed")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                    tagList.Add(Tags.AbilityTags.Physical);
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, tagList);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * 0.18f);
                    statList.Add(newStat);
                    increasedSpeed += skillTreeNode.pointsAllocated * -0.1f;
                }
            }
            if (node.name == "Thorn Totem Tree More Health")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Health, null);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * 0.1f);
                    statList.Add(newStat);
                }
            }
            if (node.name == "Thorn Totem Tree Increased Speed")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    increasedSpeed += skillTreeNode.pointsAllocated * 0.2f;
                }
            }
            if (node.name == "Thorn Totem Tree Double Damage Chance")
            {
                chanceForDoubleDamage += skillTreeNode.pointsAllocated * 0.17f;
            }
            
            
        }

        // update the mutator
        mutator.additionalDuration = additionalDuration;
        mutator.totemLimit = totemLimit;
        mutator.addedManaCostDivider = addedManaCostDivider;
        mutator.addedChargeRegen = ability.chargesGainedPerSecond * increasedCooldownRecoverySpeed;
        mutator.addedCharges = addedCharges;
        mutator.statList.Clear();
        mutator.statList.AddRange(statList);
        mutator.poisonCloudChance = poisonCloudChance;

        mutator.extraProjectiles = extraProjectiles;
        mutator.homing = homing;
        mutator.chanceForDoubleDamage = chanceForDoubleDamage;
        mutator.chanceToShredArmour = chanceToShredArmour;
        mutator.chanceToPoison = chanceToPoison;
        mutator.reducedSpread = reducedSpread;
        mutator.increasedThornTotemAttackDamage = increasedThornTotemAttackDamage;
        mutator.targetsToPierce = targetsToPierce;

        mutator.increasedManaCost = increasedManaCost;
        mutator.increasedSpeed = increasedSpeed;


        // update existing totems
        if (player.GetComponent<SummonTracker>())
        {

            // get a list of existing wolves
            List<SummonChangeTracker> totems = new List<SummonChangeTracker>();
            foreach (Summoned summon in player.GetComponent<SummonTracker>().summons)
            {
                if (summon.GetComponent<CreationReferences>() && summon.GetComponent<CreationReferences>().thisAbility == ability && summon.GetComponent<SummonChangeTracker>())
                {
                    totems.Add(summon.GetComponent<SummonChangeTracker>());
                }
            }

            // update the wolves
            foreach (SummonChangeTracker totem in totems)
            {
                totem.changeStats(statList);
                totem.changeLimitDuration(true, ability.abilityPrefab.GetComponent<SummonEntityOnDeath>().duration + additionalDuration);
            }
        }

    }
}
