using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorSkillTree : SkillTree {

    public override void setAbility()
    {
        ability = AbilityIDList.getAbility(AbilityID.meteor);
    }

    public override void updateMutator()
    {
        MeteorMutator mutator = PlayerFinder.getPlayer().GetComponent<MeteorMutator>();
        float increasedManaCost = 0f;
        float addedManaCostDivider = 0f;
        int additionalMeteors = 0;
        float increasedMeteorFrequency = 0;
        float increasedShowerRadius = 0f;
        float increasedFallSpeed = 0f;
        float increasedCastSpeed = 0f;
        bool usesAllMana = false;
        float moreDamageAgainstFullHealth = 0f;
        float shrapnelChance = 0f;
        float increasedShrapnelSpeed = 0f;
        bool shrapnelPierces = false;
        float increasedShrapnelDamage = 0f;
        float increasedStunChance = 0f;
        bool line = false;
        List<float> moreDamageInstances = new List<float>();
        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>())
        {
            if (node.name == "Meteor Skill Tree Against Full Health")
            {
                moreDamageAgainstFullHealth += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            }
            if (node.name == "Meteor Skill Tree Shrapnel Chance")
            {
                shrapnelChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.34f;
            }
            if (node.name == "Meteor Skill Tree Shrapnel Damage")
            {
                increasedShrapnelDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.25f;
            }
            if (node.name == "Meteor Skill Tree Shrapnel Speed")
            {
                increasedShrapnelSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
            }
            if (node.name == "Meteor Skill Tree Shrapnel Pierces")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    shrapnelPierces = true;
                    increasedShrapnelSpeed -= 0.2f;
                }
            }
            if (node.name == "Meteor Skill Tree Increased Damage")
            {
                moreDamageInstances.Add(node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f);
                increasedCastSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * -0.05f;
            }
            if (node.name == "Meteor Skill Tree Cast Speed")
            {
                increasedCastSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
                increasedFallSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
                increasedManaCost += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Meteor Skill Tree Stun Chance")
            {
                increasedStunChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Meteor Skill Tree Stun And Mana")
            {
                increasedStunChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
                addedManaCostDivider += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.1f;
            }
            if (node.name == "Meteor Skill Tree All Mana")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    usesAllMana = true;
                }
            }
            if (node.name == "Meteor Skill Tree Fall Speed")
            {
                increasedFallSpeed += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
            if (node.name == "Meteor Skill Tree Twin Meteors")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    additionalMeteors += 1;
                    moreDamageInstances.Add(-0.35f);
                }
            }
            if (node.name == "Meteor Skill Tree More Meteors")
            {
                additionalMeteors += node.GetComponent<SkillTreeNode>().pointsAllocated;
                moreDamageInstances.Add(node.GetComponent<SkillTreeNode>().pointsAllocated * -0.07f);
            }
            if (node.name == "Meteor Skill Tree Shower Radius")
            {
                increasedShowerRadius += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
                moreDamageInstances.Add(node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f);
            }
            if (node.name == "Meteor Skill Tree Frequency")
            {
                increasedMeteorFrequency += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.4f;
            }
            if (node.name == "Meteor Skill Tree Line")
            {
                if (node.GetComponent<SkillTreeNode>().pointsAllocated > 0)
                {
                    line = true;
                    increasedManaCost += 0.5f;
                }
            }
            if (node.name == "Meteor Skill Tree Mana Efficiency")
            {
                addedManaCostDivider += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            }
        }

        mutator.increasedManaCost = increasedManaCost;
        mutator.addedManaCostDivider = addedManaCostDivider;
        mutator.increasedFallSpeed = increasedFallSpeed;
        mutator.increasedCastSpeed = increasedCastSpeed;
        mutator.usesAllMana = usesAllMana;
        mutator.moreDamageAgainstFullHealth = moreDamageAgainstFullHealth;
        mutator.shrapnelChance = shrapnelChance;
        mutator.increasedShrapnelSpeed = increasedShrapnelSpeed;
        mutator.shrapnelPierces = shrapnelPierces;
        mutator.increasedShrapnelDamage = increasedShrapnelDamage;
        mutator.moreDamageInstances = moreDamageInstances;
        mutator.increasedStunChance = increasedStunChance;
        mutator.additionalMeteors = additionalMeteors;
        mutator.increasedMeteorFrequency = increasedMeteorFrequency;
        mutator.increasedShowerRadius = increasedShowerRadius;
        mutator.line = line;
    }
}
