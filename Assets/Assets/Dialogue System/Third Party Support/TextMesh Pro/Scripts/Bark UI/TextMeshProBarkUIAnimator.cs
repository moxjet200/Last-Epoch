using UnityEngine;
using System;
using System.Collections;

namespace PixelCrushers.DialogueSystem.TextMeshPro
{

    /// <summary>
    /// Adds animator control to a TextMeshProBarkUI.
    /// </summary>
    [AddComponentMenu("Dialogue System/Third Party/TextMesh Pro/TextMesh Pro Bark UI Animator")]
    [RequireComponent(typeof(TextMeshProBarkUI))]
    public class TextMeshProBarkUIAnimator : MonoBehaviour
    {

        [Serializable]
        public class AnimationTransitions
        {
            public string showTrigger = "Show";
            public string hideTrigger = "Hide";
        }

        [Tooltip("Optional animation transitions; panel should have an Animator")]
        public AnimationTransitions animationTransitions = new AnimationTransitions();

        private TextMeshProBarkUI barkUI = null;
        private UIShowHideController showHideController = null;
        private bool isVisible = false;

        public void Start()
        {
            barkUI = GetComponent<TextMeshProBarkUI>();
            if (barkUI == null || barkUI.textMeshPro == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning(string.Format("{0}: No TextMeshProBarkUI found on '{1}'. Not connecting TextMeshProBarkUIAnimator.", 
                    new object[] { DialogueDebug.Prefix, name }), this);
            }
            else
            {
                barkUI.ShowedBark += OnShowedBark;
                barkUI.HidBark += OnHidBark;
            }
        }

        public void OnShowedBark(TextMeshProBarkUI barkUI)
        {
            if (barkUI == null) return;
            CheckShowHideController();
            showHideController.ClearTrigger(animationTransitions.hideTrigger);
            showHideController.Show(animationTransitions.showTrigger, false, null);
            isVisible = true;
        }

        public void OnHidBark(TextMeshProBarkUI barkUI)
        {
            if (barkUI == null) return;
            ShowControls();
            CheckShowHideController();
            showHideController.ClearTrigger(animationTransitions.showTrigger);
            if (isVisible)
            {
                showHideController.Hide(animationTransitions.hideTrigger, HideControls);
            }
            else
            {
                HideControls();
            }
        }

        private void CheckShowHideController()
        {
            if (showHideController == null)
            {
                showHideController = new UIShowHideController(barkUI.textMeshPro.gameObject, null);
            }
        }

        private void ShowControls()
        {
            if (barkUI != null) Tools.SetGameObjectActive(barkUI.textMeshPro, true);
        }

        private void HideControls()
        {
            if (barkUI != null) Tools.SetGameObjectActive(barkUI.textMeshPro, false);
        }

    }

}