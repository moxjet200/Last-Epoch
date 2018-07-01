using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SendEnemiesForwardInTimeOnHit : MonoBehaviour
{
    public float duration = 0f;

    // Use this for initialization
    void Start()
    {
        // subscribe to the new enemy hit event
        GetComponent<HitDetector>().newEnemyHitEvent += sendForward;
    }

    public void sendForward(GameObject enemy)
    {
        if (enemy.transform.parent == null)
        {
            // force wait
            StateController stateController = enemy.GetComponent<StateController>();
            if (stateController && stateController.waiting && (!stateController.currentState || stateController.currentState.priority < 95))
            {
                stateController.forceChangeState(stateController.waiting);
            }

            // send forward in time
            GameObject tardis = new GameObject();
            tardis.transform.position = enemy.transform.position;
            enemy.transform.parent = tardis.transform;
            tardis.AddComponent<SelfDestroyer>();
            DisableChildrenForDurationThenRelease chameleonCircuit = tardis.AddComponent<DisableChildrenForDurationThenRelease>();
            chameleonCircuit.duration = duration;
            CreateOnDeath cod = tardis.AddComponent<CreateOnDeath>();
            cod.objectsToCreateOnDeath.Add(new CreateOnDeath.GameObjectHolder(PrefabList.getPrefab("AnomalyEndEffect")));
        }
    }

}
