using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerpentStrikeTree : SkillTree
{

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.serpentStrike);
    }

    public override void updateMutator()
    {
        SerpentStrikeMutator mutator = PlayerFinder.getPlayer().GetComponent<SerpentStrikeMutator>();
        float snakeOnKillChance = 0f;
         float increasedAttackSpeed = 0f;
         float chanceToBleed = 0f;
         float chanceToPlague = 0f;
         float chanceToBlindingPoison = 0f;
         float poisonSpitChance = 0f;
         float increasedHitDamage = 0f;
         float addedCritChance = 0f;
         float lifeOnCrit = 0f;
         float lifeOnKill = 0f;
         float cullPercent = 0f;
         float chanceToShredArmour = 0f;
         float moreDamageAgainstBlinded = 0f;
         float moreDamageAgainstPoisonBlinded = 0f;
         float poisonDamageOnAttack = 0f;
         float dotDamageOnAttack = 0f;
         float dodgeRatingOnAttack = 0f;
        
        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            if (node.name == "Serpent Strike Tree Snek")
            {
                snakeOnKillChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.07f;
            }
            if (node.name == "Serpent Strike Tree Cull")
            {
                cullPercent += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.08f;
            }
            if (node.name == "Serpent Strike Tree life Gain On Kill")
            {
                lifeOnKill += node.GetComponent<SkillTreeNode>().pointsAllocated * 10f;
            }
            if (node.name == "Serpent Strike Tree Plague Chance")
            {
                chanceToPlague += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            }
            if (node.name == "Serpent Strike Tree Poison Damage On Attack")
            {
                poisonDamageOnAttack += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.12f;
            }
            if (node.name == "Serpent Strike Tree Attack Speed")
            {
                increasedAttackSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.07f;
            }
            if (node.name == "Serpent Strike Tree Poison Projectile")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    poisonSpitChance = 1;
                    increasedAttackSpeed -= 0.2f;
                }
            }
            if (node.name == "Serpent Strike Tree DoT Damage On Attack")
            {
                dotDamageOnAttack += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Serpent Strike Tree Removes Projectile")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    poisonSpitChance = 0;
                    increasedHitDamage += 0.5f;
                }
            }
            if (node.name == "Serpent Strike Tree Blinding Poison Chance")
            {
                chanceToBlindingPoison += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.17f;
            }
            if (node.name == "Serpent Strike Tree Damage Against Blinded")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    moreDamageAgainstBlinded += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
                    moreDamageAgainstPoisonBlinded += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
                }
            }
            if (node.name == "Serpent Strike Tree Dodge Rating On Attack")
            {
                dodgeRatingOnAttack += node.GetComponent<SkillTreeNode>().pointsAllocated * 10f;
            }
            if (node.name == "Serpent Strike Tree Attack Speed And Bleed Chance")
            {
                increasedAttackSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.05f;
                chanceToBleed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.05f;
            }
            if (node.name == "Serpent Strike Tree Bleed Chance")
            {
                chanceToBleed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Serpent Strike Tree Armour Debuff")
            {
                chanceToShredArmour += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Serpent Strike Tree Crit Chance")
            {
                addedCritChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.03f;
            }
            if (node.name == "Serpent Strike Tree Life On Crit")
            {
                lifeOnCrit += node.GetComponent<SkillTreeNode>().pointsAllocated * 5f;
            }

        }
        mutator.snakeOnKillChance = snakeOnKillChance;
        mutator.increasedAttackSpeed = increasedAttackSpeed;
        mutator.chanceToBleed = chanceToBleed;
        mutator.chanceToPlague = chanceToPlague;
        mutator.chanceToBlindingPoison = chanceToBlindingPoison;
        mutator.poisonSpitChance = poisonSpitChance;
        mutator.increasedHitDamage = increasedHitDamage;
        mutator.addedCritChance = addedCritChance;
        mutator.lifeOnCrit = lifeOnCrit;
        mutator.lifeOnKill = lifeOnKill;
        mutator.cullPercent = cullPercent;
        mutator.chanceToShredArmour = chanceToShredArmour;
        mutator.moreDamageAgainstBlinded = moreDamageAgainstBlinded;

        mutator.moreDamageAgainstPoisonBlinded = moreDamageAgainstPoisonBlinded;
        mutator.poisonDamageOnAttack = poisonDamageOnAttack;
        mutator.dotDamageOnAttack = dotDamageOnAttack;
        mutator.dodgeRatingOnAttack = dodgeRatingOnAttack;
    }
}