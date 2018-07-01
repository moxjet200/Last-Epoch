using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// needs a hit detector to know when to deal damage
[RequireComponent(typeof(HitDetector))]
public class DamageEnemyOnHit : DamageStatsHolder
{

    public bool canDamageSameEnemyAgain = false;
    HitDetector hitDetector = null;

    // Use this for initialization
    void Start()
    {
        hitDetector = GetComponent<HitDetector>();
        if (hitDetector)
        {
            // subscribe to the new enemy hit event
            hitDetector.newEnemyHitEvent += applyDamage;
            // if it can damage the same enemy again also subscribe to the enemy hit again event
            if (canDamageSameEnemyAgain) { hitDetector.enemyHitAgainEvent += applyDamage; }
        }
    }

    void OnDestroy()
    {
        deactivate();
    }

    public void deactivate()
    {
        if (hitDetector)
        {
            hitDetector.newEnemyHitEvent -= applyDamage;
            hitDetector.enemyHitAgainEvent -= applyDamage;
        }
    }
    
}
