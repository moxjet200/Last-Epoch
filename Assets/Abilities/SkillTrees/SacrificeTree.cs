using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacrificeTree : SkillTree
{

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.sacrifice);
    }

    public override void updateMutator()
    {
        SacrificeMutator mutator = PlayerFinder.getPlayer().GetComponent<SacrificeMutator>();


        float nova_increasedSpeed = 0f;
        bool nova_pierces = false;
        float nova_increasedDamage = 0f;
        float nova_increasedStunChance = 0f;
        float nova_bleedChance = 0f;
        float nova_addedCritChance = 0f;
        float nova_addedCritMultiplier = 0f;
        float nova_moreDamageAgainstBleeding = 0f;

        float boneNovaChance = 0f;

        float moreDamageAgainstBleeding = 0f;
        float increasedDotDamageOnCast = 0f;
        float increasedDamage = 0f;
        float increasedStunChance = 0f;
        float increasedDamagePerMinion = 0f;

        float increasedArea = 0f;
        float increasedAreaWith3OrMoreMinions = 0f;

        float increasedCastSpeed = 0f;
        float chanceToIgnite = 0f;
        float addedFireDamage = 0f;

        float bloodWraithChance = 0f;
        List<TaggedStatsHolder.TaggableStat> bloodWraithStats = new List<TaggedStatsHolder.TaggableStat>();
        float increasedBloodWraithSize = 0f;

        float increasedDamageIfDetonatedMinionHasMoreHealth = 0f;
        float increasedDamageWithOneMinion = 0f;

        bool chainsBetweenMinions = false;

        float manaEfficiency = 0f;
        float increasedManaCost = 0f;
        bool cooldown = false;
        int addedCharges = 0;

        List<TaggedStatsHolder.TaggableStat> onHitBuffs = new List<TaggedStatsHolder.TaggableStat>();

        SkillTreeNode skillTreeNode;
        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {

            skillTreeNode = node.GetComponent<SkillTreeNode>();
            if (node.name == "Sacrifice Tree Damage Vs Mana Cost")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    increasedDamage += 0.15f * skillTreeNode.pointsAllocated;
                    increasedManaCost += 0.05f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Sacrifice Tree Damage If Minion Has More Health")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    increasedDamageIfDetonatedMinionHasMoreHealth += 0.35f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Sacrifice Tree Damage If Only One Minion")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    increasedDamageIfDetonatedMinionHasMoreHealth += 0.35f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Sacrifice Tree Added Fire Damage")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    addedFireDamage += 6f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Sacrifice Tree Ignite Chance")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    chanceToIgnite += 0.34f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Sacrifice Tree Mana Efficiency and DoT on Cast")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    manaEfficiency += 0.13f * skillTreeNode.pointsAllocated;
                    increasedDotDamageOnCast += 0.13f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Sacrifice Tree Damage Vs Bleeding")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    moreDamageAgainstBleeding += 0.2f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Sacrifice Tree Damage Per Minion")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    increasedDamagePerMinion += 0.015f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Sacrifice Tree Blood Wraith")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    bloodWraithChance += 0.15f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Sacrifice Tree Blood Wraith Bleed")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.BleedChance, null);
                    newStat.addedValue = skillTreeNode.pointsAllocated * 0.34f;
                    bloodWraithStats.Add(newStat);
                }
            }
            if (node.name == "Sacrifice Tree Blood Wraith Health And Damage")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Health, null);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * 0.3f);
                    bloodWraithStats.Add(newStat);

                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, null);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * 0.3f);
                    bloodWraithStats.Add(newStat2);
                }
            }
            if (node.name == "Sacrifice Tree Blood Wraith Size")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    cooldown = true;
                    increasedBloodWraithSize += 0.4f;
                    bloodWraithChance += 1f;

                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Health, null);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * 1f);
                    bloodWraithStats.Add(newStat);

                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, null);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * 1f);
                    bloodWraithStats.Add(newStat2);
                }
            }
            if (node.name == "Sacrifice Tree Blood Wraith Leech")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    bloodWraithChance += 0.05f * skillTreeNode.pointsAllocated;

                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.PercentLifeLeech, null);
                    newStat.addedValue = skillTreeNode.pointsAllocated * 0.25f;
                    bloodWraithStats.Add(newStat);
                }
            }
            if (node.name == "Sacrifice Tree Bone Nova")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    boneNovaChance += 1f;
                    increasedManaCost += 0.2f;
                }
            }
            if (node.name == "Sacrifice Tree Bone Nova Speed")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    nova_increasedSpeed += 0.25f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Sacrifice Tree Bone Nova Damage")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    nova_increasedDamage += 0.25f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Sacrifice Tree Bone Nova Pierces")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    nova_pierces = true;
                }
            }
            if (node.name == "Sacrifice Tree Bone Nova Bleed Chance")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    nova_bleedChance += 0.25f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Sacrifice Tree DoT Damage On Cast")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    increasedDotDamageOnCast += 0.2f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Sacrifice Tree Area And Stun Chance")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    increasedArea += 0.2f * skillTreeNode.pointsAllocated;
                    increasedStunChance += 0.2f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Sacrifice Tree Area With Three Minions")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    increasedAreaWith3OrMoreMinions += 0.4f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Sacrifice Tree Chains")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    chainsBetweenMinions = true;
                    increasedManaCost += 0.9f;
                }
            }
        }

        if (cooldown)
        {
            mutator.addedCharges = 1 + addedCharges;
            mutator.addedChargeRegen = 0.125f;
        }
        else
        {
            mutator.addedCharges = 0;
            mutator.addedChargeRegen = 0;
        }

        mutator.nova_increasedSpeed = nova_increasedSpeed;
        mutator.nova_pierces = nova_pierces;
        mutator.nova_increasedDamage = nova_increasedDamage;
        mutator.nova_increasedStunChance = nova_increasedStunChance;
        mutator.nova_bleedChance = nova_bleedChance;
        mutator.nova_addedCritChance = nova_addedCritChance;
        mutator.nova_addedCritMultiplier = nova_addedCritMultiplier;
        mutator.nova_moreDamageAgainstBleeding = nova_moreDamageAgainstBleeding;

        mutator.boneNovaChance = boneNovaChance;

        mutator.moreDamageAgainstBleeding = moreDamageAgainstBleeding;
        mutator.increasedDotDamageOnCast = increasedDotDamageOnCast;
        mutator.increasedDamage = increasedDamage;
        mutator.increasedStunChance = increasedStunChance;
        mutator.increasedDamagePerMinion = increasedDamagePerMinion;

        mutator.increasedArea = increasedArea;
        mutator.increasedAreaWith3OrMoreMinions = increasedAreaWith3OrMoreMinions;

        mutator.increasedCastSpeed = increasedCastSpeed;
        mutator.chanceToIgnite = chanceToIgnite;
        mutator.addedFireDamage = addedFireDamage;

        mutator.bloodWraithChance = bloodWraithChance;
        mutator.bloodWraithStats = bloodWraithStats;
        mutator.increasedBloodWraithSize = increasedBloodWraithSize;

        mutator.increasedDamageIfDetonatedMinionHasMoreHealth = increasedDamageIfDetonatedMinionHasMoreHealth;
        mutator.increasedDamageWithOneMinion = increasedDamageWithOneMinion;

        mutator.chainsBetweenMinions = chainsBetweenMinions;

        mutator.addedManaCostDivider = manaEfficiency;
        mutator.increasedManaCost = increasedManaCost;
    }
}
