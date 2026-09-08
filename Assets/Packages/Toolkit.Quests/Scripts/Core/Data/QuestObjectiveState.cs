using System;

namespace Toolkit.Quests
{
	/// <summary>
	/// Base class for objective states. Separates data from logic for clean serialization.
	/// </summary>
	[Serializable]
	public class QuestObjectiveState
	{
		public bool IsCompleted;
	}
}
