using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// used to locate cast points in the world
public class AmbientCastPointIndicator : MonoBehaviour
{
    public static List<AmbientCastPointIndicator> all = new List<AmbientCastPointIndicator>();

    void Awake()
    {
        all.Add(this);
    }

    void OnDestroy()
    {
        all.Remove(this);
    }


}
