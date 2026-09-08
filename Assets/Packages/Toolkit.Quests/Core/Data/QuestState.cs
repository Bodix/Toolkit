namespace Toolkit.Quests
{
	/// <summary>
	/// Represents the serializable state of a quest for saving and loading.
	/// </summary>
	[System.Serializable]
	public class QuestState
	{
		public string QuestId;
		public QuestStatus Status;
	}
}