using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// it is what it looks like
public class DamageStatsHolder : MonoBehaviour
{

    // maps a damage type to a damage value, a list of these is an atlernative to a DamageType to float dictionary that is visible in the inspector.
    [System.Serializable]
    public struct DamageTypesAndValues
    {
        public DamageType damageType;
        public float value;

        public DamageTypesAndValues(DamageType _damageType, float _value) { damageType = _damageType; value = _value; }
    }

    // basically a DamageStats, but with the damage dictionary replaced by a list of DamageTypesAndValues
    [System.Serializable]
    public class BaseDamageStats
    {
        public List<DamageTypesAndValues> damage;
        public float critChance;
        public float critMultiplier;
        public bool noCritMultiplier;
        public bool isHit;
        [Range(0, 1)]
        public float cullPercent;
        public float increasedStunChance;
        public float addedDamageScaling = 1f;
        public List<DamageConditionalEffect> conditionalEffects = new List<DamageConditionalEffect>();
		public List<DamageStats.DamagePenetrationStat> penetrationStats = new List<DamageStats.DamagePenetrationStat> ();

        public BaseDamageStats(bool _isHit = false) { damage = new List<DamageTypesAndValues>(); critChance = 0f; critMultiplier = 1f; noCritMultiplier = false;  isHit = _isHit; cullPercent = 0f; increasedStunChance = 0f; }
    }

    // the base damage stats are compiled into the damage stats when the ability object with this damage holder is constructed
    [HideInInspector]
    public DamageStats damageStats;
    public BaseDamageStats baseDamageStats = new BaseDamageStats();

    // the tags define which damage increases are applied to the damage stats
    public List<Tags.AbilityTags> tags = new List<Tags.AbilityTags>();

    // adds to the damage of a given damage type
    public void addBaseDamage(DamageType damageType, float value)
    {
        bool damageAdded = false;
        // if there is already damage of this type then modify it
        for (int i = 0; i < baseDamageStats.damage.Count; i++)
        {
            if (baseDamageStats.damage[i].damageType == damageType)
            {
                baseDamageStats.damage[i] = new DamageTypesAndValues(damageType, baseDamageStats.damage[i].value + value);
                damageAdded = true;
            }
        }
        // otherwise add it
        if (!damageAdded)
        {
            DamageTypesAndValues dtav = new DamageTypesAndValues(damageType, value);
            baseDamageStats.damage.Add(dtav);
        }
    }

    // increases all base damage by a percentage
    public void increaseBaseDamage(float value)
    {
        for (int i = 0; i < baseDamageStats.damage.Count; i++)
        {
            baseDamageStats.damage[i] = new DamageTypesAndValues(baseDamageStats.damage[i].damageType, baseDamageStats.damage[i].value * (1+value));
        }
    }

    // also increases added damage
    public void increaseAllDamage(float value)
    {
        baseDamageStats.addedDamageScaling *= (1 + value);
        increaseBaseDamage(value);
    }

    // gets the base damage for a given damage type
    public float getBaseDamage(DamageType damageType)
    {
        foreach (DamageTypesAndValues dtav in baseDamageStats.damage)
        {
            if (dtav.damageType == damageType) { return dtav.value; }
        }
        return 0f;
    }

    public void convertAllDamageOfType(DamageType from, DamageType to)
    {
        addBaseDamage(to, getBaseDamage(from));
        addBaseDamage(from, -getBaseDamage(from));
    }

    public void applyDamage(GameObject enemy)
    {
        ProtectionClass enemyProtection = enemy.GetComponent<ProtectionClass>();
        if (enemyProtection)
        {
            List<HitEvents> hitEvents = enemyProtection.ApplyDamage(damageStats, gameObject);
            // react to the hit events
            CreationReferences references = GetComponent<CreationReferences>();
            if (references && references.creator && references.creator.GetComponent<AbilityEventListener>())
            {
                // get event listeners
                AbilityEventListener parentEventsListener = references.creator.GetComponent<AbilityEventListener>();
                AbilityEventListener localEventsListener = GetComponent<AbilityEventListener>();
                // call event methods
                if (hitEvents.Contains(HitEvents.Hit)) {
                    parentEventsListener.HitEvent(references.thisAbility, enemy);
                    if (localEventsListener)
                    {
                        localEventsListener.HitEvent(references.thisAbility, enemy);
                    }
                }
                if (hitEvents.Contains(HitEvents.Crit)) {
                    parentEventsListener.CritEvent(references.thisAbility, enemy);
                    if (localEventsListener)
                    {
                        localEventsListener.CritEvent(references.thisAbility, enemy);
                    }
                }
                if (hitEvents.Contains(HitEvents.Kill)) {
                    parentEventsListener.KillEvent(references.thisAbility, enemy);
                    if (localEventsListener)
                    {
                        localEventsListener.KillEvent(references.thisAbility, enemy);
                    }
                }
            }


        }
        else
        {
            Debug.LogError("Enemy " + enemy.name + " has no protection class");
        }
    }


}
