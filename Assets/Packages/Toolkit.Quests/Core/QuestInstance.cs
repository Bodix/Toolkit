using System.Collections.Generic;
using System.Linq;
using Bodix.Evolunity.Patterns;

namespace Toolkit.Quests
{
	/// <summary>
	/// Represents an active quest during gameplay.
	/// </summary>
	public class QuestInstance
	{
		public QuestConfig Config { get; private set; }
		public QuestState State { get; private set; }

		private readonly List<IQuestObjective> _objectives;
		private readonly IEventBus _eventBus;

		public QuestInstance(QuestConfig config, QuestState state,
			List<IQuestObjective> objectives, IEventBus eventBus)
		{
			Config = config;
			State = state;
			_objectives = objectives;
			_eventBus = eventBus;
		}

		/// <summary>
		/// Initializes all objectives and subscribes to events.
		/// </summary>
		public void StartQuest()
		{
			State.Status = QuestStatus.Active;

			foreach (IQuestObjective objective in _objectives)
				objective.Initialize(_eventBus, CheckCompletion);
		}

		/// <summary>
		/// Disposes all objectives when the quest is done or canceled.
		/// </summary>
		public void StopQuest()
		{
			foreach (IQuestObjective objective in _objectives)
				objective.Dispose();
		}

		/// <summary>
		/// Checks if all objectives are completed.
		/// </summary>
		private void CheckCompletion()
		{
			if (State.Status != QuestStatus.Active)
				return;

			if (_objectives.All(o => o.IsCompleted))
			{
				State.Status = QuestStatus.Completed;

				StopQuest();
			}
		}
	}
}