using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SelfDestroyer))]
[RequireComponent(typeof(AlignmentManager))]
public class ResurrectNearbyAlliesOnDeath : MonoBehaviour {
    
    public float radius = 0f;

    Alignment alignment = null;

    // Use this for initialization
    void Start () {
        // subscribe to the new ally hit event
        GetComponent<SelfDestroyer>().deathEvent += resurrectNearbyAllies;
        // get alignment
        alignment = GetComponent<AlignmentManager>().alignment;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void resurrectNearbyAllies()
    {
        foreach(Dying dying in Dying.all)
        {
            if (dying.GetComponent<BaseHealth>() && !dying.GetComponent<CannotBeResurrected>())
            {
                if (alignment.isSameOrFriend(dying.GetComponent<AlignmentManager>().alignment))
                {
                    if (Vector3.Distance(transform.position, dying.transform.position) <= radius)
                    {
                        resurrect(dying.gameObject);
                    }
                }
            }
        }
    }

    public void resurrect(GameObject hitObject)
    {
        if (hitObject.GetComponent<StateController>())
        {
            StateController controller = hitObject.GetComponent<StateController>();
            if (controller.currentState == controller.dying)
            {
                hitObject.AddComponent<Resurrected>();
                hitObject.GetComponent<BaseHealth>().Heal(hitObject.GetComponent<BaseHealth>().maxHealth);
                controller.forceChangeState(controller.waiting);
            }
        }
    }
}
