using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuryLeapMutator : AbilityMutator {

    public bool resetCooldownOnKill = true;

    public float addedCritMultiplier = 0f;
    public bool castsLightning = false;
    public float lightningInterval = 0.25f;
    public bool lightningKillsResetCooldown = false;

    public float chanceToResetCooldownOnAnyKill = 0f;

    public float movespeedOnLanding = 0f;
    public float attackAndCastSpeedOnLanding = 0f;

    public bool eligiblePetsJumpToo = false;
    public float increasedStunChance = 0f;

    public float increasedDamage = 0f;
    public float chanceToPull = 0f;
    public float chanceToSummonVinesAtStart = 0f;
    public float chanceToSummonVinesAtEnd = 0f;

    public float moreDamageAgainstFullHealth = 0f;

    public float increasedRadius = 0f;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.furyLeap);
        base.Awake();
        GetComponent<AbilityEventListener>().onKillEvent += ResetCooldown;
    }

    public void ResetCooldown(Ability _ability, GameObject target)
    {
        // if reset cooldown on kill is true and there was a kill from this ability then reset the cooldown
        if (resetCooldownOnKill && (_ability == ability || _ability == AbilityIDList.getAbility(AbilityID.furyLeapAoe) 
            || (lightningKillsResetCooldown && _ability == AbilityIDList.getAbility(AbilityID.furyLeapLightning))))
        {
            GetComponent<ChargeManager>().addAbilityCharges(ability,1);
        }
        else if (chanceToResetCooldownOnAnyKill > 0f)
        {
            float rand = Random.Range(0f, 1f);
            if (rand < chanceToResetCooldownOnAnyKill)
            {
                GetComponent<ChargeManager>().addAbilityCharges(ability, 1);
            }
        }
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {

        if (castsLightning)
        {
            RepeatedlyCastAtNearestEnemyWithinRadius cast = abilityObject.AddComponent<RepeatedlyCastAtNearestEnemyWithinRadius>();
            cast.abilityToCast = AbilityIDList.getAbility(AbilityID.furyLeapLightning);
            cast.castInterval = lightningInterval;
            cast.radius = 3f;
        }

        if (movespeedOnLanding != 0)
        {
            BuffCreatorOnDeath buff = abilityObject.AddComponent<BuffCreatorOnDeath>();
            abilityObject.AddComponent<BuffCreatorOnDeath>().addBuffToList(Tags.Properties.Movespeed, 0f, movespeedOnLanding, null, null, 3f, null, "FuryLeapMoveSpeed");
        }

        if (attackAndCastSpeedOnLanding != 0)
        {
            BuffCreatorOnDeath buff = abilityObject.AddComponent<BuffCreatorOnDeath>();
            buff.addBuffToList(Tags.Properties.AttackSpeed, 0f, attackAndCastSpeedOnLanding, null, null, 3f, null, "FuryLeapAttackSpeed");
            buff.addBuffToList(Tags.Properties.CastSpeed, 0f, attackAndCastSpeedOnLanding, null, null, 3f, null, "FuryLeapCastSpeed");
        }

        if (eligiblePetsJumpToo)
        {
            SummonTracker summonTracker = GetComponent<SummonTracker>();
            if (summonTracker)
            {
                foreach (Summoned summon in summonTracker.summons)
                {
                    if (summon.GetComponent<CanJump>() && summon.GetComponent<UsingAbility>() && summon.GetComponent<StateController>()
                        && summon.GetComponent<StateController>().currentState && summon.GetComponent<StateController>().currentState.priority < 60)
                    {
                        summon.GetComponent<UsingAbility>().UseAbility(AbilityIDList.getAbility(AbilityID.furyLeap), targetLocation, false, false);
                    }
                }
            }
        }

        FuryLeapAoeMutator flam = abilityObject.AddComponent<FuryLeapAoeMutator>();
        flam.increasedDamage = increasedDamage;
        flam.increasedRadius = increasedRadius;
        flam.increasedStunChance = increasedStunChance;
        flam.moreDamageAgainstFullHealth = moreDamageAgainstFullHealth;
        flam.addedCritMultiplier = addedCritMultiplier;

        if (chanceToPull > 0)
        {
            float rand = Random.Range(0f, 1f);
            if (rand < chanceToPull)
            {
                abilityObject.AddComponent<CreateAbilityObjectOnDeath>().abilityToInstantiate = AbilityIDList.getAbility(AbilityID.bigPull);
            }
        }

        if (chanceToSummonVinesAtStart > 0)
        {
            float rand = Random.Range(0f, 1f);
            if (rand < chanceToSummonVinesAtStart)
            {
                GetComponent<UsingAbility>().UseAbility(AbilityIDList.getAbility(AbilityID.summonVines), transform.position, false, false);
            }
        }

        if (chanceToSummonVinesAtEnd > 0)
        {
            float rand = Random.Range(0f, 1f);
            if (rand < chanceToSummonVinesAtStart)
            {
                abilityObject.AddComponent<CreateAbilityObjectOnDeath>().abilityToInstantiate = AbilityIDList.getAbility(AbilityID.summonVines);
            }
        }

        return abilityObject;
    }
}
