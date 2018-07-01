using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballSkillTree : SkillTree {

	public override void setAbility() {
        ability = AbilityIDList.getAbility(AbilityID.fireball);
	}

    public override void updateMutator()
    {
        FireballMutator mutator = PlayerFinder.getPlayer().GetComponent<FireballMutator>();
        bool explosion = false;
        int extraProjectiles = 0;
        int targetsToPierce = 0;
        float chanceToCreateExplosionOnHit = 0f;
        float igniteChance = 0f;
        bool reduceBaseDamageBy80Percent = false;
        bool fireInSequence = false;
        float increasedCastSpeed = 0f;
        float increasedManaCost = 0f;
        float chanceForDoubleDamage = 0f;
        float fireAddedAsLightning = 0f;
        float moreDamageAgainstIgnited = 0f;
        float moreDamageAgainstChilled = 0f;
        bool homing = false;
        float increasedDamage = 0f;
        bool freeWhenOutOfMana = false;
        float manaEfficiency = 0f;
        bool alwaysFree = false;
        float increasedSpeed = 0f;
        float increasedDuration = 0f;
        bool inaccuracy = false;
        bool channelled = false;

        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            if (node.name == "Fireball Skill Tree Explosion")
            {
                explosion = (node.GetComponent<SkillTreeNode>().pointsAllocated > 0);
            }
            if (node.name == "Fireball Skill Tree Extra Projectiles")
            {
                extraProjectiles += node.GetComponent<SkillTreeNode>().pointsAllocated * 2;
                increasedCastSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * -0.15f;
            }
            if (node.name == "Fireball Skill Tree Pierce")
            {
                targetsToPierce += node.GetComponent<SkillTreeNode>().pointsAllocated;
            }
            if (node.name == "Fireball Skill Tree Damage Around Hit")
            {
                chanceToCreateExplosionOnHit = node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Fireball Skill Tree Ignite Chance")
            {
                igniteChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Fireball Skill Tree Ignite Keystone")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    reduceBaseDamageBy80Percent = true;
                    igniteChance += 1;
                }
            }
            // this is halved so it ends up as one
            if (node.name == "Fireball Skill Tree One Extra Projectile")
            {
                extraProjectiles += node.GetComponent<SkillTreeNode>().pointsAllocated * 2;
                increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * -0.07f;
                increasedManaCost += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.4f;
            }
            if (node.name == "Fireball Skill Tree Cast Speed")
            {
                increasedCastSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.06f;
            }
            if (node.name == "Fireball Skill Tree Cast Speed And Mana Cost")
            {
                increasedCastSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.12f;
                increasedManaCost += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Fireball Skill Tree Extra Projectiles And Mana Cost")
            {
                extraProjectiles += node.GetComponent<SkillTreeNode>().pointsAllocated * 2;
                increasedManaCost += node.GetComponent<SkillTreeNode>().pointsAllocated * 1f;
            }
            if (node.name == "Fireball Skill Tree Chance For Double Damage")
            {
                chanceForDoubleDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Fireball Skill Tree Fire Added As Lightning")
            {
                fireAddedAsLightning += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
                increasedCastSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * -0.04f;
            }
            if (node.name == "Fireball Skill Tree Damage Against Ignited")
            {
                moreDamageAgainstIgnited += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Fireball Skill Tree Damage Against Chilled")
            {
                moreDamageAgainstChilled += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Fireball Skill Tree Mana Efficiency")
            {
                manaEfficiency += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.28f;
            }
            if (node.name == "Fireball Skill Tree Homing")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    homing = true;
                }
            }
            if (node.name == "Fireball Skill Tree Damage Vs Speed")
            {
                increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.12f;
                increasedSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * -0.06f;
            }
            if (node.name == "Fireball Skill Tree Duration")
            {
                increasedDuration += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.06f;
            }
            if (node.name == "Fireball Skill Always Pierce")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    targetsToPierce += 100;
                    increasedDamage -= 0.2f;
                }
            }
        }

        // modifies number of projectiles granted from other nodes
        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            if (node.name == "Fireball Skill Tree Sequence")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    fireInSequence = true;
                    increasedCastSpeed -= 0.25f;
                    extraProjectiles = Mathf.CeilToInt((((float)extraProjectiles) / 2f));
                }
            }
        }

        // further modifies number of projectiles and ignite change
        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            if (node.name == "Fireball Skill Tree No Cost")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    freeWhenOutOfMana = true;
                    increasedDamage += 0.1f * extraProjectiles;
                    extraProjectiles = 0;
                }
            }
            if (node.name == "Fireball Skill Tree Zero Cost")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    alwaysFree = true;
                    increasedDamage -= 0.2f;
                    igniteChance *= 0.6f;
                }
            }
            // requires knowledge of increased cast speed
            if (node.name == "Fireball Skill Tree FlameThrower")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    channelled = true;
                    increasedDuration -= 0.7f;
                    increasedDamage += increasedCastSpeed;
                    increasedDamage -= 0.5f;
                }
            }
        }

        if (homing == true && fireInSequence != true) {
			extraProjectiles = 0;
		}

        mutator.addedManaCostDivider = manaEfficiency;
        mutator.addExplosion = explosion;
        mutator.extraProjectiles = extraProjectiles;
        mutator.targetsToPierce = targetsToPierce;
        mutator.chanceToCreateExplosionOnHit = chanceToCreateExplosionOnHit;
        mutator.igniteChance = igniteChance;
        mutator.reduceBaseDamageBy80Percent = reduceBaseDamageBy80Percent;
        mutator.fireInSequence = fireInSequence;
        mutator.increasedCastSpeed = increasedCastSpeed;
        mutator.increasedManaCost = increasedManaCost;
        mutator.chanceForDoubleDamage = chanceForDoubleDamage;
        mutator.fireAddedAsLightning = fireAddedAsLightning;
        mutator.moreDamageAgainstIgnited = moreDamageAgainstIgnited;
        mutator.moreDamageAgainstChilled = moreDamageAgainstChilled;
        mutator.homing = homing;
        mutator.increasedDamage = increasedDamage;
        mutator.freeWhenOutOfMana = freeWhenOutOfMana;
        mutator.alwaysFree = alwaysFree;
        mutator.increasedSpeed = increasedSpeed;
        mutator.increasedDuration = increasedDuration;
        mutator.inaccuracy = inaccuracy;
        mutator.channelled = channelled;
    }
}
