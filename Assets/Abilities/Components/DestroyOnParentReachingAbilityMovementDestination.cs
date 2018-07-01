using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SelfDestroyer))]
public class DestroyOnParentReachingAbilityMovementDestination : MonoBehaviour {

    MovementFromAbility parentMovementFromAbility = null;

    bool destroy = false;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (destroy) { GetComponent<SelfDestroyer>().die(); }
        if (parentMovementFromAbility == null)
        {
            parentMovementFromAbility = GetComponentInParent<MovementFromAbility>();
            if (parentMovementFromAbility != null)
            {
                parentMovementFromAbility.reachedDestinationEvent += destroyThis;
            }
        }
        if (parentMovementFromAbility != null)
        {
            if (!parentMovementFromAbility.moving) { destroyThis(); }
        }
    }

    void OnDestroy()
    {
        if (parentMovementFromAbility)
        {
            parentMovementFromAbility.reachedDestinationEvent -= destroyThis;
        }
    }

    public void destroyThis()
    {
        parentMovementFromAbility.reachedDestinationEvent -= destroyThis;
        destroy = true;
    }


}
