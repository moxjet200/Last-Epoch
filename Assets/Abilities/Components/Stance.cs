using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stance : MonoBehaviour
{
    [HideInInspector]
    public Ability ability = null;

    public bool toggle = false;

    public void Start()
    {
        CreationReferences references = GetComponent<CreationReferences>();
        if (references && references.thisAbility)
        {
            ability = references.thisAbility;
            if (transform.parent)
            {
                bool toggleOff = false;
                Stance[] stances = transform.parent.GetComponentsInChildren<Stance>();
                for (int i = 0; i < stances.Length; i++)
                {
                    if (stances[i].ability == ability && toggle && stances[i] != this)
                    {
                        toggleOff = true;
                    }
                    if (stances[i] != this)
                    {
                        SelfDestroyer destroyer = stances[i].GetComponent<SelfDestroyer>();
                        if (destroyer)
                        {
                            destroyer.die();
                        }
                    }
                }
                if (toggleOff)
                {
                    Comp<SelfDestroyer>.GetOrAdd(gameObject).die();
                }
            }
        }
    }


}

