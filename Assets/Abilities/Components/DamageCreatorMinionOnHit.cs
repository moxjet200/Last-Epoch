using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// needs a hit detector to know when to deal damage
[RequireComponent(typeof(HitDetector))]
public class DamageCreatorMinionOnHit : DamageStatsHolder
{

    public bool canDamageSameMinionAgain = false;
    HitDetector hitDetector = null;
    CreationReferences myReferences = null;

    // Use this for initialization
    void Start()
    {
        hitDetector = GetComponent<HitDetector>();
        myReferences = GetComponent<CreationReferences>();
        if (hitDetector)
        {
            // subscribe to the new ally hit event
            hitDetector.newAllyHitEvent += preApplyDamage;
            // if it can damage the same ally again also subscribe to the ally hit again event
            if (canDamageSameMinionAgain) { hitDetector.allyHitAgainEvent += preApplyDamage; }
        }
    }

    void OnDestroy()
    {
        if (hitDetector)
        {
            hitDetector.newEnemyHitEvent -= preApplyDamage;
            hitDetector.enemyHitAgainEvent -= preApplyDamage;
        }
    }

    public void preApplyDamage(GameObject enemy)
    {
        CreationReferences references = enemy.GetComponent<CreationReferences>();
        if (references && references.creator && myReferences && references.creator == myReferences.creator)
        {
            applyDamage(enemy); return;
        }
    }

}
