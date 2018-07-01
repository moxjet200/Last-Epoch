using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonSkeletonTree : SkillTree
{
    // ability references for updating minions
    Ability summonWarrior = null;
    Ability summonMage = null;
    Ability summonArcher = null;
    Ability summonWarlord = null;
    Ability summonBrawler = null;

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.summonSkeleton);
        // get ability references
        summonWarrior = Ability.getAbility(AbilityID.summonSkeletonWarrior);
        summonMage = Ability.getAbility(AbilityID.summonSkeletonMage);
        summonArcher = Ability.getAbility(AbilityID.summonSkeletonArcher);
        summonWarlord = Ability.getAbility(AbilityID.summonSkeletonWarlord);
        summonBrawler = Ability.getAbility(AbilityID.summonSkeletonBrawler);
    }

    public override void updateMutator()
    {
        GameObject player = PlayerFinder.getPlayer();

        SummonSkeletonMutator mutator = player.GetComponent<SummonSkeletonMutator>();
        float additionalDuration = 0f;
        int additionalSkeletons = 0;
        int additionalWarlords = 0;
        bool limitDuration = false;

        List<TaggedStatsHolder.TaggableStat> statList = new List<TaggedStatsHolder.TaggableStat>();

        List<TaggedStatsHolder.TaggableStat> warlordStatList = new List<TaggedStatsHolder.TaggableStat>();

        List<TaggedStatsHolder.TaggableStat> corpseStatList = new List<TaggedStatsHolder.TaggableStat>();

        List<TaggedStatsHolder.TaggableStat> corpseSelfBuffStatList = new List<TaggedStatsHolder.TaggableStat>();

        List<TaggedStatsHolder.TaggableStat> skeleDiedRecentlyStats = new List<TaggedStatsHolder.TaggableStat>();
        
        bool usesPoisonArrow = false;
        float increasedPoisonArrowCooldownSpeed = 0f;
        float increasedPoisonArrowCooldown = 0f;

        bool poisonArrowPierces = false;
        bool poisonArrowInaccurate = false;
        float poisonArrowIncreasedDamage = 0f;
        int poisonArrowDelayedAttacks = 0;

        bool usesMultishot = false;
        float increasedMultishotCooldownSpeed = 0f;

        bool usesDeathSlash = false;
        float increasedDeathSlashCooldownRecovery = 0f;

        bool usesNecroticMortar = false;
        float increasedNecroticMortarCooldownRecovery = 0f;

        bool usesInspire = false;
        float increasedInspireCooldownRecovery = 0f;

        bool cannotSummonWarriors = false;
        bool cannotSummonMages = false;
        bool canSummonArchers = false;
        bool canSummonWarlords = false;

        float healthOnSkeletonDeath = 0f;
        float wardOnSkeletonDeath = 0f;
        float manaOnSkeletonDeath = 0f;
        int additionalSkeletonsPerCast = 0;
        bool healSkeletonOnSkeletonDeath = false;

        float increasedManaCost = 0f;
        float manaEfficiency = 0f;

        SkillTreeNode skillTreeNode;
        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            skillTreeNode = node.GetComponent<SkillTreeNode>();
            if (node.name == "Summon Skeleton Tree Damage And Speed")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Movespeed, null);
                    newStat.increasedValue = skillTreeNode.pointsAllocated * 0.05f;
                    statList.Add(newStat);
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, null);
                    newStat2.moreValues.Add(skillTreeNode.pointsAllocated * 0.1f);
                    statList.Add(newStat2);
                }
            }
            if (node.name == "Summon Skeleton Tree Death Slash")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    usesDeathSlash = true;
                }
            }
            if (node.name == "Summon Skeleton Tree Slash Cooldown")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    increasedDeathSlashCooldownRecovery = 0.5f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Summon Skeleton Tree Physical Damage")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                    tagList.Add(Tags.AbilityTags.Physical);
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, tagList);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * 0.2f);
                    statList.Add(newStat);
                }
            }
            if (node.name == "Summon Skeleton Tree Commander")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    canSummonWarlords = true;
                    increasedManaCost += 0.3f;
                }
            }
            if (node.name == "Summon Skeleton Tree Commander Damage")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, null);
                    newStat2.moreValues.Add(skillTreeNode.pointsAllocated * 0.3f);
                    warlordStatList.Add(newStat2);
                }
            }
            if (node.name == "Summon Skeleton Tree Additional Commander")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    additionalWarlords += 1;
                }
            }
            if (node.name == "Summon Skeleton Tree Inspire")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    usesInspire = true;
                }
            }
            if (node.name == "Summon Skeleton Tree Inspire Cooldown")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    increasedInspireCooldownRecovery = 0.5f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Summon Skeleton Tree Attack Speed And Leech")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.AttackSpeed, null);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * 0.05f);
                    statList.Add(newStat);

                    TaggedStatsHolder.TaggableStat newStat3 = new TaggedStatsHolder.TaggableStat(Tags.Properties.CastSpeed, null);
                    newStat3.moreValues.Add(skillTreeNode.pointsAllocated * 0.05f);
                    statList.Add(newStat3);

                    List<Tags.AbilityTags> tagList2 = new List<Tags.AbilityTags>();
                    tagList2.Add(Tags.AbilityTags.Physical);
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.PercentLifeLeech, tagList2);
                    newStat2.addedValue = skillTreeNode.pointsAllocated * 0.05f;
                    statList.Add(newStat2);
                }
            }
            if (node.name == "Summon Skeleton Tree Armour Vs Damage")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                    tagList.Add(Tags.AbilityTags.Melee);
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, tagList);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * -0.15f);
                    statList.Add(newStat);
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.Armour, null);
                    newStat2.addedValue = skillTreeNode.pointsAllocated * 150f;
                    statList.Add(newStat2);
                }
            }
            if (node.name == "Summon Skeleton Tree No Mages")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    cannotSummonMages = true;
                }
            }
            if (node.name == "Summon Skeleton Tree Necrotic Damage")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                    tagList.Add(Tags.AbilityTags.Necrotic);
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, tagList);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * 0.2f);
                    statList.Add(newStat);
                }
            }
            if (node.name == "Summon Skeleton Tree Mortar")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    usesNecroticMortar = true;
                }
            }
            if (node.name == "Summon Skeleton Tree Mortar Cooldown")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    increasedNecroticMortarCooldownRecovery = 0.5f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Summon Skeleton Tree Cast Speed")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.CastSpeed, null);
                    newStat2.moreValues.Add(skillTreeNode.pointsAllocated * 0.08f);
                    statList.Add(newStat2);
                }
            }
            if (node.name == "Summon Skeleton Tree Necrotic Vs Phys Res")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                    tagList.Add(Tags.AbilityTags.Necrotic);
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, tagList);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * 0.25f);
                    statList.Add(newStat);

                    List<Tags.AbilityTags> tagList2 = new List<Tags.AbilityTags>();
                    tagList2.Add(Tags.AbilityTags.Physical);
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.DamageTaken, tagList2);
                    newStat2.moreValues.Add(skillTreeNode.pointsAllocated * 0.1f);
                    statList.Add(newStat2);
                }
            }
            if (node.name == "Summon Skeleton Tree No Warriors")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    cannotSummonWarriors = true;
                }
            }
            if (node.name == "Summon Skeleton Tree Additional Skeletons Per Cast")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    additionalSkeletonsPerCast += skillTreeNode.pointsAllocated;
                    increasedManaCost += skillTreeNode.pointsAllocated * 1.1f;
                }
            }
            //if (node.name == "Summon Skeleton Tree One Skele Per Cast")
            //{
            //    if (skillTreeNode.pointsAllocated > 0)
            //    {
            //        onlySummonOneSkeletonAtOnce = true;
            //        additionalSkeletons += 2;
            //    }
            //}
            if (node.name == "Summon Skeleton Tree Corpse Consumption")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, null);
                    newStat2.moreValues.Add(skillTreeNode.pointsAllocated * 0.3f);
                    corpseStatList.Add(newStat2);
                }
            }
            if (node.name == "Summon Skeleton Tree Corpse Damage Reduction")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.DamageTaken, null);
                    newStat2.moreValues.Add(skillTreeNode.pointsAllocated * -0.12f);
                    corpseStatList.Add(newStat2);
                }
            }
            if (node.name == "Summon Skeleton Tree Corpse Cast Speed")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.CastSpeed, null);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * 0.12f);
                    corpseSelfBuffStatList.Add(newStat);
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.CastSpeed, null);
                    newStat2.moreValues.Add(skillTreeNode.pointsAllocated * 0.12f);
                    corpseStatList.Add(newStat2);
                }
            }
            if (node.name == "Summon Skeleton Tree Health")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.Health, null);
                    newStat2.moreValues.Add(skillTreeNode.pointsAllocated * 0.2f);
                    statList.Add(newStat2);
                }
            }
            if (node.name == "Summon Skeleton Tree Max Skeles Vs Life Drain")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.CurrentHealthDrain, null);
                    newStat2.addedValue = skillTreeNode.pointsAllocated * 0.03f;
                    skeleDiedRecentlyStats.Add(newStat2);

                    additionalSkeletons += skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Summon Skeleton Tree Vitality")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.Vitality, null);
                    newStat2.addedValue = skillTreeNode.pointsAllocated * 1;
                    statList.Add(newStat2);
                }
            }
            if (node.name == "Summon Skeleton Tree Limit Duration")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    limitDuration = true;
                    increasedManaCost += 0.3f;
                }
            }
            if (node.name == "Summon Skeleton Tree Max Skeles Vs Mana Cost")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    additionalSkeletons += skillTreeNode.pointsAllocated;
                    increasedManaCost += skillTreeNode.pointsAllocated * 0.1f;
                }
            }
            if (node.name == "Summon Skeleton Tree Ward On Skele Death")
            {
                wardOnSkeletonDeath += node.GetComponent<SkillTreeNode>().pointsAllocated * 10;
            }
            if (node.name == "Summon Skeleton Tree Mana On Skele Death")
            {
                wardOnSkeletonDeath += node.GetComponent<SkillTreeNode>().pointsAllocated * 5;
            }
            if (node.name == "Summon Skeleton Tree Spell Damage On Skele Death")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    List<Tags.AbilityTags> tagList2 = new List<Tags.AbilityTags>();
                    tagList2.Add(Tags.AbilityTags.Spell);
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, tagList2);
                    newStat2.increasedValue = skillTreeNode.pointsAllocated * 0.25f;
                    skeleDiedRecentlyStats.Add(newStat2);
                }
            }
            if (node.name == "Summon Skeleton Tree Archers")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    canSummonArchers = true;
                }
            }
            if (node.name == "Summon Skeleton Tree Poison Arrow")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    usesPoisonArrow = true;
                }
            }
            if (node.name == "Summon Skeleton Tree Piercing Poison Arrow")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    poisonArrowPierces = true;
                    increasedPoisonArrowCooldown += 0.4f;
                }
            }
            if (node.name == "Summon Skeleton Tree Poison Arrow Cooldown")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    increasedPoisonArrowCooldownSpeed = 0.5f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Summon Skeleton Tree Poison Arrow Inaccuracy")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    poisonArrowInaccurate = true;
                    poisonArrowIncreasedDamage = 0.7f;
                }
            }
            if (node.name == "Summon Skeleton Tree Poison Arrow Extra Arrows")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    poisonArrowDelayedAttacks += 1 * skillTreeNode.pointsAllocated;
                    increasedPoisonArrowCooldown += 0.3f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Summon Skeleton Tree Bleed Chance")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                    tagList.Add(Tags.AbilityTags.Physical);
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.BleedChance, tagList);
                    newStat.addedValue = skillTreeNode.pointsAllocated * 0.25f;
                    statList.Add(newStat);
                }
            }
            if (node.name == "Summon Skeleton Tree Bow Attack Speed")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                    tagList.Add(Tags.AbilityTags.Bow);
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.AttackSpeed, tagList);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * 0.1f);
                    statList.Add(newStat);
                }
            }
            if (node.name == "Summon Skeleton Tree Bow Damage")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                    tagList.Add(Tags.AbilityTags.Bow);
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, tagList);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * 0.25f);
                    statList.Add(newStat);
                }
            }
            if (node.name == "Summon Skeleton Tree Multishot")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    usesMultishot = true;
                }
            }
            if (node.name == "Summon Skeleton Tree Multishot Cooldown")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    increasedMultishotCooldownSpeed = 0.5f * skillTreeNode.pointsAllocated;
                }
            }
            if (node.name == "Summon Skeleton Tree Elemental Protection")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.HealthRegen, null);
                    newStat.addedValue = skillTreeNode.pointsAllocated * 5f;
                    statList.Add(newStat);

                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.ElementalProtection, null);
                    newStat2.addedValue = skillTreeNode.pointsAllocated * 50f;
                    statList.Add(newStat2);
                }
            }
            if (node.name == "Summon Skeleton Tree Max Skeles vs Attack Speed")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    additionalSkeletons += 1;

                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.AttackSpeed, null);
                    newStat.increasedValue = skillTreeNode.pointsAllocated * -0.05f;
                    statList.Add(newStat);

                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.CastSpeed, null);
                    newStat2.increasedValue = skillTreeNode.pointsAllocated * -0.05f;
                    statList.Add(newStat2);
                }
            }
            if (node.name == "Summon Skeleton Tree Mana Efficiency")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    manaEfficiency += skillTreeNode.pointsAllocated * 0.2f;

                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.CriticalChance, null);
                    newStat2.increasedValue = skillTreeNode.pointsAllocated * 0.2f;
                    statList.Add(newStat2);
                }
            }
            if (node.name == "Summon Skeleton Tree Heal Skeleton On Skeleton Death")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {
                    healSkeletonOnSkeletonDeath = true;
                }
            }

        }

        // second loop to react to whether there is no skeleton limit
        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            skillTreeNode = node.GetComponent<SkillTreeNode>();

            if (node.name == "Summon Skeleton Tree Half Skeletons")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {

                    // if you have the node that turns the number limit into a duration limit reduce the duration
                    if (limitDuration)
                    {
                        additionalDuration = ((20f + additionalDuration) * 0.4f) - 20;
                    }

                    // otherwise reduce the number
                    else
                    {
                        additionalSkeletons = Mathf.CeilToInt((4f + (float)additionalSkeletons) / 2) - 4;
                    }

                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, null);
                    newStat.moreValues.Add(skillTreeNode.pointsAllocated * 0.35f);
                    statList.Add(newStat);
                    TaggedStatsHolder.TaggableStat newStat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.Health, null);
                    newStat2.moreValues.Add(skillTreeNode.pointsAllocated * 0.35f);
                    statList.Add(newStat2);

                    // avoid buffing warlords, by adding equivalent bonuses as quotient modifiers
                    TaggedStatsHolder.TaggableStat newStat3 = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, null);
                    newStat3.quotientValues.Add(skillTreeNode.pointsAllocated * 0.35f);
                    warlordStatList.Add(newStat3);
                    TaggedStatsHolder.TaggableStat newStat4 = new TaggedStatsHolder.TaggableStat(Tags.Properties.Health, null);
                    newStat4.quotientValues.Add(skillTreeNode.pointsAllocated * 0.35f);
                    warlordStatList.Add(newStat4);
                }
            }

            // needs to know whether to give duration or health
            if (node.name == "Summon Skeleton Tree Additional Duration")
            {
                if (skillTreeNode.pointsAllocated > 0)
                {

                    if (limitDuration)
                    {
                        additionalDuration += node.GetComponent<SkillTreeNode>().pointsAllocated * 4;
                    }
                    else
                    {
                        TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Health, null);
                        newStat.addedValue = skillTreeNode.pointsAllocated * 14f;
                        statList.Add(newStat);
                    }

                }
            }
        }

        // update the mutator
        mutator.additionalDuration = additionalDuration;
        mutator.additionalSkeletons = additionalSkeletons;
        mutator.additionalWarlords = additionalWarlords;
        mutator.limitDuration = limitDuration;

        mutator.statList = statList;
        mutator.warlordStatList = warlordStatList;
        mutator.corpseStatList = corpseStatList;
        mutator.corpseSelfBuffStatList = corpseSelfBuffStatList;
        mutator.skeleDiedRecentlyStats = skeleDiedRecentlyStats;

        mutator.usesPoisonArrow = usesPoisonArrow;
        mutator.increasedPoisonArrowCooldownSpeed = increasedPoisonArrowCooldownSpeed;
        mutator.increasedPoisonArrowCooldown = increasedPoisonArrowCooldown;

        mutator.poisonArrowPierces = poisonArrowPierces;
        mutator.poisonArrowInaccurate = poisonArrowInaccurate;
        mutator.poisonArrowIncreasedDamage = poisonArrowIncreasedDamage;
        mutator.poisonArrowDelayedAttacks = poisonArrowDelayedAttacks;

        mutator.usesMultishot = usesMultishot;
        mutator.increasedMultishotCooldownSpeed = increasedMultishotCooldownSpeed;

        mutator.usesDeathSlash = usesDeathSlash;
        mutator.increasedDeathSlashCooldownRecovery = increasedDeathSlashCooldownRecovery;

        mutator.usesNecroticMortar = usesNecroticMortar;
        mutator.increasedNecroticMortarCooldownRecovery = increasedNecroticMortarCooldownRecovery;

        mutator.usesInspire = usesInspire;
        mutator.increasedInspireCooldownRecovery = increasedInspireCooldownRecovery;

        mutator.cannotSummonWarriors = cannotSummonWarriors;
        mutator.cannotSummonMages = cannotSummonMages;
        mutator.canSummonArchers = canSummonArchers;
        mutator.canSummonWarlords = canSummonWarlords;

        mutator.healthOnSkeletonDeath = healthOnSkeletonDeath;
        mutator.wardOnSkeletonDeath = wardOnSkeletonDeath;
        mutator.manaOnSkeletonDeath = manaOnSkeletonDeath;
        mutator.additionalSkeletonsPerCast = additionalSkeletonsPerCast;
        mutator.healSkeletonOnSkeletonDeath = healSkeletonOnSkeletonDeath;

        mutator.increasedManaCost = increasedManaCost;
        mutator.addedManaCostDivider = manaEfficiency;

        // update existing wolves
        if (player.GetComponent<SummonTracker>())
        {
            // get a list of existing skeletons
            List<SummonChangeTracker> skeles = new List<SummonChangeTracker>();
            SummonChangeTracker changeTracker = null;
            CreationReferences references = null;
            foreach (Summoned summon in player.GetComponent<SummonTracker>().summons)
            {
                changeTracker = summon.GetComponent<SummonChangeTracker>();
                references = summon.GetComponent<CreationReferences>();
                if (references && changeTracker)
                {
                    if (references.thisAbility == ability || references.thisAbility == summonWarrior || references.thisAbility == summonMage || references.thisAbility == summonArcher
                        || references.thisAbility == summonWarlord || references.thisAbility == summonBrawler)
                    {
                        skeles.Add(changeTracker);
                    }
                }
            }

            // update the skeletons
            foreach (SummonChangeTracker skele in skeles)
            {
                skele.changeStats(statList);
                skele.changeLimitDuration(limitDuration, summonArcher.abilityPrefab.GetComponent<SummonEntityOnDeath>().duration + additionalDuration);
            }
        }

    }
}
