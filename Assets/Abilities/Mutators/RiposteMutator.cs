using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiposteMutator : AbilityMutator
{


    public float increasedDuration = 0f;

    public int additionalRetaliations = 0;

    public float darkBladeRetaliationChance = 0f;

    public List<TaggedStatsHolder.TaggableStat> statsWhilePrepped = new List<TaggedStatsHolder.TaggableStat>();
    
    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.riposte);
        base.Awake();
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        // add additional duration
        if (increasedDuration != 0)
        {
            DestroyAfterDuration durationObject = abilityObject.GetComponent<DestroyAfterDuration>();
            if (durationObject != null) { durationObject.duration *= (1 + increasedDuration); }
        }
        
        // set damage threshold
        if (additionalRetaliations != 0)
        {
            RetaliateWhenParentHit retaliator = abilityObject.GetComponent<RetaliateWhenParentHit>();
            if (retaliator)
            {
                retaliator.retaliationsRemaining += additionalRetaliations;
            }
        }

        // add dark blade retaliation
        if (darkBladeRetaliationChance > 0)
        {
            if (Random.Range(0f, 1f) < darkBladeRetaliationChance)
            {
                RetaliateWhenParentHit retaliator = abilityObject.AddComponent<RetaliateWhenParentHit>();
                retaliator.limitRetaliations = true;
                retaliator.onlyCountHitDamage = true;
                retaliator.ability = Ability.getAbility(AbilityID.darkBlade);
                retaliator.damageTakenTrigger = 1;
                retaliator.destroyAfterLastRetaliation = false;
                retaliator.sourceOfAbility = RetaliateWhenParentHit.SourceOfAbilityObjectConstructor.Parent;
                retaliator.retaliationsRemaining = 1;
            }

        }

        // stats while prepped
        if (statsWhilePrepped != null && statsWhilePrepped.Count > 0)
        {
            BuffParent bp = abilityObject.GetComponent<BuffParent>();
            if (!bp) { bp = abilityObject.AddComponent<BuffParent>(); }

            List<TaggedStatsHolder.TaggableStat> stats = new List<TaggedStatsHolder.TaggableStat>();

            foreach (TaggedStatsHolder.TaggableStat stat in statsWhilePrepped)
            {
                TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(stat);
                stats.Add(newStat);
            }

            bp.taggedStats.AddRange(stats);
        }


        return abilityObject;
    }
}
