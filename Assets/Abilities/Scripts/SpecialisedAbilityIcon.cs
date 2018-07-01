using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpecialisedAbilityIcon : MonoBehaviour {

    public static List<SpecialisedAbilityIcon> all = new List<SpecialisedAbilityIcon>();

    // the list of the player's specialised abilities
    SpecialisedAbilityList list = null;

    public bool locked = true;

    public int abilityNumber = 0;

    public Ability ability = null;
    public SkillTree skillTree;
    Image icon;

    Sprite openSlotSprite = null;

    GameObject lockedIcon = null;
    TextMeshProUGUI availablePoints = null;

    GameObject emptyAvailable = null;

    Text levelText = null;

    // Use this for initialization
    void Awake () {
        // get a reference to the player
        GetListReference();
        // get references to children to activate or deactivate
        foreach (Transform child in transform)
        {
            if (child.name == "skillIcon")
            {
                icon = child.GetComponent<Image>();
                openSlotSprite = icon.sprite;
            }
            if (child.name == "LevelLocked")
            {
                lockedIcon = child.gameObject;
            }
            if (child.name == "EmptyAvailable")
            {
                emptyAvailable = child.gameObject;
            }
            if (child.name == "SkillPoints")
            {
                availablePoints = child.GetComponentInChildren<TextMeshProUGUI>();
            }
            if (child.name == "Text")
            {
                levelText = child.GetComponent<Text>();
            }
        }
        all.Add(this);
	}
	
    public void GetListReference()
    {
        // get a reference to the player's specialised ability list
        if (!list)
        {
			if (PlayerFinder.getPlayer () == null) {
				return;
			}
			list = PlayerFinder.getPlayer().GetComponent<SpecialisedAbilityList>();
            if (list.abilities.Count > abilityNumber && list.abilities[abilityNumber] != null)
            {
                ability = list.abilities[abilityNumber];
                icon.sprite = ability.abilitySprite;
                findSkillTree();
            }
            list.abilityChangedEvent += checkIfAbilityChanged;
        }
    }

    void OnDestroy()
    {
        all.Remove(this);
        if (list)
        {
            list.abilityChangedEvent -= checkIfAbilityChanged;
        }
    }

    // Update is called once per frame
    void Update () {

        GetListReference();

        // work out whether this slot is unlocked
        if (ability == null && locked && list)
        {
            if (list.numberOfUnlockedSlots > abilityNumber)
            {
                locked = false;
            }
        }

        if (locked && ability != null) { locked = false; }

        // update the locked state
        if (locked) {
            lockedIcon.SetActive(true);
            emptyAvailable.SetActive(false);
            availablePoints.transform.parent.gameObject.SetActive(false);
            levelText.gameObject.SetActive(false);
        }
        else { lockedIcon.SetActive(false);
            // if there's no ability activate the empty available and disable the level text
            if (ability)
            {
                emptyAvailable.SetActive(false);
                // update the level text
                levelText.gameObject.SetActive(true);
                levelText.text = "level " + list.abilityLevels[abilityNumber];
            }
            else
            {
                emptyAvailable.SetActive(true);
                levelText.gameObject.SetActive(false);
            }
            // update the number of available points
            if (skillTree && skillTree.unspentPoints > 0)
            {
                availablePoints.transform.parent.gameObject.SetActive(true);
                availablePoints.text = "" + skillTree.unspentPoints;
            }
            else
            {
                availablePoints.transform.parent.gameObject.SetActive(false);
            }
        }
	}
    
    // finds the skill tree for the current ability
    void findSkillTree()
    {
        foreach (SkillTree tree in SkillTree.all)
        {
            if (tree.ability == ability)
            {
                skillTree = tree;
            }
        }
    }

    // called when a specialised skill is changed
    public void checkIfAbilityChanged(int abilitySlotChanged)
    {
        if (abilitySlotChanged == abilityNumber && list.abilities.Count > abilityNumber)
        {
            if (list.abilities[abilityNumber] != null)
            {
                ability = list.abilities[abilityNumber];
                icon.sprite = ability.abilitySprite;
                findSkillTree();
                locked = false;
            }
            else
            {
                ability = null;
                icon.sprite = openSlotSprite;
                skillTree = null;
            }
        }
    }

    public void clicked()
    {
        if (AbilityDragHandler.dragging) { return; }

        if (ability)
        {
            openSkillTree();
            //int abilityNumber = AbilityPanelManager.instance.abilityClicked;
            //// abilityNumber is 0 if this was opened with the S key
            //if (abilityNumber == 0) { openSkillTree(); }
            //// otherise it was opened by clicking on an ability in the ability bar
            //else
            //{
            //    list.GetComponent<AbilityList>().abilities[abilityNumber - 1] = ability;
            //    UIBase.instance.closeSkills();
            //}
        }
        else
        {
            UIBase.instance.closeSkillTrees();
            UIBase.instance.closeSpecialisationSelection();
            openSkillSelection();
        }

        // play a sound
        UISounds.playSound(UISounds.UISoundLabel.Confirm);
    }

    public void openSkillTree()
    {
        if (skillTree)
        {
            UIBase.instance.closeSkillTrees();
            skillTree.open();
        }
    }

    public void openSkillSelection()
    {
        // TODO code for choosing a skill to specialise
        UIBase uiBase = UIBase.instance;
        uiBase.openSpecialisationSelection();
        uiBase.specialisationSelection.GetComponent<SpecialisationPanelManager>().slotClicked = abilityNumber;
    }

}
