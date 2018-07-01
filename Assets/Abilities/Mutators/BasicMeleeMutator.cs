using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMeleeMutator : AbilityMutator {
    
    public float addedCritMultiplier = 0f;
    public float addedCritChance = 0f;
    public float chanceToPoison = 0f;
    public float cullPercent = 0f;

    public float increasedDamage = 0f;

    public float physicalAddedAsFire = 0f;
    public float chanceToIgnite = 0f;
    public float increasedRadius = 0f;
    
    public bool slows = false;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.basicMelee);
        base.Awake();
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {

        if (chanceToPoison > 0)
        {
            ChanceToApplyStatusOnEnemyHit newComponent = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            newComponent.statusEffect = StatusEffectList.getEffect(StatusEffectID.Poison);
            newComponent.chance = chanceToPoison;
        }

        if (chanceToIgnite > 0)
        {
            ChanceToApplyStatusOnEnemyHit chanceTo = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            chanceTo.chance = chanceToIgnite;
            chanceTo.statusEffect = StatusEffectList.getEffect(StatusEffectID.Ignite);
        }

        if (addedCritChance != 0)
        {
            foreach (DamageStatsHolder damage in abilityObject.GetComponents<DamageStatsHolder>())
            {
                damage.baseDamageStats.critChance += addedCritChance;
            }
        }

        if (addedCritMultiplier != 0)
        {
            foreach (DamageStatsHolder damage in abilityObject.GetComponents<DamageStatsHolder>())
            {
                damage.baseDamageStats.critMultiplier += addedCritMultiplier;
            }
        }

        if (cullPercent > 0)
        {
            foreach (DamageStatsHolder damage in abilityObject.GetComponents<DamageStatsHolder>())
            {
                damage.baseDamageStats.cullPercent += cullPercent;
            }
        }

        if (increasedDamage != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.addBaseDamage(DamageType.PHYSICAL, holder.getBaseDamage(DamageType.PHYSICAL) * increasedDamage);
            }
        }

        if (physicalAddedAsFire > 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.addBaseDamage(DamageType.FIRE, holder.getBaseDamage(DamageType.PHYSICAL)*physicalAddedAsFire);
            }
        }

        if (slows)
        {
            ApplyStatusOnEnemyHit newComponent = abilityObject.AddComponent<ApplyStatusOnEnemyHit>();
            newComponent.statusEffect = StatusEffectList.getEffect(StatusEffectID.Slow);
        }

        if (increasedRadius != 0)
        {
            foreach (CapsuleCollider col in abilityObject.GetComponents<CapsuleCollider>())
            {
                col.height *= (1 + increasedRadius);
                col.radius *= (1 + increasedRadius);
            }
        }

        return abilityObject;
    }
}
