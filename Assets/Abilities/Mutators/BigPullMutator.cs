using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigPullMutator : AbilityMutator
{
    public float increasedRadius = 0f;

    ProtectionClass protectionClass = null;
    StatBuffs statBuffs = null;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.bigPull);
        base.Awake();
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {

        if (increasedRadius > 0)
        {
            foreach (RepeatedlyPullEnemiesWithinRadius component in abilityObject.GetComponents<RepeatedlyPullEnemiesWithinRadius>())
            {
                component.radius *= (1 + increasedRadius);
            }
        }

        return abilityObject;
    }
}
