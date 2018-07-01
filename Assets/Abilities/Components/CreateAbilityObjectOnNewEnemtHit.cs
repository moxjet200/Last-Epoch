using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HitDetector))]
[RequireComponent(typeof(AbilityObjectConstructor))]
public class CreateAbilityObjectOnNewEnemtHit : MonoBehaviour {

    public Ability abilityToInstantiate;

    public bool spawnAtHit = true;

    [HideInInspector]
    public bool active = true;

	void Awake () {
        GetComponent<HitDetector>().newEnemyHitEvent += createAbilityObject;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void createAbilityObject(GameObject enemyHit)
    {
        if (!active) { return; }
        // create a random aim point
        Vector3 aimPoint = new Vector3(transform.position.x + Random.Range(-5f, 5f), transform.position.y, transform.position.x + Random.Range(-5f, 5f));
        // decide where to spawnt the ability
        Vector3 spawnPoint;
        if (spawnAtHit) { spawnPoint = new Vector3(enemyHit.transform.position.x, transform.position.y, enemyHit.transform.position.z); }
        else { spawnPoint = transform.position; }
        // create the ability object
        GetComponent<AbilityObjectConstructor>().constructAbilityObject(abilityToInstantiate, spawnPoint, aimPoint);
    }
}
