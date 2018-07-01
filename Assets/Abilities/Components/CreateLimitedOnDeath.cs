using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateLimitedOnDeath : MonoBehaviour {

    [System.Serializable]
    public struct GameObjectAndDuration
    {
        public GameObject gameObject;
        public float duration;
    }

    public List<GameObjectAndDuration> objectsToCreateOnDeath;

    void Awake()
    {
        GetComponent<SelfDestroyer>().deathEvent += createObjects;
    }

    public void createObjects()
    {
        foreach (GameObjectAndDuration holder in objectsToCreateOnDeath)
        {
            GameObject gameObject = Instantiate(holder.gameObject, transform.position, Quaternion.identity);
            if (gameObject.GetComponent<Dying>()) { gameObject.AddComponent<DieAfterDelay>().timeUntilDeath = holder.duration; }
            if (gameObject.GetComponent<SelfDestroyer>()) { gameObject.AddComponent<DestroyAfterDuration>().duration = holder.duration; }
        }
    }
}
