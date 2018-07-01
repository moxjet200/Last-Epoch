using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class SkillTreeUnspentPointsText : MonoBehaviour {

    public SkillTree skillTree;

    Text text;

    bool specialised;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
	}
	
    void OnEnable()
    {
        if (skillTree && PlayerFinder.getPlayer() && PlayerFinder.getPlayer().GetComponent<SpecialisedAbilityList>() && PlayerFinder.getPlayer().GetComponent<SpecialisedAbilityList>().abilities.Contains(skillTree.ability))
        {
            specialised = true;
        }
        else
        {
            specialised = false;
        }
    }

	// Update is called once per frame
	void Update () {
        if (specialised)
        {
            text.text = skillTree.unspentPoints + " unspent points";
        }
        else
        {
            text.text = "";
        }
	}
}
