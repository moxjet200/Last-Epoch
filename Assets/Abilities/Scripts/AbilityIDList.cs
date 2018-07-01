using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityIDList : MonoBehaviour {
    
    [System.Serializable]
    public struct AbilityIDAndAbility
    {
        public AbilityID id;
        public Ability ability;
    }

    public static AbilityIDList instance;

    public List<AbilityIDAndAbility> list;
    public List<Ability> playerAbilities;
    
    public static Ability getAbility(AbilityID id)
    {
        if (instance == null) { instance = FindObjectOfType<AbilityIDList>(); }
        foreach (AbilityIDAndAbility item in instance.list)
        {
            if (item.id == id) { return item.ability; }
        }
        Debug.LogError("Tried to get ability with id " + id + " but there is no ability with that id in the list.");
        return null;
    }

    void Start()
    {
        // check that no id features more than once in the list
        List<AbilityID> idList = new List<AbilityID>();
        foreach(AbilityIDAndAbility item in list)
        {
            if (!idList.Contains(item.id)) { idList.Add(item.id); }
            else { Debug.LogError("There are two entries in the Ability ID list for the id " + item.id); }
        }
    }

    public static Ability getPlayerAbility(string id)
    {
        foreach (Ability ability in instance.playerAbilities)
        {
            // change for ability.playerAbilityID
            if (ability.playerAbilityID == id) { return ability; }
        }
        return getAbility(AbilityID.nullAbility);
    }


}
