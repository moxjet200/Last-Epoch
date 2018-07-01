using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The AbilityObjectConstructor looks for ability mutators on the object that is casting and runs the mutate method of the matching one
[RequireComponent(typeof(AbilityMutatorManager))]
public abstract class AbilityMutator : MonoBehaviour {
    
    public Ability ability = null;
    [HideInInspector]
    public bool changeLocation = false;
    [HideInInspector]
    public Vector3 newLocation;
    [HideInInspector]
    public bool changeTargetLocation = false;
    [HideInInspector]
    public Vector3 newTargetLocation;
    
    public float addedManaCostDivider = 0f;
    public float increasedManaCost = 0f;
    public float addedCharges = 0f;
    public float addedChargeRegen = 0f;

    float previousAddedCharges = 0f;
    float previousAddedChargeRegen = 0f;
    AbilityMutatorManager mutatorManager = null;

    [HideInInspector]
    public ChargeManager chargeManager = null;

    protected virtual void Awake()
    {
        // get a reference to the mutator manager
        mutatorManager = GetComponent<AbilityMutatorManager>();
        if (mutatorManager == null) { mutatorManager = gameObject.AddComponent<AbilityMutatorManager>(); }
        // add to list
        mutatorManager.mutators.Add(this);
        // add to dictionary
        if (ability != null)
        {
            mutatorManager.addMutator(this);
        }
    }

    protected virtual void Start() { }

    public virtual void changeAbility(Ability newAbility)
    {
        Ability oldAbility = ability;
        ability = newAbility;
        if (mutatorManager)
        {
            // remove the old ability key from the dictionary
            if (oldAbility != null)
            {
                mutatorManager.removeMutator(this);
            }
            // add the new entry
            mutatorManager.addMutator(this);
        }
    }

    protected virtual void OnDestroy()
    {
        if (mutatorManager)
        {
            mutatorManager.mutators.Remove(this);
            mutatorManager.removeMutator(this);
        }
    }

    public virtual float getIncreasedManaCost()
    {
        return increasedManaCost;
    }
	
	// Update is called once per frame
	public virtual void Update () {
        // update the added charges and added charge recovery in the charge manager if necessary
        if (chargeManager)
        {
            if (previousAddedCharges != addedCharges || previousAddedChargeRegen != addedChargeRegen)
            {
                chargeManager.updateChargeInfo(ability);
                previousAddedChargeRegen = addedChargeRegen;
                previousAddedCharges = addedCharges;
                if (chargeManager.abilitiesStartOffCooldown)
                {
                    chargeManager.setAbilityChargesToMax(ability);
                }
            }
        }
        else { chargeManager = GetComponent<ChargeManager>(); }
	}

    public virtual List<Tags.AbilityTags> getUseTags(){
        List<Tags.AbilityTags> newList = new List<Tags.AbilityTags>();
        newList.AddRange(ability.useTags);
        return newList;
    }

    public static AbilityMutator getMutator(GameObject go, Ability _ability)
    {
        foreach (AbilityMutator mutator in go.GetComponents<AbilityMutator>())
        {
            if (mutator.ability == _ability) { return mutator; }
        }
        return null;
    }

    public virtual float mutateUseSpeed(float useSpeed)
    {
        return useSpeed;
    }

    public virtual int mutateDelayedCasts(int defaultCasts)
    {
        return defaultCasts;
    }

    public virtual float mutateDelatedCastDuration(float defaultDuration)
    {
        return defaultDuration;
    }
    
    public float getCooldown()
    {
        return (1f / (ability.chargesGainedPerSecond + addedChargeRegen));
    }

    public virtual float mutateStopRange()
    {
        return ability.stopRange;
    }

    public virtual bool mutateRequiresShield()
    {
        return ability.requiresSheild;
    }

    public virtual bool isChanneled()
    {
        return ability.channelled;
    }

    public virtual AbilityAnimation getAbilityAnimation()
    {
        return ability.animation;
    }

    public abstract GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation);
}
