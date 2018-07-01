using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityChargeContainer : MonoBehaviour {

    public static List<AbilityChargeContainer> all = new List<AbilityChargeContainer>();

    // AbilityNumbers are 1 to 5
    public int abilityNumber = 1;

    public GameObject chargePrefab = null;

    public List<AbilityCharge> charges = new List<AbilityCharge>();

    public int numberOfCharges = 0;
    public int numberOfMaxCharges = 0;


    void Awake()
    {
        all.Add(this);
    }

    void OnDestroy()
    {
        all.Remove(this);
    }

    public void updateCharges(int newNumber)
    {
        // if the new number is higher activate some charges
        if (newNumber > numberOfCharges)
        {
            for (int i = numberOfCharges; i < newNumber; i++)
            {
                attemptChargeActivation(i);
            }
        }
        // if the new number is lower deactivate some charges
        else if (newNumber < numberOfCharges)
        {
            for (int i = numberOfCharges - 1; i >= newNumber; i--)
            {
                attemptChargeDeactivation(i);
            }
        }

        // update number of charges
        numberOfCharges = newNumber;
    }


    public void updateMaxCharges(int newNumber)
    {
        // if the new number is higher add some charges
        if (newNumber > numberOfMaxCharges)
        {
            for (int i = numberOfMaxCharges; i < newNumber; i++)
            {
                addCharge();
            }
        }
        // if the new number is lower remove some charges
        else if (newNumber < numberOfMaxCharges)
        {
            for (int i = numberOfMaxCharges - 1; i >= newNumber; i--)
            {
                attemptChargeRemoval(i);
            }
        }

        // update number of charges
        numberOfMaxCharges = newNumber;
    }

    public void attemptChargeActivation(int chargeNumber)
    {
        if (chargeNumber >= 0 && chargeNumber < charges.Count) { charges[chargeNumber].activate(); }
    }

    public void attemptChargeDeactivation(int chargeNumber)
    {
        if (chargeNumber >= 0 && chargeNumber < charges.Count) { charges[chargeNumber].deactivate(); }
    }

    public void attemptChargeRemoval(int chargeNumber)
    {
        if (chargeNumber >= 0 && chargeNumber < charges.Count) {
            AbilityCharge chargeToRemove = charges[chargeNumber];
            charges.Remove(chargeToRemove);
            Destroy(chargeToRemove.gameObject);
        }
    }

    public void addCharge()
    {
        GameObject newChargeObject = Instantiate(chargePrefab, transform);
        AbilityCharge newCharge = newChargeObject.GetComponent<AbilityCharge>();
        charges.Add(newCharge);
        newCharge.deactivate();
        numberOfMaxCharges++;
    }

}
