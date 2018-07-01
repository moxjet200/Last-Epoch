using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// needs a hit detector to know when an enemy is hit and a self destroyer to destroy itself
[RequireComponent(typeof(HitDetector))]
[RequireComponent(typeof(SelfDestroyer))]
public class DestroyOnFailingToPierceEnemy : MonoBehaviour {
    
	// Use this for initialization
	void Awake () {
        GetComponent<HitDetector>().afterEnemyHitEvent += DestroyOnFailingToPierce;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DestroyOnFailingToPierce(GameObject hitObject)
    {
        // if there is no pierce component, die
        if (GetComponent<Pierce>() == null) { GetComponent<SelfDestroyer>().die(); }
        else
        {
            // pierce if the pierce component says it can, otherwise die
            if (GetComponent<Pierce>().canPierce(hitObject))
            {
                GetComponent<Pierce>().pierce(hitObject);
            }
            else
            {
                GetComponent<SelfDestroyer>().die();
            }
        }
    }

    public void alsoDestroyOnFailingToPierceAlly()
    {
        GetComponent<HitDetector>().afterAllyHitEvent += DestroyOnFailingToPierce;
    }

    public void alsoDestroyOnFailingToPierceCreatorMinion()
    {
        GetComponent<HitDetector>().afterAllyHitEvent += destroyIfHitCreatorMinion;
    }

    public void destroyIfHitCreatorMinion(GameObject hitObject)
    {
        CreationReferences references = hitObject.GetComponent<CreationReferences>();
        CreationReferences myReferences = GetComponent<CreationReferences>();
        if (references && references.creator && myReferences && references.creator == myReferences.creator)
        {
            DestroyOnFailingToPierce(hitObject); return;
        }
    }
}
