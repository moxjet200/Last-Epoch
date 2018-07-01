using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornTotemAdapter : EntityAdapter
{
    public float poisonCloudChance = 0f;
    public int extraProjectiles = 0;
    public bool homing = false;
    public float chanceForDoubleDamage = 0f;
    public float chanceToShredArmour = 0f;
    public float chanceToPoison = 0f;
    public float reducedSpread = 0f;
    public float increasedThornTotemAttackDamage = 0f;
    public int targetsToPierce = 0;
    public float increasedSpeed = 0f;

    public override GameObject adapt(GameObject entity)
    {
        if (poisonCloudChance > 0)
        {
            float rand = Random.Range(0f, 1f);
            if (rand < poisonCloudChance)
            {
                UseAbilityOnActorDeath comp = entity.AddComponent<UseAbilityOnActorDeath>();
                comp.abilityToInstantiate = AbilityIDList.getAbility(AbilityID.poisonCloud);
            }
        }

        ThornTotemAttackMutator ttam = entity.GetComponent<ThornTotemAttackMutator>();
        if (!ttam) { ttam = entity.AddComponent<ThornTotemAttackMutator>(); }
        ttam.extraProjectiles = extraProjectiles;
        ttam.homing = homing;
        ttam.chanceForDoubleDamage = chanceForDoubleDamage;
        ttam.chanceToShredArmour = chanceToShredArmour;
        ttam.chanceToPoison = chanceToPoison;
        ttam.reducedSpread = reducedSpread;
        ttam.increasedDamage = increasedThornTotemAttackDamage;
        ttam.targetsToPierce = targetsToPierce;
        ttam.increasedSpeed = increasedSpeed;

        return base.adapt(entity);
    }
}
