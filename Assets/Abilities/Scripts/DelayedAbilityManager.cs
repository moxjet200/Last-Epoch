using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DelayedAbility
{
    public Ability ability;
    public float remainingDuration = 0f;
    public Vector3 target;

    public DelayedAbility(Ability _ability, float _duration, Vector3 _target)
    {
        ability = _ability;
        remainingDuration = _duration;
        target = _target;
    }
}

[RequireComponent(typeof(UsingAbility))]
public class DelayedAbilityManager : MonoBehaviour {

    public List<DelayedAbility> delayedAbilities = new List<DelayedAbility>();

    UsingAbility usingAbility;

	// Use this for initialization
	void Start () {
        usingAbility = GetComponent<UsingAbility>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Dying.isDying(gameObject)) { return; }
        else
        {
            float delta = Time.deltaTime;
            //foreach (DelayedAbility delayedAbility in delayedAbilities)
            //{
            //    if (delayedAbility.remainingDuration <= 0)
            //    {
            //        usingAbility.UseAbility(delayedAbility.ability, delayedAbility.target, false, false, true);
            //    }
            //}

            for (int i = delayedAbilities.Count - 1; i >= 0; i--)
            {
                if (delayedAbilities[i].remainingDuration <= 0)
                {
                    usingAbility.UseAbility(delayedAbilities[i].ability, delayedAbilities[i].target, false, false, true);
                }
            }


            delayedAbilities.RemoveAll(x => x.remainingDuration <= 0);
            foreach (DelayedAbility delayedAbility in delayedAbilities)
            {
                delayedAbility.remainingDuration -= delta;
            }
        }
	}

    public void addAbility(Ability ability, float delay, Vector3 target)
    {
        delayedAbilities.Add(new DelayedAbility(ability, delay, target));
    }

    public void addAbilitiesOverDuration(Ability ability, float duration, Vector3 target, int number)
    {
        for (int i = 0; i< number; i++)
        {
            addAbility(ability, duration * (i + 1) / number, target);
        }
    }


}
