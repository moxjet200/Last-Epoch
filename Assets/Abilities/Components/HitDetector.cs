using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AlignmentManager))]
public class HitDetector : OnCreation
{
    public List<GameObject> enemiesHit = new List<GameObject>();
    public List<GameObject> alliesHit = new List<GameObject>();

    public bool cannotHaveSharedhitDetector = false;

    // the shared hit detector for the whole ability, not just this object
    // if this is not null, its lists are used instead
    public SharedHitDetector sharedHitDetector;

    public delegate void NewEnemyHitAction(GameObject enemy);
    public event NewEnemyHitAction newEnemyHitEvent;

    public delegate void EnemyHitAgainAction(GameObject enemy);
    public event EnemyHitAgainAction enemyHitAgainEvent;

    // mostly for deleting the object after it has hit an enemy
    public delegate void AfterEnemyHitAction(GameObject enemy);
    public event AfterEnemyHitAction afterEnemyHitEvent;

    public delegate void NewAllyHitAction(GameObject ally);
    public event NewAllyHitAction newAllyHitEvent;

    public delegate void AllyHitAgainAction(GameObject enemy);
    public event AllyHitAgainAction allyHitAgainEvent;

    // mostly for deleting the object after it has hit an ally
    public delegate void AfterAllyHitAction(GameObject ally);
    public event AfterAllyHitAction afterAllyHitEvent;

    public delegate void InanimateHitAction(GameObject enemy);
    public event InanimateHitAction inanimateHitEvent;

    public override void onCreation()
    {
        if (sharedHitDetector != null) { sharedHitDetector.Subscribe(GetComponent<SelfDestroyer>()); }
    }


    void OnTriggerEnter(Collider hitObject)
    {
        // if the other object is an interactable then don't interact with it
        if (hitObject.gameObject.layer == 14) { return; }

        // if the other object is an ability object, e.g. another projectile, then don't interact with it
        if (hitObject.GetComponent<AbilityObjectIndicator>()) { return; }

        // get the shared hit detector's lists
        if (sharedHitDetector != null)
        {
            enemiesHit = sharedHitDetector.enemiesHit;
            alliesHit = sharedHitDetector.alliesHit;
        }

        // if the other object is dead, then don't interact with it
        StateController otherStateController = hitObject.GetComponent<StateController>();
        if (otherStateController && otherStateController.currentState == otherStateController.dying) { return; }
        
        // if this is an enemy that has already been hit then invoke an enemy hit again event
        if (enemiesHit.Contains(hitObject.gameObject)) {
            if (enemyHitAgainEvent != null)
            {
                if (TryToHitEnemy(hitObject.gameObject))
                {
                    enemyHitAgainEvent.Invoke(hitObject.gameObject);
                    if (afterEnemyHitEvent != null) { afterEnemyHitEvent.Invoke(hitObject.gameObject); }
                }
            }
            return;
        }

        // if this is an ally that has already been hit then invoke an ally hit again event
        if (alliesHit.Contains(hitObject.gameObject))
        {
            if (allyHitAgainEvent != null)
            {
                allyHitAgainEvent.Invoke(hitObject.gameObject);
                if (afterAllyHitEvent != null) { afterAllyHitEvent.Invoke(hitObject.gameObject); }
            }
            return;
        }

        // check if the object hit is damageable
        AlignmentManager alignmentManager = GetComponent<AlignmentManager>();
        if (hitObject.GetComponent<BaseHealth>() != null && alignmentManager != null && alignmentManager.alignment != null)
        {
            // check that the object hit is a friend
            if (alignmentManager.alignment.isSameOrFriend(hitObject.GetComponent<AlignmentManager>().alignment)){
                // this is an enemy and this was not in the enemiesHit list so it is a new enemy
                alliesHit.Add(hitObject.gameObject);
                if (sharedHitDetector != null) { sharedHitDetector.alliesHit.Add(hitObject.gameObject); }
                if (newAllyHitEvent != null)
                {
                    newAllyHitEvent.Invoke(hitObject.gameObject);
                    if (afterAllyHitEvent != null) { afterAllyHitEvent.Invoke(hitObject.gameObject); }
                }
                return;
            }
            // if it is not with me, then it is against me....
            else{
                // this is an enemy and this was not in the enemiesHit list so it is a new enemy
                enemiesHit.Add(hitObject.gameObject);
                if (sharedHitDetector != null) { sharedHitDetector.enemiesHit.Add(hitObject.gameObject); }
                if (newEnemyHitEvent != null)
                {
                    if (TryToHitEnemy(hitObject.gameObject))
                    {
                        newEnemyHitEvent.Invoke(hitObject.gameObject);
                        if (afterEnemyHitEvent != null) { afterEnemyHitEvent.Invoke(hitObject.gameObject); }
                    }
                }
                return;
            }
        }

        
        // if it's got to this point, the thing being hit shows no signs of life
        if (inanimateHitEvent != null) { inanimateHitEvent.Invoke(hitObject.gameObject); }
    }

	private bool TryToHitEnemy(GameObject enemy){
        ProtectionClass protection = enemy.GetComponent<ProtectionClass>();
        if (protection)
        {
            return !(protection.RollDodgeReturnTrueIfDodged());
        }
        else
        {
            return true;
        }
    }

}
