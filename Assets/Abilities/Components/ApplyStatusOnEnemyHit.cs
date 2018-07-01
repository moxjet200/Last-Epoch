using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HitDetector))]
[RequireComponent(typeof(AbilityObjectConstructor))]
public class ApplyStatusOnEnemyHit : RequiresTaggedStats {

    public EffectDataSO statusEffect = null;

    public bool canApplyToSameEnemyAgain = false;


    // Use this for initialization
    void Start()
    {
        // subscribe to the new enemy hit event
        GetComponent<HitDetector>().newEnemyHitEvent += Apply;
        // if it can damage the same enemy again also subscribe to the enemy hit again event
        if (canApplyToSameEnemyAgain) { GetComponent<HitDetector>().enemyHitAgainEvent += Apply; }
    }

    // Update is called once per frame
    void Update()
    {

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
}
