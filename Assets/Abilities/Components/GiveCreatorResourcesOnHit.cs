using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// needs a hit detector to know when to deal damage
[RequireComponent(typeof(HitDetector))]
public class GiveCreatorResourcesOnHit : MonoBehaviour
{
    public float healthOnHit = 0f;
    public float manaOnHit = 0f;
    public float wardOnHit = 0f;

    public bool canHitSameEnemyTwice = false;

    ProtectionClass creatorProtection = null;
    BaseMana creatorMana = null;

    // Use this for initialization
    void Start()
    {
        // subscribe to the new enemy hit event
        GetComponent<HitDetector>().newEnemyHitEvent += giveResources;
        // if it can damage the same enemy again also subscribe to the enemy hit again event
        if (canHitSameEnemyTwice) { GetComponent<HitDetector>().enemyHitAgainEvent += giveResources; }
        // get a reference to the creator's mana and protection
        CreationReferences references = GetComponent<CreationReferences>();
        if (references)
        {
            GameObject creator = references.creator;
            if (creator)
            {
                if (healthOnHit != 0 || wardOnHit != 0) { creatorProtection = creator.GetComponent<ProtectionClass>(); }
                if (manaOnHit != 0) { creatorMana = creator.GetComponent<BaseMana>(); }
            }
        }

    }

    public void giveResources(GameObject enemy)
    {
        if (creatorMana && manaOnHit != 0)
        {
            creatorMana.gainMana(manaOnHit);
        }
        if (creatorProtection)
        {
            if (healthOnHit != 0 && creatorProtection.healthClass)
            {
                creatorProtection.healthClass.Heal(healthOnHit);
            }
            if (wardOnHit != 0)
            {
                creatorProtection.GainWard(wardOnHit);
            }
        }
    }
}
