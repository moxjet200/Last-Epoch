using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HitDetector))]
[RequireComponent(typeof(AbilityObjectConstructor))]
public class CreateResourceReturnAbilityObjectsForCorpsesInArea : MonoBehaviour
{

    public float radius = 0f;
    public Ability abilityObject;
    public float health = 0f;
    public float mana = 0f;
    public float ward = 0f;
    public bool eraseCorpses = false;

    public bool spawnAtHit = true;

    public GameObject prefabToSpawnAtCorpses = null;

    void Start()
    {
        AlignmentManager alignmentManager = GetComponent<AlignmentManager>();
        if (alignmentManager && alignmentManager.alignment)
        {
            Vector3 position = transform.position;
            foreach (Dying dying in Dying.all)
            {
                if (dying.isDying() && dying.myHealth && dying.myHealth.alignmentManager && dying.myHealth.alignmentManager.alignment &&
                    alignmentManager.alignment.foes.Contains(dying.myHealth.alignmentManager.alignment))
                {
                    if (Vector3.Distance(position, dying.transform.position) <= radius)
                    {
                        createAbilityObject(dying.gameObject);
                        if (eraseCorpses)
                        {
                            dying.setDelays(0f, 0f);
                        }
                    }
                }
            }
        }
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
        GameObject ao = GetComponent<AbilityObjectConstructor>().constructAbilityObject(abilityObject, spawnPoint, aimPoint);
        GiveCreatorResourcesOnCollisionWithCreator resourceComponent = ao.GetComponent<GiveCreatorResourcesOnCollisionWithCreator>();
        if (resourceComponent)
        {
            resourceComponent.manaOnHit = mana;
            resourceComponent.healthOnHit = health;
            resourceComponent.wardOnHit = ward;
        }
        // spawn a prefab
        if (prefabToSpawnAtCorpses != null)
        {
            Instantiate(prefabToSpawnAtCorpses).transform.position = enemyHit.transform.position;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }


}
