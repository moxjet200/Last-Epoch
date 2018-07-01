using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PassiveRespecPanel : MonoBehaviour
{
    public static PassiveRespecPanel instance = null;

    public int goldCost = 0;

    public void Awake()
    {
        instance = this;
    }

    public void RespecLatestPassive()
    {
        bool open = UIBase.instance.passiveTreeOpen;
        if (!open) { UIBase.instance.openPassiveTree(true); }

        if (PlayerFinder.getPlayer())
        {
            CharacterDataTracker dataTracker = PlayerFinder.getPlayer().GetComponent<CharacterDataTracker>();
            GoldTracker goldTracker = PlayerFinder.getPlayer().GetComponent<GoldTracker>();

            if (dataTracker && goldTracker && goldTracker.value >= goldCost)
            {
                if (dataTracker.charData != null && dataTracker.charData.characterTreeNodeProgression != null && dataTracker.charData.characterTreeNodeProgression.Count > 0)
                {
                    string nodeID = dataTracker.charData.characterTreeNodeProgression[dataTracker.charData.characterTreeNodeProgression.Count - 1];
                    foreach(CharacterTree tree in CharacterTree.all)
                    {
                        if (tree.characterClass == dataTracker.charData.characterClass)
                        {
                            foreach (SkillTreeNode node in tree.nodeList)
                            {
                                if (node.nodeID == nodeID)
                                {
                                    dataTracker.charData.respecs++;
                                    goldTracker.modifyGold(-goldCost);
                                    node.respecOnePoint(true);
                                }
                            }
                        }
                    }
                }
            }
        }

        if (!open) { UIBase.instance.closePassiveTree(); }

        UIBase.instance.closePassiveRespecConfirmation();
    }

    public void enable()
    {
        updateGoldCost();
    }

    public void updateGoldCost()
    {
        if (PlayerFinder.getPlayer())
        {
            CharacterDataTracker dataTracker = PlayerFinder.getPlayer().GetComponent<CharacterDataTracker>();

            if (dataTracker && dataTracker.charData != null)
            {
                goldCost = 100 + dataTracker.charData.respecs * 100;
                
                GoldTracker goldTracker = PlayerFinder.getPlayer().GetComponent<GoldTracker>();

                if (dataTracker.charData.characterTreeNodeProgression.Count <= 0)
                {
                    foreach (Transform child in transform)
                    {
                        if (child.name == "gold cost message")
                        {
                            child.GetComponent<TextMeshProUGUI>().text = "You do not have any points allocated on your passive grid";
                        }
                        else if (child.name == "Yes")
                        {
                            child.gameObject.SetActive(false);
                        }
                    }
                }
                else if (goldTracker.value < goldCost)
                {
                    foreach (Transform child in transform)
                    {
                        if (child.name == "gold cost message")
                        {
                            child.GetComponent<TextMeshProUGUI>().text = "You do not have the <color=#DAA520FF>" + goldCost + "</color> gold required to respec.";
                        }
                        else if (child.name == "Yes")
                        {
                            child.gameObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    foreach (Transform child in transform)
                    {
                        if (child.name == "gold cost message")
                        {
                            child.GetComponent<TextMeshProUGUI>().text = "This will undo your latest allocation to your passive grid at the cost of <color=#DAA520FF>" + goldCost + "</color> gold.\n\nIt will also make subsequent respecs cost 100 more gold.";
                        }
                        else if (child.name == "Yes")
                        {
                            child.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
    }
}