using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerpentStrikeMutator : AbilityMutator
{
    public float snakeOnKillChance = 0f;
    public float increasedAttackSpeed = 0f;
    public float chanceToBleed = 0f;
    public float chanceToPlague = 0f;
    public float chanceToBlindingPoison = 0f;
    public float poisonSpitChance = 0f;
    public float increasedHitDamage = 0f;
    public float addedCritChance = 0f;
    public float lifeOnCrit = 0f;
    public float lifeOnKill = 0f;
    public float cullPercent = 0f;
    public float chanceToShredArmour = 0f;
    public float moreDamageAgainstBlinded = 0f;
    public float moreDamageAgainstPoisonBlinded = 0f;
    public float poisonDamageOnAttack = 0f;
    public float dotDamageOnAttack = 0f;
    public float dodgeRatingOnAttack = 0f;

    UsingAbility usingAbility = null;
    BaseHealth baseHealth = null;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.serpentStrike);
        AbilityEventListener listener = GetComponent<AbilityEventListener>();
        if (!listener) {
            listener = gameObject.AddComponent<AbilityEventListener>();
        }
        listener.onKillEvent += onKill;
        listener.onCritEvent += onCrit;
        base.Awake();
    }

    public override float mutateUseSpeed(float useSpeed)
    {
        return base.mutateUseSpeed(useSpeed) * (1 + increasedAttackSpeed);
    }

    public void onKill(Ability _ability, GameObject target)
    {
        if (snakeOnKillChance > 0 && (_ability == ability))
        {
            float rand = Random.Range(0f, 1f);
            if (rand < snakeOnKillChance)
            {
                if (!usingAbility) { usingAbility = GetComponent<UsingAbility>(); }
                if (usingAbility)
                {
                    usingAbility.UseAbility(AbilityIDList.getAbility(AbilityID.summonSerpent), target.transform.position, false, false);
                }
            }
        }
        if (lifeOnKill != 0 && (_ability == ability))
        {
            float rand = Random.Range(0f, 1f);
            if (rand < lifeOnKill)
            {
                if (!baseHealth) { baseHealth = GetComponent<BaseHealth>(); }
                if (baseHealth)
                {
                    baseHealth.Heal(lifeOnKill);
                }
            }
        }
    }

    public void onCrit(Ability _ability, GameObject target)
    {
        if (lifeOnCrit != 0 && (_ability == ability))
        {
            float rand = Random.Range(0f, 1f);
            if (rand < lifeOnCrit)
            {
                if (!baseHealth) { baseHealth = GetComponent<BaseHealth>(); }
                if (baseHealth)
                {
                    baseHealth.Heal(lifeOnCrit);
                }
            }
        }
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        if (chanceToBleed > 0)
        {
            ChanceToApplyStatusOnEnemyHit chanceTo = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            chanceTo.chance = chanceToBleed;
            chanceTo.statusEffect = StatusEffectList.getEffect(StatusEffectID.Bleed);
        }

        if (chanceToPlague > 0)
        {
            ChanceToApplyStatusOnEnemyHit chanceTo = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            chanceTo.chance = chanceToPlague;
            chanceTo.statusEffect = StatusEffectList.getEffect(StatusEffectID.Plague);
        }

        if (chanceToBlindingPoison > 0)
        {
            ChanceToApplyStatusOnEnemyHit chanceTo = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            chanceTo.chance = chanceToBlindingPoison;
            chanceTo.statusEffect = StatusEffectList.getEffect(StatusEffectID.BlindingPoison);
        }

        if (chanceToShredArmour > 0)
        {
            ChanceToApplyStatusOnEnemyHit chanceTo = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            chanceTo.chance = chanceToShredArmour;
            chanceTo.statusEffect = StatusEffectList.getEffect(StatusEffectID.ArmourShred);
        }

        if (poisonSpitChance > 0)
        {
            float rand = Random.Range(0f, 1f);
            if (rand < poisonSpitChance)
            {
                if (!usingAbility) { usingAbility = GetComponent<UsingAbility>(); }
                if (usingAbility)
                {
                    usingAbility.UseAbility(AbilityIDList.getAbility(AbilityID.poisonSpit), targetLocation + new Vector3(0,-1.1f, 0), false, false);
                }
            }
        }
        
        if (increasedHitDamage != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(increasedHitDamage);
                //holder.addBaseDamage(DamageType.PHYSICAL, holder.getBaseDamage(DamageType.PHYSICAL) * increasedDamage);
            }
        }

        if (addedCritChance != 0)
        {
            foreach (DamageStatsHolder damage in abilityObject.GetComponents<DamageStatsHolder>())
            {
                damage.baseDamageStats.critChance += addedCritChance;
            }
        }

        if (cullPercent > 0)
        {
            foreach (DamageStatsHolder damage in abilityObject.GetComponents<DamageStatsHolder>())
            {
                damage.baseDamageStats.cullPercent += cullPercent;
            }
        }

        if (moreDamageAgainstBlinded != 0)
        {
            // create the conditional
            DamageConditionalEffect conditionalEffect = new DamageConditionalEffect();
            HasStatusEffectConditional conditional = new HasStatusEffectConditional();
            conditional.statusEffect = StatusEffectID.Blind;
            conditionalEffect.conditional = conditional;
            conditionalEffect.effect = new DamageEffectMoreDamage(moreDamageAgainstBlinded);
            // add the conditional to all damage stats holders
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.baseDamageStats.conditionalEffects.Add(conditionalEffect);
            }
        }

        if (moreDamageAgainstPoisonBlinded != 0)
        {
            // create the conditional
            DamageConditionalEffect conditionalEffect = new DamageConditionalEffect();
            HasStatusEffectConditional conditional = new HasStatusEffectConditional();
            conditional.statusEffect = StatusEffectID.BlindingPoison;
            conditionalEffect.conditional = conditional;
            conditionalEffect.effect = new DamageEffectMoreDamage(moreDamageAgainstPoisonBlinded);
            // add the conditional to all damage stats holders
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.baseDamageStats.conditionalEffects.Add(conditionalEffect);
            }
        }

        if (dodgeRatingOnAttack != 0 || poisonDamageOnAttack != 0 || dotDamageOnAttack != 0)
        {
            StatBuffs statBuffs = GetComponent<StatBuffs>();
            if (statBuffs == null) { statBuffs = gameObject.AddComponent<StatBuffs>(); }
            if (dodgeRatingOnAttack != 0)
            {
                statBuffs.addBuff(4f, Tags.Properties.DodgeRating, dodgeRatingOnAttack, 0, null, null);
            }
            if (poisonDamageOnAttack != 0)
            {
                List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                tagList.Add(Tags.AbilityTags.Poison);
                statBuffs.addBuff(4f, Tags.Properties.Damage, 0, poisonDamageOnAttack, null, null, tagList);
            }
            if (dotDamageOnAttack != 0)
            {
                List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                tagList.Add(Tags.AbilityTags.DoT);
                statBuffs.addBuff(4f, Tags.Properties.Damage, 0, dotDamageOnAttack, null, null, tagList);
            }
        }


        return abilityObject;
    }
}

