using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorMutator : AbilityMutator
{
    // shower variables
    public int additionalMeteors = 0;
    public float increasedMeteorFrequency = 0;
    public float increasedShowerRadius = 0f;
    public bool line = false;

    // variables for all damage
    public List<float> moreDamageInstances = new List<float>();

    // meteor variables
    public float increasedFallSpeed = 0f;
    public float increasedCastSpeed = 0f;
    public bool usesAllMana = false;
    public float shrapnelChance = 0f;
    public float increasedShrapnelSpeed = 0f;
    public bool shrapnelPierces = false;
    public bool replaceFireCircle = false;

    // aoe variables
    public float moreDamageAgainstFullHealth = 0f;
    public float increasedShrapnelDamage = 0f;
    public float increasedStunChance = 0f;

    BaseMana myMana = null;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.meteor);
        myMana = GetComponent<BaseMana>();
        base.Awake();
    }

    public override float mutateUseSpeed(float useSpeed)
    {
        return base.mutateUseSpeed(useSpeed) * (1 + increasedCastSpeed);
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {

        // if this is a meteor shower
        if (additionalMeteors > 0)
        {
            Destroy(abilityObject);

            abilityObject = Instantiate(AbilityIDList.getAbility(AbilityID.meteorShower).abilityPrefab);
            abilityObject.transform.position = location;
            CastAtRandomPointAfterDuration meteorCaster = abilityObject.GetComponent<CastAtRandomPointAfterDuration>();
            meteorCaster.duration /= (1 + increasedMeteorFrequency);
            meteorCaster.remainingCasts += additionalMeteors;
            meteorCaster.radius *= (1 + increasedShowerRadius);

            if (line)
            {
                StartsAtTarget sat = meteorCaster.GetComponent<StartsAtTarget>();
                if (sat)
                {
                    sat.active = false;
                    Destroy(sat);
                }
                AbilityMover mover = Comp<AbilityMover>.GetOrAdd(abilityObject);
                mover.SetDirection(targetLocation - location);
                mover.speed = 10 * (1 + increasedShowerRadius);
                meteorCaster.radius = 0;
                StartsTowardsTarget stt = abilityObject.AddComponent<StartsTowardsTarget>();
                stt.distance = 1f;
                stt.addWeaponRange = false;
            }

            // create a mutator on the meteor shower object
            MeteorMutator newMutator = abilityObject.AddComponent<MeteorMutator>();

            

            // variables for all damage
            newMutator.moreDamageInstances = moreDamageInstances;

            // meteor variables (except use all mana)
            newMutator.increasedFallSpeed = increasedFallSpeed;
            newMutator.increasedCastSpeed = increasedCastSpeed;
            newMutator.shrapnelChance = shrapnelChance;
            newMutator.increasedShrapnelSpeed = increasedShrapnelSpeed;
            newMutator.shrapnelPierces = shrapnelPierces;
            newMutator.replaceFireCircle = replaceFireCircle;

            // aoe variables
            newMutator.moreDamageAgainstFullHealth = moreDamageAgainstFullHealth;
            newMutator.increasedShrapnelDamage = increasedShrapnelDamage;
            newMutator.increasedStunChance = increasedStunChance;
            newMutator.moreDamageInstances = new List<float>();
            newMutator.moreDamageInstances.AddRange(moreDamageInstances);

            // apply mana variable once for all meteors
            if (usesAllMana && myMana)
            {
                float totalCost = myMana.getManaCost(ability);
                totalCost += myMana.currentMana;

                float proportion = totalCost / myMana.maxMana;
                newMutator.moreDamageInstances.Add(proportion * 2);

                myMana.currentMana = 0;
            }

            return abilityObject;
        }


        // if this is a regular meteor

        // apply aoe variables
        MeteorAoEMutator aoeMutator = abilityObject.AddComponent<MeteorAoEMutator>();
        aoeMutator.moreDamageAgainstFullHealth = moreDamageAgainstFullHealth;
        aoeMutator.increasedStunChance = increasedStunChance;
        aoeMutator.moreDamageInstances = new List<float>();
        aoeMutator.moreDamageInstances.AddRange(moreDamageInstances);


        // meteor variables

        if (increasedFallSpeed != 0)
        {
            abilityObject.GetComponent<AbilityMover>().speed *= (1 + increasedFallSpeed);
        }

        MeteorShrapnelMutator shrapnelMutator = null;
        if (shrapnelChance > 0)
        {
            float rand = Random.Range(0f, 1f);
            if (rand < shrapnelChance)
            {
                CreateAbilityObjectOnDeath caood = abilityObject.AddComponent<CreateAbilityObjectOnDeath>();
                caood.abilityToInstantiate = AbilityIDList.getAbility(AbilityID.meteorShrapnel);
                caood.createAtTarget = true;
                caood.offset = -caood.GetComponent<LocationDetector>().targetLocationOffset;
                shrapnelMutator = abilityObject.AddComponent<MeteorShrapnelMutator>();
                shrapnelMutator.increasedDamage = increasedShrapnelDamage;
                shrapnelMutator.increasedSpeed = increasedShrapnelSpeed;
                shrapnelMutator.pierces = shrapnelPierces;
                shrapnelMutator.increasedStunChance = increasedStunChance;
                shrapnelMutator.moreDamageInstances = new List<float>();
                shrapnelMutator.moreDamageInstances.AddRange(moreDamageInstances);
            }
        }

        if (replaceFireCircle)
        {
            CreateAtTargetLocationOnCreation component = abilityObject.GetComponent<CreateAtTargetLocationOnCreation>();
            if (component)
            {
                component.objectsToCreate.Clear();
                GameObject prefab = PrefabList.getPrefab("EnemyMeteorCircle");
                if (prefab)
                {
                    CreateAtTargetLocationOnCreation.GameObjectHolder holder = new CreateAtTargetLocationOnCreation.GameObjectHolder();
                    holder.gameObject = prefab;
                    holder.destroyWhenThisDies = true;
                    component.objectsToCreate.Add((holder));
                }
            }
        }

        if (usesAllMana && myMana)
        {
            float totalCost = myMana.getManaCost(ability);
            totalCost += myMana.currentMana;

            float proportion = totalCost / myMana.maxMana;

            aoeMutator.moreDamageInstances.Add(proportion * 2);
            if (shrapnelMutator) { shrapnelMutator.moreDamageInstances.Add(proportion * 2); }

            myMana.currentMana = 0;
        }


        return abilityObject;
    }

}
