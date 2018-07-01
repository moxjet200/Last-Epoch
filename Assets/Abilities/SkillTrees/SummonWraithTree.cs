using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonWraithTree : SkillTree
{
    // ability references for updating minions
    Ability summonWarrior = null;
    Ability summonMage = null;
    Ability summonArcher = null;
    Ability summonWarlord = null;
    Ability summonBrawler = null;

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.summonWraith);
    }

    public override void updateMutator()
    {
        GameObject player = PlayerFinder.getPlayer();

        SummonWraithMutator mutator = player.GetComponent<SummonWraithMutator>();

        bool canTarget = false;

        List<TaggedStatsHolder.TaggableStat> statList = new List<TaggedStatsHolder.TaggableStat>();

        List<TaggedStatsHolder.TaggableStat> corpseStatList = new List<TaggedStatsHolder.TaggableStat>();

        int delayedWraiths = 0;
        float reducedWraithInterval = 0f;
        bool instantWraiths = false;

        bool critFromManaCost = false;
        float healOnCrit = 0f;
        float reducedHealthDrain = 0f;
        bool stationary = false;

        float flameWraithChance = 0f;
        float putridWraithChance = 0f;
        float bloodWraithChance = 0f;

        bool cooldown = false;
        float reducedCooldown = 0f;
        float increasedCastSpeed = 0f;

        float increasedManaCost = 0f;
        float manaEfficiency = 0f;

        SkillTreeNode skillTreeNode;
        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            skillTreeNode = node.GetComponent<SkillTreeNode>();
            if (node.name == "Summon Wraith Tree Can Target")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    canTarget = true;
                    increasedManaCost += 0.2f;
                }
            }
            if (node.name == "Summon Wraith Tree Stationary")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    stationary = true;

                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, null);
                    newStat2.moreValues.Add(skillTreeNode.pointsAllocated * 0.4f);
                    statList.Add(newStat2);

                    TaggedStatsHolder.TaggableStat newStat3 = new TaggedStatsHolder.TaggableStat(Tags.Properties.Movespeed, null);
                    newStat3.moreValues.Add(skillTreeNode.pointsAllocated * -1f);
                    statList.Add(newStat3);
                }
            }
            if (node.name == "Summon Wraith Tree Melee Attack Speed")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                    tagList.Add(Tags.AbilityTags.Melee);
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.AttackSpeed, tagList);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * 0.1f);
                    statList.Add(newStat);
                }
            }
            if (node.name == "Summon Wraith Tree Multiple Wraiths")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    delayedWraiths += 3;
                    cooldown = true;
                    increasedManaCost += 0.8f;
                }
            }
            if (node.name == "Summon Wraith Tree Reduced Cooldown")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    reducedCooldown += 0.1f * skillTreeNode.pointsAllocated;
                    reducedWraithInterval += 0.1f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Summon Wraith Tree Additional Wraith")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    delayedWraiths += 1 * skillTreeNode.pointsAllocated;
                    increasedManaCost += 0.1f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Summon Wraith Tree Instant Wraiths")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    instantWraiths = true;
                }
            }
            if (node.name == "Summon Wraith Tree Increased Health")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat3 = new TaggedStatsHolder.TaggableStat(Tags.Properties.Health, null);
                    newStat3.moreValues.Add(skillTreeNode.pointsAllocated * 0.2f);
                    statList.Add(newStat3);
                }
            }
            if (node.name == "Summon Wraith Tree Corpse Consumption")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, null);
                    newStat2.moreValues.Add(skillTreeNode.pointsAllocated * 0.3f);
                    corpseStatList.Add(newStat2);
                }
            }
            if (node.name == "Summon Wraith Tree Corpse Health")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.Health, null);
                    newStat2.moreValues.Add(skillTreeNode.pointsAllocated * 0.25f);
                    corpseStatList.Add(newStat2);
                }
            }
            if (node.name == "Summon Wraith Tree Corpse Speed")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.AttackSpeed, null);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * 0.1f);
                    corpseStatList.Add(newStat);
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.CastSpeed, null);
                    newStat2.moreValues.Add(skillTreeNode.pointsAllocated * 0.1f);
                    corpseStatList.Add(newStat2);
                    TaggedStatsHolder.TaggableStat newStat3 = new TaggedStatsHolder.TaggableStat(Tags.Properties.Movespeed, null);
                    newStat3.moreValues.Add(skillTreeNode.pointsAllocated * 0.1f);
                    corpseStatList.Add(newStat3);
                }
            }
            if (node.name == "Summon Wraith Tree Blood Wraith Chance")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    bloodWraithChance += 0.2f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Summon Wraith Tree Added Necrotic Damage")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                    tagList.Add(Tags.AbilityTags.Necrotic);
                    tagList.Add(Tags.AbilityTags.Melee);
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, tagList);
                    newStat.addedValue = skillTreeNode.pointsAllocated * 4f;
                    statList.Add(newStat);

                    List<Tags.AbilityTags> tagList2 = new List<Tags.AbilityTags>();
                    tagList2.Add(Tags.AbilityTags.Necrotic);
                    tagList2.Add(Tags.AbilityTags.Spell);
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, tagList2);
                    newStat2.addedValue = skillTreeNode.pointsAllocated * 4f;
                    statList.Add(newStat2);
                }
            }
            if (node.name == "Summon Wraith Tree Cast Speed and Mana Efficiency")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    manaEfficiency += 0.14f * skillTreeNode.pointsAllocated;
                    increasedCastSpeed += 0.7f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Summon Wraith Tree Move Speed")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.Movespeed, null);
                    newStat2.increasedValue = skillTreeNode.pointsAllocated * 0.15f;
                    corpseStatList.Add(newStat2);
                }
            }
            if (node.name == "Summon Wraith Tree Flame Wraith Chance")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    flameWraithChance += 0.11f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Summon Wraith Tree Mana Crit")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    critFromManaCost = true;

                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.CriticalChance, null);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * 0.2f);
                    corpseStatList.Add(newStat);
                }
            }
            if (node.name == "Summon Wraith Tree Wraith Crit Chance")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.CriticalChance, null);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * 0.3f);
                    corpseStatList.Add(newStat);
                }
            }
            if (node.name == "Summon Wraith Tree Wraith Crit Multiplier")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.CriticalMultiplier, null);
                    newStat.addedValue = skillTreeNode.pointsAllocated * 0.4f;
                    corpseStatList.Add(newStat);
                }
            }
            if (node.name == "Summon Wraith Tree Necrotic Leech")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                    tagList.Add(Tags.AbilityTags.Necrotic);
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, tagList);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * 0.2f);
                    statList.Add(newStat);

                    List<Tags.AbilityTags> tagList2 = new List<Tags.AbilityTags>();
                    tagList2.Add(Tags.AbilityTags.Necrotic);
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.PercentLifeLeech, tagList2);
                    newStat2.addedValue = skillTreeNode.pointsAllocated * 0.1f;
                    statList.Add(newStat2);
                }
            }
            if (node.name == "Summon Wraith Tree Tanky")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.Health, null);
                    newStat2.moreValues.Add(skillTreeNode.pointsAllocated * 0.4f);
                    corpseStatList.Add(newStat2);

                    increasedManaCost += 0.1f * skillTreeNode.pointsAllocated;
                    increasedCastSpeed -= 0.05f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Summon Wraith Tree Melee Poison Chance")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    List<Tags.AbilityTags> tagList2 = new List<Tags.AbilityTags>();
                    tagList2.Add(Tags.AbilityTags.Melee);
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.PoisonChance, tagList2);
                    newStat2.addedValue = skillTreeNode.pointsAllocated * 0.2f;
                    statList.Add(newStat2);
                }
            }
            if (node.name == "Summon Wraith Tree Putrid Wraith Chance")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    putridWraithChance += 0.11f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Summon Wraith Tree Reduced Drain")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    reducedHealthDrain += 0.12f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Summon Wraith Tree Health On Crit")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    healOnCrit += 10f * skillTreeNode.pointsAllocated;
                }
            }

        }

        // update the mutator
        if (canTarget)
        {
            mutator.targetting = SummonWraithMutator.WraithTargetType.atTarget;
        }
        else
        {
            mutator.targetting = SummonWraithMutator.WraithTargetType.normal;
        }

        if (cooldown)
        {
            mutator.addedCharges = 1;
            mutator.addedChargeRegen = 0.17f * (1f - reducedCooldown);
        }
        else
        {
            mutator.addedCharges = 0;
            mutator.addedChargeRegen = 0;
        }

        if (instantWraiths)
        {
            // set wraith interval
            reducedWraithInterval = 0.8f;
            // reduce number of wraiths
            delayedWraiths = (int) (Mathf.Ceil((1 + delayedWraiths) * 0.66f) - 1);
        }

        mutator.statList = statList;

        mutator.corpseStatList = corpseStatList;

        mutator.delayedWraiths = delayedWraiths;
        mutator.reducedWraithInterval = reducedWraithInterval;

        mutator.critFromManaCost = critFromManaCost;
        mutator.healOnCrit = healOnCrit;
        mutator.reducedHealthDrain = reducedHealthDrain;
        mutator.stationary = stationary;

        mutator.flameWraithChance = flameWraithChance;
        mutator.putridWraithChance = putridWraithChance;
        mutator.bloodWraithChance = bloodWraithChance;
        mutator.increasedCastSpeed = increasedCastSpeed;

        mutator.increasedManaCost = increasedManaCost;
        mutator.addedManaCostDivider = manaEfficiency;

        // update existing wolves
        if (player.GetComponent<SummonTracker>())
        {
            // get a list of existing wraiths
            List<SummonChangeTracker> skeles = new List<SummonChangeTracker>();
            SummonChangeTracker changeTracker = null;
            CreationReferences references = null;
            foreach (Summoned summon in player.GetComponent<SummonTracker>().summons)
            {
                changeTracker = summon.GetComponent<SummonChangeTracker>();
                references = summon.GetComponent<CreationReferences>();
                if (references && changeTracker)
                {
                    if (references.thisAbility == ability)
                    {
                        skeles.Add(changeTracker);
                    }
                }
            }

            // update the wraiths
            foreach (SummonChangeTracker skele in skeles)
            {
                skele.changeStats(statList);
            }
        }

    }
}
