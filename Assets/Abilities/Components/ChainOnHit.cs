using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HitDetector))]
[RequireComponent(typeof(AbilityObjectConstructor))]
[RequireComponent(typeof(TargetFinder))]
public class ChainOnHit : MonoBehaviour {

    HitDetector detector = null;
    public Ability abilityToChain = null;
    public float range = 6f;
    public bool limitNumberOfChains = true;
    [Tooltip("On the initial ability object this is the total number of times it will chain")]
    public float chainsRemaining = 3;
    public bool hitsAllies = false;
    public bool destroyAfterChainAttempt = false;
    public bool destroyAfterSuccessfulChainAttempt = false;
    public bool cannotHitSame = false;
    public Vector3 offset = Vector3.zero;

    public bool hasChained = false;

    void Awake()
    {
        if (!GetComponent<AbilityObjectConstructor>()) { gameObject.AddComponent<AbilityObjectConstructor>(); }
    }

	// Use this for initialization
	void Start () {
        detector = GetComponent<HitDetector>();
        if (hitsAllies) { detector.newAllyHitEvent += chain; }
        else { detector.newEnemyHitEvent += chain; }
    }

    public void chain(GameObject hit)
    {
        // make sure this specific ability object only creates one chain
        if (hasChained) { return; }
        else { hasChained = true; }

        // check that there are chains remaining
        if (limitNumberOfChains)
        {
            if (chainsRemaining <= 0)
            {
                if (destroyAfterChainAttempt)
                {
                    SelfDestroyer destroyer = GetComponent<SelfDestroyer>();
                    if (destroyer) { destroyer.die(); }
                }
                return;
            }


        }

        // find potential enemies to hit
        TargetFinder targetFinder = GetComponent<TargetFinder>();
        AlignmentManager am = GetComponent<AlignmentManager>();
        BaseHealth[] potentialTargetArray = targetFinder.getPotentialTargets(am.alignment, abilityToChain.targetsAllies, null, new LifeStateHolder(abilityToChain.requiredLifeState));

        // exclude the object hit
        List<GameObject> potentialTargets = new List<GameObject>();
        foreach (BaseHealth health in potentialTargetArray)
        {
            if (health.gameObject != hit)
            {
                potentialTargets.Add(health.gameObject);
            }
        }

        // if there is a shared hit detector remove objects that have already been hit
        bool hasSharedHitDetector = (detector.sharedHitDetector != null);
        if (hasSharedHitDetector)
        {
            foreach (GameObject removeObj in detector.sharedHitDetector.enemiesHit)
            {
                potentialTargets.Remove(removeObj);
            }
        }

        // get the positions of targets in range
        List<GameObject> outOfRangeObjects = new List<GameObject>();
        Vector3 position = transform.position;
        foreach (GameObject target in potentialTargets)
        {
            if (Vector3.Distance(position, target.transform.position) > range)
            {
                outOfRangeObjects.Add(target);
            }
        }
        foreach (GameObject removeObject in outOfRangeObjects)
        {
            potentialTargets.Remove(removeObject);
        }

        // if there are no targets remaining return
        if (potentialTargets.Count <= 0)
        {
            if (destroyAfterChainAttempt)
            {
                SelfDestroyer destroyer = GetComponent<SelfDestroyer>();
                if (destroyer) { destroyer.die(); }
            }
            return;
        }
        // if there is just one target chain to that
        else if (potentialTargets.Count == 1) {
            chainTo(potentialTargets[0], hit);
            if (destroyAfterSuccessfulChainAttempt)
            {
                SelfDestroyer destroyer = GetComponent<SelfDestroyer>();
                if (destroyer) { destroyer.die(); }
            }
        }
        // if there are multiple targets, pick a random one
        else
        {
            int randTarget = Random.Range(0, potentialTargets.Count - 1);
            chainTo(potentialTargets[randTarget], hit);
            if (destroyAfterSuccessfulChainAttempt)
            {
                SelfDestroyer destroyer = GetComponent<SelfDestroyer>();
                if (destroyer) { destroyer.die(); }
            }
        }

        if (destroyAfterChainAttempt)
        {
            SelfDestroyer destroyer = GetComponent<SelfDestroyer>();
            if (destroyer) { destroyer.die(); }
        }
    }


    protected void chainTo(GameObject target, GameObject from)
    {
        AbilityObjectConstructor aoc = GetComponent<AbilityObjectConstructor>();
        GameObject newAbilityObject = aoc.constructAbilityObject(abilityToChain, transform.position, target.transform.position + offset);
        // reduce the chains remaining
        if (limitNumberOfChains)
        {
            ChainOnHit newChain = newAbilityObject.GetComponent<ChainOnHit>();
            if (!newChain) { newChain = newAbilityObject.AddComponent<ChainOnHit>(); }
            newChain.chainsRemaining = chainsRemaining - 1;
            newChain.abilityToChain = abilityToChain;
            newChain.range = range;
            newChain.destroyAfterChainAttempt = destroyAfterChainAttempt;
            newChain.cannotHitSame = cannotHitSame;
            newChain.offset = offset;
        }

        if (cannotHitSame)
        {
            HitDetector detector = newAbilityObject.GetComponent<HitDetector>();
            if (hitsAllies) {
                detector.alliesHit.Add(from);
                if (detector.sharedHitDetector)
                {
                    detector.sharedHitDetector.alliesHit.Add(from);
                }
            }
            else {
                detector.enemiesHit.Add(from);
                if (detector.sharedHitDetector)
                {
                    detector.sharedHitDetector.enemiesHit.Add(from);
                }
            }
        }
    }


}
