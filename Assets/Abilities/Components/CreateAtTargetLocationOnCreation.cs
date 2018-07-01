using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// needs to know the object's location
[RequireComponent(typeof(LocationDetector))]
public class CreateAtTargetLocationOnCreation : MonoBehaviour {

    bool placed = false;
    public Vector3 offset = new Vector3(0, 0, 0);

    [System.Serializable]
    public struct GameObjectHolder{
        public GameObject gameObject;
        public bool destroyWhenThisDies;
    }

    public List<GameObjectHolder> objectsToCreate = new List<GameObjectHolder>();

    List<GameObject> objectsToDestroyOnDeath = new List<GameObject>();

    void Awake()
    {
        if (GetComponent<SelfDestroyer>())
        {
            GetComponent<SelfDestroyer>().deathEvent += destroyObjects;
        }
    }

    void Update () {
        if (!placed)
        {
            placed = true;
            createObjects();
        }
	}
	
	public void createObjects()
    {
        Vector3 targetLocation = GetComponent<LocationDetector>().targetLocation;
        foreach(GameObjectHolder holder in objectsToCreate)
        {
            GameObject go = Instantiate(holder.gameObject, targetLocation + offset, Quaternion.identity);
            if (holder.destroyWhenThisDies)
            {
                objectsToDestroyOnDeath.Add(go);
            }
        }
    }

    public void destroyObjects()
    {
        for(int i=0; i< objectsToDestroyOnDeath.Count; i++)
        {
            if (objectsToDestroyOnDeath[i].GetComponent<Dying>())
            {
                objectsToDestroyOnDeath[i].GetComponent<StateController>().changeState(objectsToDestroyOnDeath[i].GetComponent<Dying>());
            }
            else if (objectsToDestroyOnDeath[i].GetComponent<SelfDestroyer>())
            {
                objectsToDestroyOnDeath[i].GetComponent<SelfDestroyer>().die();
            }
            else
            {
                Destroy(objectsToDestroyOnDeath[i]);
            }
        }
    }
}
