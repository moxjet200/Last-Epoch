using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntanglingRootsInitialHitMutator : AbilityMutator
{

    public float increasedDamage = 0f;
    public float chanceToPoison = 0f;
    public float increasedRadius = 0f;
    public float increasedBuffDuration = 0f;
    public float damageBuff = 0f;
    public float poisonChanceToWolves = 0f;
    public float bleedchanceToBears = 0f;
    public float castSpeedToSpriggans = 0f;
    public bool healSpriggans = false;

    public bool meleeScalingInitialHit = false;
    public bool alwaysStuns = false;

    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.entanglingRootsInitialHit);
        base.Awake();
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

        if (damageBuff != 0)
        {
            BuffOnAllyHit boah = abilityObject.AddComponent<BuffOnAllyHit>();
            boah.addBuffToList(Tags.Properties.Damage, 0, damageBuff, null, null, 4 * (1 + increasedBuffDuration), null, "Entangling Roots Damage Buff");
        }

        if (poisonChanceToWolves != 0)
        {
            BuffOnAllyHit boah = abilityObject.AddComponent<BuffOnAllyHit>();
            boah.onlyApplyToMinionFromAbility = true;
            boah.requiredAbility = AbilityIDList.getAbility(AbilityID.summonWolf);
            boah.addBuffToList(Tags.Properties.PoisonChance, 0, poisonChanceToWolves, null, null, 4 * (1 + increasedBuffDuration), null, "Entangling Roots Poison Buff");
        }

        if (bleedchanceToBears != 0)
        {
            BuffOnAllyHit boah = abilityObject.AddComponent<BuffOnAllyHit>();
            boah.onlyApplyToMinionFromAbility = true;
            boah.requiredAbility = AbilityIDList.getAbility(AbilityID.summonBear);
            boah.addBuffToList(Tags.Properties.BleedChance, 0, bleedchanceToBears, null, null, 4 * (1 + increasedBuffDuration), null, "Entangling Roots Bleed Buff");

            BuffOnAllyHit boah2 = abilityObject.AddComponent<BuffOnAllyHit>();
            boah2.onlyApplyToMinionFromAbility = true;
            boah2.requiredAbility = AbilityIDList.getAbility(AbilityID.summonSerpent);
            boah2.addBuffToList(Tags.Properties.BleedChance, 0, bleedchanceToBears, null, null, 4 * (1 + increasedBuffDuration), null, "Entangling Roots Bleed Buff");
        }

        if (castSpeedToSpriggans != 0)
        {
            BuffOnAllyHit boah = abilityObject.AddComponent<BuffOnAllyHit>();
            boah.onlyApplyToMinionFromAbility = true;
            boah.requiredAbility = AbilityIDList.getAbility(AbilityID.summonSpriggan);
            boah.addBuffToList(Tags.Properties.CastSpeed, 0, castSpeedToSpriggans, null, null, 4 * (1 + increasedBuffDuration), null, "Entangling Roots Cast Buff");
        }

        if (healSpriggans)
        {
            HealAlliesOnHit component = abilityObject.AddComponent<HealAlliesOnHit>();
            component.onlyApplyToMinionFromAbility = true;
            component.requiredAbility = AbilityIDList.getAbility(AbilityID.summonSpriggan);
            component.healAmount = 100000;
        }

        // initial melee hit
        if (!meleeScalingInitialHit)
        {
            foreach (CreateOnDeath cod in abilityObject.GetComponents<CreateOnDeath>())
            {
                cod.objectsToCreateOnDeath.Clear();
            }
            foreach (DamageEnemyOnHit damage in abilityObject.GetComponents<DamageEnemyOnHit>())
            {
                damage.baseDamageStats.damage.Clear();
                damage.baseDamageStats.addedDamageScaling = 0;
                Destroy(damage);
                abilityObject.AddComponent<CannotApplyAdditionalStatuses>();
            }
        }
        else
        {
            if (increasedDamage != 0)
            {
                foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
                {
                    holder.increaseAllDamage(increasedDamage);
                }
            }

            if (chanceToPoison > 0)
            {
                ChanceToApplyStatusOnEnemyHit newComponent = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
                newComponent.statusEffect = StatusEffectList.getEffect(StatusEffectID.Poison);
                newComponent.chance = chanceToPoison;
            }
            if (alwaysStuns)
            {
                StunEnemyOnHit component = abilityObject.AddComponent<StunEnemyOnHit>();
                component.duration = 0.8f;
            }
        }
        

        return abilityObject;
    }
}
