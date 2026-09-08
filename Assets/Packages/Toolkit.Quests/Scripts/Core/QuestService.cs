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
		private readonly List<Quest> _activeQuests;

		public IReadOnlyList<Quest> ActiveQuests => _activeQuests;

		public QuestService(IEventBus eventBus, IQuestFactory questFactory)
		{
			_eventBus = eventBus;
			_questFactory = questFactory;
			_activeQuests = new List<Quest>();
		}

		/// <summary>
		/// Starts a new quest based on the provided definition.
		/// </summary>
		public void AcceptQuest(QuestConfig config)
		{
			QuestState state = new QuestState { Quest = config, Status = QuestStatus.NotStarted };

			List<IQuestObjective> objectives = new List<IQuestObjective>();
			if (config.Objectives != null)
				foreach (QuestObjectiveConfig objectiveConfig in config.Objectives)
				{
					IQuestObjective objective = _questFactory.CreateObjective(objectiveConfig);
					if (objective != null)
						objectives.Add(objective);
				}

			List<IQuestReward> rewards = new List<IQuestReward>();
			if (config.Rewards != null)
				foreach (QuestRewardConfig rewardConfig in config.Rewards)
				{
					IQuestReward reward = _questFactory.CreateReward(rewardConfig);
					if (reward != null)
						rewards.Add(reward);
				}

			Quest instance = new Quest(config, state, objectives, rewards, _eventBus);

			instance.Completed += RemoveFromActiveQuests;

			_activeQuests.Add(instance);

			instance.StartQuest();
		}

		private void RemoveFromActiveQuests(Quest instance)
		{
			instance.Completed -= RemoveFromActiveQuests;

			_activeQuests.Remove(instance);
		}

		/// <summary>
		/// Cleans up all active quests.
		/// </summary>
		public void DisposeAll()
		{
			// Loop backwards to safely remove elements if StopQuest() triggers anything.
			for (int i = _activeQuests.Count - 1; i >= 0; i--)
			{
				Quest quest = _activeQuests[i];
				quest.Completed -= RemoveFromActiveQuests;
				quest.StopQuest();
			}

			_activeQuests.Clear();
		}

		public void CancelQuest(Quest instance)
		{
			if (_activeQuests.Contains(instance))
			{
				instance.Completed -= RemoveFromActiveQuests;
				instance.State.Status = QuestStatus.Failed;
				instance.StopQuest();

				_activeQuests.Remove(instance);
			}
		}
	}
}
