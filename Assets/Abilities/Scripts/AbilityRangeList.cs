using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AbilityList))]
public class AbilityRangeList : MonoBehaviour {
    
    [System.Serializable]
    public struct AbilityRanges
    {
        public float minRange;
        public float engageRange;
        public float maxRange;
        public float persuitRange;

        public AbilityRanges(float _persuitRange, float _maxRange, float _engageRange, float _minRange)
        {
            persuitRange = _persuitRange;
            maxRange = _maxRange;
            engageRange = _engageRange;
            minRange = _minRange;
        }
    }

    [System.Serializable]
    public struct HealthThresholds
    {
        public float maxHealthThreshold;
        public float minHealthThreshold;
    }

    public List<AbilityRanges> ranges = new List<AbilityRanges>();

    public bool useHealthThresholds = false;

    public List<HealthThresholds> healthThresholds = new List<HealthThresholds>();


    AbilityList abilityList;

    // Use this for initialization
    void Awake () {
        abilityList = GetComponent<AbilityList>();
		if (ranges.Count != abilityList.abilities.Count)
        {
            Debug.LogError(name + "'s ability list is a different length to its ability range list");
        }
	}
	
    public float getEngageRange(Ability ability)
    {
        for (int i = 0; i < abilityList.abilities.Count ; i++)
        {
            if (abilityList.abilities[i] == ability)
            {
                return ranges[i].engageRange;
            }
        }
        Debug.LogError(name + " does not have " + ability.abilityName + " in its ability list.");
        return 0;
    }

    public float getMinRange(Ability ability)
    {
        for (int i = 0; i < abilityList.abilities.Count; i++)
        {
            if (abilityList.abilities[i] == ability)
            {
                return ranges[i].minRange;
            }
        }
        Debug.LogError(name + " does not have " + ability.abilityName + " in its ability list.");
        return 0;
    }

    public float getMaxRange(Ability ability)
    {
        for (int i = 0; i < abilityList.abilities.Count; i++)
        {
            if (abilityList.abilities[i] == ability)
            {
                return ranges[i].maxRange;
            }
        }
        Debug.LogError(name + " does not have " + ability.abilityName + " in its ability list.");
        return 0;
    }
    
    public float getPersuitRange(Ability ability)
    {
        for (int i = 0; i < abilityList.abilities.Count; i++)
        {
            if (abilityList.abilities[i] == ability)
            {
                return ranges[i].persuitRange;
            }
        }
        Debug.LogError(name + " does not have " + ability.abilityName + " in its ability list.");
        return 0;
    }

    public void addRangeToStart(AbilityRanges newRanges)
    {
        List<AbilityRanges> newList = new List<AbilityRanges>();
        newList.Add(newRanges);
        newList.AddRange(ranges);
        ranges = newList;
    }
}
