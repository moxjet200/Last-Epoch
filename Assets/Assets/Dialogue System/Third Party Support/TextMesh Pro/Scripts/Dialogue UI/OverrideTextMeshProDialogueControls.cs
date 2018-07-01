using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem.TextMeshPro
{

    /// <summary>
    /// This component allows actors to override TextMesh Pro dialogue controls. It's 
    /// particularly useful to assign world space UIs such as speech bubbles above
    /// actors' heads.
    /// </summary>
    [AddComponentMenu("Dialogue System/Third Party/TextMesh Pro/Dialogue/Override TextMesh Pro Dialogue Controls")]
    public class OverrideTextMeshProDialogueControls : MonoBehaviour
    {

        [Tooltip("Use these controls when playing subtitles through this actor")]
        public TextMeshProSubtitleControls subtitle;

        [Tooltip("Use these controls when showing subtitle reminders for actor")]
        public TextMeshProSubtitleControls subtitleReminder;

        [Tooltip("Use these controls when showing a response menu involving this actor")]
        public TextMeshProResponseMenuControls responseMenu;

        public virtual void Start()
        {
            if (subtitle != null) subtitle.SetActive(false);
            if (subtitleReminder != null) subtitleReminder.SetActive(false);
            if (responseMenu != null) responseMenu.SetActive(false);
        }

    }

}
