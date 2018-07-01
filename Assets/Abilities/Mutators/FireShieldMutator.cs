using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireShieldMutator : AbilityMutator {

    
    public float additionalDuration = 0f;

    public float additionalWardRegen = 0f;

    public float additionalElementalProtection = 0f;

    public bool canCastOnAllies = false;

    public float damageThreshold = 30f;

    public float aoeDamage = 0f;

    public float aoeRadius = 3.5f;

    public bool ignitesInAoe = false;

    public float increasedIgniteFrequency = 0f;

    public float increasedFireballDamage = 0f;

    public float fireballIgniteChance = 0f;

    public bool fireballPierces = false;

    public bool grantsColdDamage = false;

    public bool grantsLightningDamage = false;

    public float igniteChanceGranted = 0f;

    /*
    public bool ignites = false;

    public float igniteInterval = 3f;
    */
    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.fireShield);
        base.Awake();
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        // add additional duration
        if (additionalDuration != 0)
        {
            DestroyAfterDuration durationObject = abilityObject.GetComponent<DestroyAfterDuration>();
            if (durationObject != null) { durationObject.duration += additionalDuration; }
        }

        // add additional ward regen
        if (additionalWardRegen > 0)
        {
            BuffParent protectionObject = abilityObject.GetComponent<BuffParent>();
            if (protectionObject != null) { protectionObject.wardRegen += additionalWardRegen; }
        }

        // add additional elemental protection
        if (additionalElementalProtection != 0 || igniteChanceGranted != 0 || grantsColdDamage || grantsLightningDamage)
        {
            BuffParent protectionObject = abilityObject.GetComponent<BuffParent>();
            if (protectionObject != null) {
                if (additionalElementalProtection != 0)
                {
                    TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.FireProtection, new List<Tags.AbilityTags>());
                    stat.addedValue = additionalElementalProtection;
                    protectionObject.taggedStats.Add(stat);
                    TaggedStatsHolder.TaggableStat stat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.ColdProtection, new List<Tags.AbilityTags>());
                    stat2.addedValue = additionalElementalProtection;
                    protectionObject.taggedStats.Add(stat2);
                    TaggedStatsHolder.TaggableStat stat3 = new TaggedStatsHolder.TaggableStat(Tags.Properties.LightningProtection, new List<Tags.AbilityTags>());
                    stat3.addedValue = additionalElementalProtection;
                    protectionObject.taggedStats.Add(stat3);
                }
                if (igniteChanceGranted != 0)
                {
                    TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.IgniteChance, new List<Tags.AbilityTags>());
                    stat.addedValue = igniteChanceGranted;
                    protectionObject.taggedStats.Add(stat);
                }
                if (grantsColdDamage)
                {
                    List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                    tagList.Add(Tags.AbilityTags.Cold);
                    TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, tagList);
                    stat.increasedValue = 0.4f;
                    protectionObject.taggedStats.Add(stat);
                }
                if (grantsLightningDamage)
                {
                    List<Tags.AbilityTags> tagList = new List<Tags.AbilityTags>();
                    tagList.Add(Tags.AbilityTags.Lightning);
                    TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Damage, tagList);
                    stat.increasedValue = 0.4f;
                    protectionObject.taggedStats.Add(stat);
                }
            }
        }

        // allow casting on allies
        if (canCastOnAllies)
        {
            abilityObject.GetComponent<AttachToCreatorOnCreation>().runOnCreation = false;
            abilityObject.AddComponent<AttachToNearestAllyOnCreation>();
            abilityObject.AddComponent<StartsAtTarget>();
        }

        // set damage threshold
        if (abilityObject.GetComponent<RetaliateWhenParentHit>())
        {
            abilityObject.GetComponent<RetaliateWhenParentHit>().damageTakenTrigger = (int) damageThreshold;
        }

        // aoe damage
        if (aoeDamage != 0)
        {
            RepeatedlyDamageEnemiesWithinRadius repeatDamage = abilityObject.GetComponent<RepeatedlyDamageEnemiesWithinRadius>();
            if (repeatDamage == null) { repeatDamage = abilityObject.AddComponent<RepeatedlyDamageEnemiesWithinRadius>(); }
            if (repeatDamage.baseDamageStats.damage == null) { repeatDamage.baseDamageStats.damage = new List<DamageStatsHolder.DamageTypesAndValues>(); }
            repeatDamage.baseDamageStats.damage.Add(new DamageStatsHolder.DamageTypesAndValues(DamageType.FIRE, aoeDamage));
            repeatDamage.damageInterval = 0.33f;
            repeatDamage.radius = aoeRadius;
            repeatDamage.baseDamageStats.addedDamageScaling = 0.17f;
            repeatDamage.tags.Add(Tags.AbilityTags.AoE);
            repeatDamage.tags.Add(Tags.AbilityTags.Spell);
            repeatDamage.tags.Add(Tags.AbilityTags.DoT);
            foreach (Transform child in abilityObject.transform)
            {
                if (child.name == "Circle")
                {
                    child.gameObject.SetActive(true);
                    child.transform.localScale = new Vector3(1.3f * aoeRadius / 3.5f, 1, 1.3f * aoeRadius / 3.5f);
                }
            }
        }

        // igniting
        if (ignitesInAoe)
        {
            CastAfterDuration cad = abilityObject.AddComponent<CastAfterDuration>();
            cad.ability = AbilityIDList.getAbility(AbilityID.invisibleIgniteNova);
            if (increasedIgniteFrequency != 0)
            {
                if (increasedIgniteFrequency >= 0.9f) { increasedIgniteFrequency = 0.9f; }
                cad.interval /= (1 + increasedIgniteFrequency);
            }
        }

        // mutating fireball
        if (fireballIgniteChance != 0 || fireballPierces || increasedFireballDamage!= 0)
        {
            FireballMutator fireballMutator = abilityObject.AddComponent<FireballMutator>();
            fireballMutator.increasedDamage = increasedFireballDamage;
            fireballMutator.igniteChance = fireballIgniteChance;
            if (fireballPierces)
            {
                fireballMutator.targetsToPierce += 1000;
            }
        }

        return abilityObject;
    }
}
