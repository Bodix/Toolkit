using Bodix.Evolunity.Collections;

namespace Toolkit.Quests
{
	public abstract class QuestObjectiveConfig : DataAsset
	{
		/// <summary>
		/// Creates the runtime instance of this objective.
		/// </summary>
		public abstract IQuestObjective CreateInstance();

		/// <summary>
		/// Creates the initial state for this objective.
		/// </summary>
		public virtual QuestObjectiveState CreateState()
		{
			return new QuestObjectiveState();
		}
	}
}
