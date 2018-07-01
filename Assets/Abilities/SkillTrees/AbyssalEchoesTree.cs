using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbyssalEchoesTree : SkillTree
{

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.abyssalEchoes);
    }

    public override void updateMutator()
    {
        AbyssalEchoesMutator mutator = PlayerFinder.getPlayer().GetComponent<AbyssalEchoesMutator>();
         bool noChain = false;
         bool pullsOnHit = false;
         float increasedPullStrength = 0f;
         float increasedArea = 0f;
         float speedDebuff = 0f;
         float armourDebuff = 0f;
         float increasedDebuffStrength = 0f;
         float increasedDebuffDuration = 0f;
         float dotTakenDebuff = 0f;
         bool dotTakenDebuffStacks = false;

         int additionalCasts = 0;
         float increasedDelay = 0f;

         List<TaggedStatsHolder.Stat> stackingStatsOnKill = new List<TaggedStatsHolder.Stat>();

         float healthRegenOnKill = 0f;
         float manaRegenOnKill = 0f;

         float increasedNonStackingBuffDuration = 0f;
         float movementSpeedOnKill = 0f;
         float attackAndCastSpeedOnKill = 0f;

         float increasedCastSpeed = 0f;

         float increasedDamage = 0f;
        float manaEfficiency = 0f;
        float increasedManaCost = 0f;
        bool stacks = false;

        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            if (node.name == "Abyssal Echoes Tree No Chain")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    noChain = true;
                    manaEfficiency += 0.4f;
                    increasedArea += 0.4f;
                }
            }
            if (node.name == "Abyssal Echoes Tree Pulls On Hit")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    pullsOnHit = true;
                    increasedManaCost += 0.35f;
                }
            }
            if (node.name == "Abyssal Echoes Tree Pull Strength")
            {
                increasedPullStrength += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Abyssal Echoes Tree Increased Area")
            {
                increasedArea += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.4f;
            }
            if (node.name == "Abyssal Echoes Tree Speed Debuff")
            {
                speedDebuff += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.05f;
            }
            if (node.name == "Abyssal Echoes Tree Armour Debuff")
            {
                armourDebuff += node.GetComponent<SkillTreeNode>().pointsAllocated * 20f;
            }
            if (node.name == "Abyssal Echoes Tree Debuff Duration")
            {
                increasedDebuffDuration += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
            }
            if (node.name == "Abyssal Echoes Tree Debuff Strength")
            {
                increasedDebuffStrength += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
            }
            if (node.name == "Abyssal Echoes Tree Mana Efficiency And Cast Speed")
            {
                increasedCastSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.05f;
                manaEfficiency += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Abyssal Echoes Tree Hit Damage")
            {
                increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
            }
            if (node.name == "Abyssal Echoes Tree Stacks")
            {
                stacks = (node.GetComponent<SkillTreeNode>().pointsAllocated > 0);
            }
            if (node.name == "Abyssal Echoes Tree Casts Again")
            {
                additionalCasts += node.GetComponent<SkillTreeNode>().pointsAllocated * 1;
                increasedManaCost += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.35f;
            }
            if (node.name == "Abyssal Echoes Tree Increased Delay")
            {
                increasedDelay += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
                manaEfficiency += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Abyssal Echoes Tree Additional Casts")
            {
                additionalCasts += node.GetComponent<SkillTreeNode>().pointsAllocated * 1;
                increasedManaCost += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.35f;
            }
            if (node.name == "Abyssal Echoes Tree DoT Taken Debuff")
            {
                dotTakenDebuff += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Abyssal Echoes Tree DoT Taken Debuff Stacks")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    dotTakenDebuffStacks = true;
                    dotTakenDebuff *= 0.5f;
                }
            }
            if (node.name == "Abyssal Echoes Tree Health Regen On Kill")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    TaggedStatsHolder.Stat stat = new TaggedStatsHolder.Stat(Tags.Properties.HealthRegen);
                    stat.increasedValue = node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
                    stackingStatsOnKill.Add(stat);
                }
            }
            if (node.name == "Abyssal Echoes Tree Tenacity On Kill")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    TaggedStatsHolder.Stat stat = new TaggedStatsHolder.Stat(Tags.Properties.Tenacity);
                    stat.addedValue = node.GetComponent<SkillTreeNode>().pointsAllocated * 15f;
                    stackingStatsOnKill.Add(stat);
                }
            }
            if (node.name == "Abyssal Echoes Tree All Regen On Kill")
            {
                healthRegenOnKill += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
                manaRegenOnKill += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.05f;
            }
            if (node.name == "Abyssal Echoes Tree On Kill Buff Duration")
            {
                increasedNonStackingBuffDuration += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Abyssal Echoes Tree Movement Speed On Kill")
            {
                movementSpeedOnKill += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Abyssal Echoes Tree Attack And Cast Speed On Kill")
            {
                attackAndCastSpeedOnKill += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
        }

        mutator.noChain = noChain;
        mutator.pullsOnHit = pullsOnHit;
        mutator.increasedPullStrength = increasedPullStrength;
        mutator.increasedRadius = Mathf.Sqrt(increasedArea + 1) - 1;
        mutator.speedDebuff = speedDebuff;
        mutator.armourDebuff = armourDebuff;
        mutator.increasedDebuffStrength = increasedDebuffStrength;
        mutator.increasedDebuffDuration = increasedDebuffDuration;
        mutator.dotTakenDebuff = dotTakenDebuff;
        mutator.dotTakenDebuffStacks = dotTakenDebuffStacks;

        mutator.additionalCasts = additionalCasts;
        mutator.increasedDelay = increasedDelay;

        mutator.stackingStatsOnKill = stackingStatsOnKill;

        mutator.healthRegenOnKill = healthRegenOnKill;
        mutator.manaRegenOnKill = manaRegenOnKill;

        mutator.increasedNonStackingBuffDuration = increasedNonStackingBuffDuration;
        mutator.movementSpeedOnKill = movementSpeedOnKill;
        mutator.attackAndCastSpeedOnKill = attackAndCastSpeedOnKill;

        mutator.increasedCastSpeed = increasedCastSpeed;

        mutator.increasedDamage = increasedDamage;
        mutator.addedManaCostDivider = manaEfficiency;
        mutator.increasedManaCost = increasedManaCost;
        mutator.stacks = stacks;
    }
}
