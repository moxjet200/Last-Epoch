using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornBurstSkillTree : SkillTree {

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.thornBurst);
    }

    public override void updateMutator()
    {
        ThornBurstMutator mutator = PlayerFinder.getPlayer().GetComponent<ThornBurstMutator>();
        float chanceToPoison = 0f;
        float chanceToBleed = 0f;
        float extraProjectilesChance = 0f;
        float pierceChance = 0f;
        float reducedSpread = 0f;
        float addedSpeed = 0f;
        bool thornShield = false;
        float addedShieldDuration = 0f;
        float chanceOfRecreatingThornShield = 0f;
        bool thornShieldAiming = false;
        bool canCastOnAllies = false;
        float castWhenHitChance = 0f;
        float increasedDamage = 0f;
        float thornTrailOnKillChance = 0f;
        float sunderingThornsOnHitChance = 0f;
        float increasedSunderingThornsCooldownRecovery = 0f;

        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            if (node.name == "Thorn Burst Skill Tree Poison Chance")
            {
                chanceToPoison += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Thorn Burst Skill Tree Extra Projectiles Chance")
            {
                extraProjectilesChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.34f;
            }
            if (node.name == "Thorn Burst Skill Tree Pierce Chance")
            {
                pierceChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Thorn Burst Skill Tree Reduced Spread")
            {
                reducedSpread += node.GetComponent<SkillTreeNode>().pointsAllocated * 20f;
            }
            if (node.name == "Thorn Burst Skill Tree Added Speed")
            {
                addedSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 1.8f;
            }
            if (node.name == "Thorn Burst Skill Tree Thorn Shield")
            {
                thornShield = (node.GetComponent<SkillTreeNode>().pointsAllocated > 0);
            }
            if (node.name == "Thorn Burst Skill Tree Added Shield Duration")
            {
                addedShieldDuration += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
            }
            if (node.name == "Thorn Burst Skill Tree Recreation Chance")
            {
                chanceOfRecreatingThornShield += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Thorn Burst Skill Tree Shield Aiming")
            {
                thornShieldAiming = (node.GetComponent<SkillTreeNode>().pointsAllocated > 0);
                reducedSpread += node.GetComponent<SkillTreeNode>().pointsAllocated * 20f;
            }
            if (node.name == "Thorn Burst Skill Tree Cast On Allies")
            {
                canCastOnAllies = (node.GetComponent<SkillTreeNode>().pointsAllocated > 0);
            }
            if (node.name == "Thorn Burst Skill Tree Cast When Hit")
            {
                castWhenHitChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Thorn Burst Skill Tree Damage and Cast When Hit")
            {
                castWhenHitChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.05f;
                increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.05f;
            }
            if (node.name == "Thorn Burst Thorn Trail")
            {
                thornTrailOnKillChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
                increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * -0.2f;
            }
            if (node.name == "Thorn Burst Sundering Thorns")
            {
                sunderingThornsOnHitChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.07f;
            }
            if (node.name == "Thorn Burst Sundering Thorns Cooldown")
            {
                increasedSunderingThornsCooldownRecovery += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.35f;
            }
            if (node.name == "Thorn Burst Bleed Chance")
            {
                chanceToBleed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
                increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
        }
        mutator.chanceToPoison = chanceToPoison;
        mutator.extraProjectilesChance = extraProjectilesChance;
        mutator.pierceChance = pierceChance;
        mutator.reducedSpread = reducedSpread;
        mutator.addedSpeed = addedSpeed;
        mutator.thornShield = thornShield;
        mutator.addedShieldDuration = addedShieldDuration;
        mutator.chanceOfRecreatingThornShield = chanceOfRecreatingThornShield;
        mutator.thornShieldAiming = thornShieldAiming;
        mutator.canCastOnAllies = canCastOnAllies;
        mutator.castWhenHitChance = castWhenHitChance;
        mutator.increasedDamage = increasedDamage;
        mutator.thornTrailOnKillChance = thornTrailOnKillChance;
        mutator.sunderingThornsOnHitChance = sunderingThornsOnHitChance;
        mutator.increasedSunderingThornsCooldownRecovery = increasedSunderingThornsCooldownRecovery;
        mutator.chanceToBleed = chanceToBleed;
    }
}
