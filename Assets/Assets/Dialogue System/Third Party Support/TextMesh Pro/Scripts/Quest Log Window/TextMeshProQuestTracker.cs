using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem.TextMeshPro {

    /// <summary>
    /// When you attach this script to the Dialogue Manager object (or a child),
    /// it will display tracked quests using the new Unity UI and TextMeshPro. It updates when the player
    /// toggles tracking in the quest log window and at the end of conversations. If you 
    /// change the state of a quest elsewhere, you must manually call UpdateTracker().
    /// </summary>
    [AddComponentMenu("Dialogue System/Third Party/TextMesh Pro/Quest/TextMesh Pro Quest Tracker HUD (on Dialogue Manager)")]
    public class TextMeshProQuestTracker : MonoBehaviour {

        /// <summary>
        /// The UI control that will hold quest track info (instantiated copies of the 
        /// quest track template). This is typically a Vertical Layout Group.
        /// </summary>
        [Tooltip("UI container that will hold instances of quest track template.")]
        public Transform container;

        /// <summary>
        /// Tick to show the container even if there's nothing to track.
        /// </summary>
        [Tooltip("Show Container even if there's nothing to track.")]
        public bool showContainerIfEmpty = true;

        /// <summary>
        /// The quest track template.
        /// </summary>
        [Tooltip("Template to instantiate for each tracked quest.")]
        public TextMeshProQuestTrackTemplate questTrackTemplate;

        /// <summary>
        /// Tick to show active quests in the tracker.
        /// </summary>
        [Tooltip("Show active quests.")]
        public bool showActiveQuests = true;

        /// <summary>
        /// Tick to show successful and failed quests in the tracker.
        /// </summary>
        [Tooltip("Show successful and failed quests.")]
        public bool showCompletedQuests = false;

        /// <summary>
        /// Tick to look up "Entry n Success" or "Entry n Failure" if the
        /// quest entry is in the success or failure state.
        /// </summary>
        [Tooltip("Show Entry n Success or Entry n Failure text if quest entry is in success/failure state.")]
        public bool showCompletedEntryText = false;
		
		public enum QuestDescriptionSource { Title, Description }

        [Tooltip("Source for the quest tracker text.")]
        public QuestDescriptionSource questDescriptionSource = QuestDescriptionSource.Title;
		
		private List<GameObject> instantiatedItems = new List<GameObject>();

		/// <summary>
		/// Wait 0.5s to update the tracker in case other start
		/// methods change the state of quests.
		/// </summary>
		public void Start() {
			if (container == null) Debug.LogWarning(string.Format("{0}: {1} Container is unassigned", new object[] { DialogueDebug.Prefix, name }));
			if (questTrackTemplate == null) {
				Debug.LogWarning(string.Format("{0}: {1} Quest Track Template is unassigned", new object[] { DialogueDebug.Prefix, name }));
			} else {
				questTrackTemplate.gameObject.SetActive(false);
			}
			Invoke("UpdateTracker", 0.5f);
		}

		/// <summary>
		/// The quest log window sends this message when the player toggles tracking.
		/// </summary>
		/// <param name="quest">Quest.</param>
		public void OnQuestTrackingEnabled(string quest) {
			UpdateTracker();
		}
		
		/// <summary>
		/// The quest log window sends this message when the player toggles tracking.
		/// </summary>
		/// <param name="quest">Quest.</param>
		public void OnQuestTrackingDisabled(string quest) {
			UpdateTracker();
		}

		/// <summary>
		/// Quests are often completed in conversations. This handles changes in quest states
		/// after conversations.
		/// </summary>
		/// <param name="actor">Actor.</param>
		public void OnConversationEnd(Transform actor) {
			UpdateTracker();
		}
		
		public void UpdateTracker() {
			DestroyInstantiatedItems();
            int numTracked = 0;
			QuestState flags = (showActiveQuests ? QuestState.Active : 0) | 
				(showCompletedQuests ? QuestState.Success | QuestState.Failure : 0);
			foreach (string quest in QuestLog.GetAllQuests(flags)) {
				if (QuestLog.IsQuestTrackingEnabled(quest)) {
					InstantiateQuestTrack(quest);
                    numTracked++;
				}
			}
            if (container != null) {
                container.gameObject.SetActive(showContainerIfEmpty || numTracked > 0);
            }                
		}

		public void DestroyInstantiatedItems() {
			foreach (var go in instantiatedItems) {
				Destroy(go);
			}
			instantiatedItems.Clear();
		}

		private void InstantiateQuestTrack(string quest) {
			if (container == null || questTrackTemplate == null) return;
			var go = Instantiate(questTrackTemplate.gameObject) as GameObject;
			if (go == null) {
				Debug.LogError(string.Format("{0}: {1} couldn't instantiate quest track template", new object[] { DialogueDebug.Prefix, name }));
				return;
			}
			instantiatedItems.Add(go);

			var questDescription = (questDescriptionSource == QuestDescriptionSource.Title)
				? QuestLog.GetQuestTitle(quest)
					: QuestLog.GetQuestDescription(quest); 
			var heading = FormattedText.Parse(questDescription, DialogueManager.MasterDatabase.emphasisSettings).text;

			go.name = heading;
			go.transform.SetParent(container.transform, false);
			go.SetActive(true);
			var questTrack = go.GetComponent<TextMeshProQuestTrackTemplate>();
			if (questTrack != null) {
				questTrack.Initialize();
				var questState = QuestLog.GetQuestState(quest);
				questTrack.SetDescription(heading, questState);
				int entryCount = QuestLog.GetQuestEntryCount(quest);
				for (int i = 1; i <= entryCount; i++) {
					var entryState = QuestLog.GetQuestEntryState(quest, i);
					var entryText = FormattedText.Parse(GetQuestEntryText(quest, i, entryState), DialogueManager.MasterDatabase.emphasisSettings).text;
					questTrack.AddEntryDescription(entryText, entryState);
				}
			}
		}

		private string GetQuestEntryText(string quest, int entryNum, QuestState entryState) {
			if (entryState == QuestState.Unassigned || entryState == QuestState.Abandoned) {
				return string.Empty;
			} else if (entryState == QuestState.Success && showCompletedEntryText) {
				var text = DialogueLua.GetQuestField(quest, "Entry " + entryNum + " Success").AsString;
				if (!string.IsNullOrEmpty(text)) return text;
			} else if (entryState == QuestState.Failure && showCompletedEntryText) {
				var text = DialogueLua.GetQuestField(quest, "Entry " + entryNum + " Failure").AsString;
				if (!string.IsNullOrEmpty(text)) return text;
			}
			return QuestLog.GetQuestEntry(quest, entryNum);
		}
		
	}

}
