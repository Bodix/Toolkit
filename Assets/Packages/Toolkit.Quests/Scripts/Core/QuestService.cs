using System;
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

		public event Action<Quest> QuestAccepted;
		public event Action<Quest> QuestCompleted;
		public event Action<Quest> QuestFailed;
		public event Action<Quest> QuestUpdated;

		public QuestService(IEventBus eventBus, IQuestFactory questFactory)
		{
			_eventBus = eventBus;
			_questFactory = questFactory;
			_activeQuests = new List<Quest>();
		}

		/// <summary>
		/// Starts a new quest based on the provided definition.
		/// </summary>
		public Quest AcceptQuest(QuestConfig config)
		{
			QuestState state = new QuestState { Quest = config, Status = QuestStatus.NotStarted };
			Quest instance = RestoreQuest(state);

			QuestAccepted?.Invoke(instance);

			return instance;
		}

		/// <summary>
		/// Restores a quest from a saved state.
		/// </summary>
		public Quest RestoreQuest(QuestState state)
		{
			if (state.Quest == null)
				return null;

			List<IQuestObjective> objectives = new List<IQuestObjective>();
			if (state.Quest.Objectives != null)
				foreach (QuestObjectiveConfig objectiveConfig in state.Quest.Objectives)
				{
					IQuestObjective objective = _questFactory.CreateObjective(objectiveConfig);
					if (objective != null)
						objectives.Add(objective);
				}

			List<IQuestReward> rewards = new List<IQuestReward>();
			if (state.Quest.Rewards != null)
				foreach (QuestRewardConfig rewardConfig in state.Quest.Rewards)
				{
					IQuestReward reward = _questFactory.CreateReward(rewardConfig);
					if (reward != null)
						rewards.Add(reward);
				}

			Quest instance = new Quest(state.Quest, state, objectives, rewards, _eventBus);

			instance.Completed += OnQuestCompleted;
			instance.Failed += OnQuestFailed;
			instance.Updated += OnQuestUpdated;

			_activeQuests.Add(instance);
			instance.StartQuest();

			return instance;
		}

		private void OnQuestCompleted(Quest instance)
		{
			UnsubscribeQuest(instance);
			_activeQuests.Remove(instance);

			QuestCompleted?.Invoke(instance);
		}

		private void OnQuestFailed(Quest instance)
		{
			UnsubscribeQuest(instance);
			_activeQuests.Remove(instance);

			QuestFailed?.Invoke(instance);
		}

		private void OnQuestUpdated(Quest instance)
		{
			QuestUpdated?.Invoke(instance);
		}

		private void UnsubscribeQuest(Quest instance)
		{
			instance.Completed -= OnQuestCompleted;
			instance.Failed -= OnQuestFailed;
			instance.Updated -= OnQuestUpdated;
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

				UnsubscribeQuest(quest);
				quest.StopQuest();
			}

			_activeQuests.Clear();
		}

		public void CancelQuest(Quest instance)
		{
			if (_activeQuests.Contains(instance))
				instance.FailQuest();
		}
	}
}