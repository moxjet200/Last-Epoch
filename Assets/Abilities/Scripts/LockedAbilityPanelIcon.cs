using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(AbilityTooltip))]
public class LockedAbilityPanelIcon : MonoBehaviour {

    GameObject player;
    Image icon;
    public Ability ability;
    public int levelRequirement = 0;
	private AbilityTooltip tooltipAbility;

    // Use this for initialization
    void Start () {
		tooltipAbility = gameObject.GetComponent<AbilityTooltip> ();
        player = PlayerFinder.getPlayer();
        PlayerFinder.playerChangedEvent += ChangePlayer;
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
                else if (child.name == "LevelRequirement")
                {
                    child.GetComponent<TextMeshProUGUI>().text = "" + levelRequirement;
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
    }

    public void clicked()
    {
        
    }

}
