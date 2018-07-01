using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolcanicOrbTree : SkillTree
{

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.volcanicOrb);
    }

    public override void updateMutator()
    {
        VolcanicOrbMutator mutator = PlayerFinder.getPlayer().GetComponent<VolcanicOrbMutator>();
        float increasedSpeed = 0f;
         float increasedShrapnelSpeed = 0f;
         bool shrapnelPierces = false;
         float increasedShrapnelDamage = 0f;
         float increasedShrapnelStunChance = 0f;
         bool convertToCold = false;
         float chillChance = 0f;
         float increasedDuration = 0f;
         bool leavesExplosion = false;
         float explosionIgniteChance = 0f;
         float increasedExplosionDamage = 0f;
         bool delayedExpolosionAtStart = false;
         float increasedCastSpeed = 0f;
         bool leavesExplosiveGround = false;
         float increasedExplosiveGroundFrequency = 0f;
         float increasedDamage = 0f;
         float moreShrapnelDamageAgainstChilled = 0f;
        bool baseDurationis2 = false;
        float increasedManaCost = 0f;
        float manaEfficiency = 0f;
        int addedShrapnelProjectiles = 0;

        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            if (node.name == "Volcanic Orb Tree Shrapnel Speed")
            {
                increasedShrapnelSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Volcanic Orb Tree Damage VS Duration")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    baseDurationis2 = true;
                    increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
                    increasedShrapnelDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
                    increasedExplosiveGroundFrequency += 0.2f;
                }
            }
            if (node.name == "Volcanic Orb Tree Increased Speed")
            {
                increasedSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Volcanic Orb Tree Frozen Orb")
            {
                convertToCold = (node.GetComponent<SkillTreeNode>().pointsAllocated > 0);
                increasedShrapnelDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Volcanic Orb Tree Chill Chance")
            {
                chillChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Volcanic Orb Tree More Damage Against Chilled")
            {
                moreShrapnelDamageAgainstChilled += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Volcanic Orb Tree Shrapnel Stun Chance")
            {
                increasedShrapnelStunChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.35f;
            }
            if (node.name == "Volcanic Orb Tree Shrapnel Damage")
            {
                increasedShrapnelDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            }
            if (node.name == "Volcanic Orb Tree Shrapnel Pierces")
            {
                shrapnelPierces = (node.GetComponent<SkillTreeNode>().pointsAllocated > 0);
                increasedShrapnelSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Volcanic Orb Tree Duration And Mana")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    manaEfficiency += 0.3f;
                    increasedSpeed += -0.2f;
                    baseDurationis2 = true;
                    increasedExplosiveGroundFrequency += 0.2f;
                }
            }
            if (node.name == "Volcanic Orb Tree Explosion On Death")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    leavesExplosion = true;
                }
            }
            if (node.name == "Volcanic Orb Tree Explosion Damage")
            {
                increasedExplosionDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
            }
            if (node.name == "Volcanic Orb Tree Explosion Ignite Chance")
            {
                explosionIgniteChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.34f;
            }
            if (node.name == "Volcanic Orb Tree Explosion On Cast Chance")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    delayedExpolosionAtStart = true;
                }
            }
            if (node.name == "Volcanic Orb Tree Orb Damage")
            {
                increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.35f;
            }
            if (node.name == "Volcanic Orb Tree Long Cast")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    // 70% longer to cast
                    increasedCastSpeed -= 0.41f;
                    increasedDamage += 0.5f;
                    increasedShrapnelDamage += 0.5f;
                    increasedExplosionDamage += 0.5f;
                    increasedManaCost += 0.5f;
                }
            }
            if (node.name == "Volcanic Orb Tree Explosive Ground")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    leavesExplosiveGround = true;
                    increasedManaCost += 0.3f;
                }
            }
            // needs 3 point maximum
            if (node.name == "Volcanic Orb Tree Explosive Ground Frequency")
            {
                increasedExplosiveGroundFrequency += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
                increasedManaCost += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.05f;
            }
            if (node.name == "Volcanic Orb Tree Less Shrapnel")
            {
                addedShrapnelProjectiles += node.GetComponent<SkillTreeNode>().pointsAllocated * -1;
                increasedShrapnelDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
                increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
            }
        }
        mutator.increasedSpeed = increasedSpeed;
        mutator.increasedShrapnelSpeed = increasedShrapnelSpeed;
        mutator.shrapnelPierces = shrapnelPierces;
        mutator.increasedShrapnelDamage = increasedShrapnelDamage;
        mutator.increasedShrapnelStunChance = increasedShrapnelStunChance;

        mutator.convertToCold = convertToCold;
        mutator.chillChance = chillChance;
        mutator.increasedDuration = increasedDuration;
        mutator.leavesExplosion = leavesExplosion;
        mutator.explosionIgniteChance = explosionIgniteChance;
        mutator.increasedExplosionDamage = increasedExplosionDamage;

        mutator.delayedExpolosionAtStart = delayedExpolosionAtStart;
        mutator.increasedCastSpeed = increasedCastSpeed;
        mutator.leavesExplosiveGround = leavesExplosiveGround;

        mutator.increasedExplosiveGroundFrequency = increasedExplosiveGroundFrequency;
        mutator.increasedDamage = increasedDamage;
        mutator.moreShrapnelDamageAgainstChilled = moreShrapnelDamageAgainstChilled;
        mutator.baseDurationis2 = baseDurationis2;
        mutator.addedShrapnelProjectiles = addedShrapnelProjectiles;

        mutator.increasedManaCost = increasedManaCost;
        mutator.addedManaCostDivider = manaEfficiency;
    }
}
