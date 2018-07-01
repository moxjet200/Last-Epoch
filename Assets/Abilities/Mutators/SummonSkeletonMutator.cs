using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonSkeletonMutator : AbilityMutator
{
    public float additionalDuration = 0f;

    public int additionalSkeletons = 0;

    public int additionalWarlords = 0;

    public bool limitDuration = false;

    public List<TaggedStatsHolder.TaggableStat> statList = new List<TaggedStatsHolder.TaggableStat>();

    public List<TaggedStatsHolder.TaggableStat> warlordStatList = new List<TaggedStatsHolder.TaggableStat>();

    public List<TaggedStatsHolder.TaggableStat> corpseStatList = new List<TaggedStatsHolder.TaggableStat>();

    public List<TaggedStatsHolder.TaggableStat> corpseSelfBuffStatList = new List<TaggedStatsHolder.TaggableStat>();

    public List<TaggedStatsHolder.TaggableStat> skeleDiedRecentlyStats = new List<TaggedStatsHolder.TaggableStat>();

    public bool usesPoisonArrow = false;
    public float increasedPoisonArrowCooldownSpeed = 0f;
    public float increasedPoisonArrowCooldown = 0f;

    public bool poisonArrowPierces = false;
    public bool poisonArrowInaccurate = false;
    public float poisonArrowIncreasedDamage = 0f;
    public int poisonArrowDelayedAttacks = 0;

    public bool usesMultishot = false;
    public float increasedMultishotCooldownSpeed = 0f;

    public bool usesDeathSlash = false;
    public float increasedDeathSlashCooldownRecovery = 0f;

    public bool usesNecroticMortar = false;
    public float increasedNecroticMortarCooldownRecovery = 0f;

    public bool usesInspire = false;
    public float increasedInspireCooldownRecovery = 0f;

    public bool cannotSummonWarriors = false;
    public bool cannotSummonMages = false;
    public bool canSummonArchers = false;
    public bool canSummonWarlords = false;

    public float healthOnSkeletonDeath = 0f;
    public float wardOnSkeletonDeath = 0f;
    public float manaOnSkeletonDeath = 0f;

    public int additionalSkeletonsPerCast = 0;
    public bool healSkeletonOnSkeletonDeath = false;

    // used in mutator
    Ability summonWarrior = null;
    Ability summonMage = null;
    Ability summonArcher = null;
    Ability summonWarlord = null;
    Ability summonBrawler = null;

    List<Ability> skeletonAbilities = new List<Ability>();
    List<BaseHealth> skeletonHealths = new List<BaseHealth>();

    BaseHealth health = null;
    ProtectionClass protectionClass = null;
    BaseMana mana = null;
    StatBuffs statBuffs = null;
    SummonTracker summonTracker = null;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.summonSkeleton);
        base.Awake();
        summonWarrior = Ability.getAbility(AbilityID.summonSkeletonWarrior);
        summonMage = Ability.getAbility(AbilityID.summonSkeletonMage);
        summonArcher = Ability.getAbility(AbilityID.summonSkeletonArcher);
        summonWarlord = Ability.getAbility(AbilityID.summonSkeletonWarlord);
        summonBrawler = Ability.getAbility(AbilityID.summonSkeletonBrawler);

        skeletonAbilities.Add(ability);
        skeletonAbilities.Add(summonWarrior);
        skeletonAbilities.Add(summonMage);
        skeletonAbilities.Add(summonArcher);
        skeletonAbilities.Add(summonWarlord);
        skeletonAbilities.Add(summonBrawler);

        summonTracker = Comp<SummonTracker>.GetOrAdd(gameObject);
        summonTracker.newSummonEvent += onNewSummon;

        protectionClass = GetComponent<ProtectionClass>();
        if (protectionClass) { health = protectionClass.healthClass; }
        else { health = GetComponent<BaseHealth>(); }
        mana = GetComponent<BaseMana>();

        statBuffs = Comp<StatBuffs>.GetOrAdd(gameObject);
    }

    public override int mutateDelayedCasts(int defaultCasts)
    {
        return base.mutateDelayedCasts(defaultCasts) + additionalSkeletonsPerCast;
    }

    public void onNewSummon(Summoned summon)
    {
        // subscribe to the death events of skeletal minions
        if (summon && summon.references && summon.references.thisAbility)
        {
            if (summon.references.thisAbility == ability || summon.references.thisAbility == summonWarrior || summon.references.thisAbility == summonMage
                || summon.references.thisAbility == summonArcher || summon.references.thisAbility == summonWarlord || summon.references.thisAbility == summonBrawler)
            {
                Dying dying = summon.GetComponent<Dying>();
                if (!dying) { Debug.LogError(summon.name + " does not have a Dying component yet, fix this script please"); return; }

                dying.deathEvent += OnSkeletonDeath;
            }
        }
    }

    public void OnSkeletonDeath(Dying dyingComponent)
    {
        if (dyingComponent.unsummoned) { return; }

        // apply on skeleton death events
        if (healthOnSkeletonDeath != 0 && health) { health.Heal(healthOnSkeletonDeath); }
        if (wardOnSkeletonDeath != 0 && protectionClass) { protectionClass.GainWard(wardOnSkeletonDeath); }
        if (manaOnSkeletonDeath != 0 && mana) { mana.gainMana(manaOnSkeletonDeath); }

        // apply stats
        if (skeleDiedRecentlyStats!= null && skeleDiedRecentlyStats.Count > 0)
        {
            TaggedBuff newBuff;
            for (int i = 0; i < skeleDiedRecentlyStats.Count; i++)
            {
                newBuff = new TaggedBuff();
                newBuff.stat = new TaggedStatsHolder.TaggableStat(skeleDiedRecentlyStats[i]);
                newBuff.remainingDuration = 4f;
                newBuff.name = "skeleton died recently buff " + newBuff.stat.property;
                if (newBuff.stat.tagList != null)
                {
                    foreach (Tags.AbilityTags tag in newBuff.stat.tagList)
                    {
                        newBuff.name += " " + tag;
                    }
                }
                statBuffs.addTaggedBuff(newBuff);
            }
        }

        // heal another skeleton
        if (healSkeletonOnSkeletonDeath && summonTracker && skeletonAbilities != null)
        {
            skeletonHealths.Clear();
            foreach (Summoned skeleton in summonTracker.getMinions(skeletonAbilities))
            {
                if (skeleton.getBaseHealth() && skeleton.getBaseHealth().currentHealth > 0)
                {
                    skeletonHealths.Add(skeleton.getBaseHealth());
                }
            }
            
            if (skeletonHealths.Count > 0)
            {
                BaseHealth skeleToHeal = skeletonHealths[Random.Range(0, skeletonHealths.Count - 1)];
                skeleToHeal.Heal(skeleToHeal.maxHealth);
                skeletonHealths.Clear();
            }

        }
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {
        // work out whether a corpse has been consumed to empower the skeleton
        bool corpseConsumed = false;

        if (corpseStatList != null && corpseStatList.Count > 0 && health && health.alignmentManager && health.alignmentManager.alignment != null)
        {
            float closestDistance = float.MaxValue;
            Dying closestDying = null;
            float distance = 0f;
            
            // check all corpses to find the closest
            foreach (Dying dying in Dying.all)
            {
                // check that the entity is dying and is an enemy
                if (dying.isDying() && dying.myHealth && dying.myHealth.alignmentManager && health.alignmentManager.alignment.foes.Contains(dying.myHealth.alignmentManager.alignment))
                {
                    // find the distance
                    distance = Maths.manhattanDistance(dying.transform.position, targetLocation);
                    // don't consume very distant corpses
                    if (distance <= 8f) {
                        // if a closest one hasn't been found yet this is the closest
                        if (closestDying == null)
                        {
                            closestDying = dying;
                            closestDistance = distance;
                        }
                        // otherwise compare distances
                        else
                        {
                            if (distance < closestDistance)
                            {
                                closestDying = dying;
                                closestDistance = distance;
                            }
                        }
                    }
                }
            }

            // consume the closest corpse
            if (closestDying)
            {
                closestDying.setDelays(0f, 0f);
                corpseConsumed = true;
            }
        }

        // self stat buffs on corpse consumption
        if (corpseConsumed)
        {
            TaggedBuff newBuff;
            for(int i = 0; i < corpseSelfBuffStatList.Count; i++)
            {
                newBuff = new TaggedBuff();
                newBuff.stat = new TaggedStatsHolder.TaggableStat(corpseSelfBuffStatList[i]);
                newBuff.remainingDuration = 4f;
                newBuff.name = "skeleton corpse buff " + newBuff.stat.property;
                if (newBuff.stat.tagList != null)
                {
                    foreach (Tags.AbilityTags tag in newBuff.stat.tagList)
                    {
                        newBuff.name += " " + tag;
                    }
                }
                statBuffs.addTaggedBuff(newBuff);
            }
        }

        // control the weights of each minion type
        CreateRandomAbilityObjectOnDeath randomSummoner = abilityObject.GetComponent<CreateRandomAbilityObjectOnDeath>();
        randomSummoner.possibleAbilities.Clear();
        randomSummoner.weights.Clear();
        if (!cannotSummonWarriors)
        {
            randomSummoner.possibleAbilities.Add(summonWarrior);
            // add extra weight if there are no warriors
            if (summonTracker.numberOfMinions(summonWarrior) <= 0)
            {
                randomSummoner.weights.Add(4);
            }
            else
            {
                randomSummoner.weights.Add(1);
            }
        }
        if (!cannotSummonMages)
        {
            randomSummoner.possibleAbilities.Add(summonMage);
            randomSummoner.weights.Add(1);
        }
        if (canSummonArchers)
        {
            randomSummoner.possibleAbilities.Add(summonArcher);
            randomSummoner.weights.Add(1);
        }
        if (canSummonWarlords)
        {
            randomSummoner.possibleAbilities.Add(summonWarlord);
            randomSummoner.weights.Add(0.45f);
        }
        if (cannotSummonWarriors && cannotSummonMages && !canSummonArchers && !canSummonWarlords)
        {
            randomSummoner.possibleAbilities.Add(summonBrawler);
            randomSummoner.weights.Add(1);
        }

        // skeleton warrior
        if (!cannotSummonWarriors)
        {
            SummonSkeletonWarriorMutator warrior = abilityObject.AddComponent<SummonSkeletonWarriorMutator>();
            warrior.statList.AddRange(statList);
            warrior.usesDeathSlash = usesDeathSlash;
            warrior.increasedDeathSlashCooldownRecovery = increasedDeathSlashCooldownRecovery;
            warrior.additionalDuration = additionalDuration;
            warrior.additionalSkeletons = additionalSkeletons;
            warrior.limitDuration = limitDuration;
            if (corpseConsumed)
            {
                warrior.tempStats.AddRange(corpseStatList);
            }
        }

        // skeleton mage
        if (!cannotSummonMages)
        {
            SummonSkeletonMageMutator mage = abilityObject.AddComponent<SummonSkeletonMageMutator>();
            mage.statList.AddRange(statList);
            mage.usesNecroticMortar = usesNecroticMortar;
            mage.increasedNecroticMortarCooldownRecovery = increasedNecroticMortarCooldownRecovery;
            mage.additionalDuration = additionalDuration;
            mage.additionalSkeletons = additionalSkeletons;
            mage.limitDuration = limitDuration;
            if (corpseConsumed)
            {
                mage.tempStats.AddRange(corpseStatList);
            }
        }

        // skeleton archer
        if (canSummonArchers)
        {
            SummonSkeletonArcherMutator archer = abilityObject.AddComponent<SummonSkeletonArcherMutator>();
            archer.statList.AddRange(statList);
            archer.usesPoisonArrow = usesPoisonArrow;
            archer.increasedPoisonArrowCooldownSpeed = increasedPoisonArrowCooldownSpeed;
            archer.increasedPoisonArrowCooldown = increasedPoisonArrowCooldown;
            archer.poisonArrowPierces = poisonArrowPierces;
            archer.poisonArrowInaccurate = poisonArrowInaccurate;
            archer.poisonArrowIncreasedDamage = poisonArrowIncreasedDamage;
            archer.poisonArrowDelayedAttacks = poisonArrowDelayedAttacks;
            archer.usesMultishot = usesMultishot;
            archer.increasedMultishotCooldownSpeed = increasedMultishotCooldownSpeed;
            archer.additionalDuration = additionalDuration;
            archer.additionalSkeletons = additionalSkeletons;
            archer.limitDuration = limitDuration;
            if (corpseConsumed)
            {
                archer.tempStats.AddRange(corpseStatList);
            }
        }

        // skeleton warlord
        if (canSummonWarlords)
        {
            SummonSkeletonWarlordMutator warlord = abilityObject.AddComponent<SummonSkeletonWarlordMutator>();
            warlord.statList.AddRange(statList);
            warlordStatList.AddRange(warlordStatList);
            warlord.usesInspire = usesInspire;
            warlord.increasedInspireCooldownRecovery = increasedInspireCooldownRecovery;
            warlord.additionalDuration = additionalDuration;
            warlord.additionalSkeletons = additionalWarlords;
            warlord.limitDuration = limitDuration;
            if (corpseConsumed)
            {
                warlord.tempStats.AddRange(corpseStatList);
            }
        }

        // skeleton brawler
        if (cannotSummonWarriors && cannotSummonMages && !canSummonArchers && !canSummonWarlords)
        {
            SummonSkeletonBrawlerMutator brawler = abilityObject.AddComponent<SummonSkeletonBrawlerMutator>();
            brawler.statList.AddRange(statList);
            brawler.additionalDuration = additionalDuration;
            brawler.additionalSkeletons = additionalSkeletons;
            brawler.limitDuration = limitDuration;
            if (corpseConsumed)
            {
                brawler.tempStats.AddRange(corpseStatList);
            }
        }

        return abilityObject;
    }
}
