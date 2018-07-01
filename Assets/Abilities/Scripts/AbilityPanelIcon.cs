using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(AbilityTooltip))]
public class AbilityPanelIcon : MonoBehaviour {

    GameObject player;
    Image icon;
    public Ability ability;
    public SkillTree skillTree;
	private AbilityTooltip tooltipAbility;
    GameObject hasTree = null;

    // Use this for initialization
    void Start () {
		tooltipAbility = gameObject.GetComponent<AbilityTooltip> ();
        player = PlayerFinder.getPlayer();
        PlayerFinder.playerChangedEvent += ChangePlayer;

        if (skillTree == null)
        {
            foreach (SkillTree tree in SkillTree.all)
            {
                if (tree.ability == ability)
                {
                    skillTree = tree;
                }
            }
        }

        if (player != null) {
			foreach (Transform child in transform) {
				if (child.name == "Sprite" && child.GetComponent<Image> ()) {
					icon = child.GetComponent<Image> ();
					icon.sprite = ability.abilitySprite;
				} else if (child.name == "Frame") {
				} else if (child.name == "Description") {
					child.GetComponent<TextMeshProUGUI> ().text = ability.description;
				} else if (child.name == "SkillName") {
					child.GetComponent<TextMeshProUGUI> ().text = ability.abilityName;
				}
                else if (child.name == "HasTree")
                {
                    hasTree = child.gameObject;
                    if (!skillTree)
                    {
                        hasTree.SetActive(false);
                    }
                }
			}
		}
    }

    void OnDestroy()
    {
        PlayerFinder.playerChangedEvent -= ChangePlayer;
    }

    public void ChangePlayer(GameObject newPlayer)
    {
        player = newPlayer;
    }
	
	// Update is called once per frame
	void Update () {

		if (ability != null) { //TODO probably move for performance
			tooltipAbility.SetAbility(ability);
		}
        
        if (skillTree == null)
        {
            foreach (SkillTree tree in SkillTree.all)
            {
                if (tree.ability == ability)
                {
                    skillTree = tree;
                }
            }
        }
    }

    public void clicked()
    {
        if (AbilityDragHandler.dragging) { return; }

        int abilityNumber = AbilityPanelManager.instance.abilityClicked;
        // abilityNumber is 0 if this was opened with the S key
        if (abilityNumber == 0)
        {
            // play a sound
            UISounds.playSound(UISounds.UISoundLabel.Confirm);
            // open the tree
            openSkillTree();
        }
        // otherise it was opened by clicking on an ability in the ability bar
        else
        {
            player.GetComponent<AbilityList>().abilities[abilityNumber - 1] = ability;
            UIBase.instance.closeSkills();
        }
    }

    public void openSkillTree()
    {
        if (skillTree)
        {
            skillTree.open();
        }
    }

}
