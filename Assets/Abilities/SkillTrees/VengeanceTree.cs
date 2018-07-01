using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VengeanceTree : SkillTree
{

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.vengeance);
    }


    public override void updateMutator()
    {
        VengeanceMutator mutator = PlayerFinder.getPlayer().GetComponent<VengeanceMutator>();

        float increasedDuration = 0f;
          int additionalRetaliations = 0;
          float darkBladeRetaliationChance = 0f;
          List<TaggedStatsHolder.TaggableStat> statsWhilePrepped = new List<TaggedStatsHolder.TaggableStat>();

          float increasedDamageWhileBelowHalfHealth = 0f;

          float increasedAttackSpeed = 0f;
          float increasedDamage = 0f;

          float increasedStunChance = 0f;
          float moreDamageAgaintDamaged = 0f;

          float reducedDamageTakenOnHit = 0f;
          float darkBladeOnVengeanceHitChance = 0f;
          float darkBladeOnVengeanceKillChance = 0f;
          float darkBladeOnRiposteKillChance = 0f;
        float darkBladeOnRiposteHitChance = 0f;
          float voidEssenceOnVengeanceKillChance = 0f;
          float voidEssenceOnRiposteHitChance = 0f;

          // dark blade
          float armourShredChance = 0f;

          bool noPierce = false;
          float chanceForDoubleDamage = 0f;
          float increasedDarkBladeDamage = 0f;
          float moreDamageAgainstStunned = 0f;
          float increasedProjectileSpeed = 0f;
          int chains = 0;
          float increasedDarkBladeStunChance = 0f;
          bool hasChained = false;

        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            if (node.name == "Vengeance Tree Double Retaliation")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    additionalRetaliations += 1;
                    increasedAttackSpeed -= 0.1f;
                }
            }
            if (node.name == "Vengeance Tree Damage While Prepped")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, new List<Tags.AbilityTags>());
                    stat.increasedValue = node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
                    statsWhilePrepped.Add(stat);
                }
            }
            if (node.name == "Vengeance Tree Damage Additional Retaliations")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    additionalRetaliations += node.GetComponent<SkillTreeNode>().pointsAllocated;
                    TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.DamageTaken, new List<Tags.AbilityTags>());
                    stat.moreValues.Add(node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f);
                    statsWhilePrepped.Add(stat);
                }
            }
            if (node.name == "Vengeance Tree Attack Speed")
            {
                increasedAttackSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.06f;
            }
            if (node.name == "Vengeance Tree Damage Below Half Health")
            {
                increasedDamageWhileBelowHalfHealth += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            }
            if (node.name == "Vengeance Tree Dark Blade On Vengeance Hit")
            {
                darkBladeOnVengeanceHitChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.05f;
            }
            if (node.name == "Vengeance Tree Dark Blade On Riposte Hit")
            {
                darkBladeOnRiposteHitChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Vengeance Tree Dark Blade Double Damage Chance")
            {
                chanceForDoubleDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            }
            if (node.name == "Vengeance Tree Dark Blade Armour Shred Chance")
            {
                armourShredChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Vengeance Tree Dark Blade Speed")
            {
                increasedProjectileSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            }
            if (node.name == "Vengeance Tree Dark Blade Chains")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    chains += 1;
                    noPierce = true;
                    increasedDarkBladeDamage += -0.15f;
                }
            }
            if (node.name == "Vengeance Tree Dark Blade Additional Chain")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    chains += node.GetComponent<SkillTreeNode>().pointsAllocated;
                    increasedProjectileSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * -0.25f;
                }
            }
            if (node.name == "Vengeance Tree Dark Blade On Riposte Kill")
            {
                darkBladeOnRiposteKillChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Vengeance Tree Dark Blade On Vengeance Kill")
            {
                darkBladeOnVengeanceKillChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Vengeance Tree Damage Against Damaged Enemies")
            {
                moreDamageAgaintDamaged += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            }
            if (node.name == "Vengeance Tree Damage VS Stun Chance")
            {
                increasedDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
                increasedStunChance += node.GetComponent<SkillTreeNode>().pointsAllocated * -0.15f;
            }
            if (node.name == "Vengeance Tree Dark Blade Retaliation")
            {
                darkBladeRetaliationChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Vengeance Tree Reduced Damage Taken")
            {
                reducedDamageTakenOnHit += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.05f;
            }
            if (node.name == "Vengeance Tree Void Essence On Riposte Hit")
            {
                voidEssenceOnRiposteHitChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            }
            if (node.name == "Vengeance Tree Void Essence On Vengeance Kill")
            {
                voidEssenceOnVengeanceKillChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            }
        }
        

        mutator.increasedDuration = increasedDuration;
        mutator.additionalRetaliations = additionalRetaliations;
        mutator.darkBladeRetaliationChance = darkBladeRetaliationChance;
        mutator.statsWhilePrepped = statsWhilePrepped;

        mutator.increasedDamageWhileBelowHalfHealth = increasedDamageWhileBelowHalfHealth;

        mutator.increasedAttackSpeed = increasedAttackSpeed;
        mutator.increasedDamage = increasedDamage;

        mutator.increasedStunChance = increasedStunChance;
        mutator.moreDamageAgaintDamaged = moreDamageAgaintDamaged;

        mutator.reducedDamageTakenOnHit = reducedDamageTakenOnHit;
        mutator.darkBladeOnVengeanceHitChance = darkBladeOnVengeanceHitChance;
        mutator.darkBladeOnVengeanceKillChance = darkBladeOnVengeanceKillChance;
        mutator.darkBladeOnRiposteKillChance = darkBladeOnRiposteKillChance;
        mutator.voidEssenceOnVengeanceKillChance = voidEssenceOnVengeanceKillChance;
        mutator.voidEssenceOnRiposteHitChance = voidEssenceOnRiposteHitChance;
        mutator.darkBladeOnRiposteHitChance = darkBladeOnRiposteHitChance;

        DarkBladeMutator mutator2 = PlayerFinder.getPlayer().GetComponent<DarkBladeMutator>();
        mutator2.armourShredChance = armourShredChance;

        mutator2.noPierce = noPierce;
        mutator2.chanceForDoubleDamage = chanceForDoubleDamage;
        mutator2.increasedDamage = increasedDarkBladeDamage;
        mutator2.moreDamageAgainstStunned = moreDamageAgainstStunned;
        mutator2.increasedProjectileSpeed = increasedProjectileSpeed;
        mutator2.chains = chains;
        mutator2.increasedStunChance = increasedDarkBladeStunChance;
        mutator2.hasChained = hasChained;
    }
}
