using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericMutator : AbilityMutator
{

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        return abilityObject;
    }
}
