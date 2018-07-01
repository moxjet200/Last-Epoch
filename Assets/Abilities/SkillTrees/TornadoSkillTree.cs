using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoSkillTree : SkillTree {

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.tornado);
    }

    public override void updateMutator()
    {
        TornadoMutator mutator = PlayerFinder.getPlayer().GetComponent<TornadoMutator>();
        float additionalDuration = 0f;
        float pullMultiplier = 1f;
        float increasedBasePhysicalDamage = 0f;
        bool castsLightning = false;
        float lightningInterval = 2f;
        float lightningRange = 1f;
        bool stationary = false;
        bool fireTornado = false;
        bool attaches = false;
        float increasedManaCost = 0f;
        float doubleCastChance = 0f;
        bool leavesStormOrbs = false;
        float increasedStormOrbFrequency = 0f;
        bool ignitesInAoe = false;
        float increasedIgniteFrequency = 0f;
        float increasedCastSpeed = 0f;

        float movementSpeedOnCast = 0f;
        float attackAndCastSpeedOnCast = 0f;
        float manaRegenOnCast = 0f;
        float increasedBuffDuration = 0f;
        float manaEfficiency = 0f;

        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            if (node.name == "Tornado Skill Tree Additional Duration")
            {
                additionalDuration += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.7f;
            }
            if (node.name == "Tornado Skill Tree Pull Multiplier")
            {
                pullMultiplier += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Tornado Skill Tree Added Physical Damage")
            {
                increasedBasePhysicalDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.17f;
                increasedManaCost += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.05f;
            }
            if (node.name == "Tornado Skill Tree Casts Lightning")
            {
                castsLightning = (node.GetComponent<SkillTreeNode>().pointsAllocated > 0);
            }
            if (node.name == "Tornado Skill Tree Casts Lightning Faster")
            {
                lightningInterval /= 1 + (node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f);
            }
            if (node.name == "Tornado Skill Tree Lightning Range")
            {
                lightningRange += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
            }
            if (node.name == "Tornado Skill Fire Tornado")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    fireTornado = true;
                    increasedBasePhysicalDamage += 0.1f;
                }
            }
            if (node.name == "Tornado Skill Tree Stationary")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    stationary = (node.GetComponent<SkillTreeNode>().pointsAllocated > 0);
                    increasedBasePhysicalDamage -= 0.2f;
                }
            }
            if (node.name == "Tornado Skill Tree Attaches")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    attaches = (node.GetComponent<SkillTreeNode>().pointsAllocated > 0);
                    increasedBasePhysicalDamage += 0.5f;
                    increasedManaCost += 0.35f;
                }
            }
            if (node.name == "Tornado Skill Tree Double Cast")
            {
                doubleCastChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
                pullMultiplier -= 0.15f;
            }
            if (node.name == "Tornado Skill Tree Storm Orbs")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    leavesStormOrbs = true;
                    increasedCastSpeed -= 0.2f;
                }
            }
            if (node.name == "Tornado Skill Tree Storm Orb Frequency")
            {
                increasedStormOrbFrequency += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Tornado Skill Tree Ignites")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    ignitesInAoe = true;
                    increasedCastSpeed -= 0.2f;
                }
            }
            if (node.name == "Tornado Skill Tree Ignite Frequency")
            {
                increasedIgniteFrequency += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Tornado Skill Tree Cast Speed")
            {
                increasedCastSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.06f;
            }
            if (node.name == "Tornado Skill Tree Movement Speed On Cast")
            {
                movementSpeedOnCast += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Tornado Skill Tree Attack And Cast Speed On Cast")
            {
                attackAndCastSpeedOnCast += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.07f;
            }
            if (node.name == "Tornado Skill Tree Mana Regen On Cast")
            {
                manaRegenOnCast += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.03f;
                manaEfficiency += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            }
            if (node.name == "Tornado Skill Tree Buff Duration")
            {
                increasedBuffDuration += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
            }
        }
        mutator.additionalDuration = additionalDuration;
        mutator.pullMultiplier = pullMultiplier;
        mutator.increasedBasePhysicalDamage = increasedBasePhysicalDamage;
        mutator.castsLightning = castsLightning;
        mutator.lightningInterval = lightningInterval;
        mutator.lightningRange = lightningRange;
        mutator.stationary = stationary;
        mutator.fireTornado = fireTornado;
        mutator.increasedManaCost = increasedManaCost;
        mutator.attaches = attaches;
        mutator.doubleCastChance = doubleCastChance;

        mutator.leavesStormOrbs = leavesStormOrbs;
        mutator.increasedStormOrbFrequency = increasedStormOrbFrequency;
        mutator.ignitesInAoe = ignitesInAoe;
        mutator.increasedIgniteFrequency = increasedIgniteFrequency;
        mutator.increasedCastSpeed = increasedCastSpeed;

        mutator.movementSpeedOnCast = movementSpeedOnCast;
        mutator.attackAndCastSpeedOnCast = attackAndCastSpeedOnCast;
        mutator.manaRegenOnCast = manaRegenOnCast;
        mutator.increasedBuffDuration = increasedBuffDuration;
        mutator.addedManaCostDivider = manaEfficiency;
    }
}
