using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class repeatedlyPlaySound : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string sound = "";
    public float interval = 1f;
    float index = 0;
    string empty = "";

    void Update()
    {
        if (index > interval)
        {
            index -= interval;
            if (sound != empty)
            {
                RuntimeManager.PlayOneShot(sound, transform.position);
            }
        }

        index += Time.deltaTime;
    }

}
