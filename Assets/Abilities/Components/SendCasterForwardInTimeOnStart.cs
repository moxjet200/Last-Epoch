using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SendCasterForwardInTimeOnStart : MonoBehaviour
{
    public float duration = 0f;

    // Use this for initialization
    void Start()
    {
        CreationReferences references = GetComponent<CreationReferences>();
        if (references && references.creator)
        {
            sendForward(references.creator);
        }
    }

    public void sendForward(GameObject caster)
    {
        if (SceneLoader.instance && SceneLoader.instance.sceneLoading) { return; }

        // force wait
        StateController stateController = caster.GetComponent<StateController>();
        if (stateController && stateController.waiting && (!stateController.currentState || stateController.currentState.priority < 95))
        {
            stateController.forceChangeState(stateController.waiting);
        }

        // send forward in time
        GameObject tardis = new GameObject();
        tardis.transform.position = caster.transform.position;
        tardis.AddComponent<SelfDestroyer>();
        DisableChildrenForDurationThenRelease chameleonCircuit = tardis.AddComponent<DisableChildrenForDurationThenRelease>();
        chameleonCircuit.duration = duration;
        CreateOnDeath cod = tardis.AddComponent<CreateOnDeath>();
        cod.objectsToCreateOnDeath.Add(new CreateOnDeath.GameObjectHolder(PrefabList.getPrefab("AnomalyEndEffect")));

        chameleonCircuit.disableSceneLoaderForDuration = true;
        chameleonCircuit.tag = "Persistent";
        chameleonCircuit.nextParent = caster.transform.parent;
        caster.transform.parent = tardis.transform;
    }

}
