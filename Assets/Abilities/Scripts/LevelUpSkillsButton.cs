using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpSkillsButton : MonoBehaviour {
    
    public GameObject levelSkillIconPrefab = null;
    Transform panel = null;
    List<Ability> abilitiesWithLevelUpButtons = new List<Ability>();
    List<GameObject> extraButtons = new List<GameObject>();
    GameObject newUnlockButton = null;

    void Start()
    {
        foreach(Transform t1 in transform)
        {
            foreach (Transform t2 in t1)
            {
                if (t2.name == "Panel")
                {
                    panel = t2;
                }
            }
        }
        deactivate();
    }


    public void clicked(Ability ability = null)
    {
        UIBase ui = UIBase.instance;
        ui.openSkills();
        AbilityPanelManager.instance.abilityClicked = 0;
        // open the relevant tree
        if (ability != null) {
            foreach (SkillTree tree in SkillTree.all)
            {
                if (tree.ability == ability)
                {
                    tree.open();
                }
            }
        }

        // play a sound
        UISounds.playSound(UISounds.UISoundLabel.Confirm);

        deactivate();
    }

    void Update()
    {
        if (UIBase.instance.skillsOpen) { deactivate(); }
    }

    public void activate()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void deactivate()
    {
        for (int i = 0; i< extraButtons.Count; i++)
        {
            Destroy(extraButtons[i]);
        }
        extraButtons.Clear();
        abilitiesWithLevelUpButtons.Clear();
        if (newUnlockButton) { Destroy(newUnlockButton); }

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void addAbilityButton(Ability ability)
    {
        activate();
        // do not create a duplicate icon
        if (!abilitiesWithLevelUpButtons.Contains(ability))
        {
            GameObject newSkillIcon = Instantiate(levelSkillIconPrefab, panel);
            newSkillIcon.GetComponent<LevelUpSkillsButtonButton>().ability = ability;
            newSkillIcon.GetComponent<Image>().sprite = ability.abilitySprite;
            abilitiesWithLevelUpButtons.Add(ability);
            extraButtons.Add(newSkillIcon);
        }
    }

    public void addNewUnlockButton()
    {
        activate();
        if (!newUnlockButton)
        {
            newUnlockButton = Instantiate(levelSkillIconPrefab, panel);
        }
    }

}
