using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AbilityObjectConstructor))]
public class RetaliateWhenParentHit : MonoBehaviour {

    public enum SourceOfAbilityObjectConstructor
    {
        This, Parent
    }

    public Ability ability;

    GameObject parent;
    public int damageTakenSinceTrigger = 0;
    public int damageTakenTrigger = 10;
    public SourceOfAbilityObjectConstructor sourceOfAbility = SourceOfAbilityObjectConstructor.This;
    public bool limitRetaliations = false;
    public int retaliationsRemaining = 1;
    public bool destroyAfterLastRetaliation = false;
    public bool onlyCountHitDamage = false;

    bool retaliated = false;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		if (!parent)
        {
            parent = transform.parent.gameObject;
            if (!parent.GetComponent<ProtectionClass>() || !parent.GetComponent<TargetFinder>())
            {
                Debug.LogError(name + " can only be attached to a parent with a base health and a target finder");
            }
            else
            {
                if (onlyCountHitDamage) { parent.GetComponent<ProtectionClass>().hitDamageEvent += parentTookDamage; }
                else { parent.GetComponent<ProtectionClass>().damageEvent += parentTookDamage; }
            }
        }
	}

    void OnDestroy()
    {
        if (parent)
        {
            parent.GetComponent<ProtectionClass>().damageEvent -= parentTookDamage;
            parent.GetComponent<ProtectionClass>().hitDamageEvent -= parentTookDamage;
        }
    }

    public void parentTookDamage(int damage)
    {
        damageTakenSinceTrigger += damage;
        if (damageTakenSinceTrigger > damageTakenTrigger)
        {
            damageTakenSinceTrigger = damageTakenSinceTrigger % damageTakenTrigger;
            if (damageTakenSinceTrigger >= damageTakenTrigger) { damageTakenSinceTrigger = damageTakenTrigger - 1; }
            Transform target = parent.GetComponent<TargetFinder>().updateTarget();
            Vector3 targetPosition = transform.forward;
            if (target) { targetPosition = new Vector3(target.position.x, target.position.y + 1f, target.position.z); }
            else { targetPosition = new Vector3(transform.forward.x, transform.forward.y + 1f, transform.forward.z); }
            if (sourceOfAbility == SourceOfAbilityObjectConstructor.This)
            {
                GetComponent<AbilityObjectConstructor>().constructAbilityObject(ability, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z),
                    targetPosition);
            }
            else
            {
                UsingAbility usingAbility = GetComponentInParent<UsingAbility>();
                if (usingAbility) { usingAbility.UseAbility(ability, targetPosition, false, false); }
            }
            if (limitRetaliations)
            {
                retaliationsRemaining--;
            }
            retaliated = true;
        }
    }

    public void LateUpdate()
    {
        if (destroyAfterLastRetaliation && limitRetaliations && retaliationsRemaining <= 0 && retaliated)
        {
            SelfDestroyer destroyer = GetComponent<SelfDestroyer>();
            if (!destroyer) { destroyer = gameObject.AddComponent<SelfDestroyer>(); }
            if (destroyer) { destroyer.die(); }
        }
    }
}
