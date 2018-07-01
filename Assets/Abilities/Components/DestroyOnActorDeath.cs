using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SelfDestroyer))]
public class DestroyOnActorDeath : MonoBehaviour {

    [Tooltip("Select an actor with a Dying component")]
    public Dying actor = null;

	// Use this for initialization
	void Start () {
        actor.deathEvent += Die;
    }
	
	public void Die (Dying _actor) {
        SelfDestroyer destroyer = GetComponent<SelfDestroyer>();
        if (destroyer) { destroyer.die(); }
    }

    void OnDestroy()
    {
        actor.deathEvent -= Die;
    }
}
