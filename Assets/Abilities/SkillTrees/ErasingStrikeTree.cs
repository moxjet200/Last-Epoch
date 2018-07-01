using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErasingStrikeTree : SkillTree
{

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.erasingStrike);
    }


    public override void updateMutator()
    {
        ErasingStrikeMutator mutator = PlayerFinder.getPlayer().GetComponent<ErasingStrikeMutator>();

        // hit stats
      float increasedDamage = 0f;
      float increasedRadius = 0f;
      float timeRotChance = 0f;
      float increasedStunChance = 0f;
      float addedCritMultiplier = 0f;
      float addedCritChance = 0f;
      float addedVoidDamage = 0f;
      float moreDamageAgainstFullHealth = 0f;
      float moreDamageAgainstDamaged = 0f;
      float cullPercent = 0f;
        float increasedArea = 0f;

      float voidRift_increasedDamage = 0f;
      float voidRift_increasedArea = 0f;
      float voidRift_timeRotChance = 0f;
      float voidRift_increasesDamageTaken = 0f;
      float voidRift_increasesDoTDamageTaken = 0f;
      float voidRift_increasedStunChance = 0f;
      float voidRift_moreDamageAgainstStunned = 0f;

    // on kill stats
      float manaGainedOnKill = 0f;
      float percentLifeGainedOnKill = 0f;
      float movementSpeedOnKill = 0f;
      float stationaryVoidBeamChance = 0f;

      bool voidBeamsOnkill = false;
      int reducedKillsRequiredForVoidBeams = 0;

    // other stats
      bool noAttackSpeedScaling = false;
      bool voidChargeOnCrit = false;

    float increasedManaCost = 0f;
        float manaEfficiency = 0f;

        bool cooldown = false;
        int addedCharges = 0;

        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            if (node.name == "Erasing Strike Tree Void Rift AoE")
            {
                voidRift_increasedArea += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.35f;
            }
            if (node.name == "Erasing Strike Tree Void Rift Time Rot")
            {
                voidRift_timeRotChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Erasing Strike Tree Void Rift DoT Taken")
            {
                voidRift_increasesDoTDamageTaken += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Erasing Strike Tree Void Rift Damage")
            {
                voidRift_increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Erasing Strike Tree Void Rift Damage VS Stunned")
            {
                voidRift_moreDamageAgainstStunned += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Erasing Strike Tree Void Rift Increased Damage Taken")
            {
                voidRift_increasesDamageTaken += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Erasing Strike Tree Stationary Void Beam")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    stationaryVoidBeamChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
                    increasedManaCost += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
                }
            }
            if (node.name == "Erasing Strike Tree Damage Against Damaged")
            {
                moreDamageAgainstDamaged += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            }
            if (node.name == "Erasing Strike Tree Damage Against Full Health")
            {
                moreDamageAgainstFullHealth += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Erasing Strike Tree Damage And Cull")
            {
                increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.01f;
                cullPercent += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.02f;
            }
            if (node.name == "Erasing Strike Tree No Attack Speed Scaling")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    noAttackSpeedScaling = true;
                    manaEfficiency += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.5f;
                }
            }
            if (node.name == "Erasing Strike Tree Void Damage")
            {
                addedVoidDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 5f;
            }
            if (node.name == "Erasing Strike Tree Cooldown And Mana Efficiency")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    cooldown = true;
                    manaEfficiency += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.5f;
                }
            }
            if (node.name == "Erasing Strike Tree Mana On Kill")
            {
                manaGainedOnKill += node.GetComponent<SkillTreeNode>().pointsAllocated * 2f;
            }
            if (node.name == "Erasing Strike Tree Damage And Mana Cost")
            {
                increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
                increasedManaCost += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Erasing Strike Tree Crit Chance")
            {
                addedCritChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.04f;
            }
            if (node.name == "Erasing Strike Tree Void Charge On Crit")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    voidChargeOnCrit = true;
                    addedCritChance -= 0.02f;
                }
            }
            if (node.name == "Erasing Strike Tree Crit Multiplier")
            {
                addedCritMultiplier += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.4f;
            }
            if (node.name == "Erasing Strike Tree Life On Kill")
            {
                percentLifeGainedOnKill += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.01f;
            }
            if (node.name == "Erasing Strike Tree Speed On Kill")
            {
                movementSpeedOnKill += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.04f;
            }
            if (node.name == "Erasing Strike Tree Void Beams")
            {
                voidBeamsOnkill = (node.GetComponent<SkillTreeNode>().pointsAllocated > 0);
            }
            if (node.name == "Erasing Strike Tree Lower Beam Requirement")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    reducedKillsRequiredForVoidBeams += node.GetComponent<SkillTreeNode>().pointsAllocated * 1;
                    increasedManaCost += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.35f;
                }
            }
            if (node.name == "Erasing Strike Tree Mana Efficiency")
            {
                manaEfficiency += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
                increasedArea += node.GetComponent<SkillTreeNode>().pointsAllocated * -0.1f;
            }
        }


        mutator.addedCharges = 0;
        mutator.addedChargeRegen = 0f;

        if (cooldown)
        {
            mutator.addedCharges = 1 + addedCharges;
            mutator.addedChargeRegen = 0.25f;
        }

        mutator.increasedDamage = increasedDamage;
        mutator.increasedRadius = increasedRadius;
        mutator.timeRotChance = timeRotChance;
        mutator.increasedStunChance = increasedStunChance;
        mutator.addedCritMultiplier = addedCritMultiplier;
        mutator.addedCritChance = addedCritChance;
        mutator.addedVoidDamage = addedVoidDamage;
        mutator.moreDamageAgainstFullHealth = moreDamageAgainstFullHealth;
        mutator.moreDamageAgainstDamaged = moreDamageAgainstDamaged;
        mutator.cullPercent = cullPercent;
        mutator.increasedRadius = Mathf.Sqrt(increasedArea + 1) - 1;

        mutator.voidRift_increasedDamage = voidRift_increasedDamage;
        mutator.voidRift_increasedRadius = Mathf.Sqrt(voidRift_increasedArea + 1) - 1;
        mutator.voidRift_timeRotChance = voidRift_timeRotChance;
        mutator.voidRift_increasesDamageTaken = voidRift_increasesDamageTaken;
        mutator.voidRift_increasesDoTDamageTaken = voidRift_increasesDoTDamageTaken;
        mutator.voidRift_increasedStunChance = voidRift_increasedStunChance;
        mutator.voidRift_moreDamageAgainstStunned = voidRift_moreDamageAgainstStunned;

        // on kill stats
        mutator.manaGainedOnKill = manaGainedOnKill;
        mutator.percentLifeGainedOnKill = percentLifeGainedOnKill;
        mutator.movementSpeedOnKill = movementSpeedOnKill;
        mutator.stationaryVoidBeamChance = stationaryVoidBeamChance;

        mutator.voidBeamsOnkill = voidBeamsOnkill;
        mutator.reducedKillsRequiredForVoidBeams = reducedKillsRequiredForVoidBeams;

        // other stats
        mutator.noAttackSpeedScaling = noAttackSpeedScaling;
        mutator.voidChargeOnCrit = voidChargeOnCrit;
        mutator.increasedManaCost = increasedManaCost;
        mutator.addedManaCostDivider = manaEfficiency;

    }
}
