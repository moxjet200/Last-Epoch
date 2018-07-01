using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilityDragHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {

    public static bool dragging = false;

	//public static GameObject abilityBeingDragged;
	private GameObject player;
	private int abilityNum;
	private Ability ability;
	private GameObject OnTop;
	private Vector3 startPos;
    private bool draggingThis = false;
    private bool safeToClearVariables = false;
    private bool specialisedIconHandler = false;

	public void Awake(){
		OnTop = GameObject.FindGameObjectWithTag ("OnTop");
        specialisedIconHandler = (GetComponent<SpecialisedAbilityIcon>() != null);
    }

	#region IBeginDragHandler implementation

	public void OnBeginDrag (PointerEventData eventData) {
		startPos = transform.position;
        dragging = true;
        draggingThis = true;
        safeToClearVariables = false;
    }

	#endregion

	#region IDragHandler implementation
	public void OnDrag (PointerEventData eventData)	{
		transform.position = Input.mousePosition + new Vector3(-16.0f, 16.0f);
	}
	#endregion

	#region IEndDragHandler implementation

	public void OnEndDrag (PointerEventData eventData) {
        safeToClearVariables = true;
        StartCoroutine(clearDragVariables());

        // find the ability
        player = PlayerFinder.getPlayer();
        if (GetComponent<AbilityPanelIcon>()) {
            ability = GetComponent<AbilityPanelIcon>().ability;
        }
        else if (GetComponent<SpecialisedAbilityIcon>() && player.GetComponent<SpecialisedAbilityList>().abilities.Count > GetComponent<SpecialisedAbilityIcon>().abilityNumber &&
            player.GetComponent<SpecialisedAbilityList>().abilities[GetComponent<SpecialisedAbilityIcon>().abilityNumber] != null)
        {
            ability = player.GetComponent<SpecialisedAbilityList>().abilities[GetComponent<SpecialisedAbilityIcon>().abilityNumber];
        }

        // if the ability is null just return
        if (ability == null) {
            transform.position = startPos;
            return;
        }

        // try to apply to an ability bar icon
        AbilityBarIcon[] abilityIcons = AbilityBarIcon.all.ToArray();
		if (abilityIcons != null) {
			AbilityBarIcon selectedIcon = abilityIcons [0];
			foreach (AbilityBarIcon a in abilityIcons) {
				if (Mathf.Abs (a.transform.position.x - eventData.position.x) < Mathf.Abs (selectedIcon.transform.position.x - eventData.position.x)) {
					selectedIcon = a;
				}
			}
			if (Mathf.Abs (selectedIcon.transform.position.x - eventData.position.x) < 30.0f) {
				abilityNum = selectedIcon.abilityNumber;
                player.GetComponent<AbilityList> ().abilities [abilityNum - 1] = ability;

                // play a sound
                UISounds.playSound(UISounds.UISoundLabel.Confirm);
            }
		}

        // try to apply to a specialisation slot
        if (!specialisedIconHandler)
        {
            // check if this ability has a tree
            bool abilityHasTree = false;
            foreach (SkillTree tree in SkillTree.all)
            {
                if (tree.ability == ability)
                {
                    abilityHasTree = true;
                }
            }

            if (abilityHasTree && !player.GetComponent<SpecialisedAbilityList>().abilities.Contains(ability))
            {
                // only look at open specialisation slots
                List<SpecialisedAbilityIcon> specialisedIcons = new List<SpecialisedAbilityIcon>();
                specialisedIcons.AddRange(SpecialisedAbilityIcon.all);
                specialisedIcons.RemoveAll(x => x.ability != null);
                specialisedIcons.RemoveAll(x => x.locked == true);

                if (specialisedIcons.Count > 0)
                {
                    SpecialisedAbilityIcon selectedIcon = specialisedIcons[0];
                    foreach (SpecialisedAbilityIcon icon in specialisedIcons)
                    {
                        if (Mathf.Abs(icon.transform.position.x - eventData.position.x) < Mathf.Abs(selectedIcon.transform.position.x - eventData.position.x))
                        {
                            selectedIcon = icon;
                        }
                    }

                    if (Mathf.Abs(selectedIcon.transform.position.x - eventData.position.x) < 30.0f)
                    {
                        player.GetComponent<SpecialisedAbilityList>().Specialise(ability, selectedIcon.abilityNumber);
                        // close the selection panel
                        UIBase.instance.closeSpecialisationSelection();
                        // open the relevant skill tree
                        foreach (SkillTree skillTree in SkillTree.all)
                        {
                            if (skillTree.ability == ability) { skillTree.open(); }
                        }
                        // play a sound
                        UISounds.playSound(UISounds.UISoundLabel.Specialise);
                    }

                }
            }
        }





		//alternate way of doing this, probably better if to find what is under the curser than the above functional way
		//this is dependant on what order objects are in, in the hierarchy
		/*
		foreach (GameObject g in eventData.hovered) {
			if (g.transform.GetComponent<AbilityBarIcon> () != null) {
				player = GameObject.FindGameObjectWithTag ("Player");
				abilityNum = g.transform.GetComponent<AbilityBarIcon> ().abilityNumber;
				ability = GetComponent<AbilityPanelIcon> ().ability;
				player.GetComponent<AbilityList>().abilities[abilityNum - 1] = ability;
				break;
			}
		}
		*/
		transform.position = startPos;
    }

    IEnumerator clearDragVariables()
    {
        for (int i = 0; i < 2; i++)
        {
            if (i >=1)
            {
                dragging = false;
                draggingThis = false;
            }
            yield return null;
        }
    }

    #endregion

    void OnDestroy()
    {
        if (draggingThis) { dragging = false; }
    }

}
