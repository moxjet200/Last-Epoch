using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityList : MonoBehaviour {

    public List<Ability> abilities = new List<Ability>();

    public void AddAbilityToStart(Ability ability)
    {
        List<Ability> newList = new List<Ability>();
        newList.Add(ability);
        newList.AddRange(abilities);
        abilities = newList;
    }

}
