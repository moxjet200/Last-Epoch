using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBlastSkillTree : SkillTree
{

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.lightningBlast);
    }

    public override void updateMutator()
    {
        LightningBlastMutator mutator = PlayerFinder.getPlayer().GetComponent<LightningBlastMutator>();


        float increasedManaCost = 0f;
        float increasedDamage = 0f;
        float increasedDamageToFirstEnemyHit = 0f;
        float increasedDamagePerChain = 0f;
        float increasedDamageToLastEnemy = 0f;
        int chains = 0;
        float lightningProtectionOnCast = 0f;
        float elementalProtectionOnCast = 0f;
        float increasedProtectionOnCast = 0f;
        float increasedProtectionDuration = 0f;
        float wardOnCast = 0f;
        float increasedCastSpeed = 0f;
        float lightningStrikeChance = 0f;
        float lightningDamageOnCast = 0f;
        float increasedDamageBuffDuration = 0f;
        float chanceToBlind = 0f;

        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            if (node.name == "Lightning Blast Skill Tree Chain")
            {
                chains += node.GetComponent<SkillTreeNode>().pointsAllocated;
                increasedManaCost += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.5f;
            }
            if (node.name == "Lightning Blast Skill Tree Chain Slow")
            {
                chains += node.GetComponent<SkillTreeNode>().pointsAllocated;
                increasedCastSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * -0.05f;
            }
            // specify "if chains for the maximum number of times" in tooltip
            if (node.name == "Lightning Blast Skill Tree Last Hit")
            {
                increasedDamageToLastEnemy += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.4f;
            }
            if (node.name == "Lightning Blast Skill Tree First Hit")
            {
                increasedDamageToFirstEnemyHit += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.18f;
            }
            if (node.name == "Lightning Blast Skill Tree Damage Per Chain")
            {
                increasedDamagePerChain += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.05f;
            }
            if (node.name == "Lightning Blast Skill Tree Ele Protection")
            {
                elementalProtectionOnCast += node.GetComponent<SkillTreeNode>().pointsAllocated * 10f;
            }
            if (node.name == "Lightning Blast Skill Tree Cast Speed")
            {
                increasedCastSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.06f;
            }
            if (node.name == "Lightning Blast Skill Tree Chance To Blind")
            {
                chanceToBlind += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Lightning Blast Skill Tree Lightning Protection")
            {
                lightningProtectionOnCast += node.GetComponent<SkillTreeNode>().pointsAllocated * 30f;
            }
            if (node.name == "Lightning Blast Skill Tree Increased Protection")
            {
                increasedProtectionOnCast += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Lightning Blast Skill Tree Protection Duration")
            {
                increasedProtectionDuration += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Lightning Blast Skill Tree Ward On Cast")
            {
                wardOnCast += node.GetComponent<SkillTreeNode>().pointsAllocated * 5f;
            }
            if (node.name == "Lightning Blast Skill Tree Lightning Damage On Cast")
            {
                lightningDamageOnCast += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.05f;
            }
            if (node.name == "Lightning Blast Skill Tree Lightning Damage Duration")
            {
                increasedDamageBuffDuration += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Lightning Blast Skill Tree Increased Damage")
            {
                lightningDamageOnCast += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Lightning Blast Skill Tree Lightning Strike Chance")
            {
                lightningStrikeChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.17f;
            }
            if (node.name == "Lightning Blast Skill Tree No Cost")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    increasedManaCost -= 100f;
                    increasedDamage -= 0.25f;
                }
            }
        }

        mutator.increasedManaCost = increasedManaCost;
        mutator.increasedDamage = increasedDamage;
        mutator.increasedDamageToFirstEnemyHit = increasedDamageToFirstEnemyHit;
        mutator.increasedDamagePerChain = increasedDamagePerChain;
        mutator.increasedDamageToLastEnemy = increasedDamageToLastEnemy;
        mutator.chains = chains;
        mutator.lightningProtectionOnCast = lightningProtectionOnCast;
        mutator.elementalProtectionOnCast = elementalProtectionOnCast;
        mutator.increasedProtectionOnCast = increasedProtectionOnCast;
        mutator.increasedProtectionDuration = increasedProtectionDuration;
        mutator.wardOnCast = wardOnCast;
        mutator.increasedCastSpeed = increasedCastSpeed;
        mutator.lightningStrikeChance = lightningStrikeChance;
        mutator.lightningDamageOnCast = lightningDamageOnCast;
        mutator.increasedDamageBuffDuration = increasedDamageBuffDuration;
        mutator.chanceToBlind = chanceToBlind;
    }
}
