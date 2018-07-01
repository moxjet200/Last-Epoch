using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HitDetector))]
[RequireComponent(typeof(AbilityObjectConstructor))]
public class CreateResourceReturnAbilityObjectOnEnemyHit : MonoBehaviour
{

    public Ability abilityObject;
    public float health = 0f;
    public float mana = 0f;
    public float ward = 0f;
    public bool hitsAlliesInstead = false;
    public bool deactivated = false;

    public bool spawnAtHit = true;

    void Awake()
    {
        if (hitsAlliesInstead)
        {
            GetComponent<HitDetector>().newAllyHitEvent += createAbilityObject;
        }
        else
        {
            GetComponent<HitDetector>().newEnemyHitEvent += createAbilityObject;
        }
    }

    public void createAbilityObject(GameObject enemyHit)
    {
        if (deactivated) { return; }

        // create a random aim point
        Vector3 aimPoint = new Vector3(transform.position.x + Random.Range(-5f, 5f), transform.position.y, transform.position.x + Random.Range(-5f, 5f));
        // decide where to spawnt the ability
        Vector3 spawnPoint;
        if (spawnAtHit) { spawnPoint = new Vector3(enemyHit.transform.position.x, transform.position.y, enemyHit.transform.position.z); }
        else { spawnPoint = transform.position; }
        // create the ability object
        GameObject ao = GetComponent<AbilityObjectConstructor>().constructAbilityObject(abilityObject, spawnPoint, aimPoint);
        GiveCreatorResourcesOnCollisionWithCreator resourceComponent = ao.GetComponent<GiveCreatorResourcesOnCollisionWithCreator>();
        if (resourceComponent)
        {
            resourceComponent.manaOnHit = mana;
            resourceComponent.healthOnHit = health;
            resourceComponent.wardOnHit = ward;
        }
    }
}
