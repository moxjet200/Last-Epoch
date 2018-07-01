using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreationReferences : MonoBehaviour {

    public GameObject creator;
    public Vector3 locationCreatedFrom;
    public Ability thisAbility;

    public void copyTo(CreationReferences newReferences)
    {
        newReferences.creator = creator;
        newReferences.locationCreatedFrom = locationCreatedFrom;
        newReferences.thisAbility = thisAbility;
    }

}
