using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovaMutator : AbilityMutator {

    public bool canIceNova = false;
    public bool canLightningNova = false;
    public bool canFireNova = false;

    public float chanceForChainingIceNova = 0f;

    public float addedCritMultiplier = 0f;
    public float addedCritChance = 0f;

    public bool canTarget = false;

    public float chanceToGainWardOnKill = 0f;
    public float wardOnKill = 0f;
    public float increasedDamage = 0f;
    public float wardOnHit = 0f;
    public float chanceToGainWardOnHit = 0f;
    public float increasedWardGained = 0f;
    public float chillChance = 0f;
    public float increasedCastSpeed = 0f;
    public float moreDamageAgainstFullHealth = 0f;
    public float chanceToAttachSparkCharge = 0f;
    public float wardRetentionOnKill = 0f;
    public float increasedRadius = 0f;
    public float shockChance = 0f;
    public float shockEffectOnHit = 0f;
    public float igniteChance = 0f;

    ProtectionClass protectionClass = null;
    StatBuffs statBuffs = null;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.nova);
        base.Awake();
        if (GetComponent<AbilityEventListener>())
        {
            GetComponent<AbilityEventListener>().onKillEvent += GainWardOnKill;
            GetComponent<AbilityEventListener>().onHitEvent += OnHit;
        }
        protectionClass = GetComponent<ProtectionClass>();
        statBuffs = GetComponent<StatBuffs>();
        if (!statBuffs) { statBuffs = gameObject.AddComponent<StatBuffs>(); }
    }

    public override float mutateUseSpeed(float useSpeed)
    {
        return base.mutateUseSpeed(useSpeed) * (1 + increasedCastSpeed);
    }

    public void GainWardOnKill(Ability _ability, GameObject target)
    {
        if (_ability == ability || _ability == AbilityIDList.getAbility(AbilityID.iceNova) || _ability == AbilityIDList.getAbility(AbilityID.lightningNova) || _ability == AbilityIDList.getAbility(AbilityID.fireNova))
        {
            if (chanceToGainWardOnKill > 0)
            {
                float rand = Random.Range(0f, 1f);
                if (rand < chanceToGainWardOnKill && protectionClass)
                {
                    protectionClass.GainWard(wardOnKill * (1 + increasedWardGained));
                }
            }
            if (wardRetentionOnKill > 0)
            {
                TaggedStatsHolder.Stat newStat = new TaggedStatsHolder.Stat(Tags.Properties.WardRetention);
                newStat.addedValue = wardRetentionOnKill;
                Buff newBuff = new Buff();
                newBuff.stat = newStat;
                newBuff.remainingDuration = 4f;
                newBuff.name = "Nova ward retention";
                statBuffs.addBuff(newBuff);
            }
        }
    }

    public void OnHit(Ability _ability, GameObject target)
    {
        if (_ability == ability || _ability == AbilityIDList.getAbility(AbilityID.iceNova) || _ability == AbilityIDList.getAbility(AbilityID.lightningNova) || _ability == AbilityIDList.getAbility(AbilityID.fireNova))
        {
            if (chanceToGainWardOnHit > 0)
            {
                float rand = Random.Range(0f, 1f);
                if (rand < chanceToGainWardOnHit && protectionClass)
                {
                    protectionClass.currentWard += wardOnHit * (1 + increasedWardGained);
                }
            }
            if (shockEffectOnHit > 0)
            {
                TaggedStatsHolder.Stat newStat = new TaggedStatsHolder.Stat(Tags.Properties.IncreasedShockEffect);
                newStat.addedValue = shockEffectOnHit;
                Buff newBuff = new Buff();
                newBuff.stat = newStat;
                newBuff.remainingDuration = 4f;
                newBuff.name = "Nova shock effect";
                statBuffs.addBuff(newBuff);
            }
        }
    }

    public override List<Tags.AbilityTags> getUseTags()
    {
        List<Tags.AbilityTags> list = base.getUseTags();
        if (canIceNova)
        {
            if (list.Contains(Tags.AbilityTags.Physical)) { list.Remove(Tags.AbilityTags.Physical); }
            list.Add(Tags.AbilityTags.Cold);
        }
        if (canFireNova)
        {
            if (list.Contains(Tags.AbilityTags.Physical)) { list.Remove(Tags.AbilityTags.Physical); }
            list.Add(Tags.AbilityTags.Fire);
        }
        if (canLightningNova)
        {
            if (list.Contains(Tags.AbilityTags.Physical)) { list.Remove(Tags.AbilityTags.Physical); }
            list.Add(Tags.AbilityTags.Lightning);
        }
        return list;
    }

    GameObject instantiateIceNova(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        Destroy(abilityObject);
        return Instantiate(AbilityIDList.getAbility(AbilityID.iceNova).abilityPrefab, location, Quaternion.Euler(targetLocation - location));
    }

    GameObject instantiateFireNova(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        Destroy(abilityObject);
        return Instantiate(AbilityIDList.getAbility(AbilityID.fireNova).abilityPrefab, location, Quaternion.Euler(targetLocation - location));
    }

    GameObject instantiateLightningNova(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        Destroy(abilityObject);
        return Instantiate(AbilityIDList.getAbility(AbilityID.lightningNova).abilityPrefab, location, Quaternion.Euler(targetLocation - location));
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        // choose the nova type
        if (canIceNova || canFireNova || canLightningNova)
        {
            int randMax = 0;
            if (canIceNova) { randMax++; }
            if (canFireNova) { randMax++; }
            if (canLightningNova) { randMax++; }
            float rand = Random.Range(0f, randMax);
            if (rand < 1f)
            {
                if (canIceNova) { abilityObject = instantiateIceNova(abilityObject, location, targetLocation); }
                else if (canFireNova) { abilityObject = instantiateFireNova(abilityObject, location, targetLocation); }
                else { abilityObject = instantiateLightningNova(abilityObject, location, targetLocation); }
            }
            else if (rand < 2f)
            {
                if (canFireNova && canIceNova) { abilityObject = instantiateFireNova(abilityObject, location, targetLocation); }
                else { abilityObject = instantiateLightningNova(abilityObject, location, targetLocation); }
            }
            else
            {
                abilityObject = instantiateLightningNova(abilityObject, location, targetLocation);
            }
        }

        if (chanceForChainingIceNova > 0)
        {
            float rand2 = Random.Range(0f, 1f);
            if (rand2 < chanceForChainingIceNova)
            {
                CreateAbilityObjectOnNewEnemtHit newComponent = abilityObject.AddComponent<CreateAbilityObjectOnNewEnemtHit>();
                newComponent.abilityToInstantiate = AbilityIDList.getAbility(AbilityID.delayerForChainingIceNova);
            }
        }

        if (chillChance > 0)
        {
            ChanceToApplyStatusOnEnemyHit newComponent = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            newComponent.statusEffect = StatusEffectList.getEffect(StatusEffectID.Chill);
            newComponent.chance = chillChance;
        }

        if (shockChance > 0)
        {
            ChanceToApplyStatusOnEnemyHit newComponent = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            newComponent.statusEffect = StatusEffectList.getEffect(StatusEffectID.Shock);
            newComponent.chance = shockChance;
        }

        if (igniteChance > 0)
        {
            ChanceToApplyStatusOnEnemyHit newComponent = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            newComponent.statusEffect = StatusEffectList.getEffect(StatusEffectID.Ignite);
            newComponent.chance = igniteChance;
        }

        if (addedCritChance != 0)
        {
            foreach (DamageStatsHolder damage in abilityObject.GetComponents<DamageStatsHolder>())
            {
                damage.baseDamageStats.critChance += addedCritChance;
            }
        }

        if (chanceToAttachSparkCharge > 0)
        {
            ChanceToCreateAbilityObjectOnNewEnemyHit newComponent = abilityObject.AddComponent<ChanceToCreateAbilityObjectOnNewEnemyHit>();
            newComponent.spawnAtHit = true;
            newComponent.chance = chanceToAttachSparkCharge;
            newComponent.abilityToInstantiate = AbilityIDList.getAbility(AbilityID.sparkCharge);
        }

        if (addedCritMultiplier != 0)
        {
            foreach (DamageStatsHolder damage in abilityObject.GetComponents<DamageStatsHolder>())
            {
                damage.baseDamageStats.critMultiplier += addedCritMultiplier;
            }
        }

        if (canTarget)
        {
            abilityObject.AddComponent<StartsAtTarget>();
        }

        if (increasedDamage != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(increasedDamage);
            }
        }

        if (moreDamageAgainstFullHealth != 0)
        {
            // create the conditional
            DamageConditionalEffect conditionalEffect = new DamageConditionalEffect();
            conditionalEffect.conditional = new FullHealthConditional();
            conditionalEffect.effect = new DamageEffectMoreDamage(moreDamageAgainstFullHealth);
            // add the conditional to all damage stats holders
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.baseDamageStats.conditionalEffects.Add(conditionalEffect);
            }
        }

        if (increasedRadius != 0)
        {
            foreach (CreateOnDeath cod in abilityObject.GetComponents<CreateOnDeath>())
            {
                cod.increasedRadius = increasedRadius;
            }
            foreach (CapsuleCollider col in abilityObject.GetComponents<CapsuleCollider>())
            {
                col.height *= (1 + increasedRadius);
                col.radius *= (1 + increasedRadius);
            }
        }

        return abilityObject;
    }
}
