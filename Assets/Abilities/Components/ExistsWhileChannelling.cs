using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SelfDestroyer))]
public class ExistsWhileChannelling : MonoBehaviour
{
    UsingAbility usingAbility = null;

    // Use this for initialization
    void Start()
    {
        CreationReferences references = GetComponent<CreationReferences>();
        if (references && references.creator)
        {
            usingAbility = references.creator.GetComponent<UsingAbility>();
            if (usingAbility)
            {
                usingAbility.channelEndEvent += DestroyThis;
            }
        }
    }

    // Update is called once per frame
    public void DestroyThis()
    {
        Comp<SelfDestroyer>.GetOrAdd(gameObject).die();
    }

    void OnDestroy()
    {
        if (usingAbility)
        {
            usingAbility.channelEndEvent -= DestroyThis;
        }
    }
}
