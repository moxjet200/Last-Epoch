/* [REMOVE]
 * [REMOVE] This script is deprecated. If you need it for an older project,
 * [REMOVE] remove the lines marked [REMOVE].
 * [REMOVE]

using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem.TextMeshPro {

	/// <summary>
	/// Place on a Unity UI Text control that also has a TextMesh Pro. 
	/// If Text.text is assigned, the text is moves to the TextMesh Pro.
	/// </summary>
	[AddComponentMenu("Dialogue System/Third Party/TextMesh Pro/Conversion/Text To TextMesh Pro")]
	public class TextToTextMeshPro : MonoBehaviour {

		public Text text = null;
		public TMPro.TextMeshProUGUI textMeshPro = null;

		void Awake() {
			if (text == null) text = GetComponentInChildren<Text>();
			if (textMeshPro == null) textMeshPro = GetComponentInChildren<TMPro.TextMeshProUGUI>();
		}

		void Start() {
			CheckText();
		}

		void Update() {
			CheckText();
		}

		public void CheckText() {
			if (text == null || textMeshPro == null) return;
			if (!string.IsNullOrEmpty(text.text)) {
				textMeshPro.text = text.text;
				text.text = string.Empty;
				if (textMeshPro.transform.parent != null) {
					LayoutRebuilder.MarkLayoutForRebuild(textMeshPro.transform.parent as RectTransform);
				}
			}
			if (textMeshPro.color != text.color) {
				textMeshPro.color = text.color;
			}
		}

	}

}
/**/