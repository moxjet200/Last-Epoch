using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodWraithAdapter : SkeletonAdapter
{
    public float increasedSize = 0f;

    public override GameObject adapt(GameObject entity)
    {

        // poison arrow
        if (increasedSize != 0)
        {
            SizeManager component = Comp<SizeManager>.GetOrAdd(entity);
            component.increaseSize(increasedSize);
        }

        return base.adapt(entity);
    }



}
