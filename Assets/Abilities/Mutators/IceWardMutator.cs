using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWardMutator : AbilityMutator {

	public float additionalDuration = 0f;
	public float additionalBlockChance = 0f;
	public float additionalBlockElementalProtection = 0f;
	public bool canCastOnAllies = false;
	public float wardOnBlock = 0f;
	public float extraManaDrain = 0f;

    public float additionalWardRegen = 0f;
    public float additionalBlockArmour = 0f;
    public float additionalArmour = 0f;
    public float additionalWardRetention = 0f;
    public float reducedManaPenalty = 0f;

    public bool castsFrostNova = false;
    public float increasedFrostNovaFrequency = 0f;
    public float novaDamage = 0f;
    public float novaCritChance = 0f;
    public float novaCritMulti = 0f;

	protected override void Awake()
	{
		ability = AbilityIDList.getAbility(AbilityID.iceWard);
		base.Awake();
	}

	public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
	{
		BuffParent buffObject = abilityObject.GetComponent<BuffParent>();

		// add additional duration
		if (additionalDuration != 0)
		{
			DestroyAfterDuration durationObject = abilityObject.GetComponent<DestroyAfterDuration>();
			if (durationObject != null) { durationObject.duration += additionalDuration; }
		}

		// add additional block chance
		if (additionalBlockChance > 0)
		{
			TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.BlockChance, new List<Tags.AbilityTags>());
			stat.addedValue = additionalBlockChance;
			buffObject.taggedStats.Add(stat);
		}

		// add additional block protections
		if (additionalBlockElementalProtection != 0)
		{
			TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.BlockFireProtection, new List<Tags.AbilityTags>());
			stat.addedValue = additionalBlockElementalProtection;
			buffObject.taggedStats.Add(stat);
			TaggedStatsHolder.TaggableStat stat2 = new TaggedStatsHolder.TaggableStat(Tags.Properties.BlockColdProtection, new List<Tags.AbilityTags>());
			stat2.addedValue = additionalBlockElementalProtection;
			buffObject.taggedStats.Add(stat2);
			TaggedStatsHolder.TaggableStat stat3 = new TaggedStatsHolder.TaggableStat(Tags.Properties.BlockLightningProtection, new List<Tags.AbilityTags>());
			stat3.addedValue = additionalBlockElementalProtection;
			buffObject.taggedStats.Add(stat3);
		}

        // add additional block armour
        if (additionalBlockArmour != 0)
        {
            TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.BlockArmor, new List<Tags.AbilityTags>());
            stat.addedValue = additionalBlockArmour;
            buffObject.taggedStats.Add(stat);
        }

        // add additional armour
        if (additionalArmour != 0)
        {
            TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.Armour, new List<Tags.AbilityTags>());
            stat.addedValue = additionalArmour;
            buffObject.taggedStats.Add(stat);
        }

        // allow casting on allies
        if (canCastOnAllies)
		{
			abilityObject.GetComponent<AttachToCreatorOnCreation>().runOnCreation = false;
			abilityObject.AddComponent<AttachToNearestAllyOnCreation>();
			abilityObject.AddComponent<StartsAtTarget>();
		}

		//adds ward gain on block
		if (wardOnBlock > 0) {
			List<Tags.AbilityTags> onBlock = new List<Tags.AbilityTags> ();
			onBlock.Add (Tags.AbilityTags.Block);
			TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.WardGain, onBlock);
			stat.addedValue = wardOnBlock;
			buffObject.taggedStats.Add(stat);
		}
        
        // ensure that reduced mana penalty is not greater than 1
        float newReducedManaPenalty = reducedManaPenalty;
        if (newReducedManaPenalty > 1) { newReducedManaPenalty = 1; } 
        // reduced mana penalties
        if (reducedManaPenalty != 0)
        {
            // reduce the "less" penalty intrinsic to the skill
            TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.ManaDrain, new List<Tags.AbilityTags>());
            // reducing a 20% less penalty by 40% is the same as adding a 10% more multiplier 
            stat.moreValues.Add(-newReducedManaPenalty);
            buffObject.taggedStats.Add(stat);
        }

		// add reduced mana regen (after being altered by reduced mana penalties)
		if (extraManaDrain != 0)
		{
			TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.ManaDrain, new List<Tags.AbilityTags>());
			stat.addedValue = extraManaDrain;
			buffObject.taggedStats.Add(stat);
		}
        
        // add ward retention
        if (additionalWardRetention != 0)
        {
            TaggedStatsHolder.TaggableStat stat = new TaggedStatsHolder.TaggableStat(Tags.Properties.WardRetention, new List<Tags.AbilityTags>());
            stat.addedValue = additionalWardRetention;
            buffObject.taggedStats.Add(stat);
        }

        // ice nova
        if (castsFrostNova)
        {
            CastAfterDuration cad = abilityObject.AddComponent<CastAfterDuration>();
            cad.limitCasts = false;
            cad.interval = 3f / (1 + increasedFrostNovaFrequency);
            cad.ability = AbilityIDList.getAbility(AbilityID.frostNova);

            FrostNovaMutator novaMutator = abilityObject.AddComponent<FrostNovaMutator>();
            novaMutator.increasedDamage = novaDamage;
            novaMutator.addedCritChance = novaCritChance;
            novaMutator.addedCritMultiplier = novaCritMulti;
        }

        // add additional ward regen
        if (additionalWardRegen > 0)
        {
            BuffParent protectionObject = abilityObject.GetComponent<BuffParent>();
            if (protectionObject != null) { protectionObject.wardRegen += additionalWardRegen; }
        }

        return abilityObject;
	}
}
