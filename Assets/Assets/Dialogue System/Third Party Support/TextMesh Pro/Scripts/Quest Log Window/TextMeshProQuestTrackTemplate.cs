using UnityEngine;

namespace PixelCrushers.DialogueSystem.TextMeshPro
{

    /// <summary>
    /// This component hooks up the elements of a TextMeshPro quest track template,
    /// which is used by the TextMeshPro Quest Tracker.
    /// Add it to your quest track template and assign the properties.
    /// </summary>
    [AddComponentMenu("Dialogue System/Third Party/TextMesh Pro/Quest/TextMesh Pro Quest Track Template")]
    public class TextMeshProQuestTrackTemplate : MonoBehaviour
    {

        [Header("Quest Heading")]
        [Tooltip("The heading - name or description depends on tracker setting")]
        public TMPro.TextMeshProUGUI description;

        public TextMeshProQuestTemplateAlternateDescriptions alternateDescriptions = new TextMeshProQuestTemplateAlternateDescriptions();

        [Header("Quest Entries")]
        [Tooltip("(Optional) If set, holds instantiated quest entries")]
        public Transform entryContainer;

        [Tooltip("Used for quest entries")]
        public TMPro.TextMeshProUGUI entryDescription;

        public TextMeshProQuestTemplateAlternateDescriptions alternateEntryDescriptions = new TextMeshProQuestTemplateAlternateDescriptions();

        public bool ArePropertiesAssigned
        {
            get
            {
                return (description != null) && (entryDescription != null);
            }
        }

        private int numEntries = 0;

        public void Initialize()
        {
            if (description != null) description.gameObject.SetActive(false);
            alternateDescriptions.SetActive(false);
            if (entryDescription != null) entryDescription.gameObject.SetActive(false);
            alternateEntryDescriptions.SetActive(false);
            if (entryContainer != null) entryContainer.gameObject.SetActive(false);
        }

        public void SetDescription(string text, QuestState questState)
        {
            if (text == null) return;
            switch (questState)
            {
                case QuestState.Active:
                    SetFirstValidTextElement(text, description);
                    break;
                case QuestState.Success:
                    SetFirstValidTextElement(text, alternateDescriptions.successDescription, description);
                    break;
                case QuestState.Failure:
                    SetFirstValidTextElement(text, alternateDescriptions.failureDescription, description);
                    break;
                default:
                    return;
            }
        }

        private void SetFirstValidTextElement(string text, params TMPro.TextMeshProUGUI[] textElements)
        {
            for (int i = 0; i < textElements.Length; i++)
            {
                if (textElements[i] != null)
                {
                    textElements[i].gameObject.SetActive(true);
                    textElements[i].text = text;
                    return;
                }
            }
        }

        public void AddEntryDescription(string text, QuestState entryState)
        {
            if (entryContainer == null)
            {

                // No container, so make entryDescription a big multi-line string:
                alternateEntryDescriptions.SetActive(false);
                if (entryDescription != null)
                {
                    if (numEntries == 0)
                    {
                        entryDescription.gameObject.SetActive(true);
                        entryDescription.text = text;
                    }
                    else
                    {
                        entryDescription.text += "\n" + text;
                    }
                }
            }
            else
            {

                // Instantiate into container:
                if (numEntries == 0)
                {
                    entryContainer.gameObject.SetActive(true);
                    if (entryDescription != null) entryDescription.gameObject.SetActive(false);
                    alternateEntryDescriptions.SetActive(false);
                }
                switch (entryState)
                {
                    case QuestState.Active:
                        InstantiateFirstValidTextElement(text, entryContainer, entryDescription);
                        break;
                    case QuestState.Success:
                        InstantiateFirstValidTextElement(text, entryContainer, alternateEntryDescriptions.successDescription, entryDescription);
                        break;
                    case QuestState.Failure:
                        InstantiateFirstValidTextElement(text, entryContainer, alternateEntryDescriptions.failureDescription, entryDescription);
                        break;
                }
            }
            numEntries++;
        }

        private void InstantiateFirstValidTextElement(string text, Transform container, params TMPro.TextMeshProUGUI[] textElements)
        {
            for (int i = 0; i < textElements.Length; i++)
            {
                if (textElements[i] != null)
                {
                    var instance = Instantiate(textElements[i].gameObject) as GameObject;
                    instance.transform.SetParent(container.transform, false);
                    instance.SetActive(true);
                    var textElement = instance.GetComponent<TMPro.TextMeshProUGUI>();
                    if (textElement != null) textElement.text = text;
                    return;
                }
            }
        }

    }

}