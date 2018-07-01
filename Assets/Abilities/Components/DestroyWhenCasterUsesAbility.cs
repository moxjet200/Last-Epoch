using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWhenCasterUsesAbility : MonoBehaviour
{
    CreationReferences references = null;
    UsingAbility usingAbility = null;

    public void Start()
    {
        references = GetComponent<CreationReferences>();
        if (references && references.creator)
        {
            usingAbility = references.creator.GetComponent<UsingAbility>();
            if (usingAbility)
            {
                usingAbility.usedAbilityManaEvent += destroyThis;
            }
        }
    }

    public void destroyThis()
    {
        Comp<SelfDestroyer>.GetOrAdd(gameObject).die();
    }

    public void OnDestroy()
    {
        if (usingAbility)
        {
            usingAbility.usedAbilityManaEvent -= destroyThis;
        }
    }
}
