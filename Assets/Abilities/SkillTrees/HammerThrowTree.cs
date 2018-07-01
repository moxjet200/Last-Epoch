using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerThrowTree : SkillTree
{

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.hammerThrow);
    }


    public override void updateMutator()
    {
        HammerThrowMutator mutator = PlayerFinder.getPlayer().GetComponent<HammerThrowMutator>();

        int extraProjectiles = 0;
          float armourShredChance = 0f;

          bool noPierce = false;
          float increasedAttackSpeed = 0f;
          float chanceForDoubleDamage = 0f;
          float increasedDamage = 0f;
          float moreDamageAgainstStunned = 0f;

          bool freeWhenOutOfMana = false;
          bool centreOnCaster = false;
          bool spiralMovement = false;

          float healthGainOnHit = 0f;
          float manaGainOnHit = 0f;

          bool aoeVoidDamage = false;
          float increasedAoEBaseDamage = 0f;

          float increasedProjectileSpeed = 0f;
          bool projectileNova = false;

          int chains = 0;
        float increasedManaCost = 0f;
        float manaEfficiency = 0f;

        bool cooldown = false;
        int addedCharges = 0;
        float increasedStunChance = 0f;
        float moreDamage = 0f;
        bool noReturn = false;

        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            if (node.name == "Hammer Throw Tree Attack Speed")
            {
                increasedAttackSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.06f;
            }
            if (node.name == "Hammer Throw Tree Damage vs Proj Speed")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
                    increasedProjectileSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * -0.1f;
                }
            }
            if (node.name == "Hammer Throw Tree Mana Efficiency")
            {
                manaEfficiency += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
            }
            if (node.name == "Hammer Throw Tree Free When Out of Mana")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * -0.2f;
                    freeWhenOutOfMana = true;
                }
            }
            if (node.name == "Hammer Throw Tree Shreds Armour")
            {
                armourShredChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Hammer Throw Tree Void Damage In Aoe")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    increasedAttackSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * -0.2f;
                    aoeVoidDamage = true;
                }
            }
            if (node.name == "Hammer Throw Tree More Aoe Damage")
            {
                increasedAoEBaseDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Hammer Throw Tree Stun Chance")
            {
                increasedStunChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Hammer Throw Tree Stun Chance vs Pierce")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    increasedStunChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.6f;
                    noPierce = true;
                }
            }
            if (node.name == "Hammer Throw Tree Stun Chain")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    increasedStunChance += node.GetComponent<SkillTreeNode>().pointsAllocated * -0.6f;
                    moreDamage += -0.3f;
                    chains += 1;
                    noReturn = true;
                }
            }
            if (node.name == "Hammer Throw Tree Additional Chain")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    increasedAttackSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * -0.1f;
                    increasedDamage = node.GetComponent<SkillTreeNode>().pointsAllocated * -0.1f;
                    chains += node.GetComponent<SkillTreeNode>().pointsAllocated * 1;
                }
            }
            if (node.name == "Hammer Throw Tree Damage To Stunned")
            {
                moreDamageAgainstStunned += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Hammer Throw Tree Health On Hit")
            {
                healthGainOnHit += node.GetComponent<SkillTreeNode>().pointsAllocated * 1f;
            }
            if (node.name == "Hammer Throw Tree Mana On Hit")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    manaGainOnHit += 3f;
                    cooldown = true;
                }
            }
            if (node.name == "Hammer Throw Tree Projectile Speed")
            {
                increasedProjectileSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            }
            if (node.name == "Hammer Throw Tree No Return")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    noReturn = true;
                    chanceForDoubleDamage += 0.3f;
                }
            }
            if (node.name == "Hammer Throw Tree Extra Projectiles")
            {
                extraProjectiles += node.GetComponent<SkillTreeNode>().pointsAllocated * 2;
                increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * -0.2f;
            }
            if (node.name == "Hammer Throw Tree Nova Projectiles")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    extraProjectiles *= 2;
                    projectileNova = true;
                }
            }
            if (node.name == "Hammer Throw Tree Spirals")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    chanceForDoubleDamage -= 0.15f;
                    extraProjectiles = Mathf.CeilToInt(extraProjectiles * 0.5f);
                    spiralMovement = true;
                }
            }
            if (node.name == "Hammer Throw Tree Moves With Caster")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    chanceForDoubleDamage -= 0.15f;
                    centreOnCaster = true;
                }
            }
        }


        if (cooldown)
        {
            mutator.addedCharges = 1 + addedCharges;
            mutator.addedChargeRegen = 0.25f;
        }
        else
        {
            mutator.addedCharges = 0;
            mutator.addedChargeRegen = 0;
        }

        if (spiralMovement && projectileNova)
        {
            extraProjectiles = Mathf.CeilToInt(extraProjectiles * 0.5f);
        }

        mutator.extraProjectiles = extraProjectiles;
        mutator.armourShredChance = armourShredChance;

        mutator.noPierce = noPierce;
        mutator.increasedAttackSpeed = increasedAttackSpeed;
        mutator.chanceForDoubleDamage = chanceForDoubleDamage;
        mutator.increasedDamage = increasedDamage;
        mutator.moreDamageAgainstStunned = moreDamageAgainstStunned;

        mutator.freeWhenOutOfMana = freeWhenOutOfMana;
        mutator.centreOnCaster = centreOnCaster;
        mutator.spiralMovement = spiralMovement;

        mutator.healthGainOnHit = healthGainOnHit;
        mutator.manaGainOnHit = manaGainOnHit;

        mutator.aoeVoidDamage = aoeVoidDamage;
        mutator.increasedAoEBaseDamage = increasedAoEBaseDamage;

        mutator.increasedProjectileSpeed = increasedProjectileSpeed;
        mutator.projectileNova = projectileNova;

        mutator.chains = chains;

        mutator.increasedManaCost = increasedManaCost;
        mutator.addedManaCostDivider = manaEfficiency;
        mutator.increasedStunChance = increasedStunChance;
        mutator.moreDamage = moreDamage;
        mutator.noReturn = noReturn;
    }
}
