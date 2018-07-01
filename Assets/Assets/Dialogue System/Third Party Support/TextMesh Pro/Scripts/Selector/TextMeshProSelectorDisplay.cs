using UnityEngine;
using UnityEngine.UI;
using System;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem.TextMeshPro
{

    /// <summary>
    /// Replaces the Selector/ProximitySelector's OnGUI method with a method
    /// that enables or disables new Unity UI and TextMeshPro controls.
    /// </summary>
    [AddComponentMenu("Dialogue System/Third Party/TextMesh Pro/Selector/TextMesh Pro Selector Display")]
    public class TextMeshProSelectorDisplay : MonoBehaviour
    {

        /// <summary>
        /// The main graphic (optional). Assign this if you have created an entire 
        /// panel for the selector.
        /// </summary>
        [Tooltip("Optional main graphic/panel for selector.")]
        public Graphic mainGraphic = null;

        /// <summary>
        /// The text for the name of the current selection.
        /// </summary>
        [Tooltip("Name of current selection.")]
        public TMPro.TextMeshProUGUI nameText = null;

        /// <summary>
        /// The text for the use message (e.g., "Press spacebar to use").
        /// </summary>
        [Tooltip("'Use' message for current selection.")]
        public TMPro.TextMeshProUGUI useMessageText = null;

        [Tooltip("Color when user is in range.")]
        public Color inRangeColor = Color.yellow;

        [Tooltip("Color when user is out of range.")]
        public Color outOfRangeColor = Color.gray;

        /// <summary>
        /// The graphic to show if the selection is in range.
        /// </summary>
        [Tooltip("Show if user is in range.")]
        public Graphic reticleInRange = null;

        /// <summary>
        /// The graphic to show if the selection is out of range.
        /// </summary>
        [Tooltip("Show if user is out of range.")]
        public Graphic reticleOutOfRange = null;

        [Serializable]
        public class AnimationTransitions
        {
            public string showTrigger = "Show";
            public string hideTrigger = "Hide";
        }

        public AnimationTransitions animationTransitions = new AnimationTransitions();

        private Selector selector = null;

        private ProximitySelector proximitySelector = null;

        private string defaultUseMessage = string.Empty;

        private Usable usable = null;

        private bool lastInRange = false;

        private UsableUnityUI usableUnityUI = null;

        private Animator animator = null;

        protected float CurrentDistance
        {
            get
            {
                return (selector != null) ? selector.CurrentDistance : 0;
            }
        }

        public void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void Start()
        {
            ConnectDelegates();
            DeactivateControls();
        }

        public void OnEnable()
        {
            ConnectDelegates();
        }

        public void OnDisable()
        {
            DisconnectDelegates();
        }

        private void ConnectDelegates()
        {
            DisconnectDelegates(); // Make sure we're not connecting twice.
            selector = GetComponent<Selector>();
            if (selector != null)
            {
                selector.useDefaultGUI = false;
                selector.SelectedUsableObject += OnSelectedUsable;
                selector.DeselectedUsableObject += OnDeselectedUsable;
                defaultUseMessage = selector.defaultUseMessage;
            }
            proximitySelector = GetComponent<ProximitySelector>();
            if (proximitySelector != null)
            {
                proximitySelector.useDefaultGUI = false;
                proximitySelector.SelectedUsableObject += OnSelectedUsable;
                proximitySelector.DeselectedUsableObject += OnDeselectedUsable;
                if (string.IsNullOrEmpty(defaultUseMessage)) defaultUseMessage = proximitySelector.defaultUseMessage;
            }
        }

        private void DisconnectDelegates()
        {
            selector = GetComponent<Selector>();
            if (selector != null)
            {
                selector.useDefaultGUI = true;
                selector.SelectedUsableObject -= OnSelectedUsable;
                selector.DeselectedUsableObject -= OnDeselectedUsable;
            }
            proximitySelector = GetComponent<ProximitySelector>();
            if (proximitySelector != null)
            {
                proximitySelector.useDefaultGUI = true;
                proximitySelector.SelectedUsableObject -= OnSelectedUsable;
                proximitySelector.DeselectedUsableObject -= OnDeselectedUsable;
            }
            HideControls();
        }

        private void OnSelectedUsable(Usable usable)
        {
            this.usable = usable;
            usableUnityUI = (usable != null) ? usable.GetComponentInChildren<UsableUnityUI>() : null;
            if (usableUnityUI != null)
            {
                usableUnityUI.Show(GetUseMessage());
            }
            else
            {
                ShowControls();
            }
            lastInRange = !IsUsableInRange();
            UpdateDisplay(!lastInRange);
        }

        private void OnDeselectedUsable(Usable usable)
        {
            if (usableUnityUI != null)
            {
                usableUnityUI.Hide();
                usableUnityUI = null;
            }
            else
            {
                HideControls();
            }
            this.usable = null;
        }

        private string GetUseMessage()
        {
            return string.IsNullOrEmpty(usable.overrideUseMessage) ? defaultUseMessage : usable.overrideUseMessage;
        }

        private void ShowControls()
        {
            if (usable == null) return;
            Tools.SetGameObjectActive(mainGraphic, true);
            Tools.SetGameObjectActive(nameText, true);
            Tools.SetGameObjectActive(useMessageText, true);
            if (nameText != null) nameText.text = usable.GetName();
            if (useMessageText != null) useMessageText.text = GetUseMessage();
            if (CanTriggerAnimations() && !string.IsNullOrEmpty(animationTransitions.showTrigger))
            {
                animator.SetTrigger(animationTransitions.showTrigger);
            }
        }

        private void HideControls()
        {
            if (CanTriggerAnimations() && !string.IsNullOrEmpty(animationTransitions.hideTrigger))
            {
                animator.SetTrigger(animationTransitions.hideTrigger);
            }
            else
            {
                DeactivateControls();
            }
        }

        private void DeactivateControls()
        {
            Tools.SetGameObjectActive(nameText, false);
            Tools.SetGameObjectActive(useMessageText, false);
            Tools.SetGameObjectActive(reticleInRange, false);
            Tools.SetGameObjectActive(reticleOutOfRange, false);
            Tools.SetGameObjectActive(mainGraphic, false);
        }

        private bool IsUsableInRange()
        {
            return (usable != null) && (CurrentDistance <= usable.maxUseDistance);
        }

        public void Update()
        {
            if (usable != null)
            {
                UpdateDisplay(IsUsableInRange());
            }
            else
            {
                HideControls();
            }
        }

        public void OnConversationStart(Transform actor)
        {
            HideControls();
        }

        public void OnConversationEnd(Transform actor)
        {
            ShowControls();
        }

        private void UpdateDisplay(bool inRange)
        {
            if ((usable != null) && (inRange != lastInRange))
            {
                lastInRange = inRange;
                if (usableUnityUI != null)
                {
                    usableUnityUI.UpdateDisplay(inRange);
                }
                else
                {
                    UpdateText(inRange);
                    UpdateReticle(inRange);
                }
            }
        }

        private void UpdateText(bool inRange)
        {
            Color color = inRange ? inRangeColor : outOfRangeColor;
            if (nameText != null) nameText.color = color;
            if (useMessageText != null) useMessageText.color = color;
        }

        private void UpdateReticle(bool inRange)
        {
            Tools.SetGameObjectActive(reticleInRange, inRange);
            Tools.SetGameObjectActive(reticleOutOfRange, !inRange);
        }

        private bool CanTriggerAnimations()
        {
            return (animator != null) && (animationTransitions != null);
        }

    }

}