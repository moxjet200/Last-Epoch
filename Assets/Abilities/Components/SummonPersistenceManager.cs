using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SummonPersistenceManager : MonoBehaviour {

    SceneLoader sceneLoader = null;

    bool summonsAreChildren = false;


	// Use this for initialization
	void Start () {
        sceneLoader = FindObjectOfType<SceneLoader>();
	}
	
	// Update is called once per frame
	void Update () {
		if (sceneLoader)
        {
            // this happens when loading starts
            if (sceneLoader.sceneLoading && !summonsAreChildren)
            {
                summonsAreChildren = true;
                SummonTracker tracker = GetComponent<SummonTracker>();
                if (tracker)
                {
                    // make all summons children of the player so that they become persistent
                    for (int i = 0; i< tracker.summons.Count; i++)
                    //foreach (Summoned summon in tracker.summons)
                    {
                        // don't add dying summons
                        if (!Dying.isDying(tracker.summons[i].gameObject))
                        {
                            tracker.summons[i].transform.parent = transform;
                        }
                        // delete old dying summons
                        else
                        {
                            Destroy(tracker.summons[i]);
                        }
                    }
                }
            }
            // this happens once a scene has loaded
            if (!sceneLoader.sceneLoading && summonsAreChildren)
            {
                summonsAreChildren = false;
                SummonTracker tracker = GetComponent<SummonTracker>();
                if (tracker)
                {
                    // put all the summons at the players position and set their parent to null
                    foreach (Summoned summon in tracker.summons)
                    {
                        // do not set their parent to null if they attach to the parent
                        if (!summon.GetComponent<AttachToSummoner>())
                        {
                            bool navmeshEnabled = false;
                            NavMeshAgent agent = summon.GetComponent<NavMeshAgent>();
                            if (agent)
                            {
                                navmeshEnabled = agent.enabled;
                                agent.enabled = false;
                            }
                            summon.transform.position = transform.position + randomPositionOnRing(0.5f);
                            summon.transform.parent = null;
                            if (agent && navmeshEnabled)
                            {
                                agent.enabled = true;
                            }
                        }
                    }
                }
            }
        }
	}


    // gets a random position on a ring for placing summons around the player after a scene loads
    Vector3 randomPositionOnRing(float radius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        return new Vector3(radius * Mathf.Cos(angle), 0, radius * Mathf.Sin(angle));
    }
}
