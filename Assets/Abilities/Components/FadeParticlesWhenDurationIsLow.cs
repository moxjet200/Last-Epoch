using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DestroyAfterDuration))]
public class FadeParticlesWhenDurationIsLow : MonoBehaviour {

    DestroyAfterDuration destroyer = null;
    public bool customFadePerSecond = false;
    public float fadePerSecond = 1f;
    bool faded = false;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (destroyer == null) { destroyer = GetComponent<DestroyAfterDuration>(); }
        else
        {
            if (!faded && destroyer.duration - destroyer.age < 2)
            {
                faded = true;
                RecursivelyAttachFaders(transform);
            }
        }
	}


    void RecursivelyAttachFaders(Transform t)
    {
        if (t.GetComponent<ParticleSystem>()) {
            ParticleSystemFader fader = t.gameObject.AddComponent<ParticleSystemFader>();
            if (customFadePerSecond) { fader.setFadePerSecond(fadePerSecond); }
        }
        foreach (Transform child in t)
        {
            RecursivelyAttachFaders(child);
        }
    }
}
