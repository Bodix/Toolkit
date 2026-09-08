using System.Collections.Generic;
using Bodix.Evolunity.Patterns;

namespace Toolkit.Quests
{
	/// <summary>
	/// Manages all active quests and their life cycles.
	/// </summary>
	public class QuestService
	{
		private readonly IEventBus _eventBus;
		private readonly IQuestFactory _questFactory;
		private readonly List<QuestInstance> _activeQuests;

		public QuestService(IEventBus eventBus, IQuestFactory questFactory)
		{
			_eventBus = eventBus;
			_questFactory = questFactory;
			_activeQuests = new List<QuestInstance>();
		}

		/// <summary>
		/// Starts a new quest based on the provided definition.
		/// </summary>
		public void AcceptQuest(QuestConfig config)
		{
			QuestState state = new QuestState { QuestId = config.Id, Status = QuestStatus.NotStarted };
			List<IQuestObjective> objectives = new List<IQuestObjective>();

			foreach (string objId in config.ObjectiveIds)
				objectives.Add(_questFactory.CreateObjective(objId));

			QuestInstance instance = new QuestInstance(config, state, objectives, _eventBus);
			_activeQuests.Add(instance);

			instance.StartQuest();
		}

		/// <summary>
		/// Cleans up all active quests.
		/// </summary>
		public void DisposeAll()
		{
			foreach (QuestInstance quest in _activeQuests)
				quest.StopQuest();

			_activeQuests.Clear();
		}
	}
}