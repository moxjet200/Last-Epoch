using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StateController))]
[RequireComponent(typeof(DestroyAfterDuration))]
public class DisableDestroyAfterDurationWhenInDyingState : MonoBehaviour {

    StateController stateController = null;
    DestroyAfterDuration destroyAfterDuration = null;

    // Use this for initialization
    void Start () {
        stateController = GetComponent<StateController>();
        destroyAfterDuration = GetComponent<DestroyAfterDuration>();
    }
	
	// Update is called once per frame
	void Update () {
        if (destroyAfterDuration)
        {
            if (stateController.currentState == stateController.dying)
            {
                destroyAfterDuration.enabled = false;
            }
            else
            {
                destroyAfterDuration.enabled = true;
            }
        }
	}
}
