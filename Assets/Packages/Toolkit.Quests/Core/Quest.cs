using System;
using System.Collections.Generic;
using System.Linq;
using Bodix.Evolunity.Patterns;

namespace Toolkit.Quests
{
	/// <summary>
	/// Represents an active quest during gameplay.
	/// </summary>
	public class Quest
	{
		private readonly List<IQuestObjective> _objectives;
		private readonly List<IQuestReward> _rewards;
		private readonly IEventBus _eventBus;

		public QuestConfig Config { get; private set; }
		public QuestState State { get; private set; }
		public IReadOnlyList<IQuestObjective> Objectives => _objectives;
		public IReadOnlyList<IQuestReward> Rewards => _rewards;

		public event Action<Quest> Completed;

		public Quest(QuestConfig config, QuestState state,
			List<IQuestObjective> objectives, List<IQuestReward> rewards, IEventBus eventBus)
		{
			Config = config;
			State = state;
			_objectives = objectives;
			_rewards = rewards;
			_eventBus = eventBus;
		}

		/// <summary>
		/// Initializes all objectives and subscribes to events.
		/// </summary>
		public void StartQuest()
		{
			if (State.Status == QuestStatus.Completed)
				return;

			State.Status = QuestStatus.Active;

			foreach (IQuestObjective objective in _objectives)
				objective.Initialize(_eventBus, CheckCompletion);

			// Immediately check in case objectives are instantly completable.
			CheckCompletion();
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

				GrantRewards();
				StopQuest();

				Completed?.Invoke(this);
			}
		}

		private void GrantRewards()
		{
			if (_rewards != null)
				foreach (IQuestReward reward in _rewards)
					reward?.GrantReward();
		}
	}
}