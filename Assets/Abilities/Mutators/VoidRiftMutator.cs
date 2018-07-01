using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidRiftMutator : AbilityMutator
{
    public float increasedDamage = 0f;
    public float increasedRadius = 0f;
    public float damageGainOnNearbyDeath = 0f;
    public float areaGainOnNearbyDeath = 0f;
    public bool growGameObjectWithArea = false;
    public bool castOnNearbyDeath = false;
    public float timeRotChance = 0f;
    public float increasesDamageTaken = 0f;
    public float increasesDoTDamageTaken = 0f;
    public float increasedStunChance = 0f;
    public float moreDamageAgainstStunned = 0f;
    public float igniteChance = 0f;
    public float moreDamageAgainstIgnited = 0f;
    public float moreDamageAgainstTimeRotting = 0f;

    // used for calculating increased radius
    float increasedArea = 0f;

    List<Dying> subbed = new List<Dying>();
    AbilityObjectConstructor aoc = null;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.voidRift);
        base.Awake();
    }
    
    IEnumerator castAfterDelay()
    {
        for (int i = 0; i < 2; i++)
        {
            if (i == 1 && aoc)
            {
                if (castOnNearbyDeath)
                {
                    aoc.constructAbilityObject(ability, transform.position, transform.position);
                    transform.localScale = new Vector3(1 + increasedArea, 1 + increasedArea, 1 + increasedArea);
                }
                increasedArea += areaGainOnNearbyDeath;
                increasedRadius = Mathf.Sqrt(increasedArea + 1) - 1;
                if (growGameObjectWithArea)
                {
                    transform.localScale = new Vector3(1 + increasedRadius, 1 + increasedRadius, 1 + increasedRadius);
                }
            }
            yield return new WaitForSeconds(0.1f + Random.Range(0f, 0.2f));
        }
    }

    public void Start()
    {
        if (damageGainOnNearbyDeath != 0 || areaGainOnNearbyDeath != 0)
        {
            GlobalActorEventManager.instance.newDyingCreatedEvent += newDying;
            foreach (Dying dying in Dying.all)
            {
                dying.deathEvent += onNearbyDeath;
                subbed.Add(dying);
            }
        }
        increasedArea = Mathf.Pow(increasedRadius + 1, 2) - 1;
        aoc = GetComponent<AbilityObjectConstructor>();
    }


    public void onNearbyDeath(Dying dyingComponent)
    {
        // do not react to unsummon
        if (dyingComponent.unsummoned){ return; }

        increasedDamage += damageGainOnNearbyDeath;
        if (areaGainOnNearbyDeath != 0)
        {
            StartCoroutine(castAfterDelay());
        }
    }

    public void newDying(Dying dying)
    {
        dying.deathEvent += onNearbyDeath;
        subbed.Add(dying);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GlobalActorEventManager.instance.newDyingCreatedEvent -= newDying;
        subbed.RemoveAll(x => x == null);
        foreach (Dying sub in subbed)
        {
            sub.deathEvent -= onNearbyDeath;
        }
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        if (increasedDamage != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(increasedDamage);
            }
        }

        if (increasedRadius > 0)
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

        if (timeRotChance > 0)
        {
            ChanceToApplyStatusOnEnemyHit newComponent = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            newComponent.statusEffect = StatusEffectList.getEffect(StatusEffectID.TimeRot);
            newComponent.chance = timeRotChance;
        }

        if (igniteChance > 0)
        {
            ChanceToApplyStatusOnEnemyHit newComponent = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            newComponent.statusEffect = StatusEffectList.getEffect(StatusEffectID.Ignite);
            newComponent.chance = igniteChance;
        }

        if (increasesDamageTaken > 0 || increasesDoTDamageTaken > 0)
        {
            DebuffOnEnemyHit doeh = abilityObject.AddComponent<DebuffOnEnemyHit>();
            if (increasesDamageTaken > 0)
            {
                doeh.addDebuffToList(Tags.Properties.DamageTaken, 0f, -increasesDamageTaken, new List<float>(), new List<float>(), 4f);
            }
            if (increasesDoTDamageTaken > 0)
            {
                List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                tagList.Add(Tags.AbilityTags.DoT);
                doeh.addDebuffToList(Tags.Properties.DamageTaken, 0f, -increasesDoTDamageTaken, new List<float>(), new List<float>(), 4f, tagList);
            }
        }

        if (increasedStunChance != 0)
        {
            foreach (DamageStatsHolder damage in abilityObject.GetComponents<DamageStatsHolder>())
            {
                damage.baseDamageStats.increasedStunChance += increasedStunChance;
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

        if (moreDamageAgainstIgnited != 0)
        {
            // create the conditional
            DamageConditionalEffect conditionalEffect = new DamageConditionalEffect();
            HasStatusEffectConditional conditional = new HasStatusEffectConditional();
            conditional.statusEffect = StatusEffectID.Ignite;
            conditionalEffect.conditional = conditional;
            conditionalEffect.effect = new DamageEffectMoreDamage(moreDamageAgainstIgnited);
            // add the conditional to all damage stats holders
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.baseDamageStats.conditionalEffects.Add(conditionalEffect);
            }
        }

        if (moreDamageAgainstTimeRotting != 0)
        {
            // create the conditional
            DamageConditionalEffect conditionalEffect = new DamageConditionalEffect();
            HasStatusEffectConditional conditional = new HasStatusEffectConditional();
            conditional.statusEffect = StatusEffectID.TimeRot;
            conditionalEffect.conditional = conditional;
            conditionalEffect.effect = new DamageEffectMoreDamage(moreDamageAgainstTimeRotting);
            // add the conditional to all damage stats holders
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.baseDamageStats.conditionalEffects.Add(conditionalEffect);
            }
        }

        return abilityObject;
    }
}
