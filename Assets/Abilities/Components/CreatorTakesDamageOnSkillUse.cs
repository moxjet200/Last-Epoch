using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatorTakesDamageOnSkillUse : MonoBehaviour
{
    BaseHealth health = null;
    UsingAbility ua = null;

    public float percentCurrentHealthTaken = 0f;

    public void Start()
    {
        CreationReferences references = GetComponent<CreationReferences>();
        if (references && references.creator)
        {
            health = references.creator.GetComponent<BaseHealth>();
            ua = references.creator.GetComponent<UsingAbility>();
            ua.usedAbilityManaEvent += dealDamage;
        }
    }


    public void dealDamage()
    {
        health.HealthDamage(health.currentHealth * percentCurrentHealthTaken);
    }

    public void OnDestroy()
    {
        if (ua)
        {
            ua.usedAbilityManaEvent -= dealDamage;
        }
    }

}
