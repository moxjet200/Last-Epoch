using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// needs a hit detector to know when to deal damage
[RequireComponent(typeof(HitDetector))]
public class HealAlliesOnHit : MonoBehaviour
{

    public bool canHealSameAllyAgain = false;

    public float healAmount = 0f;

    public GameObject attachable = null;
    public bool onlyApplyToMinionFromAbility = false;
    public Ability requiredAbility = null;

    // Use this for initialization
    void Start()
    {
        // subscribe to the new ally hit event
        GetComponent<HitDetector>().newAllyHitEvent += heal;
        // if it can damage the same ally again also subscribe to the ally hit again event
        if (canHealSameAllyAgain) { GetComponent<HitDetector>().allyHitAgainEvent += heal; }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void heal(GameObject ally)
    {
        if (onlyApplyToMinionFromAbility)
        {
            if (requiredAbility == null) { return; }
            CreationReferences refences = ally.GetComponent<CreationReferences>();
            if (!refences) { return; }
            if (refences.thisAbility != requiredAbility) { return; }
        }

        if (ally.GetComponent<BaseHealth> () != null) {
            ally.GetComponent<BaseHealth>().Heal(healAmount);
            if (attachable)
            {
                Instantiate(attachable, ally.transform);
            }
		} else {
			Debug.Log("Ally " + ally.name + " has no BaseHealth class");
		}
    }
}
