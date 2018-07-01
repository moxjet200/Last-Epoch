using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// a hit detector, shared between multiple ability objects that listens for their death events and destroys itself once there are none
public class SharedHitDetector : HitDetector
{
    public float numberOfSubscribers;
    float timeSinceLostAllSubscribers = 0;
    float destructionDelayAfterLosingAllSubscribers = 4f;

    public void Subscribe(SelfDestroyer selfDestroyer)
    {
        numberOfSubscribers++;
        selfDestroyer.deathEvent += reduceSubscribers;
    }

    public void reduceSubscribers()
    {
        numberOfSubscribers--;
    }

    void Update()
    {
        // work out whether to self destruct
        if (GetComponent<SelfDestroyer>()) {
            if (timeSinceLostAllSubscribers >= destructionDelayAfterLosingAllSubscribers) { GetComponent<SelfDestroyer>().die(); }
            if (numberOfSubscribers <= 0){timeSinceLostAllSubscribers += Time.deltaTime;}
            else { timeSinceLostAllSubscribers = 0; }
        }
    }

}
