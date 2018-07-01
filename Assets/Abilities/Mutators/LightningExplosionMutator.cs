using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningExplosionMutator : AbilityMutator
{
    public float increasedDamage = 0f;
    public float chanceToShock = 0f;

    StatBuffs statBuffs = null;
    ProtectionClass protectionClass = null;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.lightningExplosion);
        base.Awake();
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
        
        if (chanceToShock > 0)
        {
            ChanceToApplyStatusOnEnemyHit ctasoeh = abilityObject.AddComponent<ChanceToApplyStatusOnEnemyHit>();
            ctasoeh.chance = chanceToShock;
            ctasoeh.statusEffect = StatusEffectList.getEffect(StatusEffectID.Shock);
        }


        return abilityObject;
    }
}
