using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityObjectIndicator : MonoBehaviour {

    // This should be on every ability object and is used to prevent ability objects from acting on collisions with each other e.g. it means that two fireballs don't interact when they collide

    public static List<AbilityObjectIndicator> all = new List<AbilityObjectIndicator>();

    void Start()
    {
        all.Add(this);
    }

    void OnDestroy()
    {
        all.Remove(this);
    }

}
