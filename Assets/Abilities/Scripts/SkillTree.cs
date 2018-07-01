using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillTree : Tree {

    public static List<SkillTree> all = new List<SkillTree>();

    public Ability ability;

    public Transform skillTreePanel;
	public List<SkillTreeConnection> connections = new List<SkillTreeConnection>();
    public bool specialised;

    public void Awake()
    {
        all.Add(this);
        setAbility();
        treeID = ability.playerAbilityID;
        if (treeID.Length > 6) { Debug.LogError(name + " treeID is " + treeID + " which is longer than 6 characters!"); }
    }

    public void OnDestroy() {
        all.Remove(this);
    }

    public abstract void setAbility();

	public void UpdateNodeConnections(){
		foreach (SkillTreeConnection connection in connections) {
			connection.OnTreeOpen ();
		}
	}

    public void open()
    {
        UIBase.instance.openSkillTrees.Add(this);
        // switch on the nodes
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        // switch on the skill tree panel
        foreach (Transform child in skillTreePanel)
        {
            child.gameObject.SetActive(true);
        }

		connections.Clear ();
		connections.AddRange( GetComponentsInChildren<SkillTreeConnection> ());
		UpdateNodeConnections ();
        UITreeSkillName.setText(ability.abilityName);
        UITreeSkillLevel.updateForAbility(ability);
        UITreeSkillDescription.setText(ability.description);

        // check whether this skill is specialised
        specialised = false;
        GameObject player = PlayerFinder.getPlayer();
        if (player.GetComponent<SpecialisedAbilityList>()) {
            if (player.GetComponent<SpecialisedAbilityList>().abilities.Contains(ability)){
                specialised = true;
            }
        }
        // change buttons depending on whether this skill is specialised
        foreach (Transform t1 in skillTreePanel)
        {
            if (t1.name == "Panel")
            {
                foreach (Transform t2 in t1)
                {
                    if (t2.name == "Buttons")
                    {
                        foreach (Transform child in t2)
                        {
                            // activate the specialise button if this skill is not specialised and there is a free slot
                            if (child.name == "specialiseButton" && player.GetComponent<SpecialisedAbilityList>().isThereAFreeSlot() && !player.GetComponent<SpecialisedAbilityList>().abilities.Contains(ability)) {
                                child.gameObject.SetActive(true);
                            }
                            else { child.gameObject.SetActive(false); }
                            // activate the other footer buttons if this skill is specialised
                            if (child.name == "despecializeButton") { child.gameObject.SetActive(specialised); }
                            // TODO change from 'false' to 'specialised' and make these buttons functional
                            if (child.name == "confirmAllocationButton") { child.gameObject.SetActive(false); }
                            if (child.name == "cancelAllocationButton") { child.gameObject.SetActive(false); }
                        }
                    }
                }
            }
        }
    }

    public void close()
    {
        // switch off the nodes
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        // switch off the skill tree panel
        foreach (Transform child in skillTreePanel)
        {
            child.gameObject.SetActive(false);
        }
    }
    

}
