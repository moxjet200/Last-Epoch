using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialisedAbilityList : MonoBehaviour {

    public List<Ability> abilities = new List<Ability>();

    public List<int> abilityLevels = new List<int>();

    public List<float> abilityXP = new List<float>();

    public int numberOfUnlockedSlots = 0;

    public delegate void AbilitiesChangedAction(int abilityNumber);
    public AbilitiesChangedAction abilityChangedEvent;

    LevelUpSkillsButton levelUpButton = null;

	// Use this for initialization
	void Awake () {
        while (abilities.Count < 5)
        {
            abilities.Add(null);
            abilityLevels.Add(0);
            abilityXP.Add(0);
        }
        // get a reference to the level up skills button
        levelUpButton = FindObjectOfType<LevelUpSkillsButton>();
        // set the number of unlocked slots
        numberOfUnlockedSlots = SpecialisedAbilityManager.getNumberOfSpecialisationSlots(GetComponent<CharacterStats>().level);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // specialise in an ability for a specific slot
    public void Specialise(Ability ability, int slot, bool save = true)
    {
        string oldID = "";
        if (save && abilities[slot] != null) { oldID = abilities[slot].playerAbilityID; }
        abilities[slot] = ability;
        abilityLevels[slot] = 1;
        abilityXP[slot] = 0f;
        if (abilityChangedEvent != null) { abilityChangedEvent.Invoke(slot); }
        if (save) {
            SkillSavingManager.instance.changeSpecialisation(ability.playerAbilityID, slot, oldID);
            PlayerFinder.getPlayer().GetComponent<CharacterDataTracker>().SaveCharacterData();
        }
    }

    // specialise in an ability in the earliest available slot, returns false if there are no free slots
    public bool SpecialiseIfThereIsAFreeSlot(Ability ability)
    {
        // try to find an unusued slot
        for (int i = 0; i < abilities.Count; i++)
        {
            if (i < numberOfUnlockedSlots && abilities[i] == null) { Specialise(ability, i); return true; }
        }
        if (abilities.Count < numberOfUnlockedSlots) { Specialise(ability, abilities.Count); return true; }
        // if unsuccessful return false
        return false;
    }

    public bool isThereAFreeSlot()
    {
        // try to find an unusued slot
        for (int i = 0; i < abilities.Count; i++)
        {
            if (i < numberOfUnlockedSlots && abilities[i] == null) { return true; }
        }
        if (abilities.Count < numberOfUnlockedSlots) { return true; }
        // if unsuccessful return false
        return false;
    }

    // returns true if all the available specialisation slots are used
    public bool AllSlotsUsed()
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            if (abilities[i] == null) { return false; }
        }
        if (abilities.Count < numberOfUnlockedSlots) { return false; }
        return true;
    }

    public void ApplyAbilityXp(float xp)
    {
        for (int i = 0; i< abilities.Count; i++)
        {
            if (abilities[i])
            {
                // add the xp
                abilityXP[i] += xp;
                // check if any abilities have levelled up
                bool levelledAbility = true;
                int j = 0;

                while (j < 20 && levelledAbility)
                {
                    j++;
                    levelledAbility = false;
                    while (SpecialisedAbilityManager.getAbilityLevel(abilityXP[i]) > abilityLevels[i])
                    {
                        levelUpAbility(i);
                        levelledAbility = true;
                    }
                }
            }
        }
    }

    public void DespecialiseAbility(Ability ability)
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            if (abilities[i] == ability)
            {
                Ability abilityToDespecialise = abilities[i];
                // enact the despecialisation
                abilities[i] = null;
                abilityXP[i] = 0;
                abilityLevels[i] = 1;
                // Save the despecialisation
                SkillSavingManager.instance.despecialise(abilityToDespecialise.playerAbilityID);
                // invoke the event
                if (abilityChangedEvent != null) { abilityChangedEvent.Invoke(i); }
            }
        }
    }

    public void DespecialiseAbility(int abilityNumber)
    {
        if (abilityNumber >= 0 && abilityNumber < abilities.Count) { DespecialiseAbility(abilities[abilityNumber]); }
    }

    public void levelUpAbility(int abilityNumber)
    {
        abilityLevels[abilityNumber]++;
        bool skillPanelOpen = UIBase.instance.skillsOpen;
        if (!skillPanelOpen)
        {
            UIBase.instance.openSkills();
        }
        foreach (SkillTree skillTree in SkillTree.all)
        {
            if (skillTree.ability == abilities[abilityNumber])
            {
                skillTree.unspentPoints++;
                SkillSavingManager.instance.saveUnspentPoints(skillTree);
            }
        }
        if (!skillPanelOpen)
        {
            UIBase.instance.closeSkills();
        }
        // activate the level up button if this is the local player
        if (gameObject == PlayerFinder.getPlayer())
        levelUpButton.addAbilityButton(abilities[abilityNumber]);
    }

    public int getAbilityLevel(Ability _ability)
    {
        for (int i = 0; i< abilities.Count; i++)
        {
            if (abilities[i] == _ability)
            {
                return abilityLevels[i];
            }
        }
        return 0;
    }

}
