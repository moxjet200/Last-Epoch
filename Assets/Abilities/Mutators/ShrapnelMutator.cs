using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrapnelMutator : AbilityMutator
{
    public float increasedSpeed = 0f;
    public bool pierces = false;
    public float increasedDamage = 0f;
    public float increasedStunChance = 0f;
    public List<float> moreDamageInstances = new List<float>();
    public bool convertToCold = false;
    public float chillChance = 0f;
    public float moreDamageAgainstChilled = 0f;
    public int addedProjectiles = 0;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.shrapnel);
        base.Awake();
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        if (increasedSpeed != 0)
        {
            AbilityMover component = abilityObject.GetComponent<AbilityMover>();
            if (component)
            {
                component.speed *= 1 + increasedSpeed;
            }
        }

        if (convertToCold)
        {
            foreach (Transform child in abilityObject.transform)
            {
                if (child.name == "SharpnelVFX") { child.gameObject.SetActive(false); }
                if (child.name == "IceSharpnelVFX") { child.gameObject.SetActive(true); }
            }
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.addBaseDamage(DamageType.COLD, holder.getBaseDamage(DamageType.FIRE));
                holder.addBaseDamage(DamageType.FIRE, -holder.getBaseDamage(DamageType.FIRE));
            }
            foreach(ConstantRotation rot in abilityObject.GetComponents<ConstantRotation>())
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

        if (moreDamageAgainstChilled != 0)
        {
            // create the conditional
            DamageConditionalEffect conditionalEffect = new DamageConditionalEffect();
            HasStatusEffectConditional conditional = new HasStatusEffectConditional();
            conditional.statusEffect = StatusEffectID.Chill;
            conditionalEffect.conditional = conditional;
            conditionalEffect.effect = new DamageEffectMoreDamage(moreDamageAgainstChilled);
            // add the conditional to all damage stats holders
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.baseDamageStats.conditionalEffects.Add(conditionalEffect);
            }
        }

        if (addedProjectiles != 0)
        {
            ExtraProjectiles component = abilityObject.GetComponent<ExtraProjectiles>();
            if (component)
            {
                component.numberOfExtraProjectiles += addedProjectiles;
            }
        }

        if (pierces)
        {
            Pierce pierce = abilityObject.GetComponent<Pierce>();
            if (pierce == null) { pierce = abilityObject.AddComponent<Pierce>(); pierce.objectsToPierce = 0; }
            pierce.objectsToPierce += 10000;
        }

        return abilityObject;
    }


}
