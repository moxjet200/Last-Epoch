using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem.TextMeshPro
{

    public delegate void BarkUIDelegate(TextMeshProBarkUI barkUI);

    /// <summary>
    /// Implements IBarkUI using TextMeshPro. To use this component, add it to a character. 
    /// If the character has more than one TextMeshPro in its hierarchy, manually assign the 
    /// Text Mesh Pro field with the correct one.
    /// </summary>
    [AddComponentMenu("Dialogue System/Third Party/TextMesh Pro/TextMesh Pro Bark UI")]
    public class TextMeshProBarkUI : MonoBehaviour, IBarkUI
    {

        /// <summary>
        /// The TextMeshPro to use.
        /// </summary>
        [Tooltip("The TextMeshPro to use for the bark text. (Standard, non-Unity UI canvas version)")]
        public TMPro.TextMeshPro textMeshPro;

        /// <summary>
        /// The TextMeshPro to use.
        /// </summary>
        [Tooltip("The TextMeshPro to use for the bark text. (Unity UI canvas version)")]
        public TMPro.TextMeshProUGUI textMeshProUGUI;

        [Tooltip("Optional UI panel containing the TextMeshProUGUI component.")]
        public RectTransform panel;

        /// <summary>
        /// Set <c>true</c> to include the barker's name in the text.
        /// </summary>
        [Tooltip("Include barker's name in the bark text.")]
        public bool includeName = false;

        /// <summary>
        /// The duration in seconds to show the bark text before fading it out.
        /// </summary>
        [Tooltip("Show the bark text for this many seconds.")]
        public float duration = 6f;

        /// <summary>
        /// Set <c>true</c> to keep the bark text onscreen until the sequence ends.
        /// </summary>
        [Tooltip("Keep the bark text onscreen until the bark's sequence ends.")]
        public bool waitUntilSequenceEnds;

        /// <summary>
        /// Set <c>true</c> to run a raycast to the player. If the ray is blocked (e.g., a wall
        /// blocks visibility to the player), don't show the bark.
        /// </summary>
        [Tooltip("Temporarily hide the bark if the player's view of the barker is blocked.")]
        public bool checkIfPlayerVisible = true;

        /// <summary>
        /// The layer mask to use when checking for player visibility.
        /// </summary>
        [Tooltip("The layer mask to use when checking for player visibility.")]
        public LayerMask visibilityLayerMask = 1;

        /// <summary>
        /// Occurs when showing a bark. You can hook in to run a coroutine to do something
        /// special with the TextMeshPro.
        /// </summary>
        public event BarkUIDelegate ShowedBark = delegate { };

        /// <summary>
        /// Occurs when hiding a bark. You can hook in to run a coroutine to do something
        /// special with the TextMeshPro.
        /// </summary>
        public event BarkUIDelegate HidBark = delegate { };

        /// <summary>
        /// The seconds left to display the current bark.
        /// </summary>
        private float secondsLeft = 0f;

        private Transform playerCameraTransform = null;

        private Collider playerCameraCollider = null;


        public void Awake()
        {
            if (textMeshPro == null && textMeshProUGUI == null)
            {
                textMeshPro = GetComponentInChildren<TMPro.TextMeshPro>();
                if (textMeshPro == null) textMeshProUGUI = GetComponentInChildren<TMPro.TextMeshProUGUI>();
            }
            if (textMeshPro == null && textMeshProUGUI == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Assign a TextMeshPro component to the TextMeshProBarkUI on " + name, this);
                enabled = false;
            }
        }

        public void Start()
        {
            SetActive(false);
        }

        private void SetActive(bool value)
        {
            if (textMeshPro != null) Tools.SetGameObjectActive(textMeshPro, value);
            if (textMeshProUGUI != null) Tools.SetGameObjectActive(textMeshProUGUI, value);
            if (panel != null) Tools.SetGameObjectActive(panel, value);
        }

        /// <summary>
        /// Updates the seconds left and hides the label if time is up.
        /// </summary>
        public void Update()
        {
            if (secondsLeft > 0)
            {
                secondsLeft -= Time.deltaTime;
                if (checkIfPlayerVisible) CheckPlayerVisibility();
                if ((secondsLeft <= 0) && !waitUntilSequenceEnds) Hide();
            }
        }

        /// <summary>
        /// Barks the specified subtitle.
        /// </summary>
        /// <param name='subtitle'>
        /// Subtitle to bark.
        /// </param>
        public void Bark(Subtitle subtitle)
        {
            if (textMeshPro == null && textMeshProUGUI == null) return;
            if (includeName)
            {
                var text = string.Format("{0}: {1}", subtitle.speakerInfo.Name, subtitle.formattedText.text);
                if (textMeshPro != null) textMeshPro.text = text;
                if (textMeshProUGUI != null) textMeshProUGUI.text = text;
            }
            else
            {
                if (textMeshPro != null) textMeshPro.text = subtitle.formattedText.text;
                if (textMeshProUGUI != null) textMeshProUGUI.text = subtitle.formattedText.text;
            }
            Show();
            var barkDuration = Mathf.Approximately(0, duration) ? DialogueManager.GetBarkDuration(subtitle.formattedText.text) : duration;
            secondsLeft = barkDuration;
            playerCameraTransform = Camera.main.transform;
            playerCameraCollider = (playerCameraTransform != null) ? playerCameraTransform.GetComponent<Collider>() : null;
        }

        /// <summary>
        /// Indicates whether a bark is playing or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is playing; otherwise, <c>false</c>.
        /// </value>
        public bool IsPlaying
        {
            get { return secondsLeft > 0; }
        }

        public void OnBarkEnd(Transform actor)
        {
            Hide();
        }

        public void Show()
        {
            SetActive(true);
            ShowedBark(this);
        }

        public void Hide()
        {
            SetActive(false);
            HidBark(this);
            secondsLeft = 0;
        }

        private void FadeIn()
        {
            SetActive(true);
        }

        private void FadeOut()
        {
            SetActive(false);
        }

        private void CheckPlayerVisibility()
        {
            bool canSeePlayer = true;
            if (playerCameraTransform != null)
            {
                RaycastHit hit;
                if (Physics.Linecast(transform.position, playerCameraTransform.position, out hit, visibilityLayerMask))
                {
                    canSeePlayer = (hit.collider == playerCameraCollider);
                }
            }
            if (!canSeePlayer && ((textMeshPro != null && textMeshPro.gameObject.activeInHierarchy) || (textMeshProUGUI != null && textMeshProUGUI.gameObject.activeInHierarchy)))
            {
                if (textMeshPro != null) textMeshPro.gameObject.SetActive(false);
                if (textMeshProUGUI != null) textMeshProUGUI.gameObject.SetActive(false);
                if (panel != null) panel.gameObject.SetActive(false);
            }
            else if (canSeePlayer && ((textMeshPro != null && !textMeshPro.gameObject.activeInHierarchy) || (textMeshProUGUI != null && !textMeshProUGUI.gameObject.activeInHierarchy)))
            {
                if (textMeshPro != null) textMeshPro.gameObject.SetActive(true);
                if (textMeshProUGUI != null) textMeshProUGUI.gameObject.SetActive(true);
                if (panel != null) panel.gameObject.SetActive(true);
            }
        }

    }

}