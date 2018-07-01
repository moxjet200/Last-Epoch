using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HitDetector))]
[RequireComponent(typeof(AbilityObjectConstructor))]
public class CreateManaAbilityObjectOnEnemyHit : MonoBehaviour
{

    public Ability manaObject;
    public float mana = 0f;

    public bool spawnAtHit = true;

    void Awake()
    {
        GetComponent<HitDetector>().newEnemyHitEvent += createAbilityObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void createAbilityObject(GameObject enemyHit)
    {
        // create a random aim point
        Vector3 aimPoint = new Vector3(transform.position.x + Random.Range(-5f, 5f), transform.position.y, transform.position.x + Random.Range(-5f, 5f));
        // decide where to spawnt the ability
        Vector3 spawnPoint;
        if (spawnAtHit) { spawnPoint = new Vector3(enemyHit.transform.position.x, transform.position.y, enemyHit.transform.position.z); }
        else { spawnPoint = transform.position; }
        // create the ability object
        GameObject ao = GetComponent<AbilityObjectConstructor>().constructAbilityObject(manaObject, spawnPoint, aimPoint);
        GiveCreatorManaOnCollisionWithCreator manaComponent = ao.GetComponent<GiveCreatorManaOnCollisionWithCreator>();
        if (manaComponent)
        {
            manaComponent.manaOnHit = mana;
        }
    }
}
