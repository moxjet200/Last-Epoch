using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HitDetector))]
[RequireComponent(typeof(AbilityObjectConstructor))]
public class ApplyStatusToCreator : RequiresTaggedStats
{

    public EffectDataSO statusEffect = null;

    public bool applyRepeatedly = false;
    public float interval = 1f;
    float index = 0f;
    bool applied = false;
    public bool scaleWithStats = false;

    ProtectionClass creatorProtection = null;

    // Use this for initialization
    void Start()
    {
        CreationReferences references = GetComponent<CreationReferences>();
        if (references && references.creator)
        {
            creatorProtection = references.creator.GetComponent<ProtectionClass>();
        }
        if (creatorProtection)
        {
            applied = true;
            Apply(creatorProtection);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (creatorProtection && (!applied || applyRepeatedly))
        {
            index += Time.deltaTime;
            while (index > interval)
            {
                index -= interval;
                applied = true;
                Apply(creatorProtection);
            }
        }
    }

    public void Apply(ProtectionClass protection)
    {
        if (protection && protection.healthClass && protection.healthClass.alignmentManager)
        {
            GameObject effect = Instantiate(statusEffect.effectObject, protection.transform);
            Alignment enemyAlignment = protection.healthClass.alignmentManager.alignment.foes[0];
            Comp<AlignmentManager>.GetOrAdd(effect).alignment = enemyAlignment;


            // add creation reference
            CreationReferences myReferences = GetComponent<CreationReferences>();
            if (myReferences)
            {
                CreationReferences references = effect.AddComponent<CreationReferences>();
                references.creator = myReferences.creator;
                references.thisAbility = myReferences.thisAbility;
            }

            // build damage stats and apply shock effect
            if (!scaleWithStats)
            {
                foreach (DamageStatsHolder holder in effect.GetComponents<DamageStatsHolder>())
                {
                    holder.damageStats = DamageStats.buildDamageStats(holder, null);
                }
            }
            else
            {
                StatusDamage statusDamage = effect.GetComponent<StatusDamage>();
                TaggedStatsHolder myTaggedStatsHolder = GetComponent<TaggedStatsHolder>();
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
            }

            // apply
            protection.effectControl.AddEffect(effect, effect.GetComponent<StatusEffect>());
        }
    }
}
