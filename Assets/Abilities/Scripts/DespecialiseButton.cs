using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespecialiseButton : MonoBehaviour {

    public void clicked()
    {
        UIBase uiBase = UIBase.instance;
        if (uiBase && uiBase.openSkillTrees.Count > 0)
        {
            SkillTree openSkillTree = uiBase.openSkillTrees[0];
            GameObject player = PlayerFinder.getPlayer();
            if (openSkillTree && player && player.GetComponent<SpecialisedAbilityList>())
            {
                // de-allocate all the nodes
                foreach (SkillTreeNode node in SkillTreeNode.all)
                {
                    if (node.tree == openSkillTree)
                    {
                        node.pointsAllocated = 0;
                        node.updateText();
                    }
                }
                // set unspent points to 1
                openSkillTree.unspentPoints = 1;
                // update the mutator now that the points are de-allocated
                openSkillTree.updateMutator();
                // despecialise the ability
                player.GetComponent<SpecialisedAbilityList>().DespecialiseAbility(openSkillTree.ability);
                // play a sound
                UISounds.playSound(UISounds.UISoundLabel.Despecialise);
                // update tooltip mana costs
                AbilityTooltip.updateManaCosts(openSkillTree.ability);
            }
            uiBase.closeSkillTrees();
            uiBase.closeSpecialisationSelection();
        }
    }
}
