using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornTotemAttackMutator : AbilityMutator
{
    public int extraProjectiles = 0;
    public float chanceForDoubleDamage = 0f;
    public bool homing = false;
    public float chanceToShredArmour = 0f;
    public float chanceToPoison = 0f;
    public float reducedSpread = 0f;
    public bool singleProjectile = false;
    public bool firstProjectileIsAccurate = false;
    public float increasedDamage = 0f;
    public int targetsToPierce = 0;
    public float increasedSpeed = 0f;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.thornTotemAttack);
        base.Awake();
    }


    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {

        // add extra projectiles
        if (extraProjectiles != 0)
        {
            ExtraProjectiles extraProjectilesObject = abilityObject.GetComponent<ExtraProjectiles>();
            if (extraProjectilesObject == null) { extraProjectilesObject = abilityObject.AddComponent<ExtraProjectiles>(); extraProjectilesObject.numberOfExtraProjectiles = 0; }
            extraProjectilesObject.numberOfExtraProjectiles += extraProjectiles;
        }

        // add pierce
        if (targetsToPierce > 0)
        {
            Pierce pierce = abilityObject.GetComponent<Pierce>();
            if (pierce == null) { pierce = abilityObject.AddComponent<Pierce>(); pierce.objectsToPierce = 0; }
            pierce.objectsToPierce += targetsToPierce;
        }

        // increased projectile speed
        if (increasedSpeed != 0)
        {
            AbilityMover am = abilityObject.GetComponent<AbilityMover>();
            am.speed *= (1 + increasedSpeed);
        }

        if (chanceForDoubleDamage > 0)
        {
            float rand = Random.Range(0f, 1f);
            if (rand < chanceForDoubleDamage)
            {
                foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
                {
                    for (int i = 0; i < holder.baseDamageStats.damage.Count; i++)
                    {
                        holder.addBaseDamage(holder.baseDamageStats.damage[i].damageType, holder.getBaseDamage(holder.baseDamageStats.damage[i].damageType));
                    }
                }
            }
        }

        if (increasedDamage != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(increasedDamage);
            }
        }

        if (chanceToShredArmour > 0)
        {
            ChanceToApplyStatusOnEnemyHit chanceTo = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            chanceTo.chance = chanceToShredArmour;
            chanceTo.statusEffect = StatusEffectList.getEffect(StatusEffectID.ArmourShred);
        }

        if (chanceToPoison > 0)
        {
            ChanceToApplyStatusOnEnemyHit chanceTo = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            chanceTo.chance = chanceToPoison;
            chanceTo.statusEffect = StatusEffectList.getEffect(StatusEffectID.Poison);
        }

        if (singleProjectile)
        {
            ExtraProjectiles extraProjectiles = abilityObject.GetComponent<ExtraProjectiles>();
            if (extraProjectiles)
            {
                extraProjectiles.numberOfExtraProjectiles = 0;
            }
        }
        else if (reducedSpread != 0)
        {
            ExtraProjectiles extraProjectiles = abilityObject.GetComponent<ExtraProjectiles>();
            if (extraProjectiles)
            {
                extraProjectiles.angle -= reducedSpread;
                if (extraProjectiles.angle < 0) { extraProjectiles.angle = 0; }
            }
        }

        if (firstProjectileIsAccurate)
        {
            RandomiseDirection rd = abilityObject.GetComponent<RandomiseDirection>();
            if (rd)
            {
                rd.maximumAngleChange = 0;
            }
        }

        if (homing)
        {
            abilityObject.AddComponent<HomingMovement>();
        }

        return abilityObject;
    }

}
