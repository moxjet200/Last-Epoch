using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class PullComponent : MonoBehaviour {

    public abstract void Pull(GameObject enemy, float scale);

    public bool canPull(GameObject enemy)
    {
        if (enemy.GetComponent<CannotBePulled>()) { return false; }

        // do not pull the enemy if they are in a high priority state
        if (enemy.GetComponent<StateController>() && enemy.GetComponent<StateController>().currentState && enemy.GetComponent<StateController>().currentState.priority > 80) { return false; }

        // do not pull an enemy that cannot move
        if (!enemy.GetComponent<NavMeshAgent>()) { return false; }

        // do not pull an enemy that is snared
        ProtectionClass protectionClass = enemy.GetComponent<ProtectionClass>();
        if (protectionClass && protectionClass.effectControl && protectionClass.effectControl.hasStatus(12)) { return false; }

        return true;
    }


}
