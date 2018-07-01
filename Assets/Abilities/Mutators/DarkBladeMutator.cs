using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkBladeMutator : AbilityMutator
{
    public float armourShredChance = 0f;

    public bool noPierce = false;
    public float chanceForDoubleDamage = 0f;
    public float increasedDamage = 0f;
    public float moreDamageAgainstStunned = 0f;
    public float increasedProjectileSpeed = 0f;
    public int chains = 0;
    public float increasedStunChance = 0f;
    public bool hasChained = false;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.darkBlade);
        base.Awake();
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {

        if (chains > 0)
        {
            ChainOnHit chain = abilityObject.AddComponent<ChainOnHit>();
            chain.chainsRemaining = chains;
            chain.abilityToChain = ability;
            chain.range = 8f;
            chain.cannotHitSame = true;
            chain.destroyAfterSuccessfulChainAttempt = true;
            chain.offset = new Vector3(0f, 1.2f, 0f);


        }

        if (chains > 0 || hasChained)
        {
            // add a copy of this mutator to the ability object, but remove the chains (because it will chain anyway), the increased damage to first enemy hit, and the on cast stuff
            DarkBladeMutator newMutator = abilityObject.AddComponent<DarkBladeMutator>();
            newMutator.chains = 0;
            newMutator.increasedDamage = increasedDamage;
            newMutator.armourShredChance = 0f;

            newMutator.noPierce = noPierce;
            newMutator.chanceForDoubleDamage = chanceForDoubleDamage;
            newMutator.increasedDamage = increasedDamage;
            newMutator.moreDamageAgainstStunned = moreDamageAgainstStunned;
            
            newMutator.increasedProjectileSpeed = increasedProjectileSpeed;
            newMutator.increasedStunChance = increasedStunChance;
            newMutator.hasChained = true;
        }

        // remove pierce
        if (noPierce && chains <= 0 && !hasChained)
        {
            Pierce pierce = abilityObject.GetComponent<Pierce>();
            if (pierce == null) { pierce = abilityObject.AddComponent<Pierce>(); pierce.objectsToPierce = 0; }
            if (chains <= 0 && !hasChained)
            {
                abilityObject.AddComponent<DestroyOnFailingToPierceEnemy>();
            }
        }

        // add chance to shred armour
        if (armourShredChance > 0)
        {
            ChanceToApplyStatusOnEnemyHit chanceTo = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            chanceTo.chance = armourShredChance;
            chanceTo.statusEffect = StatusEffectList.getEffect(StatusEffectID.ArmourShred);
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
        

        if (chanceForDoubleDamage > 0)
        {
            float rand = Random.Range(0f, 1f);
            if (rand < chanceForDoubleDamage)
            {
                foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
                {
                    for (int i = 0; i < holder.baseDamageStats.damage.Count; i++)
                    {
                        holder.addBaseDamage(holder.baseDamageStats.damage[i].damageType, holder.getBaseDamage(holder.baseDamageStats.damage[i].damageType));
                    }
                }
            }
        }

        if (moreDamageAgainstStunned != 0)
        {
            // create the conditional
            DamageConditionalEffect conditionalEffect = new DamageConditionalEffect();
            StunnedConditional conditional = new StunnedConditional();
            conditionalEffect.conditional = conditional;
            conditionalEffect.effect = new DamageEffectMoreDamage(moreDamageAgainstStunned);
            // add the conditional to all damage stats holders
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.baseDamageStats.conditionalEffects.Add(conditionalEffect);
            }
        }

        if (increasedProjectileSpeed != 0)
        {
            AbilityMover mover = abilityObject.GetComponent<AbilityMover>();
            mover.speed *= (1 + increasedProjectileSpeed);
        }


        return abilityObject;
    }
}
