using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempestSkillTree : SkillTree
{

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.tempest);
    }

    public override void updateMutator()
    {
        TempestMutator mutator = PlayerFinder.getPlayer().GetComponent<TempestMutator>();
        List<TaggedStatsHolder.TaggableStat> statsWhileSpinning = new List<TaggedStatsHolder.TaggableStat>();

        List<TaggedStatsHolder.TaggableStat> statsWhileSpinningIfNotUsingAShield = new List<TaggedStatsHolder.TaggableStat>();

        float addedCritMultiplier = 0f;
        float addedCritChance = 0f;

        float increasedDamage = 0f;
        float timeRotChance = 0f;
        float moreDamageAgainstFullHealth = 0f;
        float increasedArea = 0f;
        float igniteChance = 0f;
        float moreDamageAgainstTimeRotting = 0f;

        float parabolicVoidOrbOnHitChance = 0f;
        bool castsAbyssalOrb = false;
        float increasedAbyssalOrbFrequence = 0f;

        float addedVoidDamage = 0f;
        float increasedDamageWhileNotUsingAShield = 0f;
        float addedManaDrain = 0f;

        bool pulls = false;
        float increasedPullArea = 0f;
        float increasedAreaWhileNotUsingAShield = 0f;
        bool strongerPull = false;

        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            if (node.name == "Tempest Tree Added Void Damage")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    addedVoidDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 2f;
                    List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                    tagList.Add(Tags.AbilityTags.Void);
                    TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.DamageTaken, tagList);
                    stat.moreValues.Add(node.GetComponent<SkillTreeNode>().pointsAllocated * -0.05f);
                    statsWhileSpinning.Add(stat);
                }
            }
            if (node.name == "Tempest Tree Parabolic Void Orb")
            {
                parabolicVoidOrbOnHitChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.08f;
            }
            if (node.name == "Tempest Tree Time Rot Chance")
            {
                timeRotChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Tempest Tree Ignite Chance")
            {
                igniteChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Tempest Tree Damage Against Time Rot")
            {
                moreDamageAgainstTimeRotting += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
            }
            if (node.name == "Tempest Tree Damage And Mana Drain")
            {
                increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
                addedManaDrain += node.GetComponent<SkillTreeNode>().pointsAllocated * 1f;
            }
            if (node.name == "Tempest Tree Abyssal Orb")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    castsAbyssalOrb = true;
                }
            }
            if (node.name == "Tempest Tree Abyssal Orb Frequency")
            {
                increasedAbyssalOrbFrequence += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
            }
            if (node.name == "Tempest Tree Pulls")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    pulls = true;
                }
            }
            if (node.name == "Tempest Tree Pull Area")
            {
                increasedPullArea += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.5f;
            }
            //if (node.name == "Tempest Tree Damage vs Speed")
            //{
            //    if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
            //    {
            //        increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            //        TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Movespeed, new List<Tags.AbilityTags>());
            //        stat.moreValues.Add(node.GetComponent<SkillTreeNode>().pointsAllocated * -0.05f);
            //        statsWhileSpinning.Add(stat);
            //    }
            //}
            if (node.name == "Tempest Tree Area")
            {
                increasedArea += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Tempest Tree Increased Damage")
            {
                increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Tempest Tree Increased Speed")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    addedManaDrain += node.GetComponent<SkillTreeNode>().pointsAllocated * 1f;
                    TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Movespeed, new List<Tags.AbilityTags>());
                    stat.increasedValue += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
                    statsWhileSpinning.Add(stat);
                }
            }
            if (node.name == "Tempest Tree Crit Chance And Mana Drain")
            {
                addedCritChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.04f;
                addedManaDrain += node.GetComponent<SkillTreeNode>().pointsAllocated * 1f;
            }
            if (node.name == "Tempest Tree Crit Damage And Mana Drain")
            {
                addedCritMultiplier += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.4f;
                addedManaDrain += node.GetComponent<SkillTreeNode>().pointsAllocated * 1f;
            }
            if (node.name == "Tempest Tree No Shield Damage")
            {
                increasedDamageWhileNotUsingAShield += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            }
            if (node.name == "Tempest Tree No Shield AoE")
            {
                increasedAreaWhileNotUsingAShield += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.5f;
            }
            if (node.name == "Tempest Tree No Shield Leech")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                    tagList.Add(Tags.AbilityTags.Melee);
                    TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.PercentLifeLeech, tagList);
                    stat.addedValue += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.07f;
                    statsWhileSpinningIfNotUsingAShield.Add(stat);
                }
            }
            if (node.name == "Tempest Tree No Shield Block")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.BlockChance, new List<Tags.AbilityTags>());
                    stat.addedValue += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.05f;
                    statsWhileSpinningIfNotUsingAShield.Add(stat);
                    TaggedStatsHolder.TaggableStat stat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.BlockArmor, new List<Tags.AbilityTags>());
                    stat2.addedValue += node.GetComponent<SkillTreeNode>().pointsAllocated * 100f;
                    statsWhileSpinningIfNotUsingAShield.Add(stat2);
                }
            }
            if (node.name == "Tempest Tree No Shield Block Protection")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat stat3 = new TaggedStatsHolder.TaggableStat(Tags.Properties.BlockElementalProtection, new List<Tags.AbilityTags>());
                    stat3.addedValue += node.GetComponent<SkillTreeNode>().pointsAllocated * 200f;
                    statsWhileSpinningIfNotUsingAShield.Add(stat3);
                }
            }
            if (node.name == "Tempest Tree Less Mana Drain")
            {
                increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.05f;
                addedManaDrain += node.GetComponent<SkillTreeNode>().pointsAllocated * -1f;
            }
            if (node.name == "Tempest Tree No Shield Mana Regen")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat stat3 = new TaggedStatsHolder.TaggableStat(Tags.Properties.ManaRegen, new List<Tags.AbilityTags>());
                    stat3.increasedValue += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
                    statsWhileSpinningIfNotUsingAShield.Add(stat3);
                }
            }
            if (node.name == "Tempest Tree Mana Regen")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat stat3 = new TaggedStatsHolder.TaggableStat(Tags.Properties.ManaRegen, new List<Tags.AbilityTags>());
                    stat3.increasedValue += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
                    statsWhileSpinning.Add(stat3);
                }
            }
            if (node.name == "Tempest Tree Mana Regen And Damage Taken")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {

                    addedManaDrain += node.GetComponent<SkillTreeNode>().pointsAllocated * -4f;
                    List<Tags.AbilityTags> tags = new List<Tags.AbilityTags>();
                    tags.Add(Tags.AbilityTags.Fire);
                    TaggedStatsHolder.TaggableStat stat4 = new TaggedStatsHolder.TaggableStat(Tags.Properties.DamageTaken, tags);
                    stat4.moreValues.Add(node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f);
                    statsWhileSpinning.Add(stat4);
                    List<Tags.AbilityTags> tags2 = new List<Tags.AbilityTags>();
                    tags2.Add(Tags.AbilityTags.Cold);
                    TaggedStatsHolder.TaggableStat stat1 = new TaggedStatsHolder.TaggableStat(Tags.Properties.DamageTaken, tags2);
                    stat1.moreValues.Add(node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f);
                    statsWhileSpinning.Add(stat1);
                    List<Tags.AbilityTags> tags3 = new List<Tags.AbilityTags>();
                    tags3.Add(Tags.AbilityTags.Lightning);
                    TaggedStatsHolder.TaggableStat stat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.DamageTaken, tags3);
                    stat2.moreValues.Add(node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f);
                    statsWhileSpinning.Add(stat2);
                }
            }
            if (node.name == "Tempest Tree Armour And Protection")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Armour, new List<Tags.AbilityTags>());
                    stat.addedValue += node.GetComponent<SkillTreeNode>().pointsAllocated * 40f;
                    statsWhileSpinning.Add(stat);
                    TaggedStatsHolder.TaggableStat stat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.ElementalProtection, new List<Tags.AbilityTags>());
                    stat2.addedValue += node.GetComponent<SkillTreeNode>().pointsAllocated * 40f;
                    statsWhileSpinning.Add(stat2);
                }
            }
            
            if (node.name == "Tempest Tree Doesn't Move")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    strongerPull = true;
                    increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.5f;
                    TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Movespeed, new List<Tags.AbilityTags>());
                    stat.moreValues.Add(-1f);
                    statsWhileSpinning.Add(stat);
                }
            }
        }


        TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.ManaDrain, new List<Tags.AbilityTags>());
        newStat.addedValue = addedManaDrain;
        statsWhileSpinning.Add(newStat);

        mutator.statsWhileSpinning = statsWhileSpinning;
        mutator.statsWhileSpinningIfNotUsingAShield = statsWhileSpinningIfNotUsingAShield;
        mutator.addedCritMultiplier = addedCritMultiplier;
        mutator.addedCritChance = addedCritChance;
        mutator.increasedDamage = increasedDamage;
        mutator.timeRotChance = timeRotChance;
        mutator.moreDamageAgainstFullHealth = moreDamageAgainstFullHealth;
        mutator.increasedRadius = Mathf.Sqrt(increasedArea + 1) - 1;
        mutator.igniteChance = igniteChance;

        mutator.moreDamageAgainstTimeRotting = moreDamageAgainstTimeRotting;
        mutator.parabolicVoidOrbOnHitChance = parabolicVoidOrbOnHitChance;
        mutator.castsAbyssalOrb = castsAbyssalOrb;
        mutator.increasedAbyssalOrbFrequence = increasedAbyssalOrbFrequence;
        mutator.addedVoidDamage = addedVoidDamage;
        mutator.increasedDamageWhileNotUsingAShield = increasedDamageWhileNotUsingAShield;
        mutator.increasedRadiusWhileNotUsingAShield = Mathf.Sqrt(increasedArea + increasedAreaWhileNotUsingAShield + 1) - 1;
        mutator.strongerPull = strongerPull;
        mutator.pulls = pulls;
        mutator.increasedPullRadius = Mathf.Sqrt(increasedPullArea + 1) - 1;
    }
}
