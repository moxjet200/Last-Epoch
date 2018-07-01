using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// needs a hit detector to know when to deal damage
[RequireComponent(typeof(HitDetector))]
public class GiveCreatorManaOnCollisionWithCreator : MonoBehaviour
{
    public float manaOnHit = 0f;
    public bool destroyAfterCollidingWithCreator = true;
    public float destructionDelay = 0f;

    BaseMana creatorMana = null;
    GameObject creator = null;

    // Use this for initialization
    void Start()
    {
        // subscribe to the new enemy hit event
        GetComponent<HitDetector>().newAllyHitEvent += giveMana;
        // get a reference to the creator's mana
        CreationReferences references = GetComponent<CreationReferences>();
        if (references)
        {
            creator = references.creator;
            if (creator)
            {
                creatorMana = creator.GetComponent<BaseMana>();
            }
        }

    }

    public void giveMana(GameObject hit)
    {
        if (hit == creator && creatorMana)
        {
            creatorMana.gainMana(manaOnHit);

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