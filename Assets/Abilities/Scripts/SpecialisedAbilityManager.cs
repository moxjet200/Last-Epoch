using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialisedAbilityManager : MonoBehaviour {

    public static SpecialisedAbilityManager instance;

    public List<float> slotUnlockLevels = new List<float>();

	// Use this for initialization
	void Start () {
        SpecialisedAbilityManager.instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static int getAbilityLevel(float xp)
    {
        return Mathf.Min(20,1 + (int) Mathf.Floor(Mathf.Pow(xp*0.00015f,0.63f) + Mathf.Pow(xp * 0.0003f, 0.37f)));
    }

    public static int getNumberOfSpecialisationSlots(int characterLevel)
    {
        int numberOfAbilitySlots = 0;
        if (characterLevel >= 4) { numberOfAbilitySlots++; }
        if (characterLevel >= 9) { numberOfAbilitySlots++; }
        if (characterLevel >= 19) { numberOfAbilitySlots++; }
        if (characterLevel >= 34) { numberOfAbilitySlots++; }
        if (characterLevel >= 49) { numberOfAbilitySlots++; }
        return numberOfAbilitySlots;
    }


}
