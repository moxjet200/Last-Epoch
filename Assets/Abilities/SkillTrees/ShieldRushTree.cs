using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldRushTree : SkillTree
{

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.shieldRush);
    }


    public override void updateMutator()
    {
        ShieldRushMutator mutator = PlayerFinder.getPlayer().GetComponent<ShieldRushMutator>();

        List<TaggedStatsHolder.TaggableStat> statsWhileTravelling = new List<TaggedStatsHolder.TaggableStat>();

        float increasedDamage = 0f;
        float increasedArea = 0f;
        float addedCritMultiplier = 0f;
        float addedCritChance = 0f;
        bool leaveDelayed = false;
        float increasedDelayLength = 0f;

        float delayIncreasedDamage = 0f;
        float delayAddedCritMultiplier = 0f;
        float delayAddedCritChance = 0f;

        float increasedTravelDamage = 0f;
        float increasedTravelStunChance = 0f;

        bool forwardVoidBeam = false;
        bool backwardsVoidBeam = false;

        bool noShieldRequirement = false;
        bool returnToStart = false;
        bool restoreMana = false;

        float percentCurrentHealthLostOnCast = 0f;

        float addedVoidDamage = 0f;
        float additionalAoEAddedVoidDamage = 0f;
        float addedVoidReducedByAttackSpeed = 0f;

        float moreTravelDamageAgainstFullHealth = 0f;
        float increasedManaCost = 0f;
        float manaEfficiency = 0f;

        bool cooldown = false;
        int addedCharges = 0;

        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            if (node.name == "Shield Rush Tree Damage While Travelling")
            {
                increasedTravelDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Shield Rush Tree Damage Against Full Health While Travelling")
            {
                moreTravelDamageAgainstFullHealth += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
            }
            if (node.name == "Shield Rush Tree Forwards Beam")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    forwardVoidBeam = true;
                    increasedManaCost += 0.2f;
                }
            }
            if (node.name == "Shield Rush Tree Backwards Beam")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    backwardsVoidBeam = true;
                    increasedManaCost += 0.1f;
                }
            }
            if (node.name == "Shield Rush Tree No Shield Requirement")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    noShieldRequirement = true;
                    addedVoidDamage += 5f;
                    increasedManaCost += 0.3f;
                }
            }
            if (node.name == "Shield Rush Tree AoE Void Damage")
            {
                additionalAoEAddedVoidDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 5f;
            }
            if (node.name == "Shield Rush Tree Attack Speed Reduced Void Damage")
            {
                addedVoidReducedByAttackSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 7f;
            }
            if (node.name == "Shield Rush Tree Mana Efficiency")
            {
                manaEfficiency += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Shield Rush Tree Less Damage Taken")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.DamageTaken, new List<Tags.AbilityTags>());
                    stat.moreValues.Add(node.GetComponent<SkillTreeNode>().pointsAllocated * -0.1f);
                    statsWhileTravelling.Add(stat);
                }
            }
            if (node.name == "Shield Rush Tree Travel Stun Chance")
            {
                increasedTravelStunChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.35f;
            }
            if (node.name == "Shield Rush Tree Return To Start")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    returnToStart = true;
                }
            }
            if (node.name == "Shield Rush Tree Mana Restore")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    restoreMana = true;
                    cooldown = true;
                    manaEfficiency += node.GetComponent<SkillTreeNode>().pointsAllocated * 2f;
                }
            }
            if (node.name == "Shield Rush Tree Adds Charges")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    addedCharges += node.GetComponent<SkillTreeNode>().pointsAllocated;
                    TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.DamageTaken, new List<Tags.AbilityTags>());
                    stat.moreValues.Add(node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f);
                    statsWhileTravelling.Add(stat);
                }
            }
            if (node.name == "Shield Rush Tree No Cooldown")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    percentCurrentHealthLostOnCast = 0.15f;
                    cooldown = false;
                }
            }
            if (node.name == "Shield Rush Tree Increase AoE")
            {
                increasedArea += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.35f;
            }
            if (node.name == "Shield Rush Tree Area Damage")
            {
                increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Shield Rush Tree Area Crit Chance")
            {
                addedCritChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.04f;
            }
            if (node.name == "Shield Rush Tree Area Crit Multi")
            {
                addedCritMultiplier += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.4f;
            }
            if (node.name == "Shield Rush Tree Second Aoe")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * -0.2f;
                    leaveDelayed = true;
                }
            }
            if (node.name == "Shield Rush Tree Second Aoe Damage And Delay")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    delayIncreasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.45f;
                    increasedDelayLength += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
                }
            }
            if (node.name == "Shield Rush Tree Second Aoe Crit Chance")
            {
                addedCritChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.04f;
            }
            if (node.name == "Shield Rush Tree Second Aoe Crit Multi")
            {
                addedCritMultiplier += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.4f;
            }
        }


        if (cooldown)
        {
            mutator.addedCharges = 1 + addedCharges;
            mutator.addedChargeRegen = 0.1f;
        }
        else
        {
            mutator.addedCharges = 0;
            mutator.addedChargeRegen = 0;
        }

        mutator.statsWhileTravelling = new List<TaggedStatsHolder.TaggableStat>();

        mutator.increasedDamage = increasedDamage;
        mutator.addedCritMultiplier = addedCritMultiplier;
        mutator.addedCritChance = addedCritChance;
        mutator.leaveDelayed = leaveDelayed;
        mutator.increasedDelayLength = increasedDelayLength;

        mutator.delayIncreasedDamage = delayIncreasedDamage;
        mutator.delayAddedCritMultiplier = delayAddedCritMultiplier;
        mutator.delayAddedCritChance = delayAddedCritChance;

        mutator.increasedTravelDamage = increasedTravelDamage;
        mutator.increasedTravelStunChance = increasedTravelStunChance;

        mutator.forwardVoidBeam = forwardVoidBeam;
        mutator.backwardsVoidBeam = backwardsVoidBeam;

        mutator.noShieldRequirement = noShieldRequirement;
        mutator.returnToStart = returnToStart;
        mutator.restoreMana = restoreMana;

        mutator.percentCurrentHealthLostOnCast = percentCurrentHealthLostOnCast;

        mutator.addedVoidDamage = addedVoidDamage;
        mutator.additionalAoEAddedVoidDamage = additionalAoEAddedVoidDamage;
        mutator.addedVoidReducedByAttackSpeed = addedVoidReducedByAttackSpeed;

        mutator.increasedRadius = Mathf.Sqrt(increasedArea + 1) - 1;

        mutator.moreTravelDamageAgainstFullHealth = moreTravelDamageAgainstFullHealth;
        mutator.increasedManaCost = increasedManaCost;
        mutator.addedManaCostDivider = manaEfficiency;
        
    }
}
