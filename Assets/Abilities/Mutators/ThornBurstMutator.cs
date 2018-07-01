using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornBurstMutator : AbilityMutator {
    
    public float extraProjectilesChance = 0f;
    public float chanceToPoison = 0f;
    public float chanceToBleed = 0f;
    public float pierceChance = 0f;
    public float reducedSpread = 0f;
    public float addedSpeed = 0f;

    public bool thornShield = false;
    public float addedShieldDuration = 0f;
    public float chanceOfRecreatingThornShield = 0f;
    public bool thornShieldAiming = false;

    public bool thisShieldRecreatesItself = false;
    public bool canCastOnAllies = false;

    public float castWhenHitChance = 0f;

    public float increasedDamage = 0f;

    public float thornTrailOnKillChance = 0f;
    public float sunderingThornsOnHitChance = 0f;
    float sunderingThornsBaseCooldown = 4f;
    public float increasedSunderingThornsCooldownRecovery = 0f;
    float sunderingThornsCooldownIndex = 0f;

    AbilityObjectConstructor aoc = null;
    UsingAbility ua = null;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.thornBurst);
        base.Awake();
        ProtectionClass protectionClass = GetComponent<ProtectionClass>();
        if (protectionClass)
        {
            GetComponent<ProtectionClass>().detailedDamageEvent += whenHit;
        }
        ua = GetComponent<UsingAbility>();
        if (ua)
        {
            AbilityEventListener listener = GetComponent<AbilityEventListener>();
            if (listener)
            {
                listener.onKillEvent += OnKill;
                listener.onHitEvent += OnHit;
            }
        }
    }

    public void OnKill(Ability _ability, GameObject target)
    {
        if (_ability == ability)
        {
            if (thornTrailOnKillChance > 0)
            {
                float rand = Random.Range(0f, 1f);
                if (rand < thornTrailOnKillChance)
                {
                    ua.UseAbility(AbilityIDList.getAbility(AbilityID.thornTrail), transform.position + transform.forward, false, false);
                }
            }
        }
    }

    public void OnHit(Ability _ability, GameObject target)
    {
        if (_ability == ability)
        {
            if (sunderingThornsOnHitChance > 0 && (sunderingThornsCooldownIndex >= sunderingThornsBaseCooldown / (1 + increasedSunderingThornsCooldownRecovery)))
            {
                float rand = Random.Range(0f, 1f);
                if (rand < sunderingThornsOnHitChance)
                {
                    sunderingThornsCooldownIndex = 0f;
                    if (target != null) { ua.UseAbility(AbilityIDList.getAbility(AbilityID.sunderingThorns), target.transform.position, false, false); }
                    else { ua.UseAbility(AbilityIDList.getAbility(AbilityID.sunderingThorns), transform.position + transform.forward, false, false); }
                }
            }
        }
    }

    public void whenHit(int damage, List<HitEvents> hitEvents)
    {
        if (castWhenHitChance > 0 && hitEvents.Contains(HitEvents.Hit))
        {
            float rand = Random.Range(0f, 1f);
            if (rand < castWhenHitChance)
            {
                if (!aoc) { aoc = GetComponent<AbilityObjectConstructor>(); }
                aoc.constructAbilityObject(ability, transform.position + new Vector3(0f, 1.1f, 0f), transform.position + transform.forward + new Vector3(0f, 1.1f, 0f));
            }
        }
    }

    public override void Update()
    {
        base.Update();
        if (sunderingThornsCooldownIndex < sunderingThornsBaseCooldown / (1 + increasedSunderingThornsCooldownRecovery))
        {
            sunderingThornsCooldownIndex += Time.deltaTime;
        }
    }

    public override List<Tags.AbilityTags> getUseTags()
    {
        List<Tags.AbilityTags> list = base.getUseTags();
        if (thornShield)
        {
            list.Add(Tags.AbilityTags.Buff);
        }
        return list;
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        // if this skill casts a thorn shield that casts thorn burst then different things needs to be done to it
        if (thornShield)
        {
            // switch the ability object for a thorn shield
            Destroy(abilityObject);
            abilityObject = Instantiate(AbilityIDList.getAbility(AbilityID.thornShield).abilityPrefab, location, Quaternion.Euler(targetLocation - location));

            // the thorn shield's thorn burst mutator must be given the correct values
            ThornBurstMutator thornShieldMutator = abilityObject.GetComponent<ThornBurstMutator>();
            if (thornShieldMutator)
            {
                thornShieldMutator.extraProjectilesChance = extraProjectilesChance;
                thornShieldMutator.chanceToPoison = chanceToPoison;
                thornShieldMutator.pierceChance = pierceChance;
                thornShieldMutator.reducedSpread = reducedSpread;
                thornShieldMutator.addedSpeed = addedSpeed;
                thornShieldMutator.thornShieldAiming = thornShieldAiming;
                thornShieldMutator.addedShieldDuration = addedShieldDuration;
                thornShieldMutator.chanceOfRecreatingThornShield = chanceOfRecreatingThornShield;
                thornShieldMutator.increasedDamage = increasedDamage;
                thornShieldMutator.chanceToBleed = chanceToBleed;
                thornShieldMutator.thornShield = false;

                if (chanceOfRecreatingThornShield > 0)
                {
                    float rand = Random.Range(0f, 1f);
                    if (rand < chanceOfRecreatingThornShield)
                    {
                        thornShieldMutator.thisShieldRecreatesItself = true;
                    }
                    else { thornShieldMutator.thisShieldRecreatesItself = false; }
                }
            }
            
            if (addedShieldDuration > 0)
            {
                if (abilityObject.GetComponent<DestroyAfterDuration>())
                {
                    abilityObject.GetComponent<DestroyAfterDuration>().duration += addedShieldDuration;
                }
            }

            // if the caster can cast on allies or this is a shield
            if (canCastOnAllies || (GetComponent<BuffParent>() && transform.parent))
            {
                abilityObject.GetComponent<AttachToCreatorOnCreation>().runOnCreation = false;
                AttachToNearestAllyOnCreation attachToAlly = abilityObject.AddComponent<AttachToNearestAllyOnCreation>();
                attachToAlly.replaceExistingBuff = false;
                attachToAlly.displacement = abilityObject.GetComponent<AttachToCreatorOnCreation>().displacement;
                abilityObject.AddComponent<StartsAtTarget>();
            }

            return abilityObject;
        }

        // only do this if the caster is a shield
        changeTargetLocation = false;
        if (GetComponent<BuffParent>() && transform.parent)
        {
            if (thornShieldAiming)
            {
                changeTargetLocation = true;
                newTargetLocation = transform.position + transform.parent.forward;
            }

            if (thisShieldRecreatesItself)
            {
                thornShield = true;
                GetComponent<AbilityObjectConstructor>().constructAbilityObject(ability, location, targetLocation, null, false);
                thornShield = false;
            }

            // reduce the delay on the extra projectiles, otherwise it looks unnatural because their cast point does not move
            ExtraProjectiles extraProj = abilityObject.GetComponent<ExtraProjectiles>();
            if (extraProj)
            {
                extraProj.delayWindow = 0.05f;
            }
        }

        if (increasedDamage != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(increasedDamage);
            }
        }

        if (chanceToPoison > 0)
        {
            ChanceToApplyStatusOnEnemyHit newComponent = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            newComponent.statusEffect = StatusEffectList.getEffect(StatusEffectID.Poison);
            newComponent.chance = chanceToPoison;
        }

        if (chanceToBleed > 0)
        {
            ChanceToApplyStatusOnEnemyHit newComponent = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            newComponent.statusEffect = StatusEffectList.getEffect(StatusEffectID.Bleed);
            newComponent.chance = chanceToBleed;
        }

        if (pierceChance > 0)
        {
            float rand = Random.Range(0f, 1f);
            if (rand < pierceChance)
            {
                Pierce pierce = abilityObject.GetComponent<Pierce>();
                if (pierce == null) { pierce = abilityObject.AddComponent<Pierce>(); pierce.objectsToPierce = 0; }
                pierce.objectsToPierce += 100;
            }
        }

        if (reducedSpread != 0)
        {
            ExtraProjectiles extraProjectilesObject = abilityObject.GetComponent<ExtraProjectiles>();
            if (extraProjectilesObject == null) { extraProjectilesObject = abilityObject.AddComponent<ExtraProjectiles>(); extraProjectilesObject.numberOfExtraProjectiles = 0; }
            extraProjectilesObject.angle -= reducedSpread;
        }

        if (extraProjectilesChance != 0)
        {
            float rand = Random.Range(0f, 1f);
            if (rand < extraProjectilesChance)
            {
                ExtraProjectiles extraProjectilesObject = abilityObject.GetComponent<ExtraProjectiles>();
                if (extraProjectilesObject == null) { extraProjectilesObject = abilityObject.AddComponent<ExtraProjectiles>(); extraProjectilesObject.numberOfExtraProjectiles = 0; }
                extraProjectilesObject.numberOfExtraProjectiles += 6;
            }
        }

        if (addedSpeed > 0)
        {
            abilityObject.GetComponent<AbilityMover>().speed += addedSpeed;
        }

        return abilityObject;
    }
}
