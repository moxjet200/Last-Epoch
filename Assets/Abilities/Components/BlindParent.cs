using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlindParent : MonoBehaviour {


    bool initialised = false;
    GameObject parent = null;
    bool buffsRemoved = false;
    AccuracyManager am = null;
    [Range(-1,0)]
    public float percentCritChanceBuff = -0.5f;
    BaseStats parentBaseStats = null;

    void Awake()
    {
        if (transform.parent)
        {
            parent = transform.parent.gameObject;
        }
    }

    void Update()
    {
        if (!initialised) { initialise(); }
    }

    void initialise()
    {
        initialised = true;
        // make sure there is a parent
        if (transform.parent == null) { return; }
        if (parent == null) { parent = transform.parent.gameObject; }
        if (parent == null) { Debug.LogError("BlindParent component has no parent"); return; }
        // find the parent's accuracy manager
        am = parent.gameObject.GetComponent<AccuracyManager>();
        
        if (am) { am.numberOfBlinds++; }

        parentBaseStats = parent.GetComponent<BaseStats>();
        if (parentBaseStats)
        {
            parentBaseStats.ChangeStatModifier(Tags.Properties.CriticalChance, percentCritChanceBuff, BaseStats.ModType.MORE);
        }

        // subscribe to a death event to remove the buffs
        if (GetComponent<SelfDestroyer>()) { GetComponent<SelfDestroyer>().deathEvent += removeBlind; }
    }

    void OnDestroy()
    {
        removeBlind();
    }

    void removeBlind()
    {
        if (buffsRemoved) { return; }

        buffsRemoved = true;

        if (am)
        {
            am.numberOfBlinds--;
        }

        if (parentBaseStats)
        {
            parentBaseStats.ChangeStatModifier(Tags.Properties.CriticalChance, percentCritChanceBuff, BaseStats.ModType.QUOTIENT);
        }
    }


}
