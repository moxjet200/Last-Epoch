using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonVineAtTargetMutator : AbilityMutator
{
    public float extraVines = 0f;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.summonVineAtTarget);
        base.Awake();
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        if (extraVines > 0)
        {
            SummonEntityOnDeath component = abilityObject.GetComponent<SummonEntityOnDeath>();
            if (component)
            {
                component.numberToSummon += extraVines;
                component.distance = 0.5f;
            }
        }

        return abilityObject;
    }
}
