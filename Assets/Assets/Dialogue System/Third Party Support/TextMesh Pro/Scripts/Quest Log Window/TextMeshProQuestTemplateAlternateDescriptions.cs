using UnityEngine;

namespace PixelCrushers.DialogueSystem.TextMeshPro
{

    [System.Serializable]
    public class TextMeshProQuestTemplateAlternateDescriptions
    {

        [Tooltip("(Optional) If set, use if state is success")]
        public TMPro.TextMeshProUGUI successDescription;

        [Tooltip("(Optional) If set, use if state is failure")]
        public TMPro.TextMeshProUGUI failureDescription;

        public void SetActive(bool value)
        {
            if (successDescription != null) successDescription.gameObject.SetActive(value);
            if (failureDescription != null) failureDescription.gameObject.SetActive(value);
        }
    }

}
