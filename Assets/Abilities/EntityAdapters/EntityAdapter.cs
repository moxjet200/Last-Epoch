using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAdapter : MonoBehaviour
{
    [System.Serializable]
    public struct ExtraAbility{
        public Ability ability;
        public float addedCharges;
        public float addedChargeRegen;
        public float minRange;
        public float engageRange;
        public float maxRange;
        public float persuitRange;
        public bool lowPriority;

        public ExtraAbility(Ability _ability, float _addedCharges, float _addedChargeRegen, float _minRange, float _engageRange, float _maxRange, float _persuitRange, bool _lowPriority)
        {
            ability = _ability;
            addedCharges = _addedCharges;
            addedChargeRegen = _addedChargeRegen;
            minRange = _minRange;
            engageRange = _engageRange;
            maxRange = _maxRange;
            persuitRange = _persuitRange;
            lowPriority = _lowPriority;
        }
    }
    
    [System.Serializable]
    public struct Retaliator
    {
        public Ability ability;
        public int threshold;
        public int startingDamage;

        public Retaliator(Ability _ability, int _threshold, int _startingDamage = 0)
        {
            ability = _ability;
            threshold = _threshold;
            startingDamage = _startingDamage;
        }

    }

    public List<Retaliator> retaliators = new List<Retaliator>();

    [Header("These are in order of lowest to highest priority, except low priority abilities which are in order of highest to lowest priority")]
    public List<ExtraAbility> extraAbilities = new List<ExtraAbility>();

    public virtual GameObject adapt(GameObject entity)
    {
        // attach extra abilities if necessary
        if (extraAbilities.Count > 0)
        {
            AbilityList list = entity.GetComponent<AbilityList>();
            AbilityRangeList rangeList = entity.GetComponent<AbilityRangeList>();
            AbilityMutatorManager mutatorManager = null;
            bool addChargeManager = false;

            // check that there is an ability list and ability range list
            if (list && rangeList)
            {
                // add each ability in order
                foreach (ExtraAbility extra in extraAbilities)
                {
                    // add the new ability
                    if (extra.lowPriority) { list.abilities.Add(extra.ability); }
                    else { list.AddAbilityToStart(extra.ability); }

                    // add the ranges to use the ability at
                    AbilityRangeList.AbilityRanges abilityRanges = new AbilityRangeList.AbilityRanges();
                    abilityRanges.minRange = extra.minRange;
                    abilityRanges.engageRange = extra.engageRange;
                    abilityRanges.maxRange = extra.maxRange;
                    abilityRanges.persuitRange = extra.persuitRange;
                    if (extra.lowPriority) { rangeList.ranges.Add(abilityRanges); }
                    else { rangeList.addRangeToStart(abilityRanges); }

                    // if the ability has a cooldown add a mutator for it unless one already exists, in which case change the cooldown on it
                    if (extra.addedCharges + extra.ability.maxCharges != 0)
                    {
                        addChargeManager = true;

                        if (!mutatorManager) { mutatorManager = entity.GetComponent<AbilityMutatorManager>(); }
                        if (!mutatorManager) { mutatorManager = entity.AddComponent<AbilityMutatorManager>(); }

                        // get a reference to, or add, a mutator
                        AbilityMutator myMutator = null;
                        List<AbilityMutator> mutators = mutatorManager.getMutators(extra.ability);
                        if (mutators != null && mutators.Count > 0) { myMutator = mutators[0]; }
                        else {
                            myMutator = entity.AddComponent<GenericMutator>();
                            myMutator.changeAbility(extra.ability);
                        }

                        // replace the cooldown on the mutator
                        myMutator.addedCharges = extra.addedCharges;
                        myMutator.addedChargeRegen = extra.addedChargeRegen;

                    }
                }
            }

            if (addChargeManager)
            {
                if (!entity.GetComponent<ChargeManager>())
                {
                    entity.AddComponent<ChargeManager>();
                }

                //check if a null ability needs to be added
                bool needsNull = false;
                foreach (ExtraAbility extra in extraAbilities)
                {
                    if (extra.lowPriority)
                    {
                        if (extra.addedCharges + extra.ability.maxCharges != 0) { needsNull = true; }
                        else { needsNull = false; }
                    }
                }
                if (needsNull && !list.abilities.Contains(AbilityIDList.getAbility(AbilityID.nullAbility)))
                {
                    list.abilities.Add(AbilityIDList.getAbility(AbilityID.nullAbility));
                    float highestRange = 0f;
                    foreach (AbilityRangeList.AbilityRanges range in rangeList.ranges)
                    {
                        if (range.persuitRange > highestRange)
                        {
                            highestRange = range.persuitRange;
                        }
                    }
                    AbilityRangeList.AbilityRanges ranges = new AbilityRangeList.AbilityRanges(highestRange, 1f, 0.5f, 0f);
                    rangeList.ranges.Add(ranges);
                }
            }
        }


        // add retaliators if necessary
        if (retaliators.Count > 0)
        {
            foreach (Retaliator retal in retaliators)
            {
                GameObject retalObj = new GameObject();
                retalObj.transform.parent = entity.transform;
                retalObj.transform.localPosition = Vector3.zero;

                RetaliateWhenParentHit retalComponent = retalObj.AddComponent<RetaliateWhenParentHit>();
                retalComponent.ability = retal.ability;
                retalComponent.sourceOfAbility = RetaliateWhenParentHit.SourceOfAbilityObjectConstructor.Parent;
                retalComponent.damageTakenTrigger = retal.threshold;
                retalComponent.damageTakenSinceTrigger = retal.startingDamage;
            }


        }

        return entity;
    }


    public virtual void addAbility(GameObject entity, Ability ability, float minRange, float maxRange, float persuitRange)
    {

    }



}
