using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemAttachesToCasterBody : MonoBehaviour
{
    public GameObject creationReferencesObject = null;

    public void Start()
    {
        if (creationReferencesObject)
        {
            CreationReferences references = creationReferencesObject.GetComponent<CreationReferences>();
            ParticleSystem particleSystem = GetComponent<ParticleSystem>();
            if (references && references.creator && particleSystem)
            {
                ActorModelManager amm = references.creator.GetComponent<ActorModelManager>();
                if (amm)
                {
                    amm.applyParticlesToBody(particleSystem);
                }
            }
        }

    }
}

