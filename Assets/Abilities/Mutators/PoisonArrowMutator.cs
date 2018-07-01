using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonArrowMutator : AbilityMutator
{

    public int extraProjectiles = 0;
    
    public int targetsToPierce = 0;

    public float increasedAttackSpeed = 0f;

    public int delayedAttacks = 0;

    public bool homing = false;

    public float increasedSpeed = 0f;

    public float increasedDuration = 0f;

    public bool inaccuracy = false;

    public float increasedDamage = 0f;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.poisonArrow);
        base.Awake();
    }

    public override float mutateUseSpeed(float useSpeed)
    {
        return base.mutateUseSpeed(useSpeed) * (1 + increasedAttackSpeed);
    }

    public override int mutateDelayedCasts(int defaultCasts)
    {
        if (delayedAttacks > 0)
        {
            return base.mutateDelayedCasts(defaultCasts) + delayedAttacks;
        }
        else
        {
            return base.mutateDelayedCasts(defaultCasts);
        }
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        // increase speed
        if (increasedSpeed != 0)
        {
            abilityObject.GetComponent<AbilityMover>().speed *= (1 + increasedSpeed);
        }

        // increase duration
        if (increasedDuration != 0)
        {
            abilityObject.GetComponent<DestroyAfterDuration>().duration *= (1 + increasedDuration);
        }

        // slightly randomise target location
        if (inaccuracy)
        {
            targetLocation += new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * 0.25f * Vector3.Distance(location, targetLocation);
        }

        // add extra projectiles
        if (extraProjectiles != 0)
        {
            ExtraProjectiles extraProjectilesObject = abilityObject.GetComponent<ExtraProjectiles>();
            if (extraProjectilesObject == null) { extraProjectilesObject = abilityObject.AddComponent<ExtraProjectiles>(); extraProjectilesObject.numberOfExtraProjectiles = 0; }
            extraProjectilesObject.numberOfExtraProjectiles += extraProjectiles;
        }

        // add pierce change
        if (targetsToPierce > 0)
        {
            Pierce pierce = abilityObject.GetComponent<Pierce>();
            if (pierce == null) { pierce = abilityObject.AddComponent<Pierce>(); pierce.objectsToPierce = 0; }
            pierce.objectsToPierce += targetsToPierce;
        }

        if (increasedDamage != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(increasedDamage);
                //holder.addBaseDamage(DamageType.FIRE, holder.getBaseDamage(DamageType.FIRE) * increasedDamage);
            }
        }

        if (homing)
        {
            HomingMovement component = abilityObject.AddComponent<HomingMovement>();
            component.changeFacingDirection = true;
        }


        return abilityObject;
    }
}
