using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntanglingRootsMutator : AbilityMutator
{
    public float increasedDamage = 0f;
    public float addedPoisonDamage = 0f;
    public float increasedRadius = 0f;

    public float initialHitIncreasedDamage = 0f;
    public float initialHitChanceToPoison = 0f;

    public int addedPatches = 0;
    public bool patchesInLine = false;

    public float vineOnKillChance = 0f;

    public float increasedBuffDuration = 0f;
    public float damageBuff = 0f;
    public float poisonChanceToWolves = 0f;
    public float bleedchanceToBears = 0f;
    public float castSpeedToSpriggans = 0f;
    public bool healSpriggans = false;

    public bool meleeScalingInitialHit = false;
    public bool InitialHitAlwaysStuns = false;
    public float increasedDuration = 0f;
    public float healingNovaChance = 0f;

    UsingAbility usingAbility = null;

    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.entanglingRoots);
        base.Awake();
        AbilityEventListener eventListener = GetComponent<AbilityEventListener>();
        if (eventListener)
        {
            eventListener.onKillEvent += OnKill;
        }
        usingAbility = GetComponent<UsingAbility>();
    }

    public void OnKill(Ability _ability, GameObject target)
    {
        if (vineOnKillChance > 0 && usingAbility && target)
        {
            if (_ability == ability || _ability == AbilityIDList.getAbility(AbilityID.entanglingRootsHit) || _ability == AbilityIDList.getAbility(AbilityID.entanglingRootsInitialHit))
            {
                float rand = Random.Range(0f, 1f);
                if (rand < vineOnKillChance)
                {
                    usingAbility.UseAbility(AbilityIDList.getAbility(AbilityID.summonVineAtTarget), target.transform.position, false, false);
                }
            }
        }
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        // changing the repeated hit mutator
        if (increasedDamage != 0 || addedPoisonDamage != 0 || increasedRadius != 0)
        {
            EntanglingRootsHitMutator hitMutator = abilityObject.AddComponent<EntanglingRootsHitMutator>();
            hitMutator.increasedDamage = increasedDamage;
            hitMutator.addedPoisonDamage = addedPoisonDamage;
            hitMutator.increasedRadius = increasedRadius;
        }

        // create the initial hit
        CastInLine initialiHitCreator = abilityObject.AddComponent<CastInLine>();
        initialiHitCreator.ability = AbilityIDList.getAbility(AbilityID.entanglingRootsInitialHit);
        initialiHitCreator.casts = 1;
        initialiHitCreator.distancePerCast = 0;
        initialiHitCreator.targetPoint = targetLocation;

        // changing the initial hit mutator
        EntanglingRootsInitialHitMutator initialHitMutator = abilityObject.AddComponent<EntanglingRootsInitialHitMutator>();
        initialHitMutator.increasedDamage = initialHitIncreasedDamage;
        initialHitMutator.chanceToPoison = initialHitChanceToPoison;
        initialHitMutator.increasedRadius = increasedRadius;
        initialHitMutator.increasedBuffDuration = increasedBuffDuration;
        initialHitMutator.damageBuff = damageBuff;
        initialHitMutator.poisonChanceToWolves = poisonChanceToWolves;
        initialHitMutator.bleedchanceToBears = bleedchanceToBears;
        initialHitMutator.castSpeedToSpriggans = castSpeedToSpriggans;
        initialHitMutator.healSpriggans = healSpriggans;
        initialHitMutator.meleeScalingInitialHit = meleeScalingInitialHit;
        initialHitMutator.alwaysStuns = InitialHitAlwaysStuns;

        // creating extra patches
        if (addedPatches > 0)
        {
            // add extra casts
            if (patchesInLine)
            {
                CastInLine component = abilityObject.AddComponent<CastInLine>();
                component.ability = AbilityIDList.getAbility(AbilityID.entanglingRoots);
                component.casts = addedPatches;
                component.distancePerCast = 4;
                component.targetPoint = targetLocation;
            }
            else
            {
                CastAtRandomPointAfterDuration component = abilityObject.AddComponent<CastAtRandomPointAfterDuration>();
                component.ability = AbilityIDList.getAbility(AbilityID.entanglingRoots);
                component.duration = 0.01f;
                component.limitCasts = true;
                component.remainingCasts = addedPatches;
                component.radius = 5f;
            }
            // copy mutator
            EntanglingRootsMutator mutator = abilityObject.AddComponent<EntanglingRootsMutator>();
            mutator.increasedDamage = increasedDamage;
            mutator.addedPoisonDamage = addedPoisonDamage;
            mutator.increasedRadius = increasedRadius;
            mutator.initialHitIncreasedDamage = initialHitIncreasedDamage;
            mutator.initialHitChanceToPoison = initialHitChanceToPoison;
            mutator.increasedBuffDuration = increasedBuffDuration;
            mutator.damageBuff = damageBuff;
            mutator.poisonChanceToWolves = poisonChanceToWolves;
            mutator.bleedchanceToBears = bleedchanceToBears;
            mutator.castSpeedToSpriggans = castSpeedToSpriggans;
            mutator.healSpriggans = healSpriggans;
            mutator.meleeScalingInitialHit = meleeScalingInitialHit;
            mutator.InitialHitAlwaysStuns = InitialHitAlwaysStuns;
            mutator.increasedDuration = increasedDuration;
            mutator.healingNovaChance = healingNovaChance;
        }

        if (increasedDuration != 0)
        {
            DestroyAfterDuration dad = abilityObject.GetComponent<DestroyAfterDuration>();
            if (dad)
            {
                dad.duration *= (1 + increasedDuration);
            }
        }

        if (increasedRadius != 0)
        {
            foreach (MagicalFX.FX_Tentacle_ultimate vfx in abilityObject.GetComponentsInChildren<MagicalFX.FX_Tentacle_ultimate>())
            {
                vfx.SpreadMin *= (1 + increasedRadius);
                vfx.SpreadMax *= (1 + increasedRadius);
                vfx.SpreadSpawn *= (1 + increasedRadius);
                vfx.Number = (int)(((float)vfx.Number) * (1 + increasedRadius));
            }
        }

        if (healingNovaChance > 0)
        {
            float rand = Random.Range(0f, 1f);
            if (rand < healingNovaChance)
            {
                CreateAbilityObjectOnDeath component = abilityObject.AddComponent<CreateAbilityObjectOnDeath>();
                component.abilityToInstantiate = AbilityIDList.getAbility(AbilityID.healingNova);
            }
        }

        return abilityObject;
    }
}
