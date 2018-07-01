using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentLosesHealthPercentageOnHit : MonoBehaviour
{
    public float flatDamage = 0f;
    public float currentPercentageDamage = 0f;
    public float maxPercentageDamage = 0f;

    AbilityEventListener abilityEventListener = null;
    BaseHealth health = null;

    void Start()
    {
        Transform parent = transform.parent;
        if (parent)
        {
            abilityEventListener = parent.GetComponent<AbilityEventListener>();
            health = parent.GetComponent<BaseHealth>();
            if (abilityEventListener)
            {
                abilityEventListener.onHitEvent += dealDamage;
            }
        }
    }

    public void dealDamage(Ability ability, GameObject hit)
    {
        if (health)
        {
            health.HealthDamage(flatDamage + health.currentHealth * currentPercentageDamage + health.maxHealth * maxPercentageDamage);
        }
    }

    void OnDestroy()
    {
        if (abilityEventListener)
        {
            abilityEventListener.onHitEvent -= dealDamage;
        }
    }


}
