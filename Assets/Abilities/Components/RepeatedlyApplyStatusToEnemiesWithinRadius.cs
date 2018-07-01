using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatedlyApplyStatusToEnemiesWithinRadius : MonoBehaviour {

	Alignment alignment;
	public EffectDataSO statusEffect = null;

	[ShowOnlyAttribute]float index = 0;

	public float radius = 0f;

	public float statusApplicationInterval = 0f;
    
    public bool useTrailTracker = false;
    List<Vector3> trailPoints = new List<Vector3>();

    TrailTracker trailTracker = null;

    // Use this for initialization
    void Start () {
		alignment = GetComponent<AlignmentManager>().alignment;
        if (useTrailTracker)
        {
            trailTracker = Comp<TrailTracker>.GetOrAdd(gameObject);
        }
	}

	void Update () {
		// only apply damage every damageInterval seconds
		index += Time.deltaTime;
		if (index > statusApplicationInterval)
		{
			index -= statusApplicationInterval;
			// find enemies
			foreach (BaseHealth health in BaseHealth.all)
			{
				// check that it's an enemy and it's not dying
				if (!alignment.isSameOrFriend(health.GetComponent<AlignmentManager>().alignment) && 
					(!health.GetComponent<StateController>() || (health.GetComponent<StateController>().currentState != health.GetComponent<StateController>().dying)))
				{
                    bool applied = false;

					// check whether the enemy's centre if within the radius
					if (Vector3.Distance(transform.position, health.transform.position) <= radius)
					{
						// apply status to the enemy
						Apply(health.gameObject);
                        applied = true;

                    }
					// if the centre is not within the radius, check if the edge of the damage sphere is within a collider of the enemy
					else
					{
						// find the point on the radius closest to the enemy
						Vector3 point = transform.position + Vector3.Normalize(health.transform.position - transform.position) * radius;
						// check if it's in any of their colliders
						bool hit = false;
						foreach (Collider collider in health.GetComponents<Collider>())
						{
							if (!hit && collider.bounds.Contains(point))
							{
								// apply status to the enemy
								Apply(health.gameObject);
                                applied = true;

                            }
						}
					}

                    // check if the target is near a point on the trail
                    if (!applied && useTrailTracker)
                    {
                        // get the trail points
                        trailPoints.Clear();
                        trailPoints = trailTracker.getTrailPoints();

                        // check all the trail points until applied
                        int i = 0;
                        while (i < trailPoints.Count && !applied)
                        {
                            // check whether the enemy's centre if within the radius
                            if (Vector3.Distance(trailPoints[i], health.transform.position) <= radius)
                            {
                                // apply status to the enemy
                                Apply(health.gameObject);
                                applied = true;

                            }
                            // if the centre is not within the radius, check if the edge of the damage sphere is within a collider of the enemy
                            else
                            {
                                // find the point on the radius closest to the enemy
                                Vector3 point = trailPoints[i] + Vector3.Normalize(health.transform.position - trailPoints[i]) * radius;
                                // check if it's in any of their colliders
                                bool hit = false;
                                foreach (Collider collider in health.GetComponents<Collider>())
                                {
                                    if (!hit && collider.bounds.Contains(point))
                                    {
                                        // apply status to the enemy
                                        Apply(health.gameObject);
                                        applied = true;

                                    }
                                }
                            }

                            // increment i to evaluate the next trail point
                            i++;
                        }

                    }
				}
			}
		}
	}	

	public void Apply(GameObject hitObject)
	{
		GameObject effect = Instantiate(statusEffect.effectObject, hitObject.transform);
		StatusDamage statusDamage = effect.GetComponent<StatusDamage>();
		TaggedStatsHolder myTaggedStatsHolder = GetComponent<TaggedStatsHolder>();
		ProtectionClass protection = hitObject.GetComponent<ProtectionClass>();
		if (protection)
		{
			if (statusDamage && myTaggedStatsHolder)
			{
				TaggedStatsHolder taggedStatsHolder = effect.AddComponent<TaggedStatsHolder>();
				taggedStatsHolder.simpleStats.AddRange(myTaggedStatsHolder.simpleStats);
				taggedStatsHolder.taggedStats.AddRange(myTaggedStatsHolder.taggedStats);

				// apply DoT damage
				foreach (TaggedStatsHolder.TaggableStat taggableStat in taggedStatsHolder.taggedStats)
				{
					if (taggableStat.tagList.Contains(Tags.AbilityTags.DoT)) { taggableStat.tagList.Remove(Tags.AbilityTags.DoT); }
				}
				// build damage stats if necessary
				foreach (DamageStatsHolder holder in effect.GetComponents<DamageStatsHolder>())
				{
					holder.damageStats = DamageStats.buildDamageStats(holder, taggedStatsHolder);
				}
			}
			// add creation reference
			CreationReferences myReferences = GetComponent<CreationReferences>();
			if (myReferences)
			{
				CreationReferences references = effect.AddComponent<CreationReferences>();
				references.creator = myReferences.creator;
				references.thisAbility = myReferences.thisAbility;
			}
			// add alignment
			AlignmentManager myAlignmentManager = GetComponent<AlignmentManager>();
			if (myAlignmentManager)
			{
				AlignmentManager alignmentManager = effect.AddComponent<AlignmentManager>();
				alignmentManager.alignment = myAlignmentManager.alignment;
			}
			// apply shock effect
			if (statusEffect.effectID == 13)
			{
				TaggedStatsHolder tsh = GetComponent<TaggedStatsHolder>();
				if (tsh)
				{
					float shockEffect = tsh.GetStatValue(Tags.Properties.IncreasedShockEffect);
					BuffParent pb = effect.GetComponent<BuffParent>();
					if (shockEffect != 0 && pb)
					{
						foreach (TaggedStatsHolder.TaggableStat stat in pb.taggedStats)
						{
							stat.addedValue *= (1 + shockEffect);
							stat.increasedValue *= (1 + shockEffect);
						}
					}
				}
			}
			// apply
			protection.effectControl.AddEffect(effect, effect.GetComponent<StatusEffect>());
		}

	}

	void OnDrawGizmos(){
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere (transform.position, radius);
	}
}	
	
