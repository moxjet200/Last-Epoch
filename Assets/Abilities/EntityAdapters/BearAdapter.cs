using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearAdapter : EntityAdapter
{
    public bool thornBear = false;

    public bool retaliates = false;
    public float thornPoisonChance = 0f;

    public float thornBurstBleedChance = 0f;
    public float thornBurstAddedSpeed = 0f;


    public float ThornAttackChanceForDoubleDamage = 0f;
    public float ThornAttackChanceToShredArmour = 0f;
    public int thornAttackEnemiesToPierce = 0;

    public bool swipes = false;

    public float clawTotemOnKillChance = 0f;

    public override GameObject adapt(GameObject entity)
    {
        if (thornBear)
        {
            // activate thorns
            foreach (EntityObjectIndicator eoi in entity.GetComponentsInChildren<EntityObjectIndicator>(true))
            {
                if (eoi.objectType == EntityObjectIndicator.EntityObjectType.Thorn)
                {
                    eoi.gameObject.SetActive(true);
                }
            }

            // change attack
            ThornTotemAttackMutator ttam = entity.AddComponent<ThornTotemAttackMutator>();
            ttam.singleProjectile = true;
            ttam.firstProjectileIsAccurate = true;
            ttam.increasedDamage = 3f;
            ttam.chanceForDoubleDamage = ThornAttackChanceForDoubleDamage;
            ttam.chanceToShredArmour = ThornAttackChanceToShredArmour;
            ttam.chanceToPoison = thornPoisonChance;
            ttam.targetsToPierce = thornAttackEnemiesToPierce;
        }

        if (retaliates)
        {
            ThornBurstMutator tbm = entity.AddComponent<ThornBurstMutator>();
            tbm.chanceToPoison = thornPoisonChance;
            tbm.chanceToBleed = thornBurstBleedChance;
            tbm.addedSpeed = thornBurstAddedSpeed;
        }

        if (clawTotemOnKillChance > 0)
        {
            ChanceToCastOnKill ctcok = entity.AddComponent<ChanceToCastOnKill>();
            ctcok.summonerCasts = true;
            ctcok.castChance = clawTotemOnKillChance;
            ctcok.ability = AbilityIDList.getAbility(AbilityID.summonClawTotem);
        }


        if (swipes)
        {
            SwipeMutator sm = entity.AddComponent<SwipeMutator>();
            sm.addedPhysicalDamage = 32;
        }

        return base.adapt(entity);
    }



}