using UnityEngine;
using System;
using System.Collections;

namespace PixelCrushers.DialogueSystem.TextMeshPro
{

    /// <summary>
    /// A TextMeshPro/Unity UI response button for use with TextMeshProDialogueControls. 
    /// Add this component to every response button in the dialogue UI.
    /// </summary>
    [AddComponentMenu("Dialogue System/Third Party/TextMesh Pro/Dialogue/TextMesh Pro Response Button")]
    public class TextMeshProResponseButton : MonoBehaviour
    {

        /// <summary>
        /// The UnityUI button.
        /// </summary>
        [Tooltip("Response button.")]
        public UnityEngine.UI.Button button;

        /// <summary>
        /// The UnityUI label that will display the response text.
        /// </summary>
        [Tooltip("Response menu text.")]
        public TMPro.TextMeshProUGUI label;

        /// <summary>
        /// The default color for response text.
        /// </summary>
        [Tooltip("Default color for response menu text.")]
        public Color defaultColor = Color.white;

        /// <summary>
        /// Set <c>true</c> to set the button color when applying emphasis tags.
        /// </summary>
        [Tooltip("Set button color when applying emphasis tags.")]
        public bool setButtonColor = true;

        /// <summary>
        /// Set <c>true</c> to set the label color when applying emphasis tags.
        /// </summary>
        [Tooltip("Set label color when applying emphasis tags.")]
        public bool setLabelColor = true;

        /// <summary>
        /// Gets or sets the response text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text
        {
            get
            {
                return (label != null) ? label.text : string.Empty;
            }
            set
            {
                if (label != null)
                {
                    label.text = value;
                }
                else
                {
                    if (DialogueDebug.LogErrors) Debug.LogError(string.Format("{0}: No TextMeshPro UI element is unassigned on {1}", new object[] { DialogueDebug.Prefix, name }));
                }
            }
        }

        /// <summary>
        /// Indicates whether the button is an allowable response.
        /// </summary>
        /// <value>
        /// <c>true</c> if clickable; otherwise, <c>false</c>.
        /// </value>
        public bool clickable
        {
            get { return (button != null) && button.interactable; }
            set { if (button != null) button.interactable = value; }
        }

        /// <summary>
        /// Indicates whether the button is shown or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if visible; otherwise, <c>false</c>.
        /// </value>
        public bool visible { get; set; }

        /// <summary>
        /// Gets or sets the response associated with this button. If the player clicks this 
        /// button, this response is sent back to the dialogue system.
        /// </summary>
        /// <value>
        /// The response.
        /// </value>
        public Response response { get; set; }

        /// <summary>
        /// Gets or sets the target that will receive click notifications.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        public Transform target { get; set; }


        /// <summary>
        /// Clears the button.
        /// </summary>
        public void Reset()
        {
            Text = string.Empty;
            clickable = false;
            visible = false;
            response = null;
            SetColor(defaultColor);
        }

        /// <summary>
        /// Sets the button's text using the specified formatted text.
        /// </summary>
        /// <param name='formattedText'>
        /// The formatted text for the button label.
        /// </param>
        public void SetFormattedText(FormattedText formattedText)
        {
            if (formattedText != null)
            {
                Text = UITools.GetUIFormattedText(formattedText);
                SetColor((formattedText.emphases.Length > 0) ? formattedText.emphases[0].color : defaultColor);
            }
        }

        /// <summary>
        /// Sets the button's text using plain text.
        /// </summary>
        /// <param name='unformattedText'>
        /// Unformatted text for the button label.
        /// </param>
        public void SetUnformattedText(string unformattedText)
        {
            Text = unformattedText;
            SetColor(defaultColor);
        }

        protected virtual void SetColor(Color currentColor)
        {
            if (button != null)
            {
                //if (setButtonColor) button.defaultColor = currentColor;
            }
            else
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning(string.Format("{0}: No Button is assigned to {1}", new object[] { DialogueDebug.Prefix, name }));
            }
            if (label != null)
            {
                if (setLabelColor) label.color = currentColor;
            }
            else
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning(string.Format("{0}: No Text is assigned to {1}", new object[] { DialogueDebug.Prefix, name }));
            }
        }

        /// <summary>
        /// Handles a button click by calling the response handler.
        /// </summary>
        public void OnClick()
        {
            if (target != null) target.SendMessage("OnClick", response, SendMessageOptions.RequireReceiver);
        }

    }

}