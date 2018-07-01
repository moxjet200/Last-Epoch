using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaStrikeSkillTree : SkillTree {

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.manaStrike);
    }

    public override void updateMutator()
    {
        ManaStrikeMutator mutator = PlayerFinder.getPlayer().GetComponent<ManaStrikeMutator>();
        float increasedAttackSpeed = 0f;
        float additionalManaOnHit = 0f;
        float manaOnKill = 0f;
        float manaOnCrit = 0f;
        float increasedDamageOnHit = 0f;
        float increasedSpellDamageOnHit = 0f;
        float increasedManaOnHit = 0f;
        float increasedDamage = 0f;
        float addedCritChance = 0f;
        bool removesCritMultiplier = false;
        bool canTeleport = false;
        int addedCharges = 0;
        float addedChargeRegen = 0f;
        float chanceToKnockBack = 0f;
        float tenacityDebuffOnHit = 0f;
        float chanceToAttachSparkCharge = 0f;

        float addedLightningPerMana = 0f;
        float critChancePerMana = 0f;
        bool lightningStrikeOnHit = false;
        float reducedManaLostOnLightningStrike = 0f;
        float increasedDamageWhileOutOfMana = 0f;
        float increasedArea = 0f;
        float manaGainOnHitWhileOutOfMana = 0f;

        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            if (node.name == "Mana Strike Skill Tree Increased Attack Speed")
            {
                increasedAttackSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.06f;
            }
            if (node.name == "Mana Strike Skill Tree Additional Mana On Hit")
            {
                additionalManaOnHit += node.GetComponent<SkillTreeNode>().pointsAllocated * 3f;
            }
            if (node.name == "Mana Strike Skill Tree Mana On Kill")
            {
                manaOnKill += node.GetComponent<SkillTreeNode>().pointsAllocated * 3f;
            }
            if (node.name == "Mana Strike Skill Tree Mana On Crit")
            {
                manaOnCrit += node.GetComponent<SkillTreeNode>().pointsAllocated * 3f;
            }
            if (node.name == "Mana Strike Skill Tree Increased Damage On Hit")
            {
                increasedDamageOnHit += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.04f;
            }
            if (node.name == "Mana Strike Skill Tree Increased Spell Damage On Hit")
            {
                increasedSpellDamageOnHit += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.05f;
            }
            if (node.name == "Mana Strike Skill Tree Target")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    canTeleport = true;
                    increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
                    increasedManaOnHit += node.GetComponent<SkillTreeNode>().pointsAllocated * -0.7f;
                }
            }
            if (node.name == "Mana Strike Skill Tree Cooldown Mana On Hit")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    if (increasedManaOnHit < 0) { increasedManaOnHit *= 0.5f; }
                    else { increasedManaOnHit = 1; }
                    addedCharges += 1;
                    addedChargeRegen += 0.25f;
                }
            }
            //if (node.name == "Mana Strike Skill Tree Crit Chance")
            //{
            //    manaOnCrit += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.06f;
            //}
            if (node.name == "Mana Strike Skill Tree No Crit Damage")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    addedCritChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.4f;
                    removesCritMultiplier = true;
                }
            }
            if (node.name == "Mana Strike Skill Tree Knockback")
            {
                chanceToKnockBack += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.34f;
            }
            if (node.name == "Mana Strike Skill Tree Tenacity Debuff")
            {
                tenacityDebuffOnHit += node.GetComponent<SkillTreeNode>().pointsAllocated * 25f;
            }
            if (node.name == "Mana Strike Skill Tree Spark Charge")
            {
                chanceToAttachSparkCharge += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Mana Strike Skill Tree Lightning Damage Per Mana")
            {
                addedLightningPerMana += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.05f;
            }
            if (node.name == "Mana Strike Skill Tree Crit Per Mana")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    critChancePerMana += 0.001f;
                    addedCritChance += -0.05f;
                }
            }
            if (node.name == "Mana Strike Skill Tree Damage While Out Of mana")
            {
                increasedDamageWhileOutOfMana += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Mana Strike Skill Tree Reduced Mana Gained")
            {
                increasedDamageWhileOutOfMana += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
                additionalManaOnHit += node.GetComponent<SkillTreeNode>().pointsAllocated * -1f;
            }
            if (node.name == "Mana Strike Skill Tree Lightning On Hit")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    lightningStrikeOnHit = true;
                }
            }
            if (node.name == "Mana Strike Skill Tree Lightning Reduced Mana Cost")
            {
                reducedManaLostOnLightningStrike += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Mana Strike Skill Tree Attack Speed And Lightning Reduced Mana Cost")
            {
                increasedAttackSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.04f;
                reducedManaLostOnLightningStrike += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Mana Strike Skill Tree Increased Area")
            {
                increasedArea += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
            }
            if (node.name == "Mana Strike Skill Tree Mana On Hit While Out Of Mana")
            {
                manaGainOnHitWhileOutOfMana += node.GetComponent<SkillTreeNode>().pointsAllocated * 5f;
            }
        }
        mutator.increasedAttackSpeed = increasedAttackSpeed;
        mutator.additionalManaOnHit = additionalManaOnHit;
        mutator.manaOnKill = manaOnKill;
        mutator.manaOnCrit = manaOnCrit;
        mutator.increasedDamageOnHit = increasedDamageOnHit;
        mutator.increasedSpellDamageOnHit = increasedSpellDamageOnHit;
        mutator.increasedManaOnHit = increasedManaOnHit;
        mutator.increasedDamage = increasedDamage;
        mutator.addedCritChance = addedCritChance;
        mutator.removesCritMultiplier = removesCritMultiplier;
        mutator.canTeleport = canTeleport;
        mutator.addedCharges = addedCharges;
        mutator.addedChargeRegen = addedChargeRegen;
        mutator.chanceToKnockBack = chanceToKnockBack;
        mutator.tenacityDebuffOnHit = tenacityDebuffOnHit;
        mutator.chanceToAttachSparkCharge = chanceToAttachSparkCharge;
        mutator.increasedDamageWhileOutOfMana = increasedDamageWhileOutOfMana;

        mutator.addedLightningPerMana = addedLightningPerMana;
        mutator.critChancePerMana = critChancePerMana;
        mutator.lightningStrikeOnHit = lightningStrikeOnHit;
        mutator.reducedManaLostOnLightningStrike = reducedManaLostOnLightningStrike;
        mutator.increasedRadius = Mathf.Sqrt(increasedArea + 1) - 1;
        mutator.manaGainOnHitWhileOutOfMana = manaGainOnHitWhileOutOfMana;
    }
}
