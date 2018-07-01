#if !(UNITY_4_3 || UNITY_4_5)
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem.TextMeshPro
{

    /// <summary>
    /// Subtitle Unity UI controls for UnityUIDialogueUI.
    /// </summary>
    [System.Serializable]
    public class TextMeshProSubtitleControls : AbstractUISubtitleControls
    {

        /// <summary>
        /// The panel containing the response menu controls. A panel is optional, but you may want one
        /// so you can include a background image, panel-wide effects, etc.
        /// </summary>
        [Tooltip("Optional panel for the subtitle elements")]
        public UnityEngine.UI.Graphic panel;

        /// <summary>
        /// The label that will show the text of the subtitle.
        /// </summary>
        [Tooltip("Subtitle text")]
        public TMPro.TextMeshProUGUI line;

        /// <summary>
        /// The label that will show the portrait image.
        /// </summary>
        [Tooltip("Optional image for speaker's portrait")]
        public UnityEngine.UI.Image portraitImage;

        /// <summary>
        /// The label that will show the name of the speaker.
        /// </summary>
        [Tooltip("Optional label for speaker's name")]
        public TMPro.TextMeshProUGUI portraitName;

        /// <summary>
        /// The continue button. This is only required if DisplaySettings.waitForContinueButton 
        /// is <c>true</c> -- in which case this button should send "OnContinue" to the UI when clicked.
        /// </summary>
        [Tooltip("Optional continue button; configure OnClick to invoke dialogue UI's OnContinue method")]
        public UnityEngine.UI.Button continueButton;

        [Tooltip("Ignore RPGMaker-style pause codes")]
        public bool ignorePauseCodes = false;

        [Serializable]
        public class AnimationTransitions
        {
            public string showTrigger = string.Empty;
            public string hideTrigger = string.Empty;
        }

        [Tooltip("Optional animation transitions; panel should have an Animator")]
        public AnimationTransitions animationTransitions = new AnimationTransitions();

        [Tooltip("Never hide this subtitle panel")]
        public bool alwaysVisible = false;

        [HideInInspector]
        public bool skipAnimation = false;

        private bool haveSavedOriginalColor = false;
        private Color originalColor = Color.white;

        [HideInInspector]
        public UIShowHideController showHideController = null;

        private TextMeshProDialogueUI dialogueUI = null;

        /// <summary>
        /// Indicates whether this subtitle is currently assigned text.
        /// </summary>
        /// <value>
        /// <c>true</c> if it has text; otherwise, <c>false</c>.
        /// </value>
        public override bool HasText
        {
            get { return (line != null) && !string.IsNullOrEmpty(line.text); }
        }

        public override void SetActive(bool value)
        {
            if (value == true || alwaysVisible)
            {
                ShowPanel();
            }
            else
            {
                HidePanel();
            }
        }

        private void ShowPanel()
        {
            ShowControls();
            CheckShowHideController();
            showHideController.Show(animationTransitions.showTrigger, false, null);
        }

        private void HidePanel()
        {
            CheckShowHideController();
            showHideController.Hide(animationTransitions.hideTrigger, HideControls);
        }

        private void CheckShowHideController()
        {
            if (showHideController == null)
            {
                showHideController = new UIShowHideController(null, panel);
            }
        }

        private void ShowControls()
        {
            Tools.SetGameObjectActive(line, true);
            Tools.SetGameObjectActive(portraitImage, true);
            Tools.SetGameObjectActive(portraitName, true);
            Tools.SetGameObjectActive(continueButton, true);
        }

        private void HideControls()
        {
            Tools.SetGameObjectActive(line, alwaysVisible);
            Tools.SetGameObjectActive(portraitImage, alwaysVisible);
            Tools.SetGameObjectActive(portraitName, alwaysVisible);
            Tools.SetGameObjectActive(continueButton, alwaysVisible);
        }

        public override void ShowContinueButton()
        {
            Tools.SetGameObjectActive(continueButton, true);
        }

        public override void HideContinueButton()
        {
            Tools.SetGameObjectActive(continueButton, false);
        }

        /// <summary>
        /// Sets the subtitle.
        /// </summary>
        /// <param name='subtitle'>
        /// Subtitle.
        /// </param>
        public override void SetSubtitle(Subtitle subtitle)
        {
            if ((subtitle != null) && !string.IsNullOrEmpty(subtitle.formattedText.text))
            {
                if (portraitImage != null) portraitImage.sprite = UITools.CreateSprite(subtitle.GetSpeakerPortrait());
                if (portraitName != null)
                {
                    portraitName.text = subtitle.speakerInfo.Name;
                    TextMeshProSendTextChangeMessage(portraitName);
                }
                if (line != null)
                {
                    var typewriterEffect = line.GetComponent<TextMeshProTypewriterEffect>();
                    if (typewriterEffect != null && typewriterEffect.enabled) typewriterEffect.Stop();
                    SetFormattedText(line, subtitle.formattedText);
                    if (typewriterEffect != null && typewriterEffect.enabled) typewriterEffect.OnEnable();
                }
                Show();
                if (alwaysVisible && line != null)
                {
                    var typewriterEffect = line.GetComponent<TextMeshProTypewriterEffect>();
                    if (typewriterEffect != null) typewriterEffect.OnEnable();
                }
            }
            else
            {
                if ((line != null) && (subtitle != null)) SetFormattedText(line, subtitle.formattedText);
                Hide();
            }
        }

        /// <summary>
        /// Clears the subtitle.
        /// </summary>
        public override void ClearSubtitle()
        {
            SetFormattedText(line, null);
        }

        /// <summary>
        /// Sets a label with formatted text.
        /// </summary>
        /// <param name='label'>
        /// Label to set.
        /// </param>
        /// <param name='formattedText'>
        /// Formatted text.
        /// </param>
        private void SetFormattedText(TMPro.TextMeshProUGUI label, FormattedText formattedText)
        {
            if (label != null)
            {
                if (formattedText != null)
                {
                    var text = UITools.GetUIFormattedText(formattedText);
                    if (ignorePauseCodes) text = UnityUITypewriterEffect.StripRPGMakerCodes(text);
                    label.text = text;
                    TextMeshProSendTextChangeMessage(label);
                    if (!haveSavedOriginalColor)
                    {
                        originalColor = label.color;
                        haveSavedOriginalColor = true;
                    }
                    label.color = (formattedText.emphases.Length > 0) ? formattedText.emphases[0].color : originalColor;
                }
                else
                {
                    label.text = string.Empty;
                }
            }
        }

        public void TextMeshProSendTextChangeMessage(TMPro.TextMeshProUGUI text)
        {
            if (text == null) return;
            if (dialogueUI == null) dialogueUI = text.GetComponentInParent<TextMeshProDialogueUI>();
            if (dialogueUI == null) return;
            dialogueUI.SendMessage("OnTextChange", text, SendMessageOptions.DontRequireReceiver);
        }

        /// <summary>
        /// Sets the portrait texture to use in the subtitle if the named actor is the speaker.
        /// This is used to immediately update the GUI control if the SetPortrait() sequencer 
        /// command changes the portrait texture.
        /// </summary>
        /// <param name="actorName">Actor name in database.</param>
        /// <param name="portraitTexture">Portrait texture.</param>
        public override void SetActorPortraitTexture(string actorName, Texture2D portraitTexture)
        {
            if ((currentSubtitle != null) && string.Equals(currentSubtitle.speakerInfo.nameInDatabase, actorName))
            {
                if (portraitImage != null) portraitImage.sprite = UITools.CreateSprite(AbstractDialogueUI.GetValidPortraitTexture(actorName, portraitTexture));  //---Was: .texture = AbstractDialogueUI.GetValidPortraitTexture(actorName, portraitTexture);
            }
        }

        /// <summary>
        /// Auto-focuses the continue button. Useful for gamepads.
        /// </summary>
        public void AutoFocus(bool allowStealFocus = true)
        {
            if (continueButton != null && allowStealFocus)
            {
                continueButton.Select();
                continueButton.OnSelect(null);
            }
        }

    }

}
#endif