using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem.TextMeshPro
{

    /// <summary>
    /// Adds a fade in/out effect to TextMeshProBarkUI.
    /// </summary>
    [AddComponentMenu("Dialogue System/Third Party/TextMesh Pro/TextMesh Pro Bark UI Fader")]
    [RequireComponent(typeof(TextMeshProBarkUI))]
    public class TextMeshProBarkUIFader : MonoBehaviour
    {

        /// <summary>
        /// The duration of the fade in.
        /// </summary>
        [Tooltip("Fade in over this many seconds.")]
        public float fadeInDuration = 3f;

        /// <summary>
        /// The duration of the fade out.
        /// </summary>
        [Tooltip("Fade out over this many seconds.")]
        public float fadeOutDuration = 3f;

        private Color originalColor = Color.white;

        public void Start()
        {
            var barkUI = GetComponent<TextMeshProBarkUI>();
            if (barkUI == null || barkUI.textMeshPro == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning(string.Format("{0}: No TextMeshProBarkUI found on '{1}'. Not connecting fader.", new object[] { DialogueDebug.Prefix, name }), this);
            }
            else
            {
                barkUI.ShowedBark += OnShowedBark;
                barkUI.HidBark += OnHidBark;
                originalColor = barkUI.textMeshPro.color;
            }
        }

        public void OnShowedBark(TextMeshProBarkUI barkUI)
        {
            if (barkUI == null) return;
            StopAllCoroutines();
            StartCoroutine(FadeIn(barkUI.textMeshPro));
        }

        public void OnHidBark(TextMeshProBarkUI barkUI)
        {
            if (barkUI == null) return;
            StopAllCoroutines();
            StartCoroutine(FadeOut(barkUI.textMeshPro));
        }

        private IEnumerator FadeIn(TMPro.TextMeshPro textMeshPro)
        {
            if (textMeshPro == null) yield break;
            var startTime = Time.time;
            var endTime = startTime + fadeInDuration;
            while (Time.time < endTime)
            {
                var elapsed = Time.time - startTime;
                var a = originalColor.a * (elapsed / fadeInDuration);
                textMeshPro.color = new Color(originalColor.r, originalColor.b, originalColor.g, a);
                yield return null;
            }
            textMeshPro.color = originalColor;
        }

        private IEnumerator FadeOut(TMPro.TextMeshPro textMeshPro)
        {
            if (textMeshPro == null) yield break;
            textMeshPro.gameObject.SetActive(true);
            var startTime = Time.time;
            var endTime = startTime + fadeOutDuration;
            while (Time.time < endTime)
            {
                var elapsed = fadeOutDuration - (Time.time - startTime);
                var a = originalColor.a * (elapsed / fadeOutDuration);
                textMeshPro.color = new Color(originalColor.r, originalColor.b, originalColor.g, a);
                yield return null;
            }
            textMeshPro.color = originalColor;
            textMeshPro.gameObject.SetActive(false);
        }

    }

}