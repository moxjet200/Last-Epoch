using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityMutatorManager : MonoBehaviour
{

    public List<AbilityMutator> mutators = new List<AbilityMutator>();

    Dictionary<Ability, List<AbilityMutator>> mutatorsForAbility = new Dictionary<Ability, List<AbilityMutator>>();

    public List<Ability> abilities = new List<Ability>();

    // initialised here for memory allocation optimisation
    List<Tags.AbilityTags> useTagsList = new List<Tags.AbilityTags>();

    public List<AbilityMutator> getMutators(Ability ability)
    {
        foreach (Ability _ability in abilities)
        {
            if (_ability == ability)
            {
                return mutatorsForAbility[ability];
            }
        }
        return null;
    }


    public float GetStopRange(Ability ability)
    {
        if (ability == null) { return 100f; }
        foreach (AbilityMutator mutator in mutators)
        {
            if (mutator.ability == ability)
            {
                return mutator.mutateStopRange();
            }
        }
        return ability.stopRange;
    }

    public void addMutator(AbilityMutator mutator)
    {
        abilities.Add(mutator.ability);

        if (mutatorsForAbility.ContainsKey(mutator.ability))
        {
            mutatorsForAbility[mutator.ability].Add(mutator);
        }
        else
        {
            List<AbilityMutator> newMutList = new List<AbilityMutator>();
            newMutList.Add(mutator);
            mutatorsForAbility.Add(mutator.ability, newMutList);
        }
    }

    public void removeMutator(AbilityMutator mutator)
    {
        if (mutatorsForAbility.ContainsKey(mutator.ability))
        {
            mutatorsForAbility[mutator.ability].Remove(mutator);
        }
    }

    public static AbilityMutatorManager GetOrAdd(GameObject _gameObject)
    {
        AbilityMutatorManager ret = _gameObject.GetComponent<AbilityMutatorManager>();
        if (ret) { return ret; }
        return _gameObject.AddComponent<AbilityMutatorManager>();
    }


    public virtual bool RequiresShield(Ability ability)
    {
        if (ability == null) { return false; }
        foreach (AbilityMutator mutator in mutators)
        {
            if (mutator.ability == ability)
            {
                return mutator.mutateRequiresShield();
            }
        }
        return ability.requiresSheild;
    }

    public virtual bool isChannelled(Ability ability)
    {
        if (ability == null) { return false; }
        foreach (AbilityMutator mutator in mutators)
        {
            if (mutator.ability == ability)
            {
                return mutator.isChanneled();
            }
        }
        return ability.channelled;
    }

    public virtual AbilityAnimation getAbilityAnimation(Ability ability)
    {
        if (ability == null) { return AbilityAnimation.Cast; }
        foreach (AbilityMutator mutator in mutators)
        {
            if (mutator.ability == ability)
            {
                return mutator.getAbilityAnimation();
            }
        }
        return ability.animation;
    }

    public virtual List<Tags.AbilityTags> getUseTags(Ability ability, bool includeFakeTags)
    {
        useTagsList.Clear();

        if (ability == null) { return useTagsList; }
        foreach (AbilityMutator mutator in mutators)
        {
            if (mutator.ability == ability)
            {
                useTagsList.AddRange(mutator.getUseTags());
            }
        }

        if (includeFakeTags && ability.fakeUseTags != null)
        {
            useTagsList.AddRange(ability.fakeUseTags);
        }

        return useTagsList;
    }

}
