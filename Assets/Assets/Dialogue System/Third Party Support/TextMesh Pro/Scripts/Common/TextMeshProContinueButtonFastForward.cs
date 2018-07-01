using UnityEngine;

namespace PixelCrushers.DialogueSystem.TextMeshPro
{

    /// <summary>
    /// This script replaces the normal continue button functionality with
    /// a two-stage process. If the typewriter effect is still playing, it
    /// simply stops the effect. Otherwise it sends OnContinue to the UI.
    /// </summary>
    [AddComponentMenu("Dialogue System/Third Party/TextMesh Pro/TextMesh Pro Continue Button Fast Forward")]
    public class TextMeshProContinueButtonFastForward : MonoBehaviour
    {

        public TextMeshProDialogueUI dialogueUI;

        public TextMeshProTypewriterEffect typewriterEffect;

        public virtual void Awake()
        {
            if (dialogueUI == null)
            {
                dialogueUI = Tools.GetComponentAnywhere<TextMeshProDialogueUI>(gameObject);
            }
            if (typewriterEffect == null)
            {
                typewriterEffect = GetComponentInChildren<TextMeshProTypewriterEffect>();
            }
        }

        public virtual void OnFastForward()
        {
            if ((typewriterEffect != null) && typewriterEffect.IsPlaying)
            {
                typewriterEffect.Stop();
            }
            else
            {
                if (dialogueUI != null) dialogueUI.OnContinue();
            }
        }

    }

}
