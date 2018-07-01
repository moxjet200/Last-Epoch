using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(AbilityTooltip))]
public class SpecialisationSelectionIcon : MonoBehaviour {

    GameObject player;
    Image icon;
    public Ability ability;
	private AbilityTooltip tooltipAbility;

    public SpecialisationPanelManager specialisationPanelmanager;

    // Use this for initialization
    void Start () {
        specialisationPanelmanager = SpecialisationPanelManager.instance;
        tooltipAbility = gameObject.GetComponent<AbilityTooltip>();
        player = PlayerFinder.getPlayer();
        PlayerFinder.playerChangedEvent += ChangePlayer;
        foreach (Transform child in transform)
        {
            if (child.name == "Sprite" && child.GetComponent<Image>())
            {
                icon = child.GetComponent<Image>();
                icon.sprite = ability.abilitySprite;
            }
            else if (child.name == "Frame") { }
            else if (child.name == "SkillName")
            {
                child.GetComponent<TextMeshProUGUI>().text = ability.abilityName;
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
    }

    public void clicked()
    {
        // specialise in the ability
        if (specialisationPanelmanager)
        {
            player.GetComponent<SpecialisedAbilityList>().Specialise(ability, specialisationPanelmanager.slotClicked);
        }
        // close the selection panel
        UIBase.instance.closeSpecialisationSelection();
        // open the relevant skill tree
        foreach(SkillTree skillTree in SkillTree.all)
        {
            if (skillTree.ability == ability) { skillTree.open(); }
        }
        // play a sound
        UISounds.playSound(UISounds.UISoundLabel.Specialise);
    }

}
