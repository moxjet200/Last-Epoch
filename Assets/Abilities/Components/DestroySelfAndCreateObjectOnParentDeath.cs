using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SelfDestroyer))]
public class DestroySelfAndCreateObjectOnParentDeath : DestroySelfOnParentDeath
{

    public GameObject createWhenDestroyingSelfOnParentDeath = null;
    public bool requireBelowMaxAge = false;
    public float maxAge = 0f;

    float age = 0f;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        age += Time.deltaTime;
    }

    public override void beforeDestroying()
    {
        if ((!requireBelowMaxAge || age <= maxAge) && createWhenDestroyingSelfOnParentDeath)
        {
            Instantiate(createWhenDestroyingSelfOnParentDeath).transform.position = transform.position;
        }
    }
}
