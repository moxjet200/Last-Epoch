using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonTracker : MonoBehaviour {
    
    public List<Summoned> summons = new List<Summoned>();

    public delegate void NewSummonAction(Summoned summon);
    public event NewSummonAction newSummonEvent = null;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddSummon(Summoned summon)
    {
        summons.Add(summon);
        if (newSummonEvent!= null)
        {
            newSummonEvent.Invoke(summon);
        }
    }

    public int numberOfMinions(Ability ability)
    {
        int number = 0;
        foreach(Summoned summon in summons)
        {
            if (summon && summon.references && summon.references.thisAbility == ability)
            {
                number++;
            }
        }

        return number;
    }

    public List<Summoned> getMinions(Ability ability)
    {
        List<Summoned> returnList = new List<Summoned>();
        
        foreach (Summoned summon in summons)
        {
            if (summon && summon.references && summon.references.thisAbility == ability)
            {
                returnList.Add(summon);
            }
        }

        return returnList;
    }

    public List<Summoned> getMinions(List<Ability> abilities)
    {
        List<Summoned> returnList = new List<Summoned>();

        foreach (Summoned summon in summons)
        {
            if (summon && summon.references && abilities.Contains(summon.references.thisAbility))
            {
                returnList.Add(summon);
            }
        }

        return returnList;
    }

    public int numberOfMinions()
    {
        if (summons == null) { return 0; }
        return summons.Count;
    }

    public Summoned getNearestMinion(Vector3 point)
    {
        // make sure all summons in the list are valid
        summons.RemoveAll(x => x == null);

        // return if there are no summons
        if (summons.Count <= 0) { return null; }

        // initialise the nearest as the first
        Summoned nearestSummon = summons[0];
        float nearestSquareDist = Maths.squareDistance(point, summons[0].transform.position);

        // try to find the nearest one
        float squareDist = 0;
        for (int i = 1; i < summons.Count; i++)
        {
            squareDist = Maths.squareDistance(point, summons[i].transform.position);
            if (squareDist < nearestSquareDist)
            {
                nearestSquareDist = squareDist;
                nearestSummon = summons[i];
            }
        }

        // return the nearest one found
        return nearestSummon;
    }

}
