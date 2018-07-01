using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnspecialisableIconGenerator : MonoBehaviour
{

    public GameObject abilityIconPrefab;

    void OnEnable()
    {
        // get a list of the player's known abilities and specialised abilities
        GameObject player = PlayerFinder.getPlayer();
        if (player != null)
        {
            List<Ability> playerAbilities = player.GetComponent<KnownAbilityList>().abilities;

            // get a list of the abilities with skill trees
            List<Ability> abilitiesWithTrees = new List<Ability>();

            foreach (SkillTree skillTree in SkillTree.all)
            {
                if (skillTree.ability != null && !abilitiesWithTrees.Contains(skillTree.ability))
                {
                    abilitiesWithTrees.Add(skillTree.ability);
                }
            }

            List<Ability> unspecialisableAbilities = new List<Ability>();

            foreach (Ability ability in playerAbilities)
            {
                if (!abilitiesWithTrees.Contains(ability))
                {
                    unspecialisableAbilities.Add(ability);
                }
            }

            Ability basicMeleeAttack = AbilityIDList.getAbility(AbilityID.basicMeleeAttack);
            if (unspecialisableAbilities.Contains(basicMeleeAttack))
            {
                unspecialisableAbilities.Remove(basicMeleeAttack);
            }

            // instantiate an ability icon for each ability in the player's known ability list that is also in the abilities with trees list and is not already specialised
            foreach (Ability abilitiy in unspecialisableAbilities)
            {
                GameObject abilityIcon = Instantiate(abilityIconPrefab, transform);
                abilityIcon.GetComponent<VisualAbilityIcon>().ability = abilitiy;
            }
        }
    }

    void OnDisable()
    {
        // remove the instantiated ability icons
        List<Transform> children = new List<Transform>();
        foreach (Transform child in transform)
        {
            children.Add(child);
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(children[i].gameObject);
        }
    }
}
