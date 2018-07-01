using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialiseButton : MonoBehaviour {

    public void clicked()
    {
        UIBase uiBase = UIBase.instance;
        if (uiBase && uiBase.openSkillTrees.Count > 0)
        {
            SkillTree openSKillTree = uiBase.openSkillTrees[0];
            GameObject player = PlayerFinder.getPlayer();
            if (openSKillTree && player && player.GetComponent<SpecialisedAbilityList>())
            {
                player.GetComponent<SpecialisedAbilityList>().SpecialiseIfThereIsAFreeSlot(openSKillTree.ability);
                // play a sound
                UISounds.playSound(UISounds.UISoundLabel.Specialise);
            }
            uiBase.closeSkillTrees();
            openSKillTree.open();
            uiBase.closeSpecialisationSelection();
        }
    }

}
