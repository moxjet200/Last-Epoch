using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WraithAdapter : SkeletonAdapter
{
    public bool stationary = false;
    public float reducedHealthDrain = 0f;

    public override GameObject adapt(GameObject entity)
    {
        
        if (stationary)
        {
            NavMeshAgent agent = entity.GetComponent<NavMeshAgent>();
            if (agent)
            {
                agent.avoidancePriority = 100;
            }

            AbilityRangeList rangeList = entity.GetComponent<AbilityRangeList>();
            if (rangeList)
            {
                for (int i = 0; i < rangeList.ranges.Count; i++)
                {
                    if (rangeList.ranges[i].engageRange >= 4)
                    {
                        rangeList.ranges[i] = new AbilityRangeList.AbilityRanges(rangeList.ranges[i].persuitRange, rangeList.ranges[i].persuitRange - 0.01f,
                            rangeList.ranges[i].persuitRange - 0.02f, rangeList.ranges[i].minRange);
                    }
                    else if (rangeList.ranges[i].engageRange < 0.7f)
                    {
                        rangeList.ranges[i] = new AbilityRangeList.AbilityRanges(rangeList.ranges[i].persuitRange, Mathf.Max(rangeList.ranges[i].maxRange, 1.1f),
                            0.7f, rangeList.ranges[i].minRange);
                    }
                }
            }
        }

        if (reducedHealthDrain != 0)
        {
            AcceleratingHealthDrain component = entity.GetComponent<AcceleratingHealthDrain>();
            if (component)
            {
                component.drainMultiplier *= (1 - reducedHealthDrain);
            }
        }

        return base.adapt(entity);
    }



}