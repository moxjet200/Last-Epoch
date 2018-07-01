using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpSkillsButtonButton : MonoBehaviour {

    public Ability ability = null;
    LevelUpSkillsButton levelUpSkillsButton = null;

	// Use this for initialization
	void Start () {
        levelUpSkillsButton = FindObjectOfType<LevelUpSkillsButton>();
	}
	
	public void clicked()
    {
        levelUpSkillsButton.clicked(ability);
    }
}
