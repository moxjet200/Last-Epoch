using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AbilityEventListener))]
[RequireComponent(typeof(AbilityObjectConstructor))]
public class CastAtCorpseOnKill : MonoBehaviour {

    public Ability abilityToCast = null;
    AbilityObjectConstructor constructor = null;

    public bool castFromCorpse = false;

    public bool eraseCorpse = false;

	// Use this for initialization
	void Start () {
        GetComponent<AbilityEventListener>().onKillEvent += Cast;
        constructor = GetComponent<AbilityObjectConstructor>();
	}

    public void Cast(Ability killingAbility, GameObject corpse){
        if (castFromCorpse)
        {
            constructor.constructAbilityObject(abilityToCast, corpse.transform.position, corpse.transform.position + Vector3.Normalize(corpse.transform.position - transform.position));
        }
        else
        {
            constructor.constructAbilityObject(abilityToCast, transform.position, corpse.transform.position);
        }
        if (eraseCorpse)
        {
            Dying dying = corpse.GetComponent<Dying>();
            if (dying) { dying.setDelays(0f, 0f); }
        }
    }
}
