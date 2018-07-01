using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSplatterMutator : AbilityMutator
{
    public float increasedRadius = 0f;
    public float increasedDamage = 0f;
    public float increasedStunChance = 0f;
    public float chanceToPoison = 0f;
    public float armourReductionChance = 0f;
    public float armourReduction = 0f;
    public bool armourReductionStacks = false;
    public float increasedArmourDebuffDuration = 0f;
    public float increasedDamagePerMinion = 0f;
    public bool reducesDarkProtectionInstead = false;

    public List<TaggedStatsHolder.TaggableStat> minionBuffs = new List<TaggedStatsHolder.TaggableStat>();

    public bool necrotic = false;

    public List<float> moreDamageInstances = new List<float>();

    CreationReferences references = null;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.bloodSplatter);
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        references = GetComponent<CreationReferences>();
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {

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

        if (necrotic)
        {
            // replace vfx
            CreateOnDeath cod = abilityObject.GetComponent<CreateOnDeath>();
            if (cod && cod.objectsToCreateOnDeath != null && cod.objectsToCreateOnDeath.Count > 0)
            {
                cod.objectsToCreateOnDeath[0] = new CreateOnDeath.GameObjectHolder(PrefabList.getPrefab("NecroticBloodSplatterOnDeathVFX"));
            }

            // convert damage
            foreach (DamageStatsHolder damage in abilityObject.GetComponents<DamageStatsHolder>())
            {
                damage.convertAllDamageOfType(DamageType.PHYSICAL, DamageType.NECROTIC);
            }
        }

        if (chanceToPoison > 0)
        {
            ChanceToApplyStatusOnEnemyHit newComponent = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            newComponent.statusEffect = StatusEffectList.getEffect(StatusEffectID.Poison);
            newComponent.chance = chanceToPoison;
        }

        if (armourReductionChance > 0 && Random.Range(0f, 1f) < armourReductionChance)
        {
            DebuffOnEnemyHit component = abilityObject.AddComponent<DebuffOnEnemyHit>();
            if (reducesDarkProtectionInstead)
            {
                if (armourReductionStacks)
                {
                    component.addDebuffToList(Tags.Properties.DarkProtection, armourReduction, 0f, null, null, 4f * (1 + increasedArmourDebuffDuration));
                }
                else
                {
                    component.addDebuffToList(Tags.Properties.DarkProtection, armourReduction, 0f, null, null, 4f * (1 + increasedArmourDebuffDuration), null, "blood splatter armour reduction");
                }
            }
            else
            {
                if (armourReductionStacks)
                {
                    component.addDebuffToList(Tags.Properties.Armour, armourReduction, 0f, null, null, 4f * (1 + increasedArmourDebuffDuration));
                }
                else
                {
                    component.addDebuffToList(Tags.Properties.Armour, armourReduction, 0f, null, null, 4f * (1 + increasedArmourDebuffDuration), null, "blood splatter armour reduction");
                }
            }
        }

        if (minionBuffs != null && minionBuffs.Count > 0)
        {
            string buffName = "blood splatter buff";
            BuffOnAllyHit component = abilityObject.AddComponent<BuffOnAllyHit>();
            component.onlyApplyToCreatorsMinions = true;
            for (int i = 0; i < minionBuffs.Count; i++)
            {
                string fullBuffName = buffName + minionBuffs[i].property;
                foreach (Tags.AbilityTags _tag in minionBuffs[i].tagList)
                {
                    fullBuffName += _tag;
                }
                component.addBuffToList(minionBuffs[i].property, minionBuffs[i].addedValue, minionBuffs[i].increasedValue, minionBuffs[i].moreValues,
                    minionBuffs[i].quotientValues, 4f, minionBuffs[i].tagList, fullBuffName);
            }
        }


        if (increasedStunChance != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.baseDamageStats.increasedStunChance += increasedStunChance;
            }
        }


        // increase damage based on the number of minions
        float realIncreasedDamage = increasedDamage;
        if (increasedDamagePerMinion != 0)
        {
            if (references && references.creator)
            {
                SummonTracker tracker = references.creator.GetComponent<SummonTracker>();
                if (tracker && tracker.summons != null)
                {
                    realIncreasedDamage += increasedDamagePerMinion * tracker.summons.Count;
                }
            }
        }


        if (realIncreasedDamage != 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(realIncreasedDamage);
            }
        }

        if (moreDamageInstances != null && moreDamageInstances.Count > 0)
        {
            float moreDamage = 1f;
            foreach (float instance in moreDamageInstances)
            {
                moreDamage *= 1 + instance;
            }

            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.increaseAllDamage(moreDamage - 1);
            }
        }

        return abilityObject;
    }


}