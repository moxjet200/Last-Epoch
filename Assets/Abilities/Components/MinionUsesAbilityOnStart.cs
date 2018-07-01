using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LocationDetector))]
public class MinionUsesAbilityOnStart : MonoBehaviour
{
    public Ability ability = null;
    public MinionUseType minionsToUseAbility = MinionUseType.Nearest;
    public bool requiresCanUseMovementAbilities = false;

    public void Start()
    {
        CreationReferences references = GetComponent<CreationReferences>();
        if (references && references.creator)
        {
            SummonTracker summonTracker = references.creator.GetComponent<SummonTracker>();
            if (summonTracker)
            {
                List<Summoned> minionsToUse = new List<Summoned>();
                List<Summoned> creatorMinions = summonTracker.summons;
                if (creatorMinions.Count > 0)
                {
                    // select minions
                    if (minionsToUseAbility == MinionUseType.Nearest)
                    {
                        Summoned selected = creatorMinions[0];
                        float selectedDistance = Vector3.Distance(transform.position,creatorMinions[0].transform.position);
                        float distance = 0;
                        foreach (Summoned minion in creatorMinions)
                        {
                            distance = Vector3.Distance(transform.position, minion.transform.position);
                            if (distance < selectedDistance && (!requiresCanUseMovementAbilities || minion.GetComponent<CanUseMovementAbilities>()))
                            {
                                selectedDistance = distance;
                                selected = minion;
                            }
                        }
                        if (!requiresCanUseMovementAbilities || selected.GetComponent<CanUseMovementAbilities>())
                        {
                            minionsToUse.Add(selected);
                        }
                    }
                    else if (minionsToUseAbility == MinionUseType.Random)
                    {
                        int attempts = 0;
                        Summoned selected = null;
                        while (attempts < 20)
                        {
                            selected = creatorMinions[Random.Range(0, creatorMinions.Count)];
                            if (!requiresCanUseMovementAbilities || selected.GetComponent<CanUseMovementAbilities>())
                            {
                                attempts = 100000;
                                minionsToUse.Add(selected);
                            }
                        }
                    }
                    else if (minionsToUseAbility == MinionUseType.All)
                    {
                        if (!requiresCanUseMovementAbilities)
                        {
                            minionsToUse.AddRange(creatorMinions);
                        }
                        else
                        {
                            foreach(Summoned minion in creatorMinions)
                            {
                                if (minion.GetComponent<CanUseMovementAbilities>())
                                {
                                    minionsToUse.Add(minion);
                                }
                            }
                        }
                    }
                    // make the minions use the ability
                    Vector3 targetPostion = GetComponent<LocationDetector>().targetLocation;
                    UsingAbility ua = null;
                    foreach (Summoned minion in minionsToUse)
                    {
                        ua = minion.GetComponent<UsingAbility>();
                        if (ua)
                        {
                            ua.UseAbility(ability, targetPostion, false, false);
                            ua.transform.LookAt(targetPostion);
                        }
                    }
                }
            }
        }
    }


    public enum MinionUseType
    {
        Nearest, Random, All
    }

}
