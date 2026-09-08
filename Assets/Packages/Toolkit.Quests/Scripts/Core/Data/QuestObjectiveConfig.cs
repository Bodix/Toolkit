using Bodix.Evolunity.Collections;

namespace Toolkit.Quests
{
	public abstract class QuestObjectiveConfig : DataAsset
	{
		/// <summary>
		/// Creates the runtime instance of this objective.
		/// </summary>
		public abstract IQuestObjective CreateInstance();
	}
}
