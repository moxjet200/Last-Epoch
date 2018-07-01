using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CreationReferences))]
public class Summoned : MonoBehaviour {

    public static List<Summoned> all = new List<Summoned>();

    public bool singeCardForAllMinions = false;

	[HideInInspector] public  CreationReferences references = null;
    SummonTracker tracker = null;

    Dying dying = null;

    void Awake()
    {
        all.Add(this);
    }

    void Start()
    {
        references = GetComponent<CreationReferences>();
        dying = GetComponent<Dying>();
        if (dying)
        {
            dying.deathEvent += OnDeath;
        }
    }

    public void initialise()
    {
        if (!tracker && references && references.creator)
        {
            tracker = references.creator.GetComponent<SummonTracker>();
            if (!tracker) { tracker = references.creator.AddComponent<SummonTracker>(); }
            tracker.AddSummon(this);
        }
    }

    void Update()
    {
        if (!tracker)
        {
            initialise();
        }
    }

    public void OnDeath(Dying me)
    {
        if (tracker)
        {
            if (tracker.summons.Contains(this))
            {
                tracker.summons.Remove(this);
            }
        }
    }

    void OnDestroy()
    {
        all.Remove(this);
        if (tracker)
        {
            if (tracker.summons.Contains(this))
            {
                tracker.summons.Remove(this);
            }
        }
    }

    public SummonTracker GetSummonTracker()
    {
        if (tracker)
        {
            return tracker;
        }
        else if(!tracker && references && references.creator)
        {
            tracker = references.creator.GetComponent<SummonTracker>();
            if (!tracker) { tracker = references.creator.AddComponent<SummonTracker>(); }
            tracker.AddSummon(this);
            return tracker;
        }
        return null;
    }

    public CreationReferences getReferences()
    {
        return references;
    }

    public BaseHealth getBaseHealth()
    {
        if (dying) { return dying.myHealth; }

        return GetComponent<BaseHealth>();
    }

    public Dying getDyingComponent()
    {
        return dying;
    }

}
