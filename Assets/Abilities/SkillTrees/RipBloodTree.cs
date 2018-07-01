using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RipBloodTree : SkillTree
{

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.ripBlood);
    }

    public override void updateMutator()
    {
        RipBloodMutator mutator = PlayerFinder.getPlayer().GetComponent<RipBloodMutator>();
        float increasedArea = 0f;
        float splatter_increasedDamage = 0f;
        float splatter_increasedStunChance = 0f;
        float splatter_chanceToPoison = 0f;
        float splatter_armourReductionChance = 0f;
        float splatter_armourReduction = 0f;
        bool splatter_armourReductionStacks = false;
        float splatter_increasedArmourDebuffDuration = 0f;
        float splatter_increasedDamagePerMinion = 0f;
        bool splatter_reducesDarkProtectionInstead = false;

    List<TaggedStatsHolder.TaggableStat> splatter_minionBuffs = new List<TaggedStatsHolder.TaggableStat>();

        float splatterChance = 0f;
        float increasedDamage = 0f;
        float increasedStunChance = 0f;
        float chanceToBleed = 0f;
        float chanceToPoison = 0f;
        float increasedDamagePerMinion = 0f;
        float increasedCastSpeed = 0f;

        float addedHealthGained = 0f;
        float increasedHealthGained = 0f;
        float increasedHealthGainedPerAttunement = 0f;
        float manaGained = 0f;
        bool convertHealthToWard = false;

        bool freeWhenOutOfMana = false;
        bool targetsAlliesInstead = false;
        bool necrotic = false;
        bool recastOnKill = false;

        float manaEfficiency = 0f;
        float increasedManaCost = 0f;

        List<TaggedStatsHolder.TaggableStat> onHitBuffs = new List<TaggedStatsHolder.TaggableStat>();

        SkillTreeNode skillTreeNode;
        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {

            skillTreeNode = node.GetComponent<SkillTreeNode>();
            if (node.name == "Rip Blood Tree Splatter Chance")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    splatterChance += 0.17f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Rip Blood Tree Always Splatter")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    splatterChance += 1f;
                    increasedManaCost += 1.2f;
                }
            }
            if (node.name == "Rip Blood Tree Area")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    increasedArea += 0.25f;
                }
            }
            if (node.name == "Rip Blood Tree Splatter Damage")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    splatter_increasedDamage += 0.15f;
                    splatter_increasedStunChance += 0.15f;
                }
            }
            if (node.name == "Rip Blood Tree Armour Reduction")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    splatter_armourReductionChance += 0.3f;
                    splatter_armourReduction += 30f;
                }
            }
            if (node.name == "Rip Blood Tree Additional Armour Reduction")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    splatter_armourReduction += 30f;
                }
            }
            if (node.name == "Rip Blood Tree Additional Reduction Chance")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    splatter_armourReductionChance += 0.18f;
                }
            }
            if (node.name == "Rip Blood Tree Reduction Duration")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    splatter_increasedArmourDebuffDuration += 0.3f;
                }
            }
            if (node.name == "Rip Blood Tree Vitality Reduction")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    splatter_reducesDarkProtectionInstead = true;
                }
            }
            if (node.name == "Rip Blood Tree Buff Minion Damage")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, null);
                    newStat2.increasedValue = skillTreeNode.pointsAllocated * 0.3f;
                    splatter_minionBuffs.Add(newStat2);
                }
            }
            if (node.name == "Rip Blood Tree Damage Per Minion")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    splatter_increasedDamagePerMinion += 0.02f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Rip Blood Tree Buff Minion Attack Speed")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.CastSpeed, null);
                    newStat.increasedValue = skillTreeNode.pointsAllocated * 0.1f;
                    splatter_minionBuffs.Add(newStat);

                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.AttackSpeed, null);
                    newStat2.increasedValue = skillTreeNode.pointsAllocated * 0.1f;
                    splatter_minionBuffs.Add(newStat2);
                }
            }
            if (node.name == "Rip Blood Tree Buff Minion Leech")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    List<Tags.AbilityTags> tagList2 = new List<Tags.AbilityTags>();
                    tagList2.Add(Tags.AbilityTags.Physical);
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.PercentLifeLeech, tagList2);
                    newStat2.addedValue = skillTreeNode.pointsAllocated * 0.1f;
                    splatter_minionBuffs.Add(newStat2);
                }
            }
            if (node.name == "Rip Blood Tree Cast Speed")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    increasedCastSpeed += 0.06f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Rip Blood Tree Mana Efficiency Vs Damage")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    manaEfficiency += 0.5f * skillTreeNode.pointsAllocated;
                    increasedDamage += -0.05f * skillTreeNode.pointsAllocated;
                    splatter_increasedDamage += -0.05f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Rip Blood Tree Free When Out Of Mana")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    freeWhenOutOfMana = true;
                    increasedHealthGained = -0.3f;
                }
            }
            if (node.name == "Rip Blood Tree Added Health Gained")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    addedHealthGained += 3f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Rip Blood Tree More Health Per Attunement")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    increasedHealthGainedPerAttunement += 0.04f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Rip Blood Tree Mana Gained")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    manaGained += 2f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Rip Blood Tree Targets Minions")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    targetsAlliesInstead = true;
                    increasedHealthGained += 0.7f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Rip Blood Tree Bleed Chance")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    chanceToBleed += 0.2f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Rip Blood Tree Blood Splatter Poison")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    splatter_chanceToPoison += 0.2f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Rip Blood Tree Spell Damage On Hit")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    List<Tags.AbilityTags> tagList2 = new List<Tags.AbilityTags>();
                    tagList2.Add(Tags.AbilityTags.Spell);
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, tagList2);
                    newStat2.addedValue = skillTreeNode.pointsAllocated * 0.05f;
                    onHitBuffs.Add(newStat2);
                }
            }
            if (node.name == "Rip Blood Tree Upfront Damage")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    increasedDamage += 0.15f;
                }
            }
            if (node.name == "Rip Blood Tree Upfront Damage Per Minion")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    increasedDamagePerMinion += 0.015f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Rip Blood Tree Necrotic")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    necrotic = true;
                }
            }
        }

        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            skillTreeNode = node.GetComponent<SkillTreeNode>();
            if (node.name == "Rip Blood Tree Cast Again On Kill")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    recastOnKill = true;
                    increasedHealthGained -= 100f;
                    manaGained = 0f;
                }
            }
        }

        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            skillTreeNode = node.GetComponent<SkillTreeNode>();
            if (node.name == "Rip Blood Tree Splatter Debuff Stacks")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    splatter_armourReductionStacks = true;
                    splatter_armourReduction *= 0.4f;
                }
            }
            if (node.name == "Rip Blood Tree Bleed To Poison")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    chanceToPoison = chanceToBleed;
                    chanceToBleed = 0f;
                }
            }
            if (node.name == "Rip Blood Tree Health To Ward")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    // half health gained
                    increasedHealthGained *= 0.5f;
                    increasedHealthGained -= 0.5f;
                    // convert to ward
                    convertHealthToWard = true;
                }
            }
        }


        mutator.splatter_increasedRadius = Mathf.Sqrt(increasedArea + 1) - 1;
        mutator.splatter_increasedDamage = splatter_increasedDamage;
        mutator.splatter_increasedStunChance = splatter_increasedStunChance;
        mutator.splatter_chanceToPoison = splatter_chanceToPoison;
        mutator.splatter_armourReductionChance = splatter_armourReductionChance;
        mutator.splatter_armourReduction = splatter_armourReduction;
        mutator.splatter_armourReductionStacks = splatter_armourReductionStacks;
        mutator.splatter_increasedArmourDebuffDuration = splatter_increasedArmourDebuffDuration;
        mutator.splatter_increasedDamagePerMinion = splatter_increasedDamagePerMinion;
        mutator.splatter_reducesDarkProtectionInstead = splatter_reducesDarkProtectionInstead;

        mutator.splatter_minionBuffs = splatter_minionBuffs;

        mutator.splatterChance = splatterChance;
        mutator.increasedDamage = increasedDamage;
        mutator.increasedStunChance = increasedStunChance;
        mutator.chanceToBleed = chanceToBleed;
        mutator.chanceToPoison = chanceToPoison;
        mutator.increasedDamagePerMinion = increasedDamagePerMinion;
        mutator.increasedCastSpeed = increasedCastSpeed;

        mutator.addedHealthGained = addedHealthGained;
        mutator.increasedHealthGained = increasedHealthGained;
        mutator.increasedHealthGainedPerAttunement = increasedHealthGainedPerAttunement;
        mutator.manaGained = manaGained;
        mutator.convertHealthToWard = convertHealthToWard;

        mutator.freeWhenOutOfMana = freeWhenOutOfMana;
        mutator.targetsAlliesInstead = targetsAlliesInstead;
        mutator.necrotic = necrotic;
        mutator.recastOnKill = recastOnKill;

        mutator.onHitBuffs = onHitBuffs;

        mutator.addedManaCostDivider = manaEfficiency;
        mutator.increasedManaCost = increasedManaCost;
    }
}
