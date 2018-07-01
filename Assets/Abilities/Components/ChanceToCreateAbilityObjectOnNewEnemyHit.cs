using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HitDetector))]
[RequireComponent(typeof(AbilityObjectConstructor))]
public class ChanceToCreateAbilityObjectOnNewEnemyHit : MonoBehaviour {

    public Ability abilityToInstantiate;

    public bool spawnAtHit = true;

    public bool canOnlyTriggerOnce = false;
    bool triggered = false;

    [Range(0f,1f)]
    public float chance = 0f;

    // Use this for initialization
    void Start () {
        GetComponent<HitDetector>().newEnemyHitEvent += createAbilityObject;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void createAbilityObject(GameObject enemyHit)
    {
        // check if it can only trigger once and has already triggered
        if (canOnlyTriggerOnce && triggered) { return; }
        // if the chance check fails return
        if (Random.Range(0f,1f) > chance) { return; }
        triggered = true;
        // create a random aim point
        Vector3 aimPoint = new Vector3(transform.position.x + Random.Range(-5f, 5f), transform.position.y, transform.position.x + Random.Range(-5f, 5f));
        // decide where to spawnt the ability
        Vector3 spawnPoint;
        if (spawnAtHit) { spawnPoint = new Vector3(enemyHit.transform.position.x, transform.position.y, enemyHit.transform.position.z); }
        else { spawnPoint = transform.position; }
        // create the ability object
        GetComponent<AbilityObjectConstructor>().constructAbilityObject(abilityToInstantiate, transform.position, aimPoint);
    }
}
