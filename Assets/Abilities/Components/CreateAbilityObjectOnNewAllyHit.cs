using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HitDetector))]
[RequireComponent(typeof(AbilityObjectConstructor))]
public class CreateAbilityObjectOnNewAllyHit : MonoBehaviour
{

    public Ability abilityToInstantiate;

    public bool spawnAtHit = true;
    public bool onlyHitCreatorMinions = true;
    public bool aimTowardsHit = false;
    HitDetector hitDetector = null;
    CreationReferences myReferences = null;


    [HideInInspector]
    public bool active = true;

    void Start()
    {
        hitDetector = GetComponent<HitDetector>();
        myReferences = GetComponent<CreationReferences>();
        if (hitDetector)
        {
            // subscribe to the new ally hit event
            hitDetector.newAllyHitEvent += createAbilityObjectIfNeeded;
        }
    }

    public void createAbilityObject(GameObject ally)
    {
        if (!active) { return; }
        // create a random aim point or aim towards hit
        Vector3 aimPoint = Vector3.zero;
        if (aimTowardsHit)
        {
            aimPoint = new Vector3(ally.transform.position.x, transform.position.y, ally.transform.position.z);
        }
        else
        {
            aimPoint = new Vector3(transform.position.x + Random.Range(-5f, 5f), transform.position.y, transform.position.x + Random.Range(-5f, 5f));
        }
        // decide where to spawnt the ability
        Vector3 spawnPoint;
        if (spawnAtHit) { spawnPoint = new Vector3(ally.transform.position.x, transform.position.y, ally.transform.position.z); }
        else { spawnPoint = transform.position; }
        // create the ability object
        GetComponent<AbilityObjectConstructor>().constructAbilityObject(abilityToInstantiate, spawnPoint, aimPoint);
    }

    void OnDestroy()
    {
        if (hitDetector)
        {
            hitDetector.newEnemyHitEvent -= createAbilityObjectIfNeeded;
            hitDetector.enemyHitAgainEvent -= createAbilityObjectIfNeeded;
        }
    }

    public void createAbilityObjectIfNeeded(GameObject ally)
    {
        if (!onlyHitCreatorMinions) { createAbilityObject(ally); return; }

        CreationReferences references = ally.GetComponent<CreationReferences>();
        if (references && references.creator && myReferences && references.creator == myReferences.creator)
        {
            createAbilityObject(ally); return;
        }
    }
}
