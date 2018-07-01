using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ManaStrikeMutator : AbilityMutator{

    public float increasedAttackSpeed = 0f;
    public float additionalManaOnHit = 0f;
    public float manaOnKill = 0f;
    public float manaOnCrit = 0f;
    public float increasedDamageOnHit = 0f;
    public float increasedSpellDamageOnHit = 0f;
    public float increasedManaOnHit = 0f;
    public float increasedDamage = 0f;
    public float addedCritChance = 0f;
    public bool removesCritMultiplier = false;
    public bool canTeleport = false;
    public float chanceToKnockBack = 0f;
    public float tenacityDebuffOnHit = 0f;
    public float chanceToAttachSparkCharge = 0f;

    public float addedLightningPerMana = 0f;
    public float critChancePerMana = 0f;
    public bool lightningStrikeOnHit = false;
    public float reducedManaLostOnLightningStrike = 0f;

    public float increasedDamageWhileOutOfMana = 0f;
    public float increasedRadius = 0f;
    public float manaGainOnHitWhileOutOfMana = 0f;

    Vector3 lightningDisplacement = Vector3.zero;

    BaseMana mana = null;
    StatBuffs statBuffs = null;
    AbilityObjectConstructor aoc = null;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.manaStrike);
        base.Awake();
        mana = GetComponent<BaseMana>();
        AbilityEventListener ael = GetComponent<AbilityEventListener>();
        if (ael)
        {
            ael.onHitEvent += onHit;
            ael.onKillEvent += onKill;
            ael.onCritEvent += onCrit;
        }
        aoc = GetComponent<AbilityObjectConstructor>();
        lightningDisplacement = new Vector3(0f, 1f, 0f);
        statBuffs = GetComponent<StatBuffs>();
        if (statBuffs == null) { statBuffs = gameObject.AddComponent<StatBuffs>(); }
    }

    public override float mutateStopRange()
    {
        if (canTeleport) { return 100f; }
        else
        {
            return base.mutateStopRange();
        }
    }

    public void onCrit(Ability _ability, GameObject target)
    {
        if (_ability == ability)
        {
            if (manaOnCrit != 0)
            {
                if (mana)
                {
                    mana.gainMana(manaOnCrit);
                }
            }
        }
    }

    public void onKill(Ability _ability, GameObject target)
    {
        if (_ability == ability)
        {
            if (manaOnKill != 0)
            {
                if (mana)
                {
                    mana.gainMana(manaOnKill);
                }
            }
        }
    }

    public void onHit(Ability _ability, GameObject target)
    {
        if (_ability == ability)
        {
            if (increasedDamageOnHit != 0)
            {
                statBuffs.addBuff(4f, Tags.Properties.Damage, 0, increasedDamageOnHit, null, null);
            }
            if (increasedSpellDamageOnHit != 0)
            {
                List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                tagList.Add(Tags.AbilityTags.Spell);
                statBuffs.addBuff(4f, Tags.Properties.Damage, 0, increasedSpellDamageOnHit, null, null, tagList);
            }
            if (mana && mana.currentMana > 0 && lightningStrikeOnHit && target)
            {
                if (reducedManaLostOnLightningStrike < 1)
                {
                    mana.reduceMana(20 * (1 - reducedManaLostOnLightningStrike));
                }
                aoc.constructAbilityObject(Ability.getAbility(AbilityID.lightning), target.transform.position + lightningDisplacement, target.transform.position + lightningDisplacement);
            }
        }
    }

    public override float mutateUseSpeed(float useSpeed)
    {
        return useSpeed * (1 + increasedAttackSpeed);
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        if (addedLightningPerMana != 0 && mana)
        {
            float addedDamage = mana.currentMana * addedLightningPerMana;
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.addBaseDamage(DamageType.LIGHTNING, addedDamage);
            }
        }

        if (critChancePerMana != 0 && mana)
        {
            float addedCrit = mana.currentMana * critChancePerMana;
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.baseDamageStats.critChance += addedCrit;
            }
        }

        if (additionalManaOnHit != 0 || increasedManaOnHit != 0 || manaGainOnHitWhileOutOfMana != 0)
        {
            GiveCreatorManaOnHit component = abilityObject.GetComponent<GiveCreatorManaOnHit>();
            if (component)
            {
                component.manaOnHit += additionalManaOnHit;
                if (mana && mana.currentMana <= 0)
                {
                    component.manaOnHit += manaGainOnHitWhileOutOfMana;
                }
                component.manaOnHit *= 1 + increasedManaOnHit;
            }
        }

        if (increasedDamage != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(increasedDamage);
            }
        }

        if (increasedDamageWhileOutOfMana != 0 && mana && mana.currentMana <= 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(increasedDamageWhileOutOfMana);
            }
        }

        if (addedCritChance != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.baseDamageStats.critChance += addedCritChance;
            }
        }

        if (chanceToAttachSparkCharge > 0)
        {
            ChanceToCreateAbilityObjectOnNewEnemyHit newComponent = abilityObject.AddComponent<ChanceToCreateAbilityObjectOnNewEnemyHit>();
            newComponent.spawnAtHit = true;
            newComponent.chance = chanceToAttachSparkCharge;
            newComponent.abilityToInstantiate = AbilityIDList.getAbility(AbilityID.sparkCharge);
        }

        if (removesCritMultiplier)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.baseDamageStats.noCritMultiplier = true;
            }
        }

        if (canTeleport)
        {
            abilityObject.GetComponent<StartsTowardsTarget>().active = false;
            abilityObject.AddComponent<StartsAtTarget>().facesAwayFromStart = true;
        }

        if (chanceToKnockBack > 0)
        {
            float rand = Random.Range(0f, 1f);
            if (rand < chanceToKnockBack)
            {
                abilityObject.AddComponent<CreateAbilityObjectOnDeath>().abilityToInstantiate = AbilityIDList.getAbility(AbilityID.knockBack);
            }
        }

        if (tenacityDebuffOnHit != 0)
        {
            DebuffOnEnemyHit component = abilityObject.AddComponent<DebuffOnEnemyHit>();
            component.addDebuffToList(Tags.Properties.Tenacity, tenacityDebuffOnHit, 0, null, null, 4f);
        }

        if (increasedRadius != 0)
        {
            foreach (CreateOnDeath cod in abilityObject.GetComponents<CreateOnDeath>())
            {
                cod.increasedRadius = increasedRadius;
                cod.increasedHeight = increasedRadius;
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
