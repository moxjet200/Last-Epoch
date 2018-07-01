using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VengeanceMutator : AbilityMutator
{
    // riposte stuff
    public float increasedDuration = 0f;
    public int additionalRetaliations = 0;
    public float darkBladeRetaliationChance = 0f;
    public List<TaggedStatsHolder.TaggableStat> statsWhilePrepped = new List<TaggedStatsHolder.TaggableStat>();

    public float increasedDamageWhileBelowHalfHealth = 0f;

    public float increasedAttackSpeed = 0f;
    public float increasedDamage = 0f;

    public float increasedStunChance = 0f;
    public float moreDamageAgaintDamaged = 0f;

    public float reducedDamageTakenOnHit = 0f;
    public float darkBladeOnVengeanceHitChance = 0f;
    public float darkBladeOnRiposteHitChance = 0f;
    public float darkBladeOnVengeanceKillChance = 0f;
    public float darkBladeOnRiposteKillChance = 0f;
    public float voidEssenceOnVengeanceKillChance = 0f;
    public float voidEssenceOnRiposteHitChance = 0f;

    BaseMana mana = null;
    BaseHealth health = null;
    StatBuffs statBuffs = null;
    UsingAbility ua = null;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.vengeance);
        base.Awake();
        mana = GetComponent<BaseMana>();
        health = GetComponent<BaseHealth>();
        AbilityEventListener ael = AbilityEventListener.GetOrAdd(gameObject);
        statBuffs = Comp<StatBuffs>.GetOrAdd(gameObject);
        ua = GetComponent<UsingAbility>();

        ael.onHitEvent += OnHit;
        ael.onHitEvent += OnKill;
    }


    public void OnHit(Ability _ability, GameObject target)
    {
        // vengeance
        if (_ability && _ability == ability)
        {
            if (reducedDamageTakenOnHit != 0)
            {
                TaggedStatsHolder.Stat newStat = new TaggedStatsHolder.Stat(Tags.Properties.DamageTaken);
                newStat.increasedValue = -reducedDamageTakenOnHit;
                Buff buff = new Buff(newStat, 2f);
                statBuffs.addBuff(buff);
            }
            if (darkBladeOnVengeanceHitChance != 0)
            {
                if (!ua) { ua = GetComponent<UsingAbility>(); }
                if (Random.Range(0f, 1f) < darkBladeOnVengeanceHitChance && ua)
                {
                    if (target)
                    {
                        ua.UseAbility(Ability.getAbility(AbilityID.darkBlade), target.transform.position, false, false);
                    }
                    else
                    {
                        ua.UseAbility(Ability.getAbility(AbilityID.darkBlade), target.transform.position + new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)), false, false);
                    }
                }
            }
        }
        // riposte
        if (_ability && _ability == Ability.getAbility(AbilityID.riposteRetaliationHit))
        {
            if (!ua) { ua = GetComponent<UsingAbility>(); }
            if (voidEssenceOnRiposteHitChance != 0 && ua)
            {
                if (Random.Range(0f, 1f) < voidEssenceOnRiposteHitChance)
                {
                    if (target)
                    {
                        ua.UseAbility(Ability.getAbility(AbilityID.voidEssence), target.transform.position, false, false);
                    }
                    else
                    {
                        ua.UseAbility(Ability.getAbility(AbilityID.voidEssence), transform.position, false, false);
                    }
                }
            }
            if (darkBladeOnRiposteHitChance != 0)
            {
                if (Random.Range(0f, 1f) < darkBladeOnRiposteHitChance && ua)
                {
                    if (target)
                    {
                        ua.UseAbility(Ability.getAbility(AbilityID.darkBlade), target.transform.position, false, false);
                    }
                    else
                    {
                        ua.UseAbility(Ability.getAbility(AbilityID.darkBlade), target.transform.position + new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)), false, false);
                    }
                }
            }
        }
    }

    public void OnKill(Ability _ability, GameObject target)
    {
        // vengeance
        if (_ability && _ability == ability)
        {
            if (!ua) { ua = GetComponent<UsingAbility>(); }
            if (darkBladeOnVengeanceKillChance != 0 && ua)
            {
                if (Random.Range(0f, 1f) < darkBladeOnVengeanceKillChance)
                {
                    ua.UseAbility(Ability.getAbility(AbilityID.darkBlade), target.transform.position + new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)), false, false);
                }
            }
            if (voidEssenceOnVengeanceKillChance != 0 && ua)
            {
                if (Random.Range(0f, 1f) < voidEssenceOnVengeanceKillChance)
                {
                    ua.UseAbility(Ability.getAbility(AbilityID.voidEssence), transform.position, false, false);
                }
            }
        }
        // riposte
        if (_ability && _ability == Ability.getAbility(AbilityID.riposteRetaliationHit))
        {
            if (!ua) { ua = GetComponent<UsingAbility>(); }
            if (darkBladeOnRiposteKillChance != 0 && ua)
            {
                if (Random.Range(0f, 1f) < darkBladeOnRiposteKillChance)
                {
                    ua.UseAbility(Ability.getAbility(AbilityID.darkBlade), target.transform.position + new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)), false, false);
                }
            }
        }
    }

    public override float mutateUseSpeed(float useSpeed)
    {
        return base.mutateUseSpeed(useSpeed) * (1 + increasedAttackSpeed);
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        RiposteMutator mut = abilityObject.AddComponent<RiposteMutator>();
        mut.increasedDuration = increasedDuration;
        mut.additionalRetaliations = additionalRetaliations;
        mut.darkBladeRetaliationChance = darkBladeRetaliationChance;
        mut.statsWhilePrepped = statsWhilePrepped;

        if (increasedDamageWhileBelowHalfHealth != 0)
        {
            if (health && health.currentHealth < health.maxHealth / 2)
            {
                foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
                {
                    holder.increaseAllDamage(increasedDamageWhileBelowHalfHealth);
                }
            }
        }
        

        if (increasedStunChance != 0)
        {
            foreach (DamageStatsHolder damage in abilityObject.GetComponents<DamageStatsHolder>())
            {
                damage.baseDamageStats.increasedStunChance += increasedStunChance;
            }
        }

        if (increasedDamage != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(increasedDamage);
            }
        }


        if (moreDamageAgaintDamaged != 0)
        {
            // create the conditional
            DamageConditionalEffect conditionalEffect = new DamageConditionalEffect();
            DamagedConditional conditional = new DamagedConditional();
            conditionalEffect.conditional = conditional;
            conditionalEffect.effect = new DamageEffectMoreDamage(moreDamageAgaintDamaged);
            // add the conditional to all damage stats holders
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.baseDamageStats.conditionalEffects.Add(conditionalEffect);
            }
        }

        return abilityObject;
    }
}
