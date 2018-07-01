using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCreatorOnCreation : MonoBehaviour
{
    public float percentCurrentHealthTaken = 0f;
    public float flatDamage = 0f;

    public void Start()
    {
        CreationReferences references = GetComponent<CreationReferences>();
        if (references && references.creator)
        {
            BaseHealth health = references.creator.GetComponent<BaseHealth>();
            if (health)
            {
                health.HealthDamage(flatDamage);
                health.HealthDamage(health.currentHealth * percentCurrentHealthTaken);
            }
        }
    }

}
