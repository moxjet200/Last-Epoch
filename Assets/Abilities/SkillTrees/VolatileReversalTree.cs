using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolatileReversalTree : SkillTree
{

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.volatileReversal);
    }

    public override void updateMutator()
    {
        VolatileReversalMutator mutator = PlayerFinder.getPlayer().GetComponent<VolatileReversalMutator>();
        float increasedDamage = 0f;
        float increasedArea = 0f;
        float timeRotChance = 0f;
        float increasesDamageTaken = 0f;
        float increasesDoTDamageTaken = 0f;
        float increasedStunChance = 0f;

        bool voidRiftAtStart = false;
        bool voidRiftAtEnd = false;

        List<TaggedStatsHolder.TaggableStat> statsWhileOnCooldown = new List<TaggedStatsHolder.TaggableStat>();
        List<TaggedStatsHolder.TaggableStat> statOnUse = new List<TaggedStatsHolder.TaggableStat>();

        float percentCooldownRecoveredOnKill = 0f;
        float additionalSecondsBack = 0f;

        bool noHealthRestoration = false;
        bool noManaRestoration = false;

        bool healsOrDamagesAtRandom = false;
        float healOrDamagePercent = 0f;
        float increasedCooldownRecoverySpeed = 0f;

        AbilityObjectConstructor aoc = null;
        ChargeManager chargeManager = null;

        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            if (node.name == "Volatile Reversal Tree Rift At Start")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    voidRiftAtStart = true;
                }
            }
            if (node.name == "Volatile Reversal Tree Increased Damage")
            {
                increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.22f;
            }
            if (node.name == "Volatile Reversal Tree Dot Damage Taken")
            {
                increasesDoTDamageTaken += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Volatile Reversal Tree Stun Chance")
            {
                increasedStunChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.4f;
            }
            if (node.name == "Volatile Reversal Tree Rift At End")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    voidRiftAtStart = true;
                }
            }
            if (node.name == "Volatile Reversal Tree Increased Area")
            {
                increasedArea += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.4f;
            }
            if (node.name == "Volatile Reversal Tree Damage Taken")
            {
                increasesDoTDamageTaken += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Volatile Reversal Tree Time Rot")
            {
                timeRotChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Volatile Reversal Tree Reduced Cooldown On Kill")
            {
                percentCooldownRecoveredOnKill += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.03f;
            }
            if (node.name == "Volatile Reversal Tree Movespeed")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Movespeed, new List<Tags.AbilityTags>());
                    stat.increasedValue += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.12f;
                    statOnUse.Add(stat);
                }
            }
            if (node.name == "Volatile Reversal Tree Attack and Cast Speed")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.AttackSpeed, new List<Tags.AbilityTags>());
                    stat.increasedValue += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.12f;
                    statOnUse.Add(stat);
                    TaggedStatsHolder.TaggableStat stat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.CastSpeed, new List<Tags.AbilityTags>());
                    stat2.increasedValue += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.12f;
                    statOnUse.Add(stat2);
                }
            }
            if (node.name == "Volatile Reversal Tree No Health Restored")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    noHealthRestoration = true;
                    increasedCooldownRecoverySpeed += 2f;
                }
            }
            if (node.name == "Volatile Reversal Tree Random Heal")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    healsOrDamagesAtRandom = true;
                    healOrDamagePercent = node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
                }
            }
            if (node.name == "Volatile Reversal Tree Cooldown vs Regen")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    increasedCooldownRecoverySpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
                    TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.HealthRegen, new List<Tags.AbilityTags>());
                    stat.increasedValue += node.GetComponent<SkillTreeNode>().pointsAllocated * -0.1f;
                    statsWhileOnCooldown.Add(stat);
                }
            }
            if (node.name == "Volatile Reversal Tree Goes Back Further")
            {
                additionalSecondsBack += node.GetComponent<SkillTreeNode>().pointsAllocated * 1f;
            }
            if (node.name == "Volatile Reversal Tree Cooldown vs Damage Taken")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    increasedCooldownRecoverySpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
                    TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.DamageTaken, new List<Tags.AbilityTags>());
                    stat.increasedValue += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
                    statsWhileOnCooldown.Add(stat);
                }
            }
            if (node.name == "Volatile Reversal Tree No Mana Restored")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    noManaRestoration = true;
                    increasedCooldownRecoverySpeed += 4f;
                }
            }
            if (node.name == "Volatile Reversal Tree Dodge On Cooldown")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.DodgeRating, new List<Tags.AbilityTags>());
                    stat.addedValue += node.GetComponent<SkillTreeNode>().pointsAllocated * 50f;
                    statsWhileOnCooldown.Add(stat);
                }
            }
        }

        mutator.increasedDamage = increasedDamage;
        mutator.increasedRadius = Mathf.Sqrt(increasedArea + 1) - 1;
        mutator.timeRotChance = timeRotChance;
        mutator.increasesDamageTaken = increasesDamageTaken;
        mutator.increasesDoTDamageTaken = increasesDoTDamageTaken;
        mutator.increasedStunChance = increasedStunChance;

        mutator.voidRiftAtStart = voidRiftAtStart;
        mutator.voidRiftAtEnd = voidRiftAtEnd;

        mutator.statsWhileOnCooldown = statsWhileOnCooldown;
        mutator.statOnUse = statOnUse;

        mutator.percentCooldownRecoveredOnKill = percentCooldownRecoveredOnKill;
        mutator.additionalSecondsBack = additionalSecondsBack;

        mutator.noHealthRestoration = noHealthRestoration;
        mutator.noManaRestoration = noManaRestoration;

        mutator.healsOrDamagesAtRandom = healsOrDamagesAtRandom;
        mutator.healOrDamagePercent = healOrDamagePercent;
        mutator.addedChargeRegen = ability.chargesGainedPerSecond * increasedCooldownRecoverySpeed;
    }
}
