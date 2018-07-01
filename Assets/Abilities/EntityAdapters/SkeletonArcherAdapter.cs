using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonArcherAdapter : SkeletonAdapter
{

    // poison arrow
    public bool usesPoisonArrow = false;
    public bool pierces = false;
    public bool inaccurate = false;
    public float increasedDamage = 0f;
    public int delayedAttacks = 0;

    public override GameObject adapt(GameObject entity)
    {

        // poison arrow
        if (usesPoisonArrow)
        {
            PoisonArrowMutator mutator = entity.AddComponent<PoisonArrowMutator>();
            if (pierces)
            {
                mutator.targetsToPierce = 1000;
            }
            mutator.inaccuracy = inaccurate;
            mutator.increasedDamage = increasedDamage;
            mutator.delayedAttacks = delayedAttacks;
        }

        return base.adapt(entity);
    }



}