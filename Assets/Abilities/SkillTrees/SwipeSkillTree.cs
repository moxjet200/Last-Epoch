using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeSkillTree : SkillTree {

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.swipe);
    }

    public override void updateMutator()
    {
        SwipeMutator mutator = PlayerFinder.getPlayer().GetComponent<SwipeMutator>();
        float chanceToPoison = 0f;
        float addedCritChance = 0f;
        float addedCritDamage = 0f;
        float cullPercent = 0f;
        bool travels = false;
        float addedSpeed = 0f;
        float increasedDamage = 0f;
        float addedFireDamage = 0f;
        float chanceToIgnite = 0f;
        float chanceOfExtraProjectiles = 0f;
        float chanceToShredArmour = 0f;
        float clawTotemOnKillChance = 0f;
        float spiritWolvesOnFirstHitChance = 0f;
        float increasedDamageOnFirstHit = 0f;
        float increasedAttackSpeedOnFirstHit = 0f;
        float increasedCritChanceOnFirstHit = 0f;
        float addedCritMultiOnFirstHit = 0f;
        bool cooldown = false;
        bool slows = false;
        float increasedAttackSpeed = 0f;
        float increasedDuration = 0f;
        bool movesRandomly = false;
        float increasedRandomisation = 0f;

        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            if (node.name == "Swipe Skill Tree Poison Chance")
            {
                chanceToPoison += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Swipe Skill Tree Crit Chance")
            {
                addedCritChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.05f;
            }
            if (node.name == "Swipe Skill Tree Crit Damage")
            {
                addedCritDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.5f;
            }
            if (node.name == "Swipe Skill Tree Cull Percent")
            {
                cullPercent += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.07f;
            }
            if (node.name == "Swipe Skill Tree Travels")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    travels = true;
                    increasedDamage -= 0.35f;
                }
            }
            if (node.name == "Swipe Skill Tree Added Speed")
            {
                addedSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 3f;
            }
            if (node.name == "Swipe Skill Tree Increased Damage")
            {
                increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Swipe Skill Tree Added Fire")
            {
                addedFireDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 5f;
            }
            if (node.name == "Swipe Skill Tree Ignite Chance")
            {
                chanceToIgnite += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.16f;
            }
            if (node.name == "Swipe Skill Tree Extra Projectiles")
            {
                chanceOfExtraProjectiles += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.143f;
            }
            if (node.name == "Swipe Skill Tree Shred Armour")
            {
                chanceToShredArmour += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Swipe Skill Tree Spirit Wolves")
            {
                spiritWolvesOnFirstHitChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            }
            if (node.name == "Swipe Skill Tree Damage On Hit")
            {
                increasedDamageOnFirstHit += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Swipe Skill Tree Attack Speed On Hit")
            {
                increasedAttackSpeedOnFirstHit += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }

            if (node.name == "Swipe Skill Tree Claw Totem")
            {
                clawTotemOnKillChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Swipe Skill Tree Slows")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    slows = true;
                    increasedDamage += 0.7f;
                    cooldown = true;
                }
            }
            if (node.name == "Swipe Skill Tree Always Spirit Wolves")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    spiritWolvesOnFirstHitChance = 1f;
                }
            }
            if (node.name == "Swipe Skill Tree Crit Chance On Hit")
            {
                increasedCritChanceOnFirstHit += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Swipe Skill Tree Crit Damage On Hit")
            {
                addedCritMultiOnFirstHit += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Swipe Skill Tree Attack Speed On Hit Keystone")
            {
                increasedAttackSpeedOnFirstHit += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            }
            if (node.name == "Swipe Skill Tree Random Movement")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    movesRandomly = true;
                    increasedDuration += 0.3f;
                }
            }
            if (node.name == "Swipe Skill Tree Increased Randomisation")
            {
                increasedRandomisation += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
                increasedAttackSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
        }
        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            if (node.name == "Swipe Skill Tree No Randomisation")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    movesRandomly = false;
                    increasedDamage -= 0.2f;
                }
            }
        }


            mutator.addedCharges = 0;
        mutator.addedChargeRegen = 0f;

        if (cooldown)
        {
            mutator.addedCharges = 1;
            mutator.addedChargeRegen = 0.5f;
        }

        mutator.chanceToPoison = chanceToPoison;
        mutator.addedCritChance = addedCritChance;
        mutator.addedCritMultiplier = addedCritDamage;
        mutator.cullPercent = cullPercent;
        mutator.travels = travels;
        mutator.addedSpeed = addedSpeed;
        mutator.increasedDamage = increasedDamage;
        mutator.addedFireDamage = addedFireDamage;
        mutator.chanceToIgnite = chanceToIgnite;
        mutator.chanceOfExtraProjectiles = chanceOfExtraProjectiles;
        mutator.chanceToShredArmour = chanceToShredArmour;
        mutator.clawTotemOnKillChance = clawTotemOnKillChance;
        mutator.slows = slows;
        mutator.spiritWolvesOnFirstHitChance = spiritWolvesOnFirstHitChance;
        mutator.increasedDamageOnFirstHit = increasedDamageOnFirstHit;
        mutator.increasedAttackSpeedOnFirstHit = increasedAttackSpeedOnFirstHit;
        mutator.increasedCritChanceOnFirstHit = increasedCritChanceOnFirstHit;
        mutator.addedCritMultiOnFirstHit = addedCritMultiOnFirstHit;

        mutator.increasedAttackSpeed = increasedAttackSpeed;
        mutator.increasedDuration = increasedDuration;
        mutator.movesRandomly = movesRandomly;
        mutator.increasedRandomisation = increasedRandomisation;
    }
}
