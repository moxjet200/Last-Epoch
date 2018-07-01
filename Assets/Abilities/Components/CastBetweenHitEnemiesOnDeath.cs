using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SelfDestroyer))]
[RequireComponent(typeof(AbilityObjectConstructor))]
[RequireComponent(typeof(HitDetector))]
public class CastBetweenHitEnemiesOnDeath : MonoBehaviour
{

    public Ability abilityToInstantiate;
    public bool castFromCreatorToo = true;
    public Vector3 offset = new Vector3(0, 0, 0);
    public bool requireNotPartOfChain = false;

    void Awake()
    {
        GetComponent<SelfDestroyer>().deathEvent += createAbilityObject;
    }

    public void createAbilityObject()
    {
        if (requireNotPartOfChain)
        {
            ChainOnHit chain = GetComponent<ChainOnHit>();
            if (chain && (chain.chainsRemaining > 0 || chain.hasChained))
            {
                return;
            }
        }


        HitDetector hitDetector = GetComponent<HitDetector>();
        if (hitDetector)
        {
            List<GameObject> enemies = new List<GameObject>();

            if (hitDetector.sharedHitDetector != null)
            {
                foreach (GameObject enemy in hitDetector.sharedHitDetector.enemiesHit)
                {
                    if (enemy != null && !enemies.Contains(enemy))
                    {
                        enemies.Add(enemy);
                    }
                }
            }
            else
            {
                foreach (GameObject enemy in hitDetector.enemiesHit)
                {
                    if (enemy != null && !enemies.Contains(enemy))
                    {
                        enemies.Add(enemy);
                    }
                }
            }

            

            CreationReferences references = GetComponent<CreationReferences>();
            if (references)
            {
                Vector3 startPos = references.locationCreatedFrom;

                // create the first ability object
                if (enemies.Count >= 1 && castFromCreatorToo)
                {
                    GetComponent<AbilityObjectConstructor>().constructAbilityObject(abilityToInstantiate, startPos + offset, enemies[0].transform.position + offset);
                }

                for (int i = 0; i < enemies.Count - 1; i++)
                {
                    // create the other ability objects
                    GetComponent<AbilityObjectConstructor>().constructAbilityObject(abilityToInstantiate, enemies[i].transform.position + offset, enemies[i + 1].transform.position + offset);
                }
                
            }
            
        }

        

    }

    public void deactivate()
    {
        SelfDestroyer sd = GetComponent<SelfDestroyer>();
        if (sd)
        {
            sd.deathEvent -= createAbilityObject;
        }
    }
}
