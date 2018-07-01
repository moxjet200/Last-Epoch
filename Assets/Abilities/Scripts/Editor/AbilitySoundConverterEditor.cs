using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FMODUnity;

[CustomEditor(typeof(AbilitySoundConverter))]
public class AbilitySoundConverterEditor : Editor
{
    bool done = false;

    public override void OnInspectorGUI()
    {
        if (done) { return; }

        GUILayout.Label("Processing...", EditorStyles.boldLabel);

        var tar = target as AbilitySoundConverter;

        if (!tar) { return; }

        GameObject go = tar.gameObject;

        StudioEventEmitter[] emitters = tar.GetComponents<StudioEventEmitter>();
        List<StudioEventEmitter> keepList = new List<StudioEventEmitter>();

        foreach (StudioEventEmitter see in emitters)
        {
            if (see.PlayEvent == EmitterGameEvent.ObjectStart && see.StopEvent == EmitterGameEvent.ObjectDestroy)
            {
                PlaySoundDuringLifetime component = go.AddComponent<PlaySoundDuringLifetime>();
                component.Event = see.Event;
            }
            else if (see.PlayEvent == EmitterGameEvent.ObjectStart && see.StopEvent == EmitterGameEvent.None)
            {
                PlayOneShotSound component = go.AddComponent<PlayOneShotSound>();
                component.sound = see.Event;
                component.playEvent = PlayOneShotSound.PlayEvent.start;
            }
            else if (see.PlayEvent == EmitterGameEvent.ObjectDestroy && see.StopEvent == EmitterGameEvent.None)
            {
                PlayOneShotSound component = go.AddComponent<PlayOneShotSound>();
                component.sound = see.Event;
                component.playEvent = PlayOneShotSound.PlayEvent.destroy;
            }
            else if (!(see.PlayEvent == EmitterGameEvent.None && see.StopEvent == EmitterGameEvent.None))
            {
                Debug.Log(go.name + " has an irregular studio event emitter, with events " + see.PlayEvent.ToString() + " and " + see.StopEvent.ToString());
                keepList.Add(see);
            }
        }

        for (int i = emitters.Length -1 ; i >= 0 ; i--)
        {
            if (!keepList.Contains(emitters[i]))
            {
                DestroyImmediate(emitters[i], true);
            }
        }

        done = true;

        DestroyImmediate(tar, true);
        
    }
}