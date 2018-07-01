using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBlastMutator : AbilityMutator{

    float wardOnCastChance = 0.3f;

    public float increasedDamage = 0f;
    public float increasedDamageToFirstEnemyHit = 0f;
    public float increasedDamagePerChain = 0f;
    public float increasedDamageToLastEnemy = 0f;
    public int chains = 0;
    public float lightningProtectionOnCast = 0f;
    public float elementalProtectionOnCast = 0f;
    public float increasedProtectionOnCast = 0f;
    public float increasedProtectionDuration = 0f;
    public float wardOnCast = 0f;
    public float increasedCastSpeed = 0f;
    public float lightningStrikeChance = 0f;
    public float lightningDamageOnCast = 0f;
    public float increasedDamageBuffDuration = 0f;
    public float chanceToBlind = 0f;
    public float increasedStunChance = 0f;

    StatBuffs statBuffs = null;
    ProtectionClass protectionClass = null;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.lightningBlast);
        statBuffs = GetComponent<StatBuffs>();
        if (statBuffs == null) { statBuffs = gameObject.AddComponent<StatBuffs>(); }
        protectionClass = GetComponent<ProtectionClass>();
        base.Awake();
    }

    public override float mutateUseSpeed(float useSpeed)
    {
        return base.mutateUseSpeed(useSpeed) * (1 + increasedCastSpeed);
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        // increasedDamageToFirstEnemyHit is removed for additional chains, so it can be applied without checks
        if (increasedDamage != 0 || increasedDamageToFirstEnemyHit != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(increasedDamage + increasedDamageToFirstEnemyHit);
            }
        }

        if (chains > 0){
            ChainOnHit chain = abilityObject.AddComponent<ChainOnHit>();
            chain.chainsRemaining = chains;
            chain.abilityToChain = ability;

            // add a copy of this mutator to the ability object, but remove the chains (because it will chain anyway), the increased damage to first enemy hit, and the on cast stuff
            LightningBlastMutator newMutator = abilityObject.AddComponent<LightningBlastMutator>();
            newMutator.chains = 0;
            increasedDamageToFirstEnemyHit = 0;
            newMutator.increasedDamage = increasedDamage;
            newMutator.lightningStrikeChance = lightningStrikeChance;
            newMutator.chanceToBlind = chanceToBlind;

            // add the increased damage per chain
            newMutator.increasedDamage += increasedDamagePerChain;

            // add increased damage to last enemy hit if appropriate
            if (chain.chainsRemaining == 0)
            {
                newMutator.increasedDamage += increasedDamageToLastEnemy;
            }
        }


        // on cast stuff
        if (lightningProtectionOnCast != 0 || elementalProtectionOnCast != 0)
        {
            Buff buff = new Buff();
            TaggedStatsHolder.Stat stat = new TaggedStatsHolder.Stat(Tags.Properties.LightningProtection);
            stat.addedValue = (lightningProtectionOnCast + elementalProtectionOnCast) * (1 + increasedProtectionOnCast);
            buff.stat = stat;
            buff.remainingDuration = 3 * (1 + increasedProtectionDuration);
            statBuffs.addBuff(buff);
        }

        if (elementalProtectionOnCast != 0)
        {
            Buff buff = new Buff();
            TaggedStatsHolder.Stat stat = new TaggedStatsHolder.Stat(Tags.Properties.ColdProtection);
            stat.addedValue = (elementalProtectionOnCast) * (1 + increasedProtectionOnCast);
            buff.stat = stat;
            buff.remainingDuration = 3 * (1 + increasedProtectionDuration);
            statBuffs.addBuff(buff);
        }

        if (elementalProtectionOnCast != 0)
        {
            Buff buff = new Buff();
            TaggedStatsHolder.Stat stat = new TaggedStatsHolder.Stat(Tags.Properties.FireProtection);
            stat.addedValue = (elementalProtectionOnCast) * (1 + increasedProtectionOnCast);
            buff.stat = stat;
            buff.remainingDuration = 3 * (1 + increasedProtectionDuration);
            statBuffs.addBuff(buff);
        }

        if (wardOnCast != 0)
        {
            float rand = Random.Range(0f, 1f);
            if (rand < wardOnCastChance)
            {
                protectionClass.currentWard += wardOnCast;
            }
        }

        if (lightningDamageOnCast != 0)
        {
            List<Tags.AbilityTags> tags = new List<Tags.AbilityTags>();
            tags.Add(Tags.AbilityTags.Lightning);
            TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, tags);
            stat.increasedValue = lightningDamageOnCast;
            TaggedBuff buff = new TaggedBuff();
            buff.stat = stat;
            buff.remainingDuration = 3 * (1 + increasedDamageBuffDuration);
            statBuffs.addTaggedBuff(buff);
        }

        if (increasedStunChance != 0)
        {
            foreach (DamageStatsHolder damage in abilityObject.GetComponents<DamageStatsHolder>())
            {
                damage.baseDamageStats.increasedStunChance += increasedStunChance;
            }
        }


        // maybe cast lightning too
        if (lightningStrikeChance > 0)
        {
            float rand = Random.Range(0f, 1f);
            if (rand <= lightningStrikeChance)
            {
                AbilityObjectConstructor aoc = GetComponent<AbilityObjectConstructor>();
                if (aoc)
                {
                    aoc.constructAbilityObject(AbilityIDList.getAbility(AbilityID.lightning), targetLocation, targetLocation);
                }
            }
        }

        // maybe blind
        if (chanceToBlind > 0)
        {
            ChanceToApplyStatusOnEnemyHit ctasoeh = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            ctasoeh.chance = chanceToBlind;
            ctasoeh.statusEffect = StatusEffectList.getEffect(StatusEffectID.Blind);
        }


        return abilityObject;
    }
}
