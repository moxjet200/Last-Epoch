using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// needs a hit detector to know when to deal damage
[RequireComponent(typeof(HitDetector))]
public class GiveCreatorManaOnHit : MonoBehaviour
{
    public float manaOnHit = 0f;

    public bool canHitSameEnemyTwice = false;

    BaseMana creatorMana = null;

    // Use this for initialization
    void Start()
    {
        // subscribe to the new enemy hit event
        GetComponent<HitDetector>().newEnemyHitEvent += giveMana;
        // if it can damage the same enemy again also subscribe to the enemy hit again event
        if (canHitSameEnemyTwice) { GetComponent<HitDetector>().enemyHitAgainEvent += giveMana; }
        // get a reference to the creator's mana
        CreationReferences references = GetComponent<CreationReferences>();
        if (references)
        {
            GameObject creator = references.creator;
            if (creator)
            {
                creatorMana = creator.GetComponent<BaseMana>();
            }
        }
        
    }

    public void giveMana(GameObject enemy)
    {
		if (creatorMana)
        {
            creatorMana.gainMana(manaOnHit);
        }
    }
}
