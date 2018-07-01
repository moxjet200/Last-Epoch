using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempestMutator : AbilityMutator
{
    public List<TaggedStatsHolder.TaggableStat> statsWhileSpinning = new List<TaggedStatsHolder.TaggableStat>();

    public List<TaggedStatsHolder.TaggableStat> statsWhileSpinningIfNotUsingAShield = new List<TaggedStatsHolder.TaggableStat>();

    public float addedCritMultiplier = 0f;
    public float addedCritChance = 0f;
    
    public float increasedDamage = 0f;
    public float timeRotChance = 0f;
    public float moreDamageAgainstFullHealth = 0f;
    public float increasedRadius = 0f;
    public float igniteChance = 0f;
    public float moreDamageAgainstTimeRotting = 0f;

    public float parabolicVoidOrbOnHitChance = 0f;
    public bool castsAbyssalOrb = false;
    public float increasedAbyssalOrbFrequence = 0f;

    public float addedVoidDamage = 0f;
    public float increasedDamageWhileNotUsingAShield = 0f;

    public bool pulls = false;
    public float increasedPullRadius = 0f;
    public bool strongerPull = false;

    // REPLACES increased radius when not using a shield
    public float increasedRadiusWhileNotUsingAShield = 0f;

    AbilityObjectConstructor aoc = null;
    WeaponInfoHolder weaponInfoHolder = null;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.tempest);
        base.Awake();
        AbilityEventListener ael = AbilityEventListener.GetOrAdd(gameObject);
        ael.onHitEvent += OnHit;
        aoc = GetComponent<AbilityObjectConstructor>();
        weaponInfoHolder = GetComponent<WeaponInfoHolder>();
    }

    public void OnHit(Ability _ability, GameObject target)
    {
        if (_ability == ability || _ability == AbilityIDList.getAbility(AbilityID.tempestHit))
        {
            if (parabolicVoidOrbOnHitChance > 0 && aoc)
            {
                float rand = Random.Range(0f, 1f);
                if (rand < parabolicVoidOrbOnHitChance)
                {
                    if (target) { aoc.constructAbilityObject(AbilityIDList.getAbility(AbilityID.parabolicVoidOrb), transform.position, target.transform.position); }
                    else { aoc.constructAbilityObject(AbilityIDList.getAbility(AbilityID.parabolicVoidOrb), transform.position, transform.position + new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f))); }
                }
            }
        }
    }

    public override List<Tags.AbilityTags> getUseTags()
    {
        List<Tags.AbilityTags> list = base.getUseTags();
        if (addedVoidDamage > 0)
        {
            list.Add(Tags.AbilityTags.Void);
        }
        return list;
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        float newIncreasedRadius = increasedRadius;

        if (increasedRadiusWhileNotUsingAShield > 0)
        {
            if (!weaponInfoHolder) { weaponInfoHolder = GetComponent<WeaponInfoHolder>(); }
            if (weaponInfoHolder && !weaponInfoHolder.hasShield)
            {
                newIncreasedRadius = increasedRadiusWhileNotUsingAShield;
            }
        }

        TempestHitMutator hitMutator = abilityObject.AddComponent<TempestHitMutator>();
        hitMutator.addedCritMultiplier = addedCritMultiplier;
        hitMutator.addedCritChance = addedCritChance;
        hitMutator.increasedDamage = increasedDamage;
        hitMutator.timeRotChance = timeRotChance;
        hitMutator.moreDamageAgainstFullHealth = moreDamageAgainstFullHealth;
        hitMutator.increasedRadius = newIncreasedRadius;
        hitMutator.igniteChance = igniteChance;
        hitMutator.addedVoidDamage = addedVoidDamage;
        hitMutator.moreDamageAgainstTimeRotting = moreDamageAgainstTimeRotting;


        if (increasedDamageWhileNotUsingAShield > 0)
        {
            if (!weaponInfoHolder) { weaponInfoHolder = GetComponent<WeaponInfoHolder>(); }
            if (weaponInfoHolder && !weaponInfoHolder.hasShield)
            {
                hitMutator.increasedDamage += increasedDamageWhileNotUsingAShield;
            }
        }
        
        if (newIncreasedRadius != 0)
        {
            foreach (Transform child in abilityObject.transform)
            {
                child.localScale = new Vector3(child.localScale.x * (1 + newIncreasedRadius), child.localScale.y, child.localScale.z * (1 + newIncreasedRadius));
            }
        }

        if (statsWhileSpinning != null && statsWhileSpinning.Count > 0)
        {
            BuffParent bp = abilityObject.GetComponent<BuffParent>();
            if (!bp) { bp = abilityObject.AddComponent<BuffParent>(); }

            List<TaggedStatsHolder.TaggableStat> stats = new List<TaggedStatsHolder.TaggableStat>();

            foreach(TaggedStatsHolder.TaggableStat stat in statsWhileSpinning)
            {
                TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(stat);
                stats.Add(newStat);
            }

            if (!weaponInfoHolder){ weaponInfoHolder = GetComponent<WeaponInfoHolder>(); }
            if (weaponInfoHolder && !weaponInfoHolder.hasShield)
            {
                foreach (TaggedStatsHolder.TaggableStat stat in statsWhileSpinningIfNotUsingAShield)
                {
                    TaggedStatsHolder.TaggableStat newStat = new TaggedStatsHolder.TaggableStat(stat);
                    stats.Add(newStat);
                }
            }

            bp.taggedStats.AddRange(stats);
        }

        if (castsAbyssalOrb)
        {
            CastAfterDuration cad = abilityObject.AddComponent<CastAfterDuration>();
            cad.ability = Ability.getAbility(AbilityID.abyssalOrb);
            cad.interval = 1f / (1f + increasedAbyssalOrbFrequence);
            cad.randomAiming = true;
        }

        if (pulls)
        {
            RepeatedlyPullEnemiesWithinRadius component = abilityObject.AddComponent<RepeatedlyPullEnemiesWithinRadius>();
            component.radius = 5f * (1 + increasedPullRadius);
            component.distanceMultiplier = 0.98f;
            if (strongerPull) { component.distanceMultiplier = 0.96f; }
            component.interval = 0.02f;

            //CastAfterDuration cad = abilityObject.AddComponent<CastAfterDuration>();
            //cad.ability = Ability.getAbility(AbilityID.bigPull);
            //cad.interval = 0.5f;
            //cad.age = Random.Range(0.1f, 0.9f);
            //BigPullMutator mut = abilityObject.AddComponent<BigPullMutator>();
            //mut.increasedRadius = increasedPullRadius;
        }

        return abilityObject;
    }
}
