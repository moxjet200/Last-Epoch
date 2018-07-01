using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AbilityList))]
public class PlayerChargeManager : ChargeManager {
    [HideInInspector]
    public List<AbilityBarIcon> icons = new List<AbilityBarIcon>();
    [HideInInspector]
    public List<Text> texts = new List<Text>();

    public List<AbilityChargeContainer> containers = new List<AbilityChargeContainer>();

    // Use this for initialization
    void Awake () {
        abilityList = GetComponent<AbilityList>();
        abilities.Clear();
        abilities.AddRange(abilityList.abilities);
        for (int i = 0; i < 5; i++)
        {
            // initialise other lists
            charges.Add(0);
            chargeRegen.Add(0);
            maxCharges.Add(0);
        }
        //get references to the ability bar icons and text
        for (int i = 0; i< 5; i++)
        {
            icons.Add(null);
            texts.Add(null);
            containers.Add(null);
        }
        foreach (AbilityBarIcon icon in FindObjectsOfType<AbilityBarIcon>())
        {
            icons[icon.abilityNumber-1] = icon;
            // text display
            texts[icon.abilityNumber-1] = icon.GetComponentInChildren<Text>();
        }
        foreach (AbilityChargeContainer container in FindObjectsOfType<AbilityChargeContainer>())
        {
            containers[container.abilityNumber - 1] = container;
        }
        // put correct information in lists
        for (int i = 0; i < 5; i++)
        {
            updateChargeInfo(i);
            updateMaxChargesDisplay(i);
            updateChargesDisplay(i);
        }
    }

    public override void updateChargesDisplay(int abilityNumber)
    {
        float _charges = charges[abilityNumber];
        // update the charges
        containers[abilityNumber].updateCharges((int)charges[abilityNumber]);
        // update the cooldown bar
        if (_charges > 0 && _charges <= 1)
        {
            icons[abilityNumber].cooldownBar.fillAmount = 1 - _charges;
        }
        else if (icons[abilityNumber].cooldownBar)
        {
            icons[abilityNumber].cooldownBar.fillAmount = 0;
        }
        // update tooltips
        AbilityTooltip.updateAllManaCosts();
    }

    public override void updateMaxChargesDisplay(int abilityNumber)
    {
        float _maxCharges = maxCharges[abilityNumber];
        // update the cooldown bar
        if (_maxCharges > 0)
        {
            icons[abilityNumber].activateCooldownBar();
        }
        else
        {
            icons[abilityNumber].deactivateCooldownBar();
        }
        // update the charges
        containers[abilityNumber].updateMaxCharges((int)maxCharges[abilityNumber]);
    }

}
