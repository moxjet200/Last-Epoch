using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// requires an alignment manager in order to give the ability object an alignment
[RequireComponent(typeof(AlignmentManager))]
public class AbilityObjectConstructor : RequiresTaggedStats{

    AbilityMutatorManager mutatorManager = null;
    AlignmentManager myAlignmentManager = null;
    TaggedStatsHolder myTaggedStatsHolder = null;

    // stay null unless this is an ability object
    CreationReferences myCreationReferences = null;
    HitDetector myHitDetector = null;

    // used for getting references to skills as they are created
    public delegate void AbilityObjectCreatedAction(Ability ability, GameObject abilityObject);
    public AbilityObjectCreatedAction abilityObjectCreatedEvent;

    List<AbilityMutator> mutators = new List<AbilityMutator>();

    // Use this for initialization
    void Start () {
        mutatorManager = GetComponent<AbilityMutatorManager>();
        myAlignmentManager = GetComponent<AlignmentManager>();
        myTaggedStatsHolder = GetComponent<TaggedStatsHolder>();

        myCreationReferences = GetComponent<CreationReferences>();
        myHitDetector = GetComponent<HitDetector>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public GameObject constructAbilityObject(Ability ability, Vector3 location, Vector3 targetLocation, GameObject overridePrefab = null, bool InheritSharedHitDetector = true)
    {
        // create the ability object
        GameObject abilityObject = null;
        if (overridePrefab == null)
        {
            // if there is no override prefab instantiate the ability's ability prefab
            abilityObject = Instantiate(ability.abilityPrefab, location, Quaternion.Euler(targetLocation - location));
        }
        else{// othewise instrantiate the override prefab
            abilityObject = Instantiate(overridePrefab, location, Quaternion.Euler(targetLocation - location));
        }

        // apply the relevant mutator if this entity has one
        if (mutatorManager)
        {
            mutators = mutatorManager.getMutators(ability);
            if (mutators != null)
            {
                foreach (AbilityMutator mutator in mutators)
                {
                    abilityObject = mutator.Mutate(abilityObject, location, targetLocation);
                    if (mutator.changeLocation) { targetLocation = mutator.newLocation; }
                    if (mutator.changeTargetLocation) { targetLocation = mutator.newTargetLocation; }
                }
            }
        }

        // move the ability object if necessary
        foreach (DefineStartLocation defineStartLocation in abilityObject.GetComponents<DefineStartLocation>())
        {
            if (defineStartLocation.active)
            {
                defineStartLocation.setLocation(location, targetLocation);
                // the direction from the cast point needs to be maintained
                if (defineStartLocation.maintainDirectionFromCastPoint())
                {
                    targetLocation = abilityObject.transform.position + targetLocation - location;
                }
            }
        }


        // initialise a location detector if necessary
        LocationDetector locationDetector = abilityObject.GetComponent<LocationDetector>();
        if (locationDetector) {
            locationDetector.startLocation = abilityObject.transform.position;
            locationDetector.targetLocation = targetLocation;
        }

        // rotate the ability object
        abilityObject.transform.LookAt(targetLocation);

        // give the ability object its alignment
        AlignmentManager abilityAlignmentManager = abilityObject.GetComponent<AlignmentManager>();
        if (!abilityAlignmentManager) { abilityAlignmentManager = abilityObject.AddComponent<AlignmentManager>(); }
        abilityAlignmentManager.alignment = myAlignmentManager.alignment;

        // give the ability object its creation references
        CreationReferences abilityCreationReferences = abilityObject.GetComponent<CreationReferences>();
        if (!abilityCreationReferences) { abilityCreationReferences = abilityObject.AddComponent<CreationReferences>(); }
        abilityCreationReferences.thisAbility = ability;
        abilityCreationReferences.locationCreatedFrom = location;
        // if this an ability object then the creator is this objects creator, otherwise the creator is this object
        if (myCreationReferences && GetComponent<AbilityObjectIndicator>()) { abilityCreationReferences.creator = myCreationReferences.creator; }
        else { abilityCreationReferences.creator = gameObject; }

        // check whether there should be a shared hit detector
        HitDetector abilityHitDetector = abilityObject.GetComponent<HitDetector>();
        if (abilityHitDetector)
        {
            SharedHitDetector sharedHitDetector = null;
            if (myHitDetector && myHitDetector.sharedHitDetector && InheritSharedHitDetector) { sharedHitDetector = myHitDetector.sharedHitDetector; }
            // create a shared hit detector
            else if (ability.sharedHitDetector) {
                GameObject newGameObject = new GameObject();
                sharedHitDetector = newGameObject.AddComponent<SharedHitDetector>();
                sharedHitDetector.gameObject.AddComponent<SelfDestroyer>();
                sharedHitDetector.name = abilityObject.name + "'s shared hit detector";
            }
            if (sharedHitDetector != null && !abilityHitDetector.cannotHaveSharedhitDetector) { abilityHitDetector.sharedHitDetector = sharedHitDetector; }
        }
        
        // build the damage stats for each damage stats holder
        foreach (DamageStatsHolder damageStatsHolder in abilityObject.GetComponents<DamageStatsHolder>())
        {
            damageStatsHolder.damageStats = DamageStats.buildDamageStats(damageStatsHolder, GetComponent<TaggedStatsHolder>());
        }
        
        // attach any on hit status appliers if this ability deals damage on hit
        if (myTaggedStatsHolder && abilityObject.GetComponent<DamageEnemyOnHit>() && !abilityObject.GetComponent<CannotApplyAdditionalStatuses>())
        {
            ChanceToApplyStatusOnEnemyHit applier;
            float poisonChance = myTaggedStatsHolder.GetStatValue(Tags.Properties.PoisonChance, ability.useTags);
            if (poisonChance > 0)
            {
                applier = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
                applier.statusEffect = StatusEffectList.getEffect(StatusEffectID.Poison);
                applier.chance = poisonChance;
                applier.canApplyToSameEnemyAgain = false;
            }
            float igniteChance = myTaggedStatsHolder.GetStatValue(Tags.Properties.IgniteChance, ability.useTags);
            if (igniteChance > 0)
            {
                applier = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
                applier.statusEffect = StatusEffectList.getEffect(StatusEffectID.Ignite);
                applier.chance = igniteChance;
                applier.canApplyToSameEnemyAgain = false;
            }
            float chillChance = myTaggedStatsHolder.GetStatValue(Tags.Properties.ChillChance, ability.useTags);
            if (chillChance > 0)
            {
                applier = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
                applier.statusEffect = StatusEffectList.getEffect(StatusEffectID.Chill);
                applier.chance = chillChance;
                applier.canApplyToSameEnemyAgain = false;
            }
            float slowChance = myTaggedStatsHolder.GetStatValue(Tags.Properties.SlowChance, ability.useTags);
            if (slowChance > 0)
            {
                applier = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
                applier.statusEffect = StatusEffectList.getEffect(StatusEffectID.Slow);
                applier.chance = slowChance;
                applier.canApplyToSameEnemyAgain = false;
            }
            float blindChance = myTaggedStatsHolder.GetStatValue(Tags.Properties.BlindChance, ability.useTags);
            if (blindChance > 0)
            {
                applier = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
                applier.statusEffect = StatusEffectList.getEffect(StatusEffectID.Blind);
                applier.chance = blindChance;
                applier.canApplyToSameEnemyAgain = false;
            }

            float bleedChance = myTaggedStatsHolder.GetStatValue(Tags.Properties.BleedChance, ability.useTags);
            if (bleedChance > 0)
            {
                applier = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
                applier.statusEffect = StatusEffectList.getEffect(StatusEffectID.Bleed);
                applier.chance = bleedChance;
                applier.canApplyToSameEnemyAgain = false;
            }
            float shockChance = myTaggedStatsHolder.GetStatValue(Tags.Properties.ShockChance, ability.useTags);
            if (shockChance > 0)
            {
                applier = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
                applier.statusEffect = StatusEffectList.getEffect(StatusEffectID.Shock);
                applier.chance = shockChance;
                applier.canApplyToSameEnemyAgain = false;
            }
        }


        // if the ability has an ability object constructor give it a tagged stats holder with your tagged stats
        if (myTaggedStatsHolder && abilityObject.GetComponent<RequiresTaggedStats>())
        {
            TaggedStatsHolder holder = abilityObject.GetComponent<TaggedStatsHolder>();
            if (holder == null) { holder = abilityObject.AddComponent<TaggedStatsHolder>(); }
			holder.simpleStats.AddRange(myTaggedStatsHolder.simpleStats);
			holder.taggedStats.AddRange(myTaggedStatsHolder.taggedStats);
        }

        // set the direction if there is an ability mover
        if (abilityObject.GetComponent<AbilityMover>())
        {
            abilityObject.GetComponent<AbilityMover>().positionDelta = Vector3.Normalize(targetLocation - location);
            // if the ability object defines its own start direction then let it
            if (abilityObject.GetComponent<DefineStartDirection>())
            {
                abilityObject.GetComponent<DefineStartDirection>().setDirection();
            }
        }

        // run any on creation methods
        foreach(OnCreation component in abilityObject.GetComponents<OnCreation>())
        {
            if (component.runOnCreation)
            {
                component.onCreation();
            }
        }

        // invoke the event
        if (abilityObjectCreatedEvent != null)
        {
            abilityObjectCreatedEvent.Invoke(ability, abilityObject);
        }

        return abilityObject;
    }

    
    public static AbilityObjectConstructor GetOrAdd(GameObject _gameObject)
    {
        if (!_gameObject) { return null; }
        AbilityObjectConstructor aoc = _gameObject.GetComponent<AbilityObjectConstructor>();
        if (aoc) { return aoc; }
        return _gameObject.AddComponent<AbilityObjectConstructor>();
    }

    
}
