using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem.TextMeshPro
{

    /// <summary>
    /// This component implements IDialogueUI using TextMeshPro and Unity UI. It's based on 
    /// AbstractDialogueUI and compiles the TextMeshPro versions of controls (and Unity UI
    /// versions where TextMeshPro doesn't apply) defined in TextMeshProSubtitleControls, 
    /// UnityUIResponseMenuControls, TextMeshProAlertControls, etc.
    ///
    /// To use this component, build a UI layout (or drag a pre-built one in the Prefabs folder
    /// into your scene) and assign the UI control properties. You must assign a scene instance 
    /// to the DialogueManager; you can't use prefabs with TextMeshPro dialogue UIs.
    /// 
    /// The required controls are:
    /// - NPC subtitle line
    /// - PC subtitle line
    /// - Response menu buttons
    /// 
    /// The other control properties are optional. This component will activate and deactivate
    /// controls as they are needed in the conversation.
    /// </summary>
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3_OR_NEWER
    [HelpURL("http://pixelcrushers.com/dialogue_system/manual/html/text_mesh_pro.html")]
#endif
    [AddComponentMenu("Dialogue System/Third Party/TextMesh Pro/Dialogue/TextMesh Pro Dialogue UI")]
    public class TextMeshProDialogueUI : AbstractDialogueUI
    {

        /// <summary>
        /// The UI root.
        /// </summary>
        public UnityUIRoot unityUIRoot;

        /// <summary>
        /// The dialogue controls used in conversations.
        /// </summary>
        public TextMeshProDialogueControls dialogue;

        /// <summary>
        /// QTE (Quick Time Event) indicators.
        /// </summary>
        public UnityEngine.UI.Graphic[] qteIndicators;

        /// <summary>
        /// The alert message controls.
        /// </summary>
        public TextMeshProAlertControls alert;

        /// <summary>
        /// Wait for Hide animation to finish before progressing when player clicks continue.
        /// Doesn't wait if the next line is also the same speaker.
        /// </summary>
        [Tooltip("Wait for Hide animation to finish before progressing when player clicks continue")]
        public bool waitForHideAnimationOnContinue = true;

        /// <summary>
        /// Set <c>true</c> to always keep a control focused; useful for gamepads.
        /// </summary>
        [Tooltip("Always keep a control focused; useful for gamepads")]
        public bool autoFocus = false;

        /// <summary>
        /// Allow the dialogue UI to steal focus if a non-dialogue UI panel has it.
        /// </summary>
        [Tooltip("Allow the dialogue UI to steal focus if a non-dialogue UI panel has it.")]
        public bool allowStealFocus = false;

        /// <summary>
        /// If auto focusing, check on this frequency in seconds that the control is focused.
        /// </summary>
        [Tooltip("If auto focusing, check on this frequency in seconds that the control is focused.")]
        public float autoFocusCheckFrequency = 0.5f;

        /// <summary>
        /// Look for OverrideUnityUIDialogueControls on actors.
        /// </summary>
        [Tooltip("Look for OverrideUnityUIDialogueControls on actors")]
        public bool findActorOverrides = false;

        /// <summary>
        /// Set <c>true</c> to add an EventSystem if one isn't in the scene.
        /// </summary>
        [Tooltip("Add an EventSystem if one isn't in the scene.")]
        public bool addEventSystemIfNeeded = true;

        private UnityUIQTEControls qteControls;
        private float nextAutoFocusCheckTime = 0;
        private GameObject lastSelection = null;

        public override AbstractUIRoot UIRoot
        {
            get { return unityUIRoot; }
        }

        public override AbstractDialogueUIControls Dialogue
        {
            get { return dialogue; }
        }

        public override AbstractUIQTEControls QTEs
        {
            get { return qteControls; }
        }

        public override AbstractUIAlertControls Alert
        {
            get { return alert; }
        }

        private class QueuedAlert
        {
            public string message;
            public float duration;
            public QueuedAlert(string message, float duration)
            {
                this.message = message;
                this.duration = duration;
            }
        }

        private Queue<QueuedAlert> alertQueue = new Queue<QueuedAlert>();

        // References to the original controls in case an actor temporarily overrides them:
        private TextMeshProSubtitleControls originalNPCSubtitle;
        private TextMeshProSubtitleControls originalPCSubtitle;
        private TextMeshProResponseMenuControls originalResponseMenu;

        // Caches overrides by actor so we only need to search an actor once:
        private Dictionary<Transform, OverrideTextMeshProDialogueControls> overrideCache = new Dictionary<Transform, OverrideTextMeshProDialogueControls>();

        #region Initialization

        /// <summary>
        /// Sets up the component.
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            FindControls();
        }

#if !(UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3)
        public virtual void OnEnable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public virtual void OnDisable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }
#endif

        /// <summary>
        /// Logs warnings if any critical controls are unassigned.
        /// </summary>
        private void FindControls()
        {
            if (addEventSystemIfNeeded) UITools.RequireEventSystem();
            qteControls = new UnityUIQTEControls(qteIndicators);
            if (DialogueDebug.LogErrors)
            {
                if (DialogueDebug.LogWarnings)
                {
                    if (dialogue.npcSubtitle.line == null) Debug.LogWarning(string.Format("{0}: TextMeshProDialogueUI NPC Subtitle Line needs to be assigned.", DialogueDebug.Prefix));
                    if (dialogue.pcSubtitle.line == null) Debug.LogWarning(string.Format("{0}: TextMeshProDialogueUI PC Subtitle Line needs to be assigned.", DialogueDebug.Prefix));
                    if (dialogue.responseMenu.buttons.Length == 0 && dialogue.responseMenu.buttonTemplate == null) Debug.LogWarning(string.Format("{0}: TextMeshProDialogueUI Response buttons need to be assigned.", DialogueDebug.Prefix));
                    if (alert.line == null) Debug.LogWarning(string.Format("{0}: TextMeshProDialogueUI Alert Line needs to be assigned.", DialogueDebug.Prefix));
                }
            }
            originalNPCSubtitle = dialogue.npcSubtitle;
            originalPCSubtitle = dialogue.pcSubtitle;
            originalResponseMenu = dialogue.responseMenu;
        }

        private OverrideTextMeshProDialogueControls FindActorOverride(Transform actor)
        {
            if (actor == null) return null;
            if (!overrideCache.ContainsKey(actor))
            {
                overrideCache.Add(actor, (actor != null) ? actor.GetComponentInChildren<OverrideTextMeshProDialogueControls>() : null);
            }
            return overrideCache[actor];
        }

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3
        public void OnLevelWasLoaded(int level)
        {
            UITools.RequireEventSystem();
        }
#else
        public void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            UITools.RequireEventSystem();
        }
#endif

        public override void Open()
        {
            overrideCache.Clear();
            base.Open();
        }

        #endregion

        #region Alerts

        public override void ShowAlert(string message, float duration)
        {
            if (alert.queueAlerts)
            {
                alertQueue.Enqueue(new QueuedAlert(message, duration));
                if (!alert.IsVisible) ShowNextQueuedAlert();
            }
            else
            {
                StartShowingAlert(message, duration);
            }
        }

        public override void HideAlert()
        {
            base.HideAlert();
            if (alert.queueAlerts) ShowNextQueuedAlert();
        }

        protected void ShowNextQueuedAlert()
        {
            if (alertQueue.Count > 0)
            {
                var queuedAlert = alertQueue.Dequeue();
                StartShowingAlert(queuedAlert.message, queuedAlert.duration);
            }
        }

        protected void StartShowingAlert(string message, float duration)
        {
            base.ShowAlert(message, duration);
            if (autoFocus) alert.AutoFocus();
            CancelInvoke("HideAlert");
            Invoke("HideAlert", duration);
        }

        #endregion

        #region Subtitles

        protected TextMeshProSubtitleControls currentSubtitleControls = null;

        public override void ShowSubtitle(Subtitle subtitle)
        {
            if (findActorOverrides)
            {
                var overrideControls = FindActorOverride(subtitle.speakerInfo.transform);
                if (subtitle.speakerInfo.characterType == CharacterType.NPC)
                {
                    dialogue.npcSubtitle = (overrideControls != null) ? overrideControls.subtitle : originalNPCSubtitle;
                }
                else
                {
                    dialogue.pcSubtitle = (overrideControls != null) ? overrideControls.subtitle : originalPCSubtitle;
                }
            }
            currentSubtitleControls = (subtitle.speakerInfo.characterType == CharacterType.NPC) ? dialogue.npcSubtitle : dialogue.pcSubtitle;
            HideResponses();
            base.ShowSubtitle(subtitle);
            CheckSubtitleAutoFocus(subtitle);
        }

        public override void HideSubtitle(Subtitle subtitle)
        {
            base.HideSubtitle(subtitle);
            currentSubtitleControls = null;
        }

        public void CheckSubtitleAutoFocus(Subtitle subtitle)
        {
            if (autoFocus)
            {
                if (subtitle.speakerInfo.IsPlayer)
                {
                    dialogue.pcSubtitle.AutoFocus(allowStealFocus);
                }
                else
                {
                    dialogue.npcSubtitle.AutoFocus(allowStealFocus);
                }
            }
        }

        public override void OnContinue()
        {
            CancelInvoke("HideAlert");
            if (IsOpen && waitForHideAnimationOnContinue)
            {
                StartCoroutine(WaitForHideThenContinue());
            }
            else
            {
                base.OnContinue();
            }
        }

        protected IEnumerator WaitForHideThenContinue()
        {
            var areCurrentSubtitleControlsValid = (currentSubtitleControls != null && currentSubtitleControls.showHideController != null);
            var state = DialogueManager.CurrentConversationState;
            var isNextSpeakerDifferent = state.subtitle.speakerInfo.IsNPC ?
                ((state.HasNPCResponse && state.subtitle.speakerInfo.id != state.FirstNPCResponse.destinationEntry.ActorID) || (!state.HasNPCResponse && state.HasPCResponses))
                : state.HasNPCResponse;
            if (!state.HasAnyResponses) isNextSpeakerDifferent = true; // Next "speaker" is no one, which is different from actual speakers.
            if (areCurrentSubtitleControlsValid)
            {
                currentSubtitleControls.skipAnimation = !isNextSpeakerDifferent;
                if (isNextSpeakerDifferent)
                {
                    currentSubtitleControls.skipAnimation = false;
                    currentSubtitleControls.Hide();
                    var timeout = Time.realtimeSinceStartup + 10f;
                    while (currentSubtitleControls.line.gameObject.activeInHierarchy && Time.realtimeSinceStartup < timeout)
                    {
                        yield return null;
                    }
                }
            }
            base.OnContinue();
        }

        #endregion

        #region Responses

        public override void ShowResponses(Subtitle subtitle, Response[] responses, float timeout)
        {
            if (findActorOverrides)
            {
                // Use speaker's (NPC's) world space canvas for subtitle reminder, and for menu if set:
                var overrideControls = FindActorOverride(subtitle.speakerInfo.transform);
                var subtitleReminder = (overrideControls != null) ? overrideControls.subtitleReminder : originalResponseMenu.subtitleReminder;
                if (overrideControls != null && overrideControls.responseMenu.panel != null)
                {
                    dialogue.responseMenu = (overrideControls != null && overrideControls.responseMenu.panel != null) ? overrideControls.responseMenu : originalResponseMenu;
                }
                else
                {
                    // Otherwise use PC's world space canvas for menu if set:
                    overrideControls = FindActorOverride(subtitle.listenerInfo.transform);
                    dialogue.responseMenu = (overrideControls != null && overrideControls.responseMenu.panel != null) ? overrideControls.responseMenu : originalResponseMenu;
                }
                // Either way, use speaker's (NPC's) subtitle reminder:
                dialogue.responseMenu.subtitleReminder = subtitleReminder;
            }
            base.ShowResponses(subtitle, responses, timeout);
            ClearSelection();
            CheckResponseMenuAutoFocus();
        }

        public void CheckResponseMenuAutoFocus()
        {
            if (autoFocus) dialogue.responseMenu.AutoFocus();
        }

        public override void HideResponses()
        {
            dialogue.responseMenu.DestroyInstantiatedButtons();
            base.HideResponses();
        }

        public void ClearSelection()
        {
            if (autoFocus)
            {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
                lastSelection = null;
            }
        }

        public override void Update()
        {
            base.Update();

            // Check alert queue:
            if (alertQueue.Count > 0 && alert.queueAlerts && !alert.IsVisible && !(alert.waitForHideAnimation && alert.IsHiding))
            {
                ShowNextQueuedAlert();
            }

            // Auto focus dialogue:
            if (autoFocus && IsOpen)
            {
                if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != null)
                {
                    lastSelection = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
                }
                if (autoFocusCheckFrequency > 0.001f && Time.realtimeSinceStartup > nextAutoFocusCheckTime)
                {
                    nextAutoFocusCheckTime = Time.realtimeSinceStartup + autoFocusCheckFrequency;
                    if (dialogue.responseMenu.panel != null && dialogue.responseMenu.panel.gameObject.activeInHierarchy)
                    {
                        dialogue.responseMenu.AutoFocus(lastSelection, allowStealFocus);
                    }
                    else if (dialogue.pcSubtitle.panel != null && dialogue.pcSubtitle.panel.gameObject.activeInHierarchy)
                    {
                        dialogue.pcSubtitle.AutoFocus(allowStealFocus);
                    }
                    else
                    {
                        dialogue.npcSubtitle.AutoFocus(allowStealFocus);
                    }
                }
            }
        }

        #endregion

    }

}