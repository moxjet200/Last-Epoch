using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// creates a CastAfterDuration when an actor collides with the collider
[RequireComponent(typeof(HitDetector))]
public class MineTrigger : MonoBehaviour {

    public enum ActorType
    {
        Enemy, Ally, Any
    }

    // the type of actor that triggers this mine
    public ActorType triggerType = ActorType.Enemy;
    // the ability to cast when triggered
    public Ability ability = null;
    // the time between the actor coming into range and the ability triggering
    public float castDelay = 0f;

    bool triggered = false;

    // Use this for initialization
    void Start () {
        HitDetector hitDetector = GetComponent<HitDetector>();
        if (triggerType == ActorType.Enemy || triggerType == ActorType.Any) { hitDetector.newEnemyHitEvent += Trigger; }
        if (triggerType == ActorType.Ally || triggerType == ActorType.Any) { hitDetector.newAllyHitEvent += Trigger; }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Trigger(GameObject triggerObject)
    {
        if (!triggered)
        {
            CreateAbilityObjectOnDeath caood = gameObject.AddComponent<CreateAbilityObjectOnDeath>();
            caood.abilityToInstantiate = ability;
            DestroyAfterDuration destroyer = gameObject.AddComponent<DestroyAfterDuration>();
            destroyer.duration = castDelay;
        }
    }

}
