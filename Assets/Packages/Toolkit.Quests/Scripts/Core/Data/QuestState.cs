using System.Collections.Generic;

namespace Toolkit.Quests
{
	/// <summary>
	/// Represents the serializable state of a quest for saving and loading.
	/// </summary>
	[System.Serializable]
	public class QuestState
	{
		public QuestConfig Quest;
		public QuestStatus Status;
		public List<string> ObjectiveStates = new List<string>();
	}
}
