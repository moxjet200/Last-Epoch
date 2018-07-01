using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SelfDestroyer))]
public class DestroySelfOnParentDeath : MonoBehaviour {
	
	// Update is called once per frame
	protected virtual void Update () {
		if (transform.parent != null)
        {
            StateController controller = GetComponentInParent<StateController>();
            if (controller != null)
            {
                if (controller.currentState == controller.dying)
                {
                    // before destruction effects
                    beforeDestroying();
                    // destroy self
                    Comp<SelfDestroyer>.GetOrAdd(gameObject).die();
                }
            }
        }
	}

    public virtual void beforeDestroying()
    {

    }
}
