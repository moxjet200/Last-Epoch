using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HitDetector))]
public class PullEnemiesAfterCollision : PercentagePullComponent {

    public List<GameObject> enemiesToPull = new List<GameObject>();

    public bool useRangeLimit = false;

    public float rangeLimit = 3f;

    // Use this for initialization
    void Start () {
        // subscribe to the new enemy hit event
        GetComponent<HitDetector>().newEnemyHitEvent += startPullingEnemy;
    }

    // Update is called once per frame
    void Update()
    {
        // remove any null enemies
        enemiesToPull.RemoveAll(x => x == null);
        foreach (GameObject enemy in enemiesToPull)
        {
            if (!useRangeLimit || Vector3.Distance(transform.position, enemy.transform.position) < rangeLimit)
            {
                Pull(enemy, Time.deltaTime * 60f);
            }
        }
    }


    public void startPullingEnemy(GameObject enemy)
    {
        // add the enemy to the list
        enemiesToPull.Add(enemy);
        // subscribe to its death event to remove it from the list
        Dying dyingState = enemy.GetComponent<Dying>();
        if (dyingState)
        {
            dyingState.deathEvent += stopPullingEnemy;
        }
    }

    public void stopPullingEnemy(GameObject enemy)
    {
        enemiesToPull.Remove(enemy);
    }

    // need to unsubscribe from all the death events when this object is destroyed
    public void OnDeath()
    {
        Dying dyingState = null;
        foreach (GameObject enemy in enemiesToPull)
        {
            dyingState = enemy.GetComponent<Dying>();
            if (dyingState)
            {
                dyingState.deathEvent -= stopPullingEnemy;
            } 
        }
    }

    public void stopPullingEnemy(Dying dyingComponent)
    {
        stopPullingEnemy(dyingComponent.gameObject);
    }


}
