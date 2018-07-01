using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(AbilityTooltip))]
public class VisualAbilityIcon : MonoBehaviour
{
    Image icon;
    public Ability ability;
    private AbilityTooltip tooltipAbility;

    // Use this for initialization
    void Start()
    {
        tooltipAbility = gameObject.GetComponent<AbilityTooltip>();
        foreach (Transform child in transform)
        {
            if (child.name == "Sprite" && child.GetComponent<Image>())
            {
                icon = child.GetComponent<Image>();
                icon.sprite = ability.abilitySprite;
            }
            else if (child.name == "Frame") { }
            else if (child.name == "SkillName")
            {
                child.GetComponent<TextMeshProUGUI>().text = ability.abilityName;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (ability != null)
        { //TODO probably move for performance
            tooltipAbility.SetAbility(ability);
        }
    }

}
