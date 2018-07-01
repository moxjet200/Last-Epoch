using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarrowShardsTree : SkillTree
{

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.marrowShards);
    }

    public override void updateMutator()
    {
        MarrowShardsMutator mutator = PlayerFinder.getPlayer().GetComponent<MarrowShardsMutator>();

        float nova_increasedSpeed = 0f;
        float nova_increasedDamage = 0f;
        float nova_increasedStunChance = 0f;
        float nova_bleedChance = 0f;
        float nova_addedCritChance = 0f;
        float nova_addedCritMultiplier = 0f;
        float nova_moreDamageAgainstBleeding = 0f;

        float doubleCastChance = 0f;
        float physLeechOnCast = 0f;
        float increasedDuration = 0f;

        float addedCritChance = 0f;
        float addedCritMultiplier = 0f;
        float percentCurrentHealthLostOnCrit = 0f;
        float critChanceOnCast = 0f;

        bool createsSplintersAtEnd = false;
        bool endsAtTargetPoint = false;
        float chanceToShredArmour = 0f;
        float chanceToBleed = 0f;

        bool damagesMinions = false;
        bool doesntPierce = false;

        float healthGainedOnMinionKill = 0f;
        float physLeechOnKill = 0f;
        float increasedHealthCost = 0f;

        float moreDamageAgainstBleeding = 0f;
        float increasedDamage = 0f;
        float increasedStunChance = 0f;
        float increasedDamagePerMinion = 0f;
        float increasedDamageFromMinionDrain = 0f;

        float returnHealthChance = 0f;
        float healthReturned = 0f;
        float manaReturnChance = 0f;
        float manaReturned = 0f;

        float increasedCastSpeed = 0f;
        float increasedDamageWithOneMinion = 0f;
        float moreDamage = 0f;
        float critChanceOnKill = 0f;

        float manaEfficiency = 0f;
        float increasedManaCost = 0f;

        SkillTreeNode skillTreeNode;
        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {

            skillTreeNode = node.GetComponent<SkillTreeNode>();
            if (node.name == "Marrow Shards Tree No Pierce")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    doesntPierce = true;
                    moreDamage += 1f;
                }
            }
            if (node.name == "Marrow Shards Tree Physical Leech On Cast")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    physLeechOnCast += 0.06f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Marrow Shards Tree Splinters")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    createsSplintersAtEnd = true;
                    increasedHealthCost += 0.5f;
                    doesntPierce = true;
                }
            }
            if (node.name == "Marrow Shards Tree Splinter Damage")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    nova_increasedDamage += 0.18f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Marrow Shards Tree Splinter Speed")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    nova_increasedSpeed += 0.25f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Marrow Shards Tree Splinter Bleed Chance")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    nova_bleedChance += 0.25f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Marrow Shards Tree Splinter Damage Against Bleeding")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    nova_moreDamageAgainstBleeding += 0.25f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Marrow Shards Tree Splinter Crit Chance")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    nova_addedCritChance += 0.02f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Marrow Shards Tree Splinter Crit Multiplier")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    nova_addedCritMultiplier += 0.35f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Marrow Shards Tree Damage Vs Range")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    increasedDamage += 0.15f * skillTreeNode.pointsAllocated;
                    increasedDuration += -0.13f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Marrow Shards Tree Crit Chance And Multiplier")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    nova_addedCritChance += 0.01f * skillTreeNode.pointsAllocated;
                    nova_addedCritMultiplier += 0.15f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Marrow Shards Tree Crit Multiplier")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    nova_addedCritMultiplier += 0.2f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Marrow Shards Tree Health Lost On Crit")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    nova_addedCritMultiplier += 0.3f * skillTreeNode.pointsAllocated;
                    percentCurrentHealthLostOnCrit += 0.01f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Marrow Shards Tree Crit Chance Buff With Two Minions")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    critChanceOnCast += 0.4f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Marrow Shards Tree Splinters At End")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    createsSplintersAtEnd = true;
                    increasedHealthCost += 0.4f;
                    increasedDuration += -0.2f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Marrow Shards Tree Ends At Target")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    endsAtTargetPoint = true;
                    increasedHealthCost += 0.1f;
                }
            }
            if (node.name == "Marrow Shards Tree Double Cast Chance")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    doubleCastChance += 0.14f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Marrow Shards Tree Armour Shred Chance")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    chanceToShredArmour += 0.14f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Marrow Shards Tree Damages Minions")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    damagesMinions = true;
                    increasedDamage += 0.25f;
                }
            }
            if (node.name == "Marrow Shards Tree Health On Minion Kill")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    healthGainedOnMinionKill += 10f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Marrow Shards Tree Crit Chance On Kill")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    critChanceOnKill += 0.3f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Marrow Shards Tree Leech On Kill")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    physLeechOnKill += 0.08f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Marrow Shards Tree Bleed Chance And Health Cost")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    chanceToBleed += 0.25f * skillTreeNode.pointsAllocated;
                    increasedHealthCost += 0.1f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Marrow Shards Tree Consumes Minion")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    increasedDamageFromMinionDrain += 0.75f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Marrow Shards Tree Chance To Return Health")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    returnHealthChance += 0.2f;
                    healthReturned += 15f;
                }
            }
            if (node.name == "Marrow Shards Tree Health Returned")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    healthReturned += 7f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Marrow Shards Tree Additional Chance For Health Return")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    returnHealthChance += 0.1f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Marrow Shards Tree Mana Returned")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    manaReturnChance += 0.33f;
                    manaReturned += 15f;
                }
            }
            if (node.name == "Marrow Shards Tree Damage Against Bleeding")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    moreDamageAgainstBleeding += 0.2f * skillTreeNode.pointsAllocated;
                }
            }
        }

        mutator.nova_increasedSpeed = nova_increasedSpeed;
        mutator.nova_increasedDamage = nova_increasedDamage;
        mutator.nova_increasedStunChance = nova_increasedStunChance;
        mutator.nova_bleedChance = nova_bleedChance;
        mutator.nova_addedCritChance = nova_addedCritChance;
        mutator.nova_addedCritMultiplier = nova_addedCritMultiplier;
        mutator.nova_moreDamageAgainstBleeding = nova_moreDamageAgainstBleeding;

        mutator.doubleCastChance = doubleCastChance;
        mutator.physLeechOnCast = physLeechOnCast;
        mutator.increasedDuration = increasedDuration;

        mutator.addedCritChance = addedCritChance;
        mutator.addedCritMultiplier = addedCritMultiplier;
        mutator.percentCurrentHealthLostOnCrit = percentCurrentHealthLostOnCrit;
        mutator.critChanceOnCast = critChanceOnCast;

        mutator.createsSplintersAtEnd = createsSplintersAtEnd;
        mutator.endsAtTargetPoint = endsAtTargetPoint;
        mutator.chanceToShredArmour = chanceToShredArmour;
        mutator.chanceToBleed = chanceToBleed;

        mutator.damagesMinions = damagesMinions;
        mutator.doesntPierce = doesntPierce;

        mutator.healthGainedOnMinionKill = healthGainedOnMinionKill;
        mutator.physLeechOnKill = physLeechOnKill;
        mutator.increasedHealthCost = increasedHealthCost;

        mutator.moreDamageAgainstBleeding = moreDamageAgainstBleeding;
        mutator.increasedDamage = increasedDamage;
        mutator.increasedStunChance = increasedStunChance;
        mutator.increasedDamagePerMinion = increasedDamagePerMinion;
        mutator.increasedDamageFromMinionDrain = increasedDamageFromMinionDrain;

        mutator.returnHealthChance = returnHealthChance;
        mutator.healthReturned = healthReturned;
        mutator.manaReturnChance = manaReturnChance;
        mutator.manaReturned = manaReturned;

        mutator.increasedCastSpeed = increasedCastSpeed;
        mutator.increasedDamageWithOneMinion = increasedDamageWithOneMinion;
        mutator.moreDamage = moreDamage;
        mutator.critChanceOnKill = critChanceOnKill;

        mutator.addedManaCostDivider = manaEfficiency;
        mutator.increasedManaCost = increasedManaCost;
    }
}
