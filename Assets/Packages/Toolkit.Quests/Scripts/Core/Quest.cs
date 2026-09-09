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

		public event Action<Quest> Completed;
		public event Action<Quest> Updated;
		public event Action<Quest> Failed;

		public QuestConfig Config { get; private set; }
		public QuestState State { get; private set; }
		public IReadOnlyList<IQuestObjective> Objectives => _objectives;
		public IReadOnlyList<IQuestReward> Rewards => _rewards;

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
			if (State.Status != QuestStatus.NotStarted)
				return;

			State.Status = QuestStatus.Active;

			for (int i = 0; i < _objectives.Count; i++)
			{
				// Prevents initializing remaining objectives if the quest was instantly completed.
				if (State.Status != QuestStatus.Active)
					break;

				IQuestObjective objective = _objectives[i];
				if (objective == null)
				{
					State.ObjectiveStates[i].IsCompleted = true;

					continue;
				}

				QuestObjectiveState objState = State.ObjectiveStates[i];
				objective.Initialize(_eventBus, objState, OnObjectiveProgressUpdated);
			}

			// Immediately check in case objectives are instantly completable.
			CheckCompletion();
		}

		/// <summary>
		/// Disposes all objectives when the quest is done or canceled.
		/// </summary>
		public void StopQuest()
		{
			foreach (IQuestObjective objective in _objectives)
			{
				if (objective != null && objective.State != null)
					objective.Dispose();
			}
		}

		private void OnObjectiveProgressUpdated()
		{
			Updated?.Invoke(this);
			CheckCompletion();
		}

		/// <summary>
		/// Checks if all objectives are completed.
		/// </summary>
		private void CheckCompletion()
		{
			if (State.Status != QuestStatus.Active)
				return;

			if (State.ObjectiveStates.All(s => s.IsCompleted))
			{
				State.Status = QuestStatus.Completed;

				GrantRewards();
				StopQuest();

				Completed?.Invoke(this);
			}
		}

		public void FailQuest()
		{
			if (State.Status != QuestStatus.Active)
				return;

			State.Status = QuestStatus.Failed;

			StopQuest();

			Failed?.Invoke(this);
		}

		private void GrantRewards()
		{
			if (_rewards != null)
				foreach (IQuestReward reward in _rewards)
					reward?.GrantReward();
		}
	}
}