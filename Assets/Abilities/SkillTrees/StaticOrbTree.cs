using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticOrbTree : SkillTree
{

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.staticOrb);
    }

    public override void updateMutator()
    {
        StaticOrbMutator mutator = PlayerFinder.getPlayer().GetComponent<StaticOrbMutator>();
        float doubleCastChance = 0f;
        float increasedExplosionDamage = 0f;
        float explosionChanceToShock = 0f;
        bool explodesAtTarget = false;
        float explosionWardGainedOnKill = 0f;
        float explosionWardGainedOnKillChance = 0f;
        float manaOnHitChance = 0f;
        float manaOnHit = 0f;
        float chanceToAttachSparkCharge = 0f;
        float increasedProjectileDamage = 0f;
        bool removeExplosion = false;
        bool freeWhenOutOfMana = false;
        bool removePull = false;
        float increasedSpeed = 0f;
        float lightningAegisChance = 0f;
        float knockBackOnCastChance = 0f;
        float chargedGroundAtEndChance = 0f;
        float manaEfficiency = 0f;
        float increasedManaCost = 0f;
        float shockChance = 0f;

        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            if (node.name == "Static Orb Tree Explodes At Target")
            {
                explodesAtTarget = (node.GetComponent<SkillTreeNode>().pointsAllocated > 0);
            }
            if (node.name == "Static Orb Tree Explosion Damage")
            {
                increasedExplosionDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Static Orb Tree Explosion Shock")
            {
                explosionChanceToShock += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.34f;
            }
            if (node.name == "Static Orb Tree Explosion Ward")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    explosionWardGainedOnKill += 10;
                    explosionWardGainedOnKillChance += 0.25f;
                }
            }
            if (node.name == "Static Orb Tree Explosion Extra Ward")
            {
                explosionWardGainedOnKill += node.GetComponent<SkillTreeNode>().pointsAllocated * 5f;
            }
            if (node.name == "Static Orb Tree Charged Ground")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    chargedGroundAtEndChance += 1f;
                    increasedManaCost += 3f;
                }
            }
            if (node.name == "Static Orb Tree Mana Efficiency")
            {
                manaEfficiency += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.35f;
            }

            if (node.name == "Static Orb Tree Projectile Shock")
            {
                shockChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Static Orb Tree Mana On Pull Chance")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    manaOnHit += 14f;
                    manaOnHitChance += 0.1f;
                }
            }
            if (node.name == "Static Orb Tree Extra Mana On Pull")
            {
                manaOnHit += node.GetComponent<SkillTreeNode>().pointsAllocated * 6f;
            }
            if (node.name == "Static Orb Tree Spark Charge")
            {
                chanceToAttachSparkCharge += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Static Orb Tree Projectile Damage")
            {
                increasedProjectileDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Static Orb Tree Removes Explosion")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    removeExplosion = true;
                    freeWhenOutOfMana = true;
                }
            }
            if (node.name == "Static Orb Tree Mana Good Efficiency")
            {
                manaEfficiency += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.5f;
            }
            if (node.name == "Static Orb Tree Removes Pull")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    removePull = true;
                    increasedProjectileDamage += 1f;
                }
            }
            if (node.name == "Static Orb Tree Double Cast")
            {
                doubleCastChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.14f;
            }
            if (node.name == "Static Orb Tree Speed")
            {
                increasedSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.4f;
            }
            if (node.name == "Static Orb Tree Lightning Aegis")
            {
                lightningAegisChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Static Orb Tree Knock Back")
            {
                knockBackOnCastChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }

        }
        mutator.doubleCastChance = doubleCastChance;
        mutator.increasedExplosionDamage = increasedExplosionDamage;
        mutator.explosionChanceToShock = explosionChanceToShock;
        mutator.explodesAtTarget = explodesAtTarget;
        mutator.explosionWardGainedOnKill = explosionWardGainedOnKill;
        mutator.explosionWardGainedOnKillChance = explosionWardGainedOnKillChance;
        mutator.manaOnHitChance = manaOnHitChance;
        mutator.manaOnHit = manaOnHit;
        mutator.chanceToAttachSparkCharge = chanceToAttachSparkCharge;
        mutator.increasedProjectileDamage = increasedProjectileDamage;
        mutator.removeExplosion = removeExplosion;
        mutator.freeWhenOutOfMana = freeWhenOutOfMana;
        mutator.removePull = removePull;
        mutator.increasedSpeed = increasedSpeed;
        mutator.lightningAegisChance = lightningAegisChance;
        mutator.knockBackOnCastChance = knockBackOnCastChance;
        mutator.chargedGroundAtEndChance = chargedGroundAtEndChance;
        mutator.addedManaCostDivider = manaEfficiency;
        mutator.increasedManaCost = increasedManaCost;
        mutator.shockChance = shockChance;

    }
}
