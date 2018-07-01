using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalAbility : MonoBehaviour {

    public Ability ability;

    public GameObject currentPortal;

    public Vector3 portalLocation = Vector3.zero;
    public string portalScene = "";

    // the range at which spawners are disabled when going back through the portal
    float disableSpawnerRange = 13f;

    bool initialised = false;

    bool moveOnNextSceneLoad = false;

    public void Awake()
    {
        if (!initialised) { initialise(); }
    }

    public void Start()
    {
        if (!initialised) { initialise(); }
    }

    public void OnDestroy()
    {
        if (!SceneLoader.instance) { return; }
        SceneLoader.instance.sceneLoadedEvent -= sceneLoaded;
    }

    public void initialise()
    {
        if (!SceneLoader.instance) { return; }
        SceneLoader.instance.sceneLoadedEvent += sceneLoaded;
        initialised = true;
    }

    public void sceneLoaded(string scenename)
    {
        if (moveOnNextSceneLoad) { moveToPortalLocation(); }
        else if (scenename == "PortalBack")
        {
            moveOnNextSceneLoad = true;
        }
    }

    public void moveToPortalLocation()
    {
        ActorMover.moveActor(gameObject, portalLocation, true);
        moveOnNextSceneLoad = false;

        // set the player's respawn point
        GetComponent<DyingPlayer>().respawnPoint = transform.position;
        // stop the player moving
        if (GetComponent<MovementFromAbility>())
        {
            GetComponent<MovementFromAbility>().onReachingDestination();
        }
        // stop the player's ability use
        if (GetComponent<UsingAbility>())
        {
            GetComponent<UsingAbility>().stopUsingAbility();
        }
        // reset the player's position recorder
        if (GetComponent<PositionRecorder>())
        {
            GetComponent<PositionRecorder>().reset();
        }
        // disable spawns that are too near to where the player has entered the area
        foreach (SpawnController spawn in SpawnController.all)
        {
            if (Vector3.Distance(spawn.transform.position, transform.position) < disableSpawnerRange)
            {
                spawn.deactivate();
            }
        }
    }

}
