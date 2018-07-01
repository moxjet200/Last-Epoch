using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOneShotSound : MonoBehaviour
{

    [System.Serializable]
    public enum PlayEvent
    {
        start, destroy, none
    } 

    public PlayEvent playEvent = PlayEvent.start;
    [FMODUnity.EventRef]
    public string sound = "";

    [HideInInspector]
    public bool active = true;

    void Start()
    {
        if (active && playEvent == PlayEvent.start)
        {
            playSound();
        }
    }

    void OnDestroy()
    {
        if (active && playEvent == PlayEvent.destroy)
        {
            playSound();
        }
    }

    public void playSound()
    {
        OneShotSoundManager.playSoundAtLocation(sound, transform.position);
    }

}
