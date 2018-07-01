using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SelfDestroyer))]
public class DestroySelfOnParentAbilityUse : MonoBehaviour
{
    UsingAbility usingAbility = null;

    // Use this for initialization
    void Start()
    {
        if (transform.parent)
        {
            usingAbility = transform.parent.GetComponent<UsingAbility>();
            if (usingAbility)
            {
                usingAbility.usedAbilityEvent += die;
            }
        }
    }

    public void die()
    {
        GetComponent<SelfDestroyer>().die();
    }

    public void OnDestroy()
    {
        if (usingAbility) { usingAbility.usedAbilityEvent -= die; }
    }
    
}