using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyIfNotAbilityOfCaster : MonoBehaviour
{
    CreationReferences references = null;
    AbilityList list = null;

    public void Start()
    {
        references = GetComponent<CreationReferences>();
        if (references && references.creator)
        {
            list = references.creator.GetComponent<AbilityList>();
        }
    }

    public void Update()
    {
        if (list)
        {
            if (!list.abilities.Contains(references.thisAbility))
            {
                SelfDestroyer destroyer = Comp<SelfDestroyer>.GetOrAdd(gameObject);
                destroyer.die();
            }
        }
    }

}
