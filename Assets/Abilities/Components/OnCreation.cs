using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// indicates to the abilityObjectCreator that this component has something that should execute between start and awake.
public abstract class OnCreation : MonoBehaviour {

    public bool runOnCreation = true;

    public abstract void onCreation();
}
