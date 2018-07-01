using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPanelIconGenerator : MonoBehaviour {

    public GameObject abilityIconPrefab;
    public GameObject lockedAbilityIconPrefab;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnEnable()
    {
		if (PlayerFinder.getPlayer () == null) {
			return;
		}
		List<Ability> abilities = PlayerFinder.getPlayer ().GetComponent<KnownAbilityList> ().abilities;

        // find the list of locked abilities
        List<CharacterClass.AbilityAndLevel> lockedAbilities = new List<CharacterClass.AbilityAndLevel>();
        CharacterData data = PlayerFinder.getPlayer().GetComponent<CharacterDataTracker>().charData;
        if (data !=null && data.characterClass != null) {
            foreach (CharacterClass.AbilityAndLevel aal in data.characterClass.unlockableAbilities)
            {
                if (!abilities.Contains(aal.ability))
                {
                    lockedAbilities.Add(aal);
                }
            }
        }

        lockedAbilities.Sort((la1, la2) => la1.level.CompareTo(la2.level));

        // instantiate an ability icon for each ability in the player's known ability list
        foreach(Ability abilitiy in abilities)
        {
            GameObject abilityIcon = Instantiate(abilityIconPrefab, transform);
            abilityIcon.GetComponent<AbilityPanelIcon>().ability = abilitiy;
        }

        foreach (CharacterClass.AbilityAndLevel aal in lockedAbilities)
        {
            GameObject abilityIcon = Instantiate(lockedAbilityIconPrefab, transform);
            abilityIcon.GetComponent<LockedAbilityPanelIcon>().ability = aal.ability;
            abilityIcon.GetComponent<LockedAbilityPanelIcon>().levelRequirement = aal.level;
        }
    }
		
    void OnDisable()
    {
        // remove the instantiated ability icons
        List<Transform> children = new List<Transform>();
        foreach(Transform child in transform)
        {
            children.Add(child);
        }
        for (int i = 0; i<transform.childCount; i++)
        {
            Destroy(children[i].gameObject);
        }
    }
}
