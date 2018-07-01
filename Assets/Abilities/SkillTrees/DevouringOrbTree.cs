using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevouringOrbTree : SkillTree
{

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.devouringOrb);
    }

    public override void updateMutator()
    {
        DevouringOrbMutator mutator = PlayerFinder.getPlayer().GetComponent<DevouringOrbMutator>();
        bool dealsDamage = false;
        float increasedDamage = 0f;
        float movementSpeedOnCast = 0f;
        bool orbitsCaster = false;
        float increasedOrbitDistance = 0f;
        float increasedDuration = 0f;
        bool castsAbyssalOrb = false;
        float increasedAbyssalOrbFrequency = 0f;

        bool voidEruptionOnDeath = false;
        float increasedCastSpeed = 0f;

        // void rift stats
        float voidRift_increasedDamage = 0f;
        float voidRift_increasedAoE = 0f;
        float voidRift_timeRotChance = 0f;
        float voidRift_increasesDamageTaken = 0f;
        float voidRift_increasesDoTDamageTaken = 0f;
        float voidRift_increasedStunChance = 0f;
        float voidRift_moreDamageAgainstStunned = 0f;
        float voidRift_igniteChance = 0f;
        float voidRift_damageAgainstIgnited = 0f;
        float voidRift_damageAgainstTimeRotting = 0f;
        float voidRift_increasedDamageGrowth = 0f;
        float voidRift_increasedAreaGrowth = 0f;
        bool voidRift_noGrowth = false;

        float increasedManaCost = 0f;
        float manaEfficiency = 0f;
        float cooldownRecoverySpeed = 0f;

        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            if (node.name == "Devouring Orb Tree Orbits")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    orbitsCaster = true;
                    increasedManaCost += 0.3f;
                    voidRift_increasedAreaGrowth += -0.5f;
                    voidRift_increasedDamageGrowth += -0.5f;
                }
            }
            if (node.name == "Devouring Orb Tree Orbit Distance")
            {
                increasedOrbitDistance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.5f;
            }
            if (node.name == "Devouring Orb Tree Movespeed")
            {
                movementSpeedOnCast += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Devouring Orb Tree Deals Damage")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    dealsDamage = true;
                    increasedManaCost += 0.2f;
                }
            }
            if (node.name == "Devouring Orb Tree Increased Damage")
            {
                increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
            }
            if (node.name == "Devouring Orb Tree Mana Efficiency")
            {
                manaEfficiency += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Devouring Orb Tree Duration vs growth")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    increasedDuration += 0.5f;
                    voidRift_increasedAreaGrowth = -100f;
                    voidRift_increasedDamageGrowth = -100f;
                }
            }
            if (node.name == "Devouring Orb Tree Abyssal Orb")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    castsAbyssalOrb = true;
                }
            }
            if (node.name == "Devouring Orb Tree Abyssal Orb Frequency")
            {
                manaEfficiency += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
            }
            if (node.name == "Devouring Orb Tree Erupts On Death")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    voidEruptionOnDeath = true;
                    increasedDuration -= 0.4f;
                }
            }
            if (node.name == "Devouring Orb Tree Reduced Duration")
            {
                increasedDuration += node.GetComponent<SkillTreeNode>().pointsAllocated * -0.1f;
                manaEfficiency += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.35f;
            }
            if (node.name == "Devouring Orb Tree Cast Speed")
            {
                cooldownRecoverySpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
                increasedCastSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.05f;
            }
            if (node.name == "Devouring Orb Tree Cooldown Recovery")
            {
                cooldownRecoverySpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Devouring Orb Tree Rift Damage")
            {
                voidRift_increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.12f;
            }
            if (node.name == "Devouring Orb Tree Rift Damage Growth")
            {
                voidRift_increasedDamageGrowth += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            }
            if (node.name == "Devouring Orb Tree Rift AoE Growth")
            {
                voidRift_increasedAreaGrowth += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Devouring Orb Tree Rift AoE")
            {
                voidRift_increasedAoE += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
            }
            if (node.name == "Devouring Orb Tree Rift Damage vs Stunned")
            {
                voidRift_moreDamageAgainstStunned += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            }
            if (node.name == "Devouring Orb Tree Rift Time Rot")
            {
                voidRift_timeRotChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            }
            if (node.name == "Devouring Orb Tree Rift Damage vs Time Rot")
            {
                voidRift_damageAgainstTimeRotting += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            }
            if (node.name == "Devouring Orb Tree Rift Ignite")
            {
                voidRift_igniteChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            }
            if (node.name == "Devouring Orb Tree Rift Damage vs Ignite")
            {
                voidRift_damageAgainstIgnited += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
        }
        
        if (voidRift_increasedAreaGrowth < -1) { voidRift_increasedAreaGrowth = -1; }
        if (voidRift_increasedDamageGrowth < -1) { voidRift_increasedDamageGrowth = -1; }


        mutator.dealsDamage = dealsDamage;
        mutator. increasedDamage = increasedDamage;
        mutator. movementSpeedOnCast = movementSpeedOnCast;
        mutator.orbitsCaster = orbitsCaster;
        mutator. increasedOrbitDistance = increasedOrbitDistance;
        mutator. increasedDuration = increasedDuration;
        mutator.castsAbyssalOrb = castsAbyssalOrb;
        mutator. increasedAbyssalOrbFrequency = increasedAbyssalOrbFrequency;

        mutator.voidEruptionOnDeath = voidEruptionOnDeath;
        mutator. increasedCastSpeed = increasedCastSpeed;

        // void rift stats
        mutator. voidRift_increasedDamage = voidRift_increasedDamage;
        mutator. voidRift_timeRotChance = voidRift_timeRotChance;
        mutator. voidRift_increasesDamageTaken = voidRift_increasesDamageTaken;
        mutator. voidRift_increasesDoTDamageTaken = voidRift_increasesDoTDamageTaken;
        mutator. voidRift_increasedStunChance = voidRift_increasedStunChance;
        mutator. voidRift_moreDamageAgainstStunned = voidRift_moreDamageAgainstStunned;
        mutator. voidRift_igniteChance = voidRift_igniteChance;
        mutator. voidRift_damageAgainstIgnited = voidRift_damageAgainstIgnited;
        mutator. voidRift_damageAgainstTimeRotting = voidRift_damageAgainstTimeRotting;
        mutator. voidRift_increasedDamageGrowth = voidRift_increasedDamageGrowth;
        mutator. voidRift_increasedAreaGrowth = voidRift_increasedAreaGrowth;
        mutator.voidRift_noGrowth = voidRift_noGrowth;

        mutator.voidRift_increasedRadius = Mathf.Sqrt(voidRift_increasedAoE + 1) - 1;
        mutator.increasedManaCost = increasedManaCost;
        mutator.addedManaCostDivider = manaEfficiency;
        mutator.addedChargeRegen = cooldownRecoverySpeed * ability.chargesGainedPerSecond;
    }
}
