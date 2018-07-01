using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceCasterMaterialUntilDestroyed : MonoBehaviour
{
    public Material newMaterial = null;
    Material oldMaterial = null;
    ActorModelManager amm = null;

    public void Start()
    {
        if (newMaterial != null) {
            CreationReferences references = GetComponent<CreationReferences>();
            if (references && references.creator)
            {
                amm = references.creator.GetComponent<ActorModelManager>();
                if (amm && amm.body)
                {
                    oldMaterial = amm.body.material;
                    amm.body.material = newMaterial;
                }
            }
        }
    }

    public void OnDestroy()
    {
        if (amm && amm.body)
        {
            amm.body.material = oldMaterial;
        }
    }
}
