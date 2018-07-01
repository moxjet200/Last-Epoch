using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedFireCircleMutator : AbilityMutator
{
    public float increasedDamage = 0f;
    public float increasedStunChance = 0f;
    public List<float> moreDamageInstances = new List<float>();
    public bool convertToCold = false;
    public float chillChance = 0f;
    public float igniteChance = 0f;

    // Use this for initialization
    protected override void Awake()
    {
        ability = AbilityIDList.getAbility(AbilityID.delayedFireCircle);
        base.Awake();
    }

    public override GameObject Mutate(GameObject abilityObject, Vector3 location, Vector3 targetLocation)
    {

        if (convertToCold)
        {
            foreach (Transform child in abilityObject.transform)
            {
                if (child.name == "DelayQuickFireCircleVFX") { child.gameObject.SetActive(false); }
                if (child.name == "DelayedQuickIceCircleVFX") { child.gameObject.SetActive(true); }
            }
        }

        FireCircleMutator mut = abilityObject.AddComponent<FireCircleMutator>();
        mut.increasedDamage = increasedDamage;
        mut.increasedStunChance = increasedStunChance;
        mut.moreDamageInstances = moreDamageInstances;
        mut.convertToCold = convertToCold;
        mut.igniteChance = igniteChance;
        mut.chillChance = chillChance;

        return abilityObject;
    }


}
