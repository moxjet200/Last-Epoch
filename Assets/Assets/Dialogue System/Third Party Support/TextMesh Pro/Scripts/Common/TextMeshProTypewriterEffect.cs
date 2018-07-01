using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace PixelCrushers.DialogueSystem.TextMeshPro
{

    /// <summary>
    /// This is a typewriter effect for TextMesh Pro.
    /// </summary>
    [AddComponentMenu("Dialogue System/Third Party/TextMesh Pro/TextMesh Pro Typewriter Effect")]
    [DisallowMultipleComponent]
    public class TextMeshProTypewriterEffect : MonoBehaviour
    {

        /// <summary>
        /// How fast to "type."
        /// </summary>
        [Tooltip("How fast to type. This is separate from Dialogue Manager > Subtitle Settings > Chars Per Second")]
        public float charactersPerSecond = 50;

        /// <summary>
        /// The audio clip to play with each character.
        /// </summary>
        [Tooltip("Optional audio clip to play with each character")]
        public AudioClip audioClip = null;

        /// <summary>
        /// The audio source through which to play the clip. If unassigned, will look for an
        /// audio source on this GameObject.
        /// </summary>
        [Tooltip("Optional audio source through which to play the clip")]
        public AudioSource audioSource = null;

        /// <summary>
        /// If audio clip is still playing from previous character, stop and restart it when typing next character.
        /// </summary>
        [Tooltip("If audio clip is still playing from previous character, stop and restart it when typing next character")]
        public bool interruptAudioClip = false;

        /// <summary>
        /// Ensures this GameObject has only one typewriter effect.
        /// </summary>
        [Tooltip("Ensure this GameObject has only one typewriter effect")]
        public bool removeDuplicateTypewriterEffects = true;

        /// <summary>
        /// Wait one frame to allow layout elements to setup first.
        /// </summary>
        [Tooltip("Wait one frame to allow layout elements to setup first")]
        public bool waitOneFrameBeforeStarting = false;

        [System.Serializable]
        public class AutoScrollSettings
        {
            [Tooltip("Automatically scroll to bottom of scroll rect. Useful for long text. Works best with left justification.")]
            public bool autoScrollEnabled = false;
            public UnityEngine.UI.ScrollRect scrollRect = null;
            public UnityUIScrollbarEnabler scrollbarEnabler = null;
        }

        /// <summary>
        /// Optional auto-scroll settings.
        /// </summary>
        public AutoScrollSettings autoScrollSettings = new AutoScrollSettings();

        public UnityEvent onBegin = new UnityEvent();
        public UnityEvent onCharacter = new UnityEvent();
        public UnityEvent onEnd = new UnityEvent();

        /// <summary>
        /// Indicates whether the effect is playing.
        /// </summary>
        /// <value><c>true</c> if this instance is playing; otherwise, <c>false</c>.</value>
        public bool IsPlaying { get; private set; }

        private TMPro.TMP_Text textComponent;
        private bool started = false;
        private bool paused = false;
        private int charactersTyped = 0;

        public void Awake()
        {
            textComponent = GetComponent<TMPro.TMP_Text>();
            if (audioSource == null) audioSource = GetComponent<AudioSource>();
            if (removeDuplicateTypewriterEffects) RemoveIfDuplicate();
        }

        private void RemoveIfDuplicate()
        {
            var effects = GetComponents<TextMeshProTypewriterEffect>();
            if (effects.Length > 1)
            {
                TextMeshProTypewriterEffect keep = effects[0];
                for (int i = 1; i < effects.Length; i++)
                {
                    if (effects[i].GetInstanceID() < keep.GetInstanceID())
                    {
                        keep = effects[i];
                    }
                }
                for (int i = 0; i < effects.Length; i++)
                {
                    if (effects[i] != keep)
                    {
                        Destroy(effects[i]);
                    }
                }
            }
        }

        public void Start()
        {
            if (!IsPlaying)
            {
                StopAllCoroutines();
                StartCoroutine(Play());
            }
            started = true;
        }

        public void OnEnable()
        {
            if (!IsPlaying && started && gameObject.activeInHierarchy)
            {
                StopAllCoroutines();
                StartCoroutine(Play());
            }
        }

        public void OnDisable()
        {
            Stop();
        }

        /// <summary>
        /// Pauses the effect.
        /// </summary>
        public void Pause()
        {
            paused = true;
        }

        /// <summary>
        /// Unpauses the effect. The text will resume at the point where it
        /// was paused; it won't try to catch up to make up for the pause.
        /// </summary>
        public void Unpause()
        {
            paused = false;
        }

        public void Rewind()
        {
            charactersTyped = 0;
        }

        /// <summary>
        /// Plays the typewriter effect.
        /// </summary>
        public IEnumerator Play()
        {
            if ((textComponent != null) && (charactersPerSecond > 0))
            {
                if (waitOneFrameBeforeStarting) yield return null;
                if (audioSource != null) audioSource.clip = audioClip;
                onBegin.Invoke();
                IsPlaying = true;
                paused = false;
                float delay = 1 / charactersPerSecond;
                float lastTime = DialogueTime.time;
                float elapsed = 0;
                textComponent.maxVisibleCharacters = 0;
                textComponent.ForceMeshUpdate();
                yield return null;
                textComponent.maxVisibleCharacters = 0;
                textComponent.ForceMeshUpdate();
                TMPro.TMP_TextInfo textInfo = textComponent.textInfo;
                int totalVisibleCharacters = textInfo.characterCount; // Get # of Visible Character in text object
                charactersTyped = 0;
                while (charactersTyped < totalVisibleCharacters)
                {
                    if (!paused)
                    {
                        var deltaTime = DialogueTime.time - lastTime;
                        elapsed += deltaTime;
                        var goal = elapsed * charactersPerSecond;
                        while (charactersTyped < goal)
                        {
                            PlayCharacterAudio();
                            onCharacter.Invoke();
                            charactersTyped++;
                        }
                    }
                    textComponent.maxVisibleCharacters = charactersTyped;
                    HandleAutoScroll();
                    //---Uncomment the line below to debug: 
                     //Debug.Log(textComponent.text.Substring(0, charactersTyped).Replace("<", "[").Replace(">", "]") + " (typed=" + charactersTyped + ")");
                    lastTime = DialogueTime.time;
                    var delayTime = DialogueTime.time + delay;
                    int delaySafeguard = 0;
                    while (DialogueTime.time < delayTime && delaySafeguard < 999)
                    {
                        delaySafeguard++;
                        yield return null;
                    }
                }
            }
            Stop();
        }

        private void PlayCharacterAudio()
        {
            if (audioClip == null || audioSource == null) return;
            if (interruptAudioClip)
            {
                if (audioSource.isPlaying) audioSource.Stop();
                audioSource.Play();
            }
            else
            {
                if (!audioSource.isPlaying) audioSource.Play();
            }
        }

        /// <summary>
        /// Stops the effect.
        /// </summary>
        public void Stop()
        {
            StopAllCoroutines();
            if (IsPlaying) onEnd.Invoke();
            IsPlaying = false;
            if (textComponent != null) textComponent.maxVisibleCharacters = textComponent.textInfo.characterCount;
            HandleAutoScroll();
        }

        private void HandleAutoScroll()
        {
            if (!autoScrollSettings.autoScrollEnabled) return;
            if (autoScrollSettings.scrollRect != null)
            {
                autoScrollSettings.scrollRect.normalizedPosition = new Vector2(0, 0);
            }
            if (autoScrollSettings.scrollbarEnabler != null)
            {
                autoScrollSettings.scrollbarEnabler.CheckScrollbar();
            }
        }

    }

}
