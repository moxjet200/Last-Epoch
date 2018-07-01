using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuryLeapSkillTree : SkillTree {

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.furyLeap);
    }

    public override void updateMutator()
    {
        FuryLeapMutator mutator = PlayerFinder.getPlayer().GetComponent<FuryLeapMutator>();
        bool resetCooldownOnKill = true;
        float addedCritMultiplier = 0f;
        bool castsLightning = false;
        float lightningInterval = 0.25f;
        bool lightningKillsResetCooldown = false;
        float chanceToResetCooldownOnAnyKill = 0f;
        float movespeedOnLanding = 0f;
        float attackAndCastSpeedOnLanding = 0f;
        bool eligiblePetsJumpToo = false;
        float increasedStunChance = 0f;
        float increasedDamage = 0f;
        float chanceToPull = 0f;
        float chanceToSummonVinesAtStart = 0f;
        float chanceToSummonVinesAtEnd = 0f;
        float increasedArea = 0f;
        int addedCharges = 0;
        float moreDamageAgainstFullHealth = 0f;

        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            if (node.name == "Fury Leap Skill Tree Extra Charges")
            {
                addedCharges += node.GetComponent<SkillTreeNode>().pointsAllocated * 1;
            }
            if (node.name == "Fury Leap Skill Tree Crit Damage")
            {
                addedCritMultiplier += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.5f;
            }
            if (node.name == "Fury Leap Skill Tree Casts Lightning")
            {
                castsLightning = (node.GetComponent<SkillTreeNode>().pointsAllocated > 0);
            }
            if (node.name == "Fury Leap Skill Tree Casts Lightning Faster")
            {
                lightningInterval /= (1 + node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f);
            }
            if (node.name == "Fury Leap Skill Tree Lightning Resets Cooldown")
            {
                lightningKillsResetCooldown = (node.GetComponent<SkillTreeNode>().pointsAllocated > 0);
            }
            if (node.name == "Fury Leap Skill Tree Global Reset Chance")
            {
                chanceToResetCooldownOnAnyKill += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Fury Leap Skill Tree Movement Speed On Landing")
            {
                movespeedOnLanding += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            }
            if (node.name == "Fury Leap Skill Tree Attack And Cast Speed On Landing")
            {
                attackAndCastSpeedOnLanding += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Fury Leap Skill Tree Pets Jump")
            {
                eligiblePetsJumpToo = (node.GetComponent<SkillTreeNode>().pointsAllocated > 0);
            }
            if (node.name == "Fury Leap Skill Tree Stun Chance")
            {
                increasedStunChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Fury Leap Skill Tree No Reset")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    resetCooldownOnKill = false;
                    increasedDamage = 0.5f;
                }
            }
            if (node.name == "Fury Leap Skill Tree Pull Chance")
            {
                chanceToPull += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Fury Leap Skill Tree Vines At Start")
            {
                chanceToSummonVinesAtStart += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Fury Leap Skill Tree Vines At End")
            {
                chanceToSummonVinesAtEnd += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Fury Leap Skill Tree Area")
            {
                increasedArea += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.35f;
            }
            if (node.name == "Fury Leap Skill Tree Damage Against Full Life")
            {
                increasedArea += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.35f;
            }
        }

        mutator.resetCooldownOnKill = resetCooldownOnKill;
        mutator.addedCritMultiplier = addedCritMultiplier;
        mutator.castsLightning = castsLightning;
        mutator.lightningInterval = lightningInterval;
        mutator.lightningKillsResetCooldown = lightningKillsResetCooldown;
        mutator.chanceToResetCooldownOnAnyKill = chanceToResetCooldownOnAnyKill;
        mutator.movespeedOnLanding = movespeedOnLanding;
        mutator.attackAndCastSpeedOnLanding = attackAndCastSpeedOnLanding;
        mutator.eligiblePetsJumpToo = eligiblePetsJumpToo;
        mutator.increasedStunChance = increasedStunChance;
        mutator.increasedDamage = increasedDamage;
        mutator.chanceToPull = chanceToPull;
        mutator.chanceToSummonVinesAtStart = chanceToSummonVinesAtStart;
        mutator.chanceToSummonVinesAtEnd = chanceToSummonVinesAtEnd;
        mutator.addedCharges = addedCharges;
        mutator.increasedRadius = Mathf.Sqrt(increasedArea + 1) - 1;
        mutator.moreDamageAgainstFullHealth = moreDamageAgainstFullHealth;
    }
}
