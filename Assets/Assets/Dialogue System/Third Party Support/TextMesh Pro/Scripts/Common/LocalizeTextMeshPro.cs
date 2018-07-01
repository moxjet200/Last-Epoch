using UnityEngine;
using System.Collections;

namespace PixelCrushers.DialogueSystem.TextMeshPro
{

    /// <summary>
    /// Localizes the text content of a TextMeshPro and/or TextMeshProUGUI component.
    /// </summary>
    [AddComponentMenu("Dialogue System/Third Party/TextMesh Pro/Localize TextMesh Pro")]
    public class LocalizeTextMeshPro : LocalizeUIText
    {

        protected TMPro.TextMeshPro textMeshPro;
        protected TMPro.TextMeshProUGUI textMeshProUGUI;

        public override void LocalizeText()
        {
            if (!started) return;

            // Skip if no language set:
            if (string.IsNullOrEmpty(PixelCrushers.DialogueSystem.Localization.Language)) return;
            if (localizedTextTable == null)
            {
                localizedTextTable = DialogueManager.DisplaySettings.localizationSettings.localizedText;
                if (localizedTextTable == null)
                {
                    if (DialogueDebug.LogWarnings) Debug.LogWarning(DialogueDebug.Prefix + ": No localized text table is assigned to " + name + " or the Dialogue Manager.", this);
                    return;
                }
            }

            if (!HasCurrentLanguage())
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning(DialogueDebug.Prefix + "Localized text table '" + localizedTextTable + "' does not have a language '" + PixelCrushers.DialogueSystem.Localization.Language + "'", this);
                return;
            }

            // Make sure we have a Text or Dropdown:
            if (textMeshPro == null && textMeshProUGUI == null)
            {
                if (textMeshPro == null) textMeshPro = GetComponent<TMPro.TextMeshPro>();
                if (textMeshProUGUI == null) textMeshProUGUI = GetComponent<TMPro.TextMeshProUGUI>();
                if (textMeshPro == null && textMeshProUGUI == null)
                {
                    if (DialogueDebug.LogWarnings) Debug.LogWarning(DialogueDebug.Prefix + ": LocalizeTextMeshPro didn't find a TextMeshPro or TextMeshProUGUI component on " + name + ".", this);
                    return;
                }
            }

            // Get the original values to use as field lookups:
            if (string.IsNullOrEmpty(fieldName))
            {
                fieldName = (textMeshPro != null) ? textMeshPro.text 
                    : ((textMeshProUGUI != null) ? textMeshProUGUI.text : string.Empty);
            }

            // Localize Text:
            if (!localizedTextTable.ContainsField(fieldName))
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning(DialogueDebug.Prefix + ": Localized text table '" + localizedTextTable.name + "' does not have a field: " + fieldName, this);
            }
            else
            {
                var localizedText = localizedTextTable[fieldName];
                if (textMeshPro != null) textMeshPro.text = localizedText;
                if (textMeshProUGUI != null) textMeshProUGUI.text = localizedText;
            }
        }

        /// <summary>
        /// Sets the field name, which is the key to use in the localized text table.
        /// By default, the field name is the initial value of the Text component.
        /// </summary>
        /// <param name="fieldName"></param>
        public override void UpdateFieldName(string newFieldName = "")
        {
            if (textMeshPro == null) textMeshPro = GetComponent<TMPro.TextMeshPro>();
            if (textMeshProUGUI == null) textMeshProUGUI = GetComponent<TMPro.TextMeshProUGUI>();
            fieldName = !string.IsNullOrEmpty(newFieldName) ? newFieldName
                : ((textMeshPro != null) ? textMeshPro.text : ((textMeshProUGUI != null) ? textMeshProUGUI.text : string.Empty));
        }

    }
}