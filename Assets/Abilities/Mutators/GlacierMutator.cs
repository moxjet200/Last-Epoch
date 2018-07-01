using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlacierMutator : AbilityMutator
{

    [Range(0f, 1f)]
    public float chanceToCreateIceVortexGlobal = 0f;
    [Range(0f, 1f)]
    public float chanceToCreateIceVortex1 = 0f;
    [Range(0f, 1f)]
    public float chanceToCreateIceVortex2 = 0f;
    [Range(0f, 1f)]
    public float chanceToCreateIceVortex3 = 0f;


    [Range(0f, 1f)]
    public float chanceForSuperIceVortex = 0f;

    [Range(0f, 1f)]
    public float chillChanceGlobal = 0f;
    [Range(0f, 1f)]
    public float chillChance1 = 0f;
    [Range(0f, 1f)]
    public float chillChance2 = 0f;
    [Range(0f, 1f)]
    public float chillChance3 = 0f;

    public float increasedDamage1 = 0f;
    public float increasedDamage2 = 0f;
    public float increasedDamage3 = 0f;

    public float moreDamageAgainstChilled = 0f;

    public float increasedStunChance = 0f;
    public float addedCritChance = 0f;

    public bool reverseExplosions = false;
    public bool removeLargestExplosion = false;
    public bool noMovement = false;

    public float percentManaGainedOnKill = 0f;
    public float percentManaGainedOnHit = 0f;

    BaseMana mana = null;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.glacier);
        mana = GetComponent<BaseMana>();
        AbilityEventListener eventListener = GetComponent<AbilityEventListener>();
        if (eventListener)
        {
            eventListener.onKillEvent += GainManaOnKill;
            eventListener.onHitEvent += GainManaOnHit;
        }
        base.Awake();
    }

    public void GainManaOnKill(Ability _ability, GameObject target)
    {
        if (percentManaGainedOnKill > 0 && (_ability == ability || _ability == AbilityIDList.getAbility(AbilityID.glacier1)
            || _ability == AbilityIDList.getAbility(AbilityID.glacier2) || _ability == AbilityIDList.getAbility(AbilityID.glacier3)))
        {
            mana.gainMana(percentManaGainedOnKill * mana.maxMana);
        }
    }

    public void GainManaOnHit(Ability _ability, GameObject target)
    {
        if (percentManaGainedOnHit > 0 && (_ability == ability || _ability == AbilityIDList.getAbility(AbilityID.glacier1)
            || _ability == AbilityIDList.getAbility(AbilityID.glacier2) || _ability == AbilityIDList.getAbility(AbilityID.glacier3)))
        {
            mana.gainMana(percentManaGainedOnHit * mana.maxMana);
        }
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        // attach mutators
        Glacier1Mutator g1m = abilityObject.AddComponent<Glacier1Mutator>();
        g1m.chanceToCreateIceVortex = chanceToCreateIceVortexGlobal + chanceToCreateIceVortex1;
        g1m.chillChance = chillChanceGlobal + chillChance1;
        g1m.increasedDamage = increasedDamage1;
        g1m.moreDamageAgainstChilled = moreDamageAgainstChilled;
        g1m.increasedStunChance = increasedStunChance;
        g1m.addedCritChance = addedCritChance;
        g1m.chanceForSuperIceVortex = chanceForSuperIceVortex;

        Glacier2Mutator g2m = abilityObject.AddComponent<Glacier2Mutator>();
        g2m.chanceToCreateIceVortex = chanceToCreateIceVortexGlobal + chanceToCreateIceVortex2;
        g2m.chillChance = chillChanceGlobal + chillChance2;
        g2m.increasedDamage = increasedDamage2;
        g2m.moreDamageAgainstChilled = moreDamageAgainstChilled;
        g2m.increasedStunChance = increasedStunChance;
        g2m.addedCritChance = addedCritChance;
        g2m.chanceForSuperIceVortex = chanceForSuperIceVortex;

        Glacier3Mutator g3m = abilityObject.AddComponent<Glacier3Mutator>();
        g3m.chanceToCreateIceVortex = chanceToCreateIceVortexGlobal + chanceToCreateIceVortex3;
        g3m.chillChance = chillChanceGlobal + chillChance3;
        g3m.increasedDamage = increasedDamage3;
        g3m.moreDamageAgainstChilled = moreDamageAgainstChilled;
        g3m.increasedStunChance = increasedStunChance;
        g3m.addedCritChance = addedCritChance;
        g3m.chanceForSuperIceVortex = chanceForSuperIceVortex;


        // reverse order of explosions
        if (reverseExplosions)
        {
            CastAfterDuration cad1 = null;
            CastAfterDuration cad3 = null;
            foreach (CastAfterDuration cad in abilityObject.GetComponents<CastAfterDuration>())
            {
                if (cad.ability == AbilityIDList.getAbility(AbilityID.glacier1)) { cad1 = cad; }
                else if (cad.ability == AbilityIDList.getAbility(AbilityID.glacier3)) { cad3 = cad; }
            }

            if (cad1 && cad3)
            {
                cad1.ability = AbilityIDList.getAbility(AbilityID.glacier3);
                cad3.ability = AbilityIDList.getAbility(AbilityID.glacier1);
            }
        }

        // remove the largest explosion
        if (removeLargestExplosion)
        {
            CastAfterDuration cadToDestroy = null;
            foreach (CastAfterDuration cad in abilityObject.GetComponents<CastAfterDuration>())
            {
                if (cad.ability == AbilityIDList.getAbility(AbilityID.glacier3)) { cadToDestroy = cad; }
            }
            cadToDestroy.enabled = false;
            Destroy(cadToDestroy);
        }

        // make all the explosions occur at the target location
        if (noMovement)
        {
            abilityObject.GetComponent<AbilityMover>().speed = 0;
            abilityObject.AddComponent<StartsAtTarget>();
        }




        return abilityObject;
    }
}