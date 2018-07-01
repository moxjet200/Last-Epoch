using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// tracks the changes that have been made to a summon creature by the SummonEntity component that summoned it
public class SummonChangeTracker : MonoBehaviour {

    public bool followsCreator = false;

    public List<FollowRangeAndPriority> followRangesAndPriorities = new List<FollowRangeAndPriority>();

    public bool limitDuration = false;

    public float duration = 5f;

    public float addedHealth = 0f;

    public float addedSpeed = 0f;

    public float addedHealthRegen = 0f;

    public float increasedDamage = 0f;

    public List<TaggedStatsHolder.TaggableStat> statList = new List<TaggedStatsHolder.TaggableStat>();

    public BaseStats baseStats = null;
    
    
    void Awake()
    {
        baseStats = GetComponent<BaseStats>();
    }

    public void changeFollowsCreator(bool _followsCreator, GameObject creator)
    {
        if (followsCreator && !_followsCreator) {
            // remove the following states
            StateController controller = GetComponent<StateController>();
            List<State> followings = new List<State>();
            followings.AddRange(GetComponents<Following>());
            if (controller)
            {
                if (followings.Contains(controller.currentState)) { controller.forceChangeState(controller.waiting); }
                for (int i = 0; i<followings.Count; i++)
                {
                    Destroy(followings[i]);
                }
            }
        }
        else if (!followsCreator && _followsCreator)
        {
            // add following states

            // if no follow ranges have been defined then use the defaults
            if (followRangesAndPriorities.Count <= 0)
            {
                gameObject.AddComponent<Following>().leader = creator;
            }
            // otherwise create follow states for each entry in the list
            Following following;
            foreach (FollowRangeAndPriority frar in followRangesAndPriorities)
            {
                following = gameObject.AddComponent<Following>();
                following.leader = creator;
                following.startFollowingRange = frar.range;
                following.priority = frar.priority;
            }
        }

        // follow creator has now changed
        followsCreator = _followsCreator;
    }

    public void changeLimitDuration(bool _limitDuration, float _duration)
    {
        // limit the duration if that is now necessary
        if (!limitDuration && _limitDuration)
        {
            // use a self destroyer if it has ones, otherwise use the dying state
            if (GetComponent<SelfDestroyer>())
            {
                DestroyAfterDuration destroyer = gameObject.AddComponent<DestroyAfterDuration>();
                destroyer.duration = _duration;
            }
            else
            {
                DieAfterDelay destroyer = null;
                if (!GetComponent<DieAfterDelay>())
                {
                    destroyer = gameObject.AddComponent<DieAfterDelay>();
                    destroyer.timeUntilDeath = duration;
                }
                // if there is already a destroy after duration component, lower the duration if this component's duration is lower than the one already there
                else
                {
                    destroyer = GetComponent<DieAfterDelay>();
                    destroyer.timeUntilDeath = Mathf.Min(destroyer.timeUntilDeath, duration);
                }
            }
        }
        // stop limitting the duration if that is now necessary
        if (limitDuration && !_limitDuration)
        {
            List<DestroyAfterDuration> destroyers = new List<DestroyAfterDuration>();
            destroyers.AddRange(GetComponents<DestroyAfterDuration>());
            for (int i = 0; i < destroyers.Count; i++)
            {
                Destroy(destroyers[i]);
            }
            List<DieAfterDelay> dads = new List<DieAfterDelay>();
            dads.AddRange(GetComponents<DieAfterDelay>());
            for (int i = 0; i < dads.Count; i++)
            {
                Destroy(dads[i]);
            }
        }
        // change the duration if necessary
        if (duration != _duration && _limitDuration)
        {
            foreach (DestroyAfterDuration destroyAfterDurion in GetComponents<DestroyAfterDuration>())
            {
                destroyAfterDurion.duration += _duration - duration;
            }
            foreach (DieAfterDelay dad in GetComponents<DieAfterDelay>())
            {
                dad.timeUntilDeath += _duration - duration;
            }
        }

        // change the limit duration and duration variables
        limitDuration = _limitDuration;
        duration = _duration;

    }

    public void changeStats(List<TaggedStatsHolder.TaggableStat> newStats)
    {
        // remove old stats
        foreach (TaggedStatsHolder.TaggableStat stat in statList)
        {
            baseStats.removeStat(stat);
        }
        // add new stats
        foreach (TaggedStatsHolder.TaggableStat stat in newStats)
        {
            baseStats.addStat(stat);
        }
        statList.Clear();
        statList.AddRange(newStats);
    }

    //public void changeAddedHealth(float _addedHealth)
    //{
    //    // change health
    //    if (addedHealth != _addedHealth)
    //    {
    //        BaseHealth health = GetComponent<BaseHealth>();
    //        if (health)
    //        {
    //            health.maxHealth += (int) (_addedHealth - addedHealth);
    //        }
    //    }
    //    // updated added health
    //    addedHealth = _addedHealth;
    //}

    //public void changeHealthRegen(float _addedHealthRegen)
    //{
    //    // change health regen
    //    if (addedHealthRegen != _addedHealthRegen)
    //    {
    //        BaseHealth health = GetComponent<BaseHealth>();
    //        if (health)
    //        {
    //            health.baseHealthRegenPerSecond += (int)(_addedHealthRegen - addedHealthRegen);
    //        }
    //    }
    //    // updated added health regen
    //    addedHealthRegen = _addedHealthRegen;
    //}

    //public void changeAddedSpeed(float _addedSpeed)
    //{
    //    // change speed
    //    if (addedSpeed != _addedSpeed)
    //    {
    //        BaseStats stats = GetComponent<BaseStats>();
    //        if (stats)
    //        {
				//stats.ChangeStatModifier(Tags.Properties.Movespeed, _addedSpeed - addedSpeed, BaseStats.ModType.ADDED);
    //        }
    //    }
    //    // updated added speed
    //    addedSpeed = _addedSpeed;
    //}

    //public void changeIncreasedDamage(float _increasedDamage)
    //{
    //    // change damage
    //    if (increasedDamage != _increasedDamage)
    //    {
    //        BaseStats stats = GetComponent<BaseStats>();
    //        if (stats)
    //        {
				//stats.ChangeStatModifier(Tags.Properties.Damage, _increasedDamage - increasedDamage, BaseStats.ModType.INCREASED);
    //        }
    //    }
    //    // updated increased damage
    //    increasedDamage = _increasedDamage;
    //}


}
