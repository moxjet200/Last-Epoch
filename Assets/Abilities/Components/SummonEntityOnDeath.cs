using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct FollowRangeAndPriority
{
    public float priority;
    public float range;
}

[RequireComponent(typeof(SelfDestroyer))]
[RequireComponent(typeof(AbilityObjectConstructor))]
public class SummonEntityOnDeath : MonoBehaviour {
    
    public GameObject entity;

    public float numberToSummon = 1f;

    public Vector3 displacement = new Vector3(0,0,0);

    public float distance = 0f;

    public bool followsCreator = false;

    public List<FollowRangeAndPriority> followRangesAndPriorities = new List<FollowRangeAndPriority>();

    public bool limitDuration = false;

    public float duration = 5f;

    public bool attachCreationReferences = false;

    public bool passOnAlignment = true;

    public bool giveCustomName = false;

    public List<string> customNames = new List<string>();

    public bool giveCustomTag = false;

    public string customTag = "";
    
    [Tooltip("requires attachCreationReferences, requires summon to have a prefab reference")]
    public bool limitNumber = false;

    public int limit = 0;

    public List<Ability> abilitiesThatCountForLimit = new List<Ability>();

    public List<TaggedStatsHolder.TaggableStat> statList = new List<TaggedStatsHolder.TaggableStat>();

    public bool movesAwayFromSummonerWhenClose = false;

    public bool singeCardForAllMinions = false;

    /*
    public bool applyCustomMaterial = false;

    public Material customMaterial;

    public bool giveCustomAbility = false;

    public Ability customAbility;

    public bool giveCustomRange = false;

    public float customEngageRange = 4f;

    public float customMaxRange = 6f;
    */

    // Use this for initialization
    void Start () {
        GetComponent<SelfDestroyer>().deathEvent += Summon;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Summon()
    {
        for (int i = 0; i < numberToSummon; i++)
        {
            // get a reference to this objects creation references
            CreationReferences myReferences = GetComponent<CreationReferences>();

            // remove previous summons if necessary
            if (limitNumber && myReferences && attachCreationReferences && entity.GetComponent<Prefabs>())
            {
                abilitiesThatCountForLimit.Add(myReferences.thisAbility);
                List<GameObject> existingSummons = new List<GameObject>();
                foreach (Summoned summoned in Summoned.all)
                {
                    if (abilitiesThatCountForLimit.Contains(summoned.references.thisAbility))
                    {
                        if (summoned.references.creator = myReferences.creator)
                        {
                            existingSummons.Add(summoned.gameObject);
                        }
                    }
                }

                // components for removing summons
                SelfDestroyer selfDestroyer = null;
                Dying dying = null;

                while (existingSummons.Count >= limit)
                {
                    GameObject SummonToKill = getLowestPriority(existingSummons);
                    existingSummons.Remove(SummonToKill);

                    selfDestroyer = SummonToKill.GetComponent<SelfDestroyer>();
                    dying = SummonToKill.GetComponent<Dying>();

                    // death events
                    if (dying && dying.getController())
                    {
                        dying.unsummoned = true;
                        dying.die();
                    }

                    // self destroyer events
                    if (selfDestroyer)
                    {
                        selfDestroyer.die();
                    }
                    
                    // if neither exist, resort to destroying the game object
                    if (!dying && !selfDestroyer) { Destroy(SummonToKill); }
                }
            }

            // decide the position
            Vector3 pos = displacement;
            if (distance > 0)
            {
                float angle = Random.Range(0, Mathf.PI * 2);
                pos = new Vector3(distance * Mathf.Cos(angle), displacement.y, distance * Mathf.Sin(angle));
            }

            pos = transform.position + pos;
            pos = new Vector3(pos.x, getY(pos), pos.z);

            // create the entity
            GameObject summon = Instantiate(entity, pos, Quaternion.Euler(0, 0, 0));

            // adapt the entity
            foreach (EntityAdapter adapter in GetComponents<EntityAdapter>())
            {
                summon = adapter.adapt(summon);
            }

            // attach creation references if necessary, if not then update them
            CreationReferences references = null;
            if (attachCreationReferences && myReferences != null)
            {
                if (!summon.GetComponent<CreationReferences>())
                {
                    references = summon.AddComponent<CreationReferences>();
                }
                else
                {
                    references = summon.GetComponent<CreationReferences>();
                }
                references.creator = myReferences.creator;
                references.thisAbility = myReferences.thisAbility;
            }

            // add a summoned component
            Summoned summonedComponent = summon.AddComponent<Summoned>();
            summonedComponent.singeCardForAllMinions = singeCardForAllMinions;

            // initialise the summoned component
            summonedComponent.references = references;
            summonedComponent.initialise();

            //adds recent events tracker
            summon.AddComponent<CharacterStatusTracker>();

            // get a reference to the minion's stats
            BaseStats stats = summon.GetComponent<BaseStats>();

            // give it damage bonuses based on a tagged stats holder
            if (GetComponent<TaggedStatsHolder>() && stats)
            {
                TaggedStatsHolder holder = GetComponent<TaggedStatsHolder>();
                foreach (TaggedStatsHolder.TaggableStat ts in holder.taggedStats)
                {
                    List<Tags.AbilityTags> newTagList = new List<Tags.AbilityTags>();
                    newTagList.AddRange(ts.tagList);
                    if (newTagList.Contains(Tags.AbilityTags.Minion))
                    {
                        newTagList.Remove(Tags.AbilityTags.Minion);

                        stats.ChangeStatModifier(ts.property, ts.addedValue, BaseStats.ModType.ADDED, newTagList);
                        stats.ChangeStatModifier(ts.property, ts.increasedValue, BaseStats.ModType.INCREASED, newTagList);
                        foreach (float value in ts.moreValues)
                        {
                            stats.ChangeStatModifier(ts.property, value, BaseStats.ModType.MORE, newTagList);
                        }
                        foreach (float value in ts.quotientValues)
                        {
                            stats.ChangeStatModifier(ts.property, value, BaseStats.ModType.QUOTIENT, newTagList);
                        }
                    }
                }
            }

            // make it follow the creator if necessary
            if (followsCreator)
            {
                if (GetComponent<CreationReferences>())
                {
                    // if no follow ranges have been defined then use the defaults
                    if (followRangesAndPriorities.Count <= 0)
                    {
                        summon.AddComponent<Following>().leader = myReferences.creator;
                    }
                    // otherwise create follow states for each entry in the list
                    Following following;
                    foreach (FollowRangeAndPriority frar in followRangesAndPriorities)
                    {
                        following = summon.AddComponent<Following>();
                        following.leader = myReferences.creator;
                        following.startFollowingRange = frar.range;
                        following.priority = frar.priority;
                    }
                }
            }

            // make it move away from the creator when close if necessary
            if (movesAwayFromSummonerWhenClose)
            {
                if (myReferences)
                {
                    summon.AddComponent<MovingAwayFromSummoner>().summoner = myReferences.creator;
                }
            }

            // if necessary limit the duration by adding a destroy after duration component
            if (limitDuration)
            {
                // use a self destroyer if it has ones, otherwise use the dying state
                if (summon.GetComponent<SelfDestroyer>())
                {
                    DestroyAfterDuration destroyer = summon.AddComponent<DestroyAfterDuration>();
                    destroyer.duration = duration;
                }
                else
                {
                    DieAfterDelay destroyer = null;
                    if (!summon.GetComponent<DieAfterDelay>())
                    {
                        destroyer = summon.AddComponent<DieAfterDelay>();
                        destroyer.timeUntilDeath = duration;
                    }
                    // if there is already a destroy after duration component, lower the duration if this component's duration is lower than the one already there
                    else
                    {
                        destroyer = summon.GetComponent<DieAfterDelay>();
                        destroyer.timeUntilDeath = Mathf.Min(destroyer.timeUntilDeath, duration);
                    }
                    // make the destroyer unsummon rather than kill
                    if (destroyer)
                    {
                        destroyer.countsAsUnsummoned = true;
                    }
                }
            }

            // pass on alignment if necessary, may require an alignment manager to be added to the entity
            if (passOnAlignment && GetComponent<AlignmentManager>())
            {
                AlignmentManager alignmentManager = null;
                if (!summon.GetComponent<AlignmentManager>())
                {
                    alignmentManager = summon.AddComponent<AlignmentManager>();
                }
                else
                {
                    alignmentManager = summon.GetComponent<AlignmentManager>();
                }
                alignmentManager.alignment = GetComponent<AlignmentManager>().alignment;
            }

            // give the entity a custom name if necessary, may require a display information component to be added to the entity
            if (giveCustomName && customNames.Count > 0)
            {
                DisplayInformation displayInformation = null;
                if (!summon.GetComponent<DisplayInformation>())
                {
                    displayInformation = summon.AddComponent<DisplayInformation>();
                }
                else
                {
                    displayInformation = summon.GetComponent<DisplayInformation>();
                }
                int nameIndex = Random.Range(0, customNames.Count);
                displayInformation.displayName = customNames[nameIndex];
            }

            // add stats
            if (statList != null && statList.Count > 0) {
                if (stats)
                {
                    foreach (TaggedStatsHolder.TaggableStat stat in statList)
                    {
                        stats.addStat(stat);
                    }
                    stats.UpdateStats();
                    stats.myHealth.currentHealth = stats.myHealth.maxHealth;
                }
            }
            
            // give the entity a custom tag if necessary
            if (giveCustomTag)
            {
                summon.tag = customTag;
            }

            if (stats) {
                stats.UpdateStats();
                if (stats.myHealth) { stats.myHealth.currentHealth = stats.myHealth.maxHealth; }
            }

            // pass on this summon information to a summonChangeTracker
            SummonChangeTracker sct = summon.AddComponent<SummonChangeTracker>();
            sct.statList.AddRange(statList);
            sct.limitDuration = limitDuration;

            sct.followsCreator = followsCreator;
            sct.followRangesAndPriorities = new List<FollowRangeAndPriority>();
            sct.followRangesAndPriorities.AddRange(followRangesAndPriorities);

            sct.limitDuration = limitDuration;
            sct.duration = duration;
        }

    }



    // finds the lowest priority summon to keep alive
    public GameObject getLowestPriority(List<GameObject> list)
    {
        if (list.Count == 0) { return null; }
        else if (list.Count == 1) { return list[0]; }
        else
        {
            GameObject lowestPriority = list[0];
            float lowestDuration = getDuration(list[0]);
            float lowestHealth = list[0].GetComponent<BaseHealth>().currentHealth;
            float duration = 0;
            float health = 0;
            foreach (GameObject go in list)
            {
                duration = getDuration(go);
                health = go.GetComponent<BaseHealth>().currentHealth;
                if (duration != -1)
                {
                    if (duration < lowestDuration)
                    {
                        lowestPriority = go;
                        lowestDuration = duration;
                        lowestHealth = health;
                    }
                }
                else if (health < lowestHealth)
                {
                    lowestPriority = go;
                    lowestDuration = duration;
                    lowestHealth = health;
                }
            }
            return lowestPriority;
        }
    }

    public float getDuration(GameObject go)
    {
        if (go.GetComponent<DestroyAfterDuration>())
        {
            return go.GetComponent<DestroyAfterDuration>().duration - go.GetComponent<DestroyAfterDuration>().age;
        }
        else return -1;
    }

    // gets the Y coordinate of the ground at a position
    public float getY(Vector3 position)
    {
        // the maximum height above the ground to look for a surface
        float height = 10f;
        // raycast down from this height
        RaycastHit hit;
        LayerMask mask = 1 << LayerMask.NameToLayer("Default");
        if (Physics.Raycast(new Vector3(position.x, position.y + height, position.z), Vector3.down, out hit, Mathf.Infinity, mask))
        {
            return hit.point.y;
        }
        // if nothing was hit, just return the y value of the point
        return position.y;
    }

}
