using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// needs a hit detector to know when to deal damage
[RequireComponent(typeof(HitDetector))]
public class GiveCreatorResourcesOnCollisionWithCreator : MonoBehaviour
{
    public float healthOnHit = 0f;
    public float manaOnHit = 0f;
    public float wardOnHit = 0f;

    public bool destroyAfterCollidingWithCreator = true;
    public float destructionDelay = 0f;

    ProtectionClass creatorProtection = null;
    BaseMana creatorMana = null;
    GameObject creator = null;

    // Use this for initialization
    void Start()
    {
        // subscribe to the new enemy hit event
        GetComponent<HitDetector>().newAllyHitEvent += giveResources;
        // get a reference to the creator's mana
        CreationReferences references = GetComponent<CreationReferences>();
        if (references)
        {
            creator = references.creator;
            if (creator)
            {
                creatorMana = creator.GetComponent<BaseMana>();
                creatorProtection = creator.GetComponent<ProtectionClass>();
            }
        }

    }

    public void giveResources(GameObject hit)
    {
        if (hit == creator)
        {
            if (creatorMana && manaOnHit != 0)
            {
                creatorMana.gainMana(manaOnHit);
            }
            if (creatorProtection && creatorProtection.healthClass && healthOnHit != 0)
            {
                creatorProtection.healthClass.Heal(healthOnHit);
            }
            if (creatorProtection && wardOnHit != 0)
            {
                creatorProtection.GainWard(wardOnHit);
            }

            if (destroyAfterCollidingWithCreator)
            {
                SelfDestroyer dest = GetComponent<SelfDestroyer>();
                if (dest)
                {
                    if (destructionDelay == 0) { dest.die(); }
                    else
                    {
                        gameObject.AddComponent<DestroyAfterDuration>().duration = destructionDelay;
                    }
                }
            }
        }
    }
}