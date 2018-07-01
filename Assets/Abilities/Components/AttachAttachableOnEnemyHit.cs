using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HitDetector))]
public class AttachAttachableOnEnemyHit : MonoBehaviour {

    public GameObject attachable = null;

    public bool canAttachToSameEnemyAgain = false;

    // Use this for initialization
    void Start () {

        // subscribe to the new enemy hit event
        GetComponent<HitDetector>().newEnemyHitEvent += Attach;
        // if it can damage the same enemy again also subscribe to the enemy hit again event
        if (canAttachToSameEnemyAgain) { GetComponent<HitDetector>().enemyHitAgainEvent += Attach; }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Attach(GameObject hitObject)
    {
        GameObject attached = Instantiate(attachable, hitObject.transform);
        // build damage stats if necessary
        foreach (DamageStatsHolder holder in attached.GetComponents<DamageStatsHolder>())
        {
            holder.damageStats = DamageStats.buildDamageStats(holder);
        }
    }
}
