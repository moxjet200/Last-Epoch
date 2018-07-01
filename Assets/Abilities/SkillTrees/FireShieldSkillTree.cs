using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireShieldSkillTree : SkillTree {

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.fireShield);
    }

    public override void updateMutator()
    {
        FireShieldMutator mutator = PlayerFinder.getPlayer().GetComponent<FireShieldMutator>();
        float additionalDuration = 0f;
        float additionalWardRegen = 0f;
        float additionalElementalProtection = 0f;
        bool canCastOnAllies = false;
        float damageThreshold = 30f;
        float aoeDamage = 0f;
        float aoeRadius = 3f;
        bool ignitesInAoe = false;
        float increasedIgniteFrequency = 0f;
        float increasedFireballDamage = 0f;
        float fireballIgniteChance = 0f;
        bool fireballPierces = false;
        bool grantsColdDamage = false;
        bool grantsLightningDamage = false;
        float igniteChanceGranted = 0f;
        float increasedManaCost = 0f;
        foreach(SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            if (node.name == "Fire Shield Skill Tree Additional Duration")
            {
                additionalDuration += node.GetComponent<SkillTreeNode>().pointsAllocated * 3;
            }
            if (node.name == "Fire Shield Skill Tree Additional Ward Regen")
            {
                additionalWardRegen += node.GetComponent<SkillTreeNode>().pointsAllocated * 1;
            }
            if (node.name == "Fire Shield Skill Tree Additional Elemental Protection")
            {
                additionalElementalProtection += node.GetComponent<SkillTreeNode>().pointsAllocated * 25;
            }
            if (node.name == "Fire Shield Skill Tree Can Cast On Allies")
            {
                canCastOnAllies = (node.GetComponent<SkillTreeNode>().pointsAllocated > 0);
            }
            if (node.name == "Fire Shield Skill Tree Reduced Damage Threshold")
            {
                damageThreshold -= node.GetComponent<SkillTreeNode>().pointsAllocated * 4;
            }
            if (node.name == "Fire Shield Skill Tree Aoe Damage")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0) {
                    aoeDamage += 2;
                    increasedManaCost = 0.4f;
                }
            }
            if (node.name == "Fire Shield Skill Tree Extra Aoe Damage")
            {
                aoeDamage += 1 * node.GetComponent<SkillTreeNode>().pointsAllocated;
            }
            if (node.name == "Fire Shield Skill Tree Aoe Radius")
            {
                aoeRadius += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.45f;
            }
            if (node.name == "Fire Shield Skill Tree Aoe Ignite")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    ignitesInAoe = true;
                    additionalDuration -= 4f;
                }
            }
            if (node.name == "Fire Shield Skill Tree Ignite Frequency")
            {
                increasedIgniteFrequency += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            }
            if (node.name == "Fire Shield Skill Tree Fireball Damage")
            {
                increasedFireballDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Fire Shield Skill Tree Fireball Ignite Chance")
            {
                fireballIgniteChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Fire Shield Skill Tree Fireball Pierce")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    fireballPierces = true;
                }
            }
            if (node.name == "Fire Shield Skill Tree Cold Damage")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    grantsColdDamage = true;
                    additionalDuration -= 4f;
                }
            }
            if (node.name == "Fire Shield Skill Tree Lightning Damage")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    grantsLightningDamage = true;
                    additionalDuration -= 4f;
                }
            }
            if (node.name == "Fire Shield Skill Tree Ignite Chance Granted")
            {
                igniteChanceGranted += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
        }

        mutator.additionalDuration = additionalDuration;
        mutator.additionalWardRegen = additionalWardRegen;
        mutator.additionalElementalProtection = additionalElementalProtection;
        mutator.canCastOnAllies = canCastOnAllies;
        mutator.damageThreshold = damageThreshold;
        mutator.aoeDamage = aoeDamage;
        mutator.aoeRadius = aoeRadius;

        mutator.ignitesInAoe = ignitesInAoe;
        mutator.increasedIgniteFrequency = increasedIgniteFrequency;
        mutator.increasedFireballDamage = increasedFireballDamage;
        mutator.fireballIgniteChance = fireballIgniteChance;
        mutator.fireballPierces = fireballPierces;
        mutator.grantsColdDamage = grantsColdDamage;
        mutator.grantsLightningDamage = grantsLightningDamage;
        mutator.igniteChanceGranted = igniteChanceGranted;

        mutator.increasedManaCost = increasedManaCost;
    }
}
