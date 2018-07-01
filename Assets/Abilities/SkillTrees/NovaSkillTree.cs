using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovaSkillTree : SkillTree {

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.nova);
    }

    public override void updateMutator()
    {
        NovaMutator mutator = PlayerFinder.getPlayer().GetComponent<NovaMutator>();
        bool canIceNova = false;
        bool canFireNova = false;
        bool canLightningNova = false;
        float chanceForChainingIceNova = 0f;
        float addedCritChance = 0f;
        float addedCritDamage = 0f;
        bool canTarget = false;
        float chanceToGainWardOnKill = 0f;
        float wardOnKill = 0f;
        float igniteChance = 0f;
        float increasedDamage = 0f;
        float chanceToGainWardOnHit = 0f;
        float increasedWardGained = 0f;
        float chillChance = 0f;
        float increasedCastSpeed = 0f;
        float moreDamageAgainstFullHealth = 0f;
        float addedCharges = 0;
        float addedChargeRegen = 0;
        float chanceToAttachSparkCharge = 0f;
        float wardOnHit = 0f;
        float wardRetentionOnKill = 0f;
        float increasedArea = 0f;
        float shockChance = 0f;
        float shockEffectOnHit = 0f;
        float increasedManaCost = 0f;
        float manaEfficiency = 0f;
        float increasedCooldownRecoverySpeed = 0f;

        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            if (node.name == "Nova Skill Tree Ice Nova")
            {
                canIceNova = (node.GetComponent<SkillTreeNode>().pointsAllocated > 0);
            }
            if (node.name == "Nova Skill Tree Fire Nova")
            {
                canFireNova = (node.GetComponent<SkillTreeNode>().pointsAllocated > 0);
            }
            if (node.name == "Nova Skill Tree Lightning Nova")
            {
                canLightningNova = (node.GetComponent<SkillTreeNode>().pointsAllocated > 0);
            }
            if (node.name == "Nova Skill Tree Chaining Ice Nova")
            {
                chanceForChainingIceNova += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Nova Skill Tree Crit Chance")
            {
                addedCritChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.03f;
            }
            if (node.name == "Nova Skill Tree Chill Chance")
            {
                chillChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Nova Skill Tree Crit Damage")
            {
                addedCritDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.35f;
            }
            if (node.name == "Nova Skill Tree Target")
            {
                canTarget = (node.GetComponent<SkillTreeNode>().pointsAllocated > 0);
            }
            if (node.name == "Nova Skill Tree Chance To Gain Ward On Kill")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    chanceToGainWardOnKill += 0.1f;
                    wardOnKill += node.GetComponent<SkillTreeNode>().pointsAllocated * 15f;
                }
            }
            if (node.name == "Nova Skill Tree Ward Retention")
            {
                wardRetentionOnKill += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            }
            if (node.name == "Nova Skill Tree Cooldown Recovery")
            {
                increasedCooldownRecoverySpeed = node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Nova Skill Tree Increased Damage")
            {
                increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
                increasedManaCost = node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Nova Skill Tree Chance To Gain Ward On Hit")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    chanceToGainWardOnHit += 0.07f;
                    wardOnHit += node.GetComponent<SkillTreeNode>().pointsAllocated * 10f;
                }
            }
            if (node.name == "Nova Skill Tree Increased Ward Gained")
            {
                increasedWardGained += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Nova Skill Tree Cast Speed")
            {
                increasedCastSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.05f;
                moreDamageAgainstFullHealth += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.05f;
            }
            if (node.name == "Nova Skill Tree Spark Charge")
            {
                chanceToAttachSparkCharge += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
                increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.05f;
            }
            if (node.name == "Nova Skill Tree Shock Chance And Mana Efficiency")
            {
                shockChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
                manaEfficiency += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Nova Skill Tree Shock Effect On Hit")
            {
                shockEffectOnHit += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Nova Skill Tree Ignite Chance")
            {
                igniteChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Nova Skill Tree Increased Area")
            {
                increasedArea += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.4f;
            }
            if (node.name == "Nova Skill Tree Increased Area Cooldown")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    addedCharges += 1;
                    addedChargeRegen += 0.25f;
                    increasedArea += 3f;
                }
            }
            if (node.name == "Nova Skill Tree Extra Charge")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    addedCharges += node.GetComponent<SkillTreeNode>().pointsAllocated;
                }
            }
        }



        mutator.canIceNova = canIceNova;
        mutator.canFireNova = canFireNova;
        mutator.canLightningNova = canLightningNova;
        mutator.chanceForChainingIceNova = chanceForChainingIceNova;
        mutator.addedCritChance = addedCritChance;
        mutator.addedCritMultiplier = addedCritDamage;
        mutator.canTarget = canTarget;
        mutator.chanceToGainWardOnKill = chanceToGainWardOnKill;
        mutator.wardOnKill = wardOnKill;
        mutator.igniteChance = igniteChance;
        mutator.increasedDamage = increasedDamage;
        mutator.chanceToGainWardOnHit = chanceToGainWardOnHit;
        mutator.increasedWardGained = increasedWardGained;
        mutator.chillChance = chillChance;
        mutator.increasedCastSpeed = increasedCastSpeed;
        mutator.moreDamageAgainstFullHealth = moreDamageAgainstFullHealth;
        mutator.addedCharges = addedCharges;
        mutator.addedChargeRegen = addedChargeRegen;
        mutator.chanceToAttachSparkCharge = chanceToAttachSparkCharge;
        mutator.wardOnHit = wardOnHit;
        mutator.wardRetentionOnKill = wardRetentionOnKill;
        mutator.increasedRadius = Mathf.Sqrt(increasedArea + 1) - 1;
        mutator.shockChance = shockChance;
        mutator.shockEffectOnHit = shockEffectOnHit;
        mutator.increasedManaCost = increasedManaCost;
        mutator.addedManaCostDivider = manaEfficiency;

        mutator.addedChargeRegen = mutator.addedChargeRegen * (1 + increasedCooldownRecoverySpeed);
    }
}
