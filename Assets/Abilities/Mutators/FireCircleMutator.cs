using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCircleMutator : AbilityMutator
{
    public float increasedDamage = 0f;
    public float increasedStunChance = 0f;
    public List<float> moreDamageInstances = new List<float>();
    public bool convertToCold = false;
    public float chillChance = 0f;
    public float igniteChance = 0f;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.fireCircle);
        base.Awake();
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {

        if (convertToCold)
        {
            foreach (CreateOnDeath cod in abilityObject.GetComponents<CreateOnDeath>())
            {
                cod.objectsToCreateOnDeath.Clear();
                cod.objectsToCreateOnDeath.Add(new CreateOnDeath.GameObjectHolder(PrefabList.getPrefab("QuickIceCircleVFX")));
            }
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.addBaseDamage(DamageType.COLD, holder.getBaseDamage(DamageType.FIRE));
                holder.addBaseDamage(DamageType.FIRE, -holder.getBaseDamage(DamageType.FIRE));
            }
            foreach (ConstantRotation rot in abilityObject.GetComponents<ConstantRotation>())
            {
                rot.degreesPerSecond = 0f;
            }
        }

        if (increasedStunChance != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.baseDamageStats.increasedStunChance += increasedStunChance;
            }
        }

        if (increasedDamage != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(increasedDamage);
            }
        }

        if (moreDamageInstances != null && moreDamageInstances.Count > 0)
        {
            float moreDamage = 1f;
            foreach (float instance in moreDamageInstances)
            {
                moreDamage *= 1 + instance;
            }

            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(moreDamage - 1);
            }
        }

        if (chillChance > 0)
        {
            ChanceToApplyStatusOnEnemyHit newComponent = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            newComponent.statusEffect = StatusEffectList.getEffect(StatusEffectID.Chill);
            newComponent.chance = chillChance;
        }

        if (igniteChance > 0)
        {
            ChanceToApplyStatusOnEnemyHit newComponent = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            newComponent.statusEffect = StatusEffectList.getEffect(StatusEffectID.Ignite);
            if (convertToCold) { newComponent.statusEffect = StatusEffectList.getEffect(StatusEffectID.Chill); }
            newComponent.chance = igniteChance;
        }

        return abilityObject;
    }


}
