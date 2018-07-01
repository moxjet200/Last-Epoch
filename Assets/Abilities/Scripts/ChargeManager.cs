using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AbilityList))]
public class ChargeManager : MonoBehaviour
{

    //[HideInInspector]
    public List<float> charges = new List<float>();
    //[HideInInspector]
    public List<float> chargeRegen = new List<float>();
    //[HideInInspector]
    public List<float> maxCharges = new List<float>();
    [HideInInspector]
    public List<Ability> abilities = new List<Ability>();

    [HideInInspector]
    public AbilityList abilityList;

    bool putChargesToMax = false;

    public bool abilitiesStartOffCooldown = false;

    // Use this for initialization
    void Awake()
    {
        abilityList = GetComponent<AbilityList>();
        abilities.Clear();
        abilities.AddRange(abilityList.abilities);
        for (int i = 0; i < abilityList.abilities.Count; i++)
        {
            // initialise other lists
            charges.Add(0);
            chargeRegen.Add(0);
            maxCharges.Add(0);
        }
        for (int i = 0; i < abilityList.abilities.Count; i++)
        {
            updateChargeInfo(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // there are more abilities than before update the charges for the new ones
        if (abilities.Count < abilityList.abilities.Count)
        {
            int oldNumberOfabilities = abilities.Count;
            abilities = abilityList.abilities;
            for (int i = oldNumberOfabilities; i < abilityList.abilities.Count; i++)
            {
                // initialise other lists
                charges.Add(0);
                chargeRegen.Add(0);
                maxCharges.Add(0);
                // put correct information in lists
                for (int j = 0; j < abilities.Count; j++)
                {
                    updateChargeInfo(j);
                }
            }
        }
        //update charge info
        for (int i = 0; i < abilities.Count; i++)
        {
            // if an ability is different than before update the charge info for it
            if (abilities[i] != abilityList.abilities[i])
            {
                abilities[i] = abilityList.abilities[i];
                updateChargeInfo(i);
            }
            // increment the number of charges if necessary
            if (maxCharges[i] > 0)
            {
                if (charges[i] < maxCharges[i])
                {
                    charges[i] += chargeRegen[i] * Time.deltaTime;
                    if (charges[i] > maxCharges[i]) { charges[i] = maxCharges[i]; }
                    // update the display
                    updateChargesDisplay(i);
                }
            }
            updateMaxChargesDisplay(i);
            updateChargesDisplay(i);
        }

        if (putChargesToMax)
        {
            putChargesToMax = false;
            privateSetChargesToMax();
        }

    }


    // updates the information about the charges for a specific ability slot
    public void updateChargeInfo(int abilityNumber)
    {
        charges[abilityNumber] = 0;
        chargeRegen[abilityNumber] = abilityList.abilities[abilityNumber].chargesGainedPerSecond;
        maxCharges[abilityNumber] = abilityList.abilities[abilityNumber].maxCharges;
        // apply changes from any relevant mutators
        foreach (AbilityMutator mutator in GetComponents<AbilityMutator>())
        {
            if (mutator.ability == abilities[abilityNumber])
            {
                maxCharges[abilityNumber] = mutator.ability.maxCharges + mutator.addedCharges;
                chargeRegen[abilityNumber] = mutator.ability.chargesGainedPerSecond + mutator.addedChargeRegen;
            }
        }
        updateMaxChargesDisplay(abilityNumber);
        updateChargesDisplay(abilityNumber);
    }

    // a version for an ability rather than an ability number
    public void updateChargeInfo(Ability ability)
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            if (abilities[i] == ability)
            {
                updateChargeInfo(i);
            }
        }
    }

    // gets the number of charges for a given ability (returns 0 if that ability is not in the abilities list)
    public int getCharges(Ability ability)
    {
        int returnCharges = 0;
        for (int i = 0; i < abilities.Count; i++)
        {
            if (ability == abilities[i])
            {
                returnCharges += (int)charges[i];
            }
        }
        return returnCharges;
    }

    // returns whether an ability is in the list and has non-zero max charge
    public bool hasCooldown(Ability ability)
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            if (ability == abilities[i] && maxCharges[i] != 0)
            {
                return true;
            }
        }
        return false;
    }

    // returns whether an ability is on cooldown
    public bool onCoooldown(Ability ability)
    {
        if (!hasCooldown(ability)) { return false; }
        for (int i = 0; i < abilities.Count; i++)
        {
            if (abilities[i] == ability)
            {
                return (charges[i] < 1);
            }
        }
        return false;
    }

    // gets the number of charges for a given ability plus the proportion of the next charge that has already been gained
    public float getChargesFloat(Ability ability)
    {
        float returnCharges = 0;
        for (int i = 0; i < abilities.Count; i++)
        {
            if (ability == abilities[i])
            {
                returnCharges += charges[i];
            }
        }
        return returnCharges;
    }

    // deducts the charge cost for using the given ability
    public void useCharges(Ability ability)
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            if (ability == abilities[i] && maxCharges[i] > 0)
            {
                charges[i] -= 1;
                updateChargesDisplay(i);
            }
        }
    }

    // sets the charge count to maximum for the specified ability
    public void setAbilityChargesToMax(Ability ability)
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            if (ability == abilities[i])
            {
                charges[i] = maxCharges[i];
                updateChargesDisplay(i);
            }
        }
    }

    public void addAbilityCharges(Ability ability, float addedCharges)
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            if (ability == abilities[i])
            {
                charges[i] += addedCharges;
                if (charges[i] > maxCharges[i])
                {
                    charges[i] = maxCharges[i];
                }
                updateChargesDisplay(i);
            }
        }
    }

    // sets all the charge count for all abilities in the abilityList to their maximum charges
    public void setChargesToMax()
    {
        putChargesToMax = true;
    }

    // sets all the charge count for all abilities in the abilityList to their maximum charges
    void privateSetChargesToMax()
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            charges[i] = maxCharges[i];
            updateChargesDisplay(i);
        }
    }

    public virtual void updateChargesDisplay(int abilityNumber)
    {

    }

    public virtual void updateMaxChargesDisplay(int abilityNumber)
    {

    }

    public void recoverPercentageCooldown(Ability ability, float percentage)
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            if (abilities[i] == ability)
            {
                charges[i] += percentage;
                if (charges[i] > maxCharges[i]) { charges[i] = maxCharges[i]; }
            }
        }
    }

    public float getCooldown(Ability ability)
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            if (abilities[i] == ability)
            {
                if (charges[i] == 0) { return 0f; }
                return Mathf.CeilToInt(1 / chargeRegen[i]);
            }
        }
        return 0f;
    }

}
