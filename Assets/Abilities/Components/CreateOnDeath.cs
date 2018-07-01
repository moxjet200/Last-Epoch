using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// needs to listen for the object's death
[RequireComponent(typeof(SelfDestroyer))]
public class CreateOnDeath : MonoBehaviour {

    public bool attachToCaster = false;
    public bool passOnLocationDetector = false;
    public bool passOnCreationReferences = false;
    public float increasedRadius = 0f;
    public float increasedHeight = 0f;
    public bool createOnStartInstead = false;

    // the type of rotation to pass on to the child
    public enum RotationToPassOn
    {
        None, Own, Parent
    }

    [System.Serializable]
    public struct GameObjectHolder{
        public GameObject gameObject;

        public GameObjectHolder(GameObject _gameObject) { gameObject = _gameObject; }
    }

    public RotationToPassOn rotationToPassOn = RotationToPassOn.None;

    [Header("Some Variables only affect first in list")]
    public List<GameObjectHolder> objectsToCreateOnDeath = new List<GameObjectHolder>();

	void Awake () {
        if (!createOnStartInstead){
            GetComponent<SelfDestroyer>().deathEvent += createObjects;
        }
	}

    void Start()
    {
        if (createOnStartInstead)
        {
            createObjects();
        }
    }
	
	public void createObjects()
    {
        objectsToCreateOnDeath.RemoveAll(x => x.gameObject == null);
        Transform caster = null;
        GameObject go = null;
        if (attachToCaster) {
            CreationReferences references = GetComponent<CreationReferences>();
            if (references && references.creator)
            {
                caster = GetComponent<CreationReferences>().creator.transform;
            }
        }
        if (rotationToPassOn == RotationToPassOn.Own)
        {
            foreach (GameObjectHolder holder in objectsToCreateOnDeath)
            {
                go = Instantiate(holder.gameObject, transform.position, transform.rotation);
                if (caster) { go.transform.parent = caster; go.transform.localPosition = new Vector3(0, 0, 0); }
            }
        }
        else if (rotationToPassOn == RotationToPassOn.Parent && transform.parent)
        {
            foreach (GameObjectHolder holder in objectsToCreateOnDeath)
            {
                go = Instantiate(holder.gameObject, transform.position, transform.parent.transform.rotation);
                if (caster) { go.transform.parent = caster; go.transform.localPosition = new Vector3(0, 0, 0); }
            }
        }
        else
        {
            foreach (GameObjectHolder holder in objectsToCreateOnDeath)
            {
                go = Instantiate(holder.gameObject, transform.position, Quaternion.identity);
                if (caster) { go.transform.parent = caster; go.transform.localPosition = new Vector3(0, 0, 0); }
            }
        }

        if (go && passOnLocationDetector)
        {
            LocationDetector detector = GetComponent<LocationDetector>();
            if (detector)
            {
                LocationDetector newDetector = go.GetComponent<LocationDetector>();
                if (!newDetector) { newDetector = go.AddComponent<LocationDetector>(); }
                LocationDetector.copy(newDetector, detector);
            }
        }
        if (go && passOnCreationReferences)
        {
            CreationReferences references = GetComponent<CreationReferences>();
            if (references)
            {
                CreationReferences newReferences = go.GetComponent<CreationReferences>();
                if (!newReferences) { newReferences = go.AddComponent<CreationReferences>(); }
                references.copyTo(newReferences);
            }
        }
        // apply increases to radius and height
        if (increasedRadius != 0 || increasedHeight != 0)
        {
            go.transform.localScale = new Vector3(go.transform.localScale.x * (1 + increasedRadius), go.transform.localScale.y * (1 + increasedHeight), go.transform.localScale.z * (1 + increasedRadius));
        }
    }

    // adds an object to the list to create on death
    public void add(GameObject go)
    {
        objectsToCreateOnDeath.Add(new GameObjectHolder(go));
    }
}
