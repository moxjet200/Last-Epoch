using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SelfDestroyer))]
[RequireComponent(typeof(CreationReferences))]
public class MoveCreatorToLocationOnDeath : MonoBehaviour {

    public bool dontMoveCreatorIfDeath = false;

	// Use this for initialization
	void Start () {
        GetComponent<SelfDestroyer>().deathEvent += moveCreator;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void moveCreator()
    {
        GameObject caster = GetComponent<CreationReferences>().creator;

        if (!caster) { return; }

        if (dontMoveCreatorIfDeath)
        {
            StateController controller = caster.GetComponent<StateController>();
            if (controller.currentState == controller.dying)
            {
                return;
            }
        }
        
        caster.transform.LookAt(transform.position);
        ActorMover.moveActor(caster, transform.position);
    }
}
