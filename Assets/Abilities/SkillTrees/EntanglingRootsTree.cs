using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntanglingRootsTree : SkillTree
{

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.entanglingRoots);
    }

    public override void updateMutator()
    {
        EntanglingRootsMutator mutator = PlayerFinder.getPlayer().GetComponent<EntanglingRootsMutator>();
        float increasedDamage = 0f;
        float addedPoisonDamage = 0f;
        float increasedArea = 0f;

        float initialHitIncreasedDamage = 0f;
        float initialHitChanceToPoison = 0f;

        int addedPatches = 0;
        bool patchesInLine = false;

        float vineOnKillChance = 0f;

        float increasedBuffDuration = 0f;
        float damageBuff = 0f;
        float poisonChanceToWolves = 0f;
        float bleedchanceToBears = 0f;
        float castSpeedToSpriggans = 0f;
        bool healSpriggans = false;

        bool meleeScalingInitialHit = false;
        bool InitialHitAlwaysStuns = false;
        float increasedDuration = 0f;
        float increasedManaCost = 0f;
        float manaEfficiency = 0f;
        float healingNovaChance = 0f;

        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            if (node.name == "Entangling Roots Tree Vine On Kill")
            {
                vineOnKillChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Entangling Roots Tree Repeat Damage")
            {
                increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Entangling Roots Tree Increased Duration")
            {
                increasedDuration += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
                increasedBuffDuration += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            }
            if (node.name == "Entangling Roots Tree Repeat Poison Chance")
            {
                addedPoisonDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.24f;
            }
            if (node.name == "Entangling Roots Tree Increased Area")
            {
                increasedArea += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
            }
            if (node.name == "Entangling Roots Tree Patches")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    addedPatches += 2;
                    increasedArea += -0.6f;
                    increasedManaCost += 0.2f;
                }
            }
            if (node.name == "Entangling Roots Tree Additional Patches")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    addedPatches += 1;
                    increasedDuration += -0.3f;
                    increasedDamage += -0.1f;
                }
            }
            if (node.name == "Entangling Roots Tree Line")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    patchesInLine = true;
                }
            }
            if (node.name == "Entangling Roots Tree Damage Buff")
            {
                damageBuff += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Entangling Roots Tree Buff Duration")
            {
                increasedBuffDuration += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Entangling Roots Tree Wolf Poison")
            {
                poisonChanceToWolves += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.34f;
            }
            if (node.name == "Entangling Roots Tree Bear Bleed")
            {
                bleedchanceToBears += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.34f;
            }
            if (node.name == "Entangling Roots Tree Spriggan Cast Speed")
            {
                castSpeedToSpriggans += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Entangling Roots Tree Heal Spriggans")
            {
                healSpriggans = (node.GetComponent<SkillTreeNode>().pointsAllocated > 0);
            }
            if (node.name == "Entangling Roots Tree Healing Nova")
            {
                healingNovaChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.34f;
            }
            if (node.name == "Entangling Roots Tree Mana Duration")
            {
                manaEfficiency += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
                increasedDuration += node.GetComponent<SkillTreeNode>().pointsAllocated * -0.15f;
            }
            if (node.name == "Entangling Roots Tree Melee Initial Hit")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    meleeScalingInitialHit = true;
                    increasedManaCost += 0.3f;
                }
            }
            if (node.name == "Entangling Roots Tree Initial Hit Damage")
            {
                initialHitIncreasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Entangling Roots Tree Initial Hit Long Stun")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    InitialHitAlwaysStuns = true;
                    increasedManaCost += 0.1f;
                }
            }
        }
        mutator.increasedDamage = increasedDamage;
        mutator.addedPoisonDamage = addedPoisonDamage;
        mutator.increasedRadius = Mathf.Sqrt(increasedArea + 1) - 1;

        mutator.initialHitIncreasedDamage = initialHitIncreasedDamage;
        mutator.initialHitChanceToPoison = initialHitChanceToPoison;

        mutator.addedPatches = addedPatches;
        mutator.patchesInLine = patchesInLine;

        mutator.vineOnKillChance = vineOnKillChance;

        mutator.increasedBuffDuration = increasedBuffDuration;
        mutator.damageBuff = damageBuff;
        mutator.poisonChanceToWolves = poisonChanceToWolves;
        mutator.bleedchanceToBears = bleedchanceToBears;
        mutator.castSpeedToSpriggans = castSpeedToSpriggans;
        mutator.healSpriggans = healSpriggans;

        mutator.meleeScalingInitialHit = meleeScalingInitialHit;
        mutator.InitialHitAlwaysStuns = InitialHitAlwaysStuns;
        mutator.increasedDuration = increasedDuration;
        mutator.increasedManaCost = increasedManaCost;
        mutator.addedManaCostDivider = manaEfficiency;
        mutator.healingNovaChance = healingNovaChance;
    }
}
