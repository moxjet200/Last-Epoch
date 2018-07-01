using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// needs a hit detector to know when to deal damage
[RequireComponent(typeof(HitDetector))]
public class RepeatedlyGiveCreatorResources : MonoBehaviour
{
    float index = 0;
    public float interval = 0f;

    public float health = 0f;
    public float mana = 0f;
    public float ward = 0f;
    
    ProtectionClass creatorProtection = null;
    BaseMana creatorMana = null;

    // Use this for initialization
    void Start()
    {
        // get a reference to the creator's mana and protection
        CreationReferences references = GetComponent<CreationReferences>();
        if (references)
        {
            GameObject creator = references.creator;
            if (creator)
            {
                if (health != 0 || ward != 0) { creatorProtection = creator.GetComponent<ProtectionClass>(); }
                if (mana != 0) { creatorMana = creator.GetComponent<BaseMana>(); }
            }
        }

    }


    public void Update()
    {
        if (index >= interval)
        {
            index -= interval;
            giveResources();
        }

        index += Time.deltaTime;
    }

    public void giveResources()
    {
        if (creatorMana && mana != 0)
        {
            creatorMana.gainMana(mana);
        }
        if (creatorProtection)
        {
            if (health != 0 && creatorProtection.healthClass)
            {
                creatorProtection.healthClass.Heal(health);
            }
            if (ward != 0)
            {
                creatorProtection.GainWard(ward);
            }
        }
    }
}
