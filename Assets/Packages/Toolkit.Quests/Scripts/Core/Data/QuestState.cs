using System.Collections.Generic;
using UnityEngine;

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
		
		[SerializeReference]
		public List<QuestObjectiveState> ObjectiveStates = new List<QuestObjectiveState>();
	}
}
