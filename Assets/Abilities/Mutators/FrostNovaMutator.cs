using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostNovaMutator : AbilityMutator
{
    public float chanceForChainingIceNova = 0f;

    public float addedCritMultiplier = 0f;
    public float addedCritChance = 0f;

    public bool canTarget = false;

    public bool ignites = false;
    public float increasedDamage = 0f;
    public float chillChance = 0f;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.frostNova);
        base.Awake();
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {

        if (chanceForChainingIceNova > 0)
        {
            float rand2 = Random.Range(0f, 1f);
            if (rand2 < chanceForChainingIceNova)
            {
                CreateAbilityObjectOnNewEnemtHit newComponent = abilityObject.AddComponent<CreateAbilityObjectOnNewEnemtHit>();
                newComponent.abilityToInstantiate = AbilityIDList.getAbility(AbilityID.delayerForChainingIceNova);
            }
        }

        if (chillChance > 0)
        {
            ChanceToApplyStatusOnEnemyHit newComponent = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            newComponent.statusEffect = StatusEffectList.getEffect(StatusEffectID.Chill);
            newComponent.chance = chillChance;
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

        if (canTarget)
        {
            abilityObject.AddComponent<StartsAtTarget>();
        }

        if (ignites)
        {
            ApplyStatusOnEnemyHit apply = abilityObject.AddComponent<ApplyStatusOnEnemyHit>();
            apply.statusEffect = StatusEffectList.getEffect(StatusEffectID.Ignite);
        }

        if (increasedDamage != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(increasedDamage);
            }
        }

        return abilityObject;
    }
}
