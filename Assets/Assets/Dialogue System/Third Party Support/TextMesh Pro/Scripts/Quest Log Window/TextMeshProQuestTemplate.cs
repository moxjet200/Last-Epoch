using UnityEngine;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem.TextMeshPro {

	/// <summary>
	/// This component hooks up the elements of a Unity UI quest template.
	/// Add it to your quest template and assign the properties.
	/// </summary>
	[AddComponentMenu("Dialogue System/Third Party/TextMesh Pro/Quest/TextMesh Pro Quest Template")]
	public class TextMeshProQuestTemplate : MonoBehaviour	{

		public UnityEngine.UI.Button heading;

		public TMPro.TextMeshProUGUI description;

		public TMPro.TextMeshProUGUI entryDescription;

		public UnityEngine.UI.Button trackButton;

		public UnityEngine.UI.Button abandonButton;

		public bool ArePropertiesAssigned {
			get {
				return (heading != null) &&
					(description != null) && (entryDescription != null) &&
					(trackButton != null) && (abandonButton != null);
			}
		}

	}

}
