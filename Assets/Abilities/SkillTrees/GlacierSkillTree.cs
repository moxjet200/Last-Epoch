using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlacierSkillTree : SkillTree
{

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.glacier);
    }

    public override void updateMutator()
    {
        GlacierMutator mutator = PlayerFinder.getPlayer().GetComponent<GlacierMutator>();
        float chanceToCreateIceVortexGlobal = 0f;
        float chanceToCreateIceVortex1 = 0f;
        float chanceToCreateIceVortex2 = 0f;
        float chanceToCreateIceVortex3 = 0f;
    
        float chillChanceGlobal = 0f;
        float chillChance1 = 0f;
        float chillChance2 = 0f;
        float chillChance3 = 0f;

        float increasedDamage1 = 0f;
        float increasedDamage2 = 0f;
        float increasedDamage3 = 0f;

        float moreDamageAgainstChilled = 0f;

        float increasedStunChance = 0f;
        float addedCritChance = 0f;

        bool reverseExplosions = false;
        bool removeLargestExplosion = false;
        bool noMovement = false;

        float percentManaGainedOnKill = 0f;
        float percentManaGainedOnHit = 0f;

        float increasedManaCost = 0f;
        float chanceForSuperIceVortex = 0f;

        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            if (node.name == "Glacier Skill Tree Chill Chance")
            {
                chillChanceGlobal = node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Glacier Skill Tree Chill Chance 1")
            {
                chillChance1 = node.GetComponent<SkillTreeNode>().pointsAllocated * 0.5f;
            }
            if (node.name == "Glacier Skill Tree Chill Chance 2 And Damage")
            {
                chillChance2 = node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
                increasedDamage2 = node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Glacier Skill Tree Chill Chance 3 And Damage")
            {
                chillChance3 = node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
                increasedDamage3 = node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            }
            if (node.name == "Glacier Skill Tree Vortex Chance")
            {
                chanceToCreateIceVortexGlobal = node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            }
            if (node.name == "Glacier Skill Tree Vortex Chance 1")
            {
                chanceToCreateIceVortex1 = node.GetComponent<SkillTreeNode>().pointsAllocated * 0.4f;
            }
            if (node.name == "Glacier Skill Tree Vortex Chance 2 And Damage")
            {
                chanceToCreateIceVortex2 = node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
                increasedDamage2 = node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Glacier Skill Tree Vortex Chance 3 And Damage")
            {
                chanceToCreateIceVortex3 = node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
                increasedDamage3 = node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            }
            if (node.name == "Glacier Skill Tree Super Vortex Chance")
            {
                chanceForSuperIceVortex = node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Glacier Skill Tree Damage 1")
            {
                increasedDamage1 = node.GetComponent<SkillTreeNode>().pointsAllocated * 0.6f;
            }
            if (node.name == "Glacier Skill Tree Damage 2")
            {
                increasedDamage2 = node.GetComponent<SkillTreeNode>().pointsAllocated * 0.5f;
            }
            if (node.name == "Glacier Skill Tree Damage 3")
            {
                increasedDamage3 = node.GetComponent<SkillTreeNode>().pointsAllocated * 0.35f;
            }
            if (node.name == "Glacier Skill Tree Removes Largest")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0) {
                    removeLargestExplosion = true;
                    increasedManaCost -= 0.8f;
                }
            }
            if (node.name == "Glacier Skill Tree Super Middle Explosion")
            {
                increasedDamage2 = node.GetComponent<SkillTreeNode>().pointsAllocated * 1f;
                increasedManaCost += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.4f;
            }
            if (node.name == "Glacier Skill Tree Mana On Hit")
            {
                percentManaGainedOnHit = node.GetComponent<SkillTreeNode>().pointsAllocated * 0.01f;
            }
            if (node.name == "Glacier Skill Tree Mana On Kill")
            {
                percentManaGainedOnKill = node.GetComponent<SkillTreeNode>().pointsAllocated * 0.01f;
            }
            if (node.name == "Glacier Skill Tree Reverse")
            {
                reverseExplosions = (node.GetComponent<SkillTreeNode>().pointsAllocated > 0);
            }

            if (node.name == "Glacier Skill No Movement")
            {
                noMovement = (node.GetComponent<SkillTreeNode>().pointsAllocated > 0);
                addedCritChance = node.GetComponent<SkillTreeNode>().pointsAllocated * -0.05f;
            }
            if (node.name == "Glacier Skill Stun Chance")
            {
                increasedStunChance = node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
            }
            if (node.name == "Glacier Skill Crit Chance")
            {
                addedCritChance = node.GetComponent<SkillTreeNode>().pointsAllocated * 0.03f;
            }
        }

        mutator.chanceToCreateIceVortexGlobal = chanceToCreateIceVortexGlobal;
        mutator.chanceToCreateIceVortex1 = chanceToCreateIceVortex1;
        mutator.chanceToCreateIceVortex2 = chanceToCreateIceVortex2;
        mutator.chanceToCreateIceVortex3 = chanceToCreateIceVortex3;

        mutator.chillChanceGlobal = chillChanceGlobal;
        mutator.chillChance1 = chillChance1;
        mutator.chillChance2 = chillChance2;
        mutator.chillChance3 = chillChance3;

        mutator.increasedDamage1 = increasedDamage1;
        mutator.increasedDamage2 = increasedDamage2;
        mutator.increasedDamage3 = increasedDamage3;

        mutator.moreDamageAgainstChilled = moreDamageAgainstChilled;

        mutator.increasedStunChance = increasedStunChance;
        mutator.addedCritChance = addedCritChance;

        mutator.reverseExplosions = reverseExplosions;
        mutator.removeLargestExplosion = removeLargestExplosion;
        mutator.noMovement = noMovement;

        mutator.percentManaGainedOnKill = percentManaGainedOnKill;
        mutator.percentManaGainedOnHit = percentManaGainedOnHit;

        mutator.increasedManaCost = increasedManaCost;
        mutator.chanceForSuperIceVortex = chanceForSuperIceVortex;
    }
}