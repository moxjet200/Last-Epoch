using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonWraithMutator : AbilityMutator
{
    public enum WraithTargetType
    {
        normal, onSpot, atTarget
    }

    public WraithTargetType targetting = WraithTargetType.normal;

    public List<TaggedStatsHolder.TaggableStat> statList = new List<TaggedStatsHolder.TaggableStat>();

    public List<TaggedStatsHolder.TaggableStat> corpseStatList = new List<TaggedStatsHolder.TaggableStat>();
    
    public int delayedWraiths = 0;
    public float reducedWraithInterval = 0f;

    public bool critFromManaCost = false;
    public float healOnCrit = 0f;
    public float reducedHealthDrain = 0f;
    public bool stationary = false;

    public float flameWraithChance = 0f;
    public float putridWraithChance = 0f;
    public float bloodWraithChance = 0f;
    public float increasedCastSpeed = 0f;

    [HideInInspector]
    public GameObject flameWraithPrefab = null;
    [HideInInspector]
    public GameObject putridWraithPrefab = null;
    [HideInInspector]
    public GameObject bloodWraithPrefab = null;

    BaseHealth health = null;
    BaseMana mana = null;
    StatBuffs statBuffs = null;
    SummonTracker tracker = null;

    [HideInInspector]
    public GameObject spiritEscapePrefab = null;
    Vector3 spiritEscapeOffset = Vector3.zero;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.summonWraith);
        health = GetComponent<BaseHealth>();
        mana = GetComponent<BaseMana>();

        if (health)
        {
            statBuffs = Comp<StatBuffs>.GetOrAdd(gameObject);
            tracker = Comp<SummonTracker>.GetOrAdd(gameObject);
            AbilityEventListener listener = Comp<AbilityEventListener>.GetOrAdd(gameObject);
            listener.onCritEvent += OnCrit;
        }

        // get wraith prefabs
        if (flameWraithPrefab == null)
        {
            Ability wraithAbility = Ability.getAbility(AbilityID.summonFlameWraith);
            if (wraithAbility && wraithAbility.abilityPrefab)
            {
                SummonEntityOnDeath summon = wraithAbility.abilityPrefab.GetComponent<SummonEntityOnDeath>();
                if (summon)
                {
                    flameWraithPrefab = summon.entity;
                }
            }

            wraithAbility = Ability.getAbility(AbilityID.summonPutridWraith);
            if (wraithAbility && wraithAbility.abilityPrefab)
            {
                SummonEntityOnDeath summon = wraithAbility.abilityPrefab.GetComponent<SummonEntityOnDeath>();
                if (summon)
                {
                    putridWraithPrefab = summon.entity;
                }
            }

            wraithAbility = Ability.getAbility(AbilityID.summonBloodWraith);
            if (wraithAbility && wraithAbility.abilityPrefab)
            {
                SummonEntityOnDeath summon = wraithAbility.abilityPrefab.GetComponent<SummonEntityOnDeath>();
                if (summon)
                {
                    bloodWraithPrefab = summon.entity;
                }
            }
        }

        // spirit escape prefab
        if (spiritEscapePrefab == null)
        {
            spiritEscapePrefab = PrefabList.getPrefab("spiritEscape");
        }

        base.Awake();
    }


    public void OnCrit(Ability _ability, GameObject target)
    {
        if (healOnCrit != 0 && tracker)
        {
            foreach (Summoned wraith in tracker.getMinions(ability))
            {
                if (wraith && wraith.getBaseHealth())
                {
                    wraith.getBaseHealth().Heal(healOnCrit);
                }
            }
        }
    }
    
    public override float mutateUseSpeed(float useSpeed)
    {
        return base.mutateUseSpeed(useSpeed) * (1 + increasedCastSpeed);
    }



    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        // get a reference to the child's SummonEntityOnDeath component, this the component that makes the wolf
        SummonEntityOnDeath summoner = abilityObject.GetComponent<SummonEntityOnDeath>();
        
        // add additional stats
        summoner.statList.AddRange(statList);

        // crit on cast buff
        if (critFromManaCost && mana && statBuffs)
        {
            TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.CriticalChance, null);
            stat.increasedValue = mana.getManaCost(ability) * 0.01f;

            statBuffs.addTaggedBuff(new TaggedBuff(stat, 4f, "Summon wraith mana crit"));
        }

        // add corpse consumption stats
        bool corpseConsumed = false;
        if (corpseStatList != null && corpseStatList.Count > 0 && health && health.alignmentManager && health.alignmentManager.alignment != null)
        {
            float closestDistance = float.MaxValue;
            Dying closestDying = null;
            float distance = 0f;

            // check all corpses to find the closest
            foreach (Dying dying in Dying.all)
            {
                // check that the entity is dying and is an enemy
                if (dying.isDying() && dying.myHealth && dying.myHealth.alignmentManager && health.alignmentManager.alignment.foes.Contains(dying.myHealth.alignmentManager.alignment))
                {
                    // find the distance
                    distance = Maths.manhattanDistance(dying.transform.position, targetLocation);
                    // don't consume very distant corpses
                    if (distance <= 8f)
                    {
                        // if a closest one hasn't been found yet this is the closest
                        if (closestDying == null)
                        {
                            closestDying = dying;
                            closestDistance = distance;
                        }
                        // otherwise compare distances
                        else
                        {
                            if (distance < closestDistance)
                            {
                                closestDying = dying;
                                closestDistance = distance;
                            }
                        }
                    }
                }
            }

            // consume the closest corpse
            if (closestDying)
            {
                closestDying.setDelays(0f, 0f);
                corpseConsumed = true;
                // apply the stats
                summoner.statList.AddRange(corpseStatList);
                // create a death vfx
                Instantiate(spiritEscapePrefab).transform.position = closestDying.transform.position + spiritEscapeOffset;
            }
        }



        float realBloodWraithChance = 0f;
        if (corpseConsumed) { realBloodWraithChance = bloodWraithChance; }
        // summon a different type of wraith
        if (flameWraithChance != 0 || realBloodWraithChance != 0 || putridWraithChance != 0)
        {
            float rand = Random.Range(0f, 1f);
            if (rand < flameWraithChance)
            {
                summoner.entity = flameWraithPrefab;
            }
            else if (rand < flameWraithChance + realBloodWraithChance)
            {
                summoner.entity = bloodWraithPrefab;
            }
            else if (rand < flameWraithChance + realBloodWraithChance + putridWraithChance)
            {
                summoner.entity = putridWraithPrefab;
            }
        }

        if (targetting == WraithTargetType.onSpot)
        {
            StartsTowardsTarget component = abilityObject.GetComponent<StartsTowardsTarget>();
            if (component)
            {
                component.active = false;
            }
            
            if (summoner)
            {
                summoner.distance = 0.5f;
            }
        }

        if (targetting == WraithTargetType.atTarget)
        {
            StartsTowardsTarget component = abilityObject.GetComponent<StartsTowardsTarget>();
            if (component)
            {
                component.active = false;
            }

            abilityObject.AddComponent<StartsAtTarget>();
        }

        // delayed wraith casts
        if (delayedWraiths > 0)
        {
            CastAfterDuration cad = abilityObject.AddComponent<CastAfterDuration>();
            cad.ability = ability;
            cad.interval = 0.5f * (1 - reducedWraithInterval);
            cad.limitCasts = true;
            cad.remainingCasts = delayedWraiths;

            DestroyAfterDuration dad = abilityObject.GetComponent<DestroyAfterDuration>();
            dad.duration = (0.5f * (1 + delayedWraiths)) * (1f - reducedWraithInterval);


            SummonWraithMutator mutator = abilityObject.AddComponent<SummonWraithMutator>();
            
            // copy stats
            mutator.reducedHealthDrain = reducedHealthDrain;
            mutator.stationary = stationary;
            mutator.flameWraithChance = flameWraithChance;
            mutator.bloodWraithChance = realBloodWraithChance;
            mutator.putridWraithChance = putridWraithChance;

            // wraith prefabs
            mutator.flameWraithPrefab = flameWraithPrefab;
            mutator.bloodWraithPrefab = bloodWraithPrefab;
            mutator.putridWraithPrefab = putridWraithPrefab;

            // delayed wraith changes
            mutator.targetting = WraithTargetType.onSpot;
            mutator.delayedWraiths = 0;
            mutator.reducedWraithInterval = 0f;
            mutator.critFromManaCost = false;
            mutator.healOnCrit = 0f;

            // stats and corpse stats (does not consume multiple corpses)
            mutator.statList.AddRange(statList);
            if (corpseConsumed)
            {
                mutator.statList.AddRange(corpseStatList);
            }

        }

        // change the adapter
        WraithAdapter adapter = abilityObject.AddComponent<WraithAdapter>();

        adapter.reducedHealthDrain = reducedHealthDrain;
        adapter.stationary = stationary;

        return abilityObject;
    }
}
