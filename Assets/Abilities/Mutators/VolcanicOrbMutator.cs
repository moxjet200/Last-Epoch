using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolcanicOrbMutator : AbilityMutator
{
    public float increasedSpeed = 0f;
    public float increasedShrapnelSpeed = 0f;
    public bool shrapnelPierces = false;
    public float increasedShrapnelDamage = 0f;
    public float increasedShrapnelStunChance = 0f;
    public List<float> moreDamageInstances = new List<float>();
    public bool convertToCold = false;
    public float chillChance = 0f;
    public float increasedDuration = 0f;
    public bool leavesExplosion = false;
    public float explosionIgniteChance = 0f;
    public float increasedExplosionDamage = 0f;
    public bool delayedExpolosionAtStart = false;
    public float increasedCastSpeed = 0f;
    public bool leavesExplosiveGround = false;
    public float increasedExplosiveGroundFrequency = 0f;
    public float increasedDamage = 0f;
    public float moreShrapnelDamageAgainstChilled = 0f;
    public bool baseDurationis2 = false;
    public int addedShrapnelProjectiles = 0;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.volcanicOrb);
        base.Awake();
    }

    public override float mutateUseSpeed(float useSpeed)
    {
        return base.mutateUseSpeed(useSpeed) * (1 + increasedCastSpeed);
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
                if (child.name == "VolcanicOrbVFX") { child.gameObject.SetActive(false); }
                if (child.name == "FrozenOrbVFX") { child.gameObject.SetActive(true); }
            }
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.addBaseDamage(DamageType.COLD, holder.getBaseDamage(DamageType.FIRE));
                holder.addBaseDamage(DamageType.FIRE, -holder.getBaseDamage(DamageType.FIRE));
            }
            foreach (CreateOnDeath cod in abilityObject.GetComponents<CreateOnDeath>())
            {
                cod.objectsToCreateOnDeath.Clear();
                //cod.objectsToCreateOnDeath.Add(new CreateOnDeath.GameObjectHolder(PrefabList.getPrefab("QuickIceCircleVFX")));
            }
        }

        ShrapnelMutator shrapMut = abilityObject.AddComponent<ShrapnelMutator>();
        shrapMut.increasedDamage = increasedShrapnelDamage;
        shrapMut.increasedSpeed = increasedShrapnelSpeed;
        shrapMut.increasedStunChance = increasedShrapnelStunChance;
        shrapMut.convertToCold = convertToCold;
        shrapMut.pierces = shrapnelPierces;
        shrapMut.chillChance = chillChance;
        shrapMut.moreDamageInstances = moreDamageInstances;
        shrapMut.moreDamageAgainstChilled = moreShrapnelDamageAgainstChilled;
        shrapMut.addedProjectiles = addedShrapnelProjectiles;


        if (chillChance > 0)
        {
            ChanceToApplyStatusOnEnemyHit newComponent = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            newComponent.statusEffect = StatusEffectList.getEffect(StatusEffectID.Chill);
            newComponent.chance = chillChance;
        }

        if (increasedDuration != 0 || baseDurationis2)
        {
            DestroyAfterDuration dad = abilityObject.GetComponent<DestroyAfterDuration>();
            if (dad)
            {
                if (baseDurationis2) {
                    dad.duration = 2f + (1f * increasedDuration);
                }
                else
                {
                    dad.duration *= (1f + increasedDuration);
                }
            }
        }

        if (leavesExplosion)
        {
            CreateAbilityObjectOnDeath component = abilityObject.AddComponent<CreateAbilityObjectOnDeath>();
            component.abilityToInstantiate = AbilityIDList.getAbility(AbilityID.fireCircle);
            component.offset = new Vector3(0f, -1f, 0f);

            FireCircleMutator circleMut = abilityObject.AddComponent<FireCircleMutator>();
            circleMut.chillChance = chillChance;
            circleMut.convertToCold = convertToCold;
            circleMut.igniteChance = explosionIgniteChance;
            circleMut.increasedDamage = increasedExplosionDamage;
        }

        if (delayedExpolosionAtStart)
        {
            CastAfterDuration component = abilityObject.AddComponent<CastAfterDuration>();
            component.ability = AbilityIDList.getAbility(AbilityID.delayedFireCircle);
            component.limitCasts = true;
            component.remainingCasts = 1;
            component.age = 10f;
            component.interval = 1f;
            component.offset = new Vector3(0f, -1.3f, 0f);

            DelayedFireCircleMutator circleMut = abilityObject.AddComponent<DelayedFireCircleMutator>();
            circleMut.chillChance = chillChance;
            circleMut.convertToCold = convertToCold;
            circleMut.igniteChance = explosionIgniteChance;
            circleMut.increasedDamage = increasedExplosionDamage;
        }

        if (leavesExplosiveGround)
        {
            CastAfterDuration cad = abilityObject.AddComponent<CastAfterDuration>();
            if (convertToCold) { cad.ability = AbilityIDList.getAbility(AbilityID.shatteringGround); }
            else { cad.ability = AbilityIDList.getAbility(AbilityID.explosiveGround); }
            cad.limitCasts = false;
            cad.interval = 0.79f / (1f + increasedExplosiveGroundFrequency);
            cad.age = 0.6f / (1f + increasedExplosiveGroundFrequency);
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
