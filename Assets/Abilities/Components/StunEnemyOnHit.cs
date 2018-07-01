using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// needs a hit detector to know when to deal damage
[RequireComponent(typeof(HitDetector))]
public class StunEnemyOnHit : MonoBehaviour
{
    public float duration = 0.5f;

    public bool canStunSameEnemyAgain = false;

    // Use this for initialization
    void Start()
    {
        // subscribe to the new enemy hit event
        GetComponent<HitDetector>().newEnemyHitEvent += stun;
        // if it can damage the same enemy again also subscribe to the enemy hit again event
        if (canStunSameEnemyAgain) { GetComponent<HitDetector>().enemyHitAgainEvent += stun; }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void stun(GameObject enemy)
    {
        Stunned stunned = enemy.GetComponent<Stunned>();
        StateController controller = enemy.GetComponent<StateController>();
        if (stunned && controller)
        {
            stunned.stun(duration);
        }
    }
}
