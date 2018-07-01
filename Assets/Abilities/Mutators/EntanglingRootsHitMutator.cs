using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntanglingRootsHitMutator : AbilityMutator
{
    public float increasedDamage = 0f;
    public float addedPoisonDamage = 0f;
    public float increasedRadius = 0f;

    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.entanglingRootsHit);
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

        if (addedPoisonDamage > 0)
        {
            foreach (DamageStatsHolder holder in abilityObject.GetComponents<DamageStatsHolder>())
            {
                holder.addBaseDamage(DamageType.POISON, addedPoisonDamage);
            }
        }

        if (increasedRadius != 0)
        {
            foreach (CapsuleCollider collider in abilityObject.GetComponents<CapsuleCollider>())
            {
                collider.height *= (1 + increasedRadius);
            }
        }

        return abilityObject;
    }
}
