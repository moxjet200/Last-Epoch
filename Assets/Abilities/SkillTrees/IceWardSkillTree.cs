using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWardSkillTree : SkillTree {

	public override void setAbility()
	{
		ability = AbilityIDList.getAbility(AbilityID.iceWard);
	}

	public override void updateMutator()
	{
		IceWardMutator mutator = PlayerFinder.getPlayer ().GetComponent<IceWardMutator> ();
		float additionalDuration = 0f;
		float additionalBlockElementalProtection = 0f;
		bool canCastOnAllies = false;
		float additionalBlockChance = 0f;
		float wardOnBlock = 0f;
		float extraManaDrain = 0f;
        float additionalWardRegen = 0f;
        float additionalBlockArmour = 0f;
        float additionalArmour = 0f;
        float additionalWardRetention = 0f;
        float reducedManaPenalty = 0f;
        float increasedManaCost = 0f;
        bool castsFrostNova = false;
        float increasedFrostNovaFrequency = 0f;
        float novaDamage = 0f;
        float novaCritChance = 0f;
        float novaCritMulti = 0f;


        foreach (SkillTreeNode node in GetComponentsInChildren<SkillTreeNode>()) {
            if (node.name == "Ice Ward Skill Tree Block")
            {
                additionalBlockChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.09f;
                additionalBlockArmour += node.GetComponent<SkillTreeNode>().pointsAllocated * 200;
                extraManaDrain += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.33f;
            }
			if (node.name == "Ice Ward Skill Tree Additional Duration") {
				additionalDuration += node.GetComponent<SkillTreeNode> ().pointsAllocated * 3;
			}
			if (node.name == "Ice Ward Skill Tree Additional Block Elemental Protection")
			{
				additionalBlockElementalProtection += node.GetComponent<SkillTreeNode>().pointsAllocated * 150;
            }
            if (node.name == "Ice Ward Skill Tree Additional Armour")
            {
                additionalArmour += node.GetComponent<SkillTreeNode>().pointsAllocated * 25;
            }

            if (node.name == "Ice Ward Skill Tree Ward Retention")
            {
                additionalWardRetention += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.15f;
            }
            if (node.name == "Ice Ward Skill Tree Ward Regen")
            {
                additionalWardRegen += node.GetComponent<SkillTreeNode>().pointsAllocated * 1f;
            }

            if (node.name == "Ice Ward Skill Tree Additional Block Chance") {
				additionalBlockChance += node.GetComponent<SkillTreeNode> ().pointsAllocated * 0.03f;
			}
            if (node.name == "Ice Ward Skill Tree Block Immunity")
            {
                if ((node.GetComponent<SkillTreeNode>().pointsAllocated > 0))
                {
                    additionalBlockChance += -0.10f;
                    additionalBlockArmour += 2000f;
                    additionalBlockElementalProtection += 2000f;
                }
            }
            if (node.name == "Ice Ward Skill Tree Ward Gain on Block") {
				wardOnBlock += node.GetComponent<SkillTreeNode> ().pointsAllocated * 10;
			}
			if (node.name == "Ice Ward Skill Tree Reduced Mana Regeneration Penalty") {
                reducedManaPenalty += node.GetComponent<SkillTreeNode> ().pointsAllocated * 0.25f;
			}
            if (node.name == "Ice Ward Skill Tree Can Cast On Allies")
            {
                if ((node.GetComponent<SkillTreeNode>().pointsAllocated > 0))
                {
                    canCastOnAllies = true;
                    increasedManaCost += 1f;
                    reducedManaPenalty = 1f;
                }
            }

            if (node.name == "Ice Ward Skill Tree Ice Nova")
            {
                if ((node.GetComponent<SkillTreeNode>().pointsAllocated > 0))
                {
                    castsFrostNova = true;
                    additionalWardRegen -= 5;
                }
            }
            if (node.name == "Ice Ward Skill Tree Nova Damage")
            {
                novaDamage += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.3f;
                extraManaDrain += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.33f;
            }
            if (node.name == "Ice Ward Skill Tree Nova Crit Chance")
            {
                novaCritChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.07f;
            }
            if (node.name == "Ice Ward Skill Tree Nova Crit Multi")
            {
                novaCritChance += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.4f;
            }
            if (node.name == "Ice Ward Skill Tree Nova Frequency")
            {
                increasedFrostNovaFrequency += node.GetComponent<SkillTreeNode>().pointsAllocated * 0.2f;
            }
        }

        mutator.additionalDuration = additionalDuration;
		mutator.additionalBlockElementalProtection = additionalBlockElementalProtection;
		mutator.canCastOnAllies = canCastOnAllies;
		mutator.additionalBlockChance = additionalBlockChance;
		mutator.wardOnBlock = wardOnBlock;
		mutator.extraManaDrain = extraManaDrain;

        mutator.additionalWardRegen = additionalWardRegen;
        mutator.additionalBlockArmour = additionalBlockArmour;
        mutator.additionalArmour = additionalArmour;
        mutator.additionalWardRetention = additionalWardRetention;
        mutator.reducedManaPenalty = reducedManaPenalty;

        mutator.increasedManaCost = increasedManaCost;

        mutator.castsFrostNova = castsFrostNova;
        mutator.increasedFrostNovaFrequency = increasedFrostNovaFrequency;
        mutator.novaDamage = novaDamage;
        mutator.novaCritChance = novaCritChance;
        mutator.novaCritMulti = novaCritMulti;
    }
}
