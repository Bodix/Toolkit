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
		public event Action<Quest> Updated;
		public event Action<Quest> Failed;

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
			if (State.Status == QuestStatus.Completed || State.Status == QuestStatus.Failed)
				return;

			State.Status = QuestStatus.Active;

			for (int i = 0; i < _objectives.Count; i++)
			{
				IQuestObjective objective = _objectives[i];
				objective.Initialize(_eventBus, OnObjectiveProgressUpdated);

				if (State.ObjectiveStates != null && i < State.ObjectiveStates.Count)
					if (!string.IsNullOrEmpty(State.ObjectiveStates[i]))
						objective.RestoreState(State.ObjectiveStates[i]);
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
				objective.Dispose();
		}

		private void OnObjectiveProgressUpdated()
		{
			SaveObjectiveStates();
			Updated?.Invoke(this);
			CheckCompletion();
		}

		private void SaveObjectiveStates()
		{
			State.ObjectiveStates = State.ObjectiveStates ?? new List<string>();
			State.ObjectiveStates.Clear();

			foreach (IQuestObjective objective in _objectives)
				State.ObjectiveStates.Add(objective.GetSerializedState());
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

				SaveObjectiveStates();
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

			SaveObjectiveStates();
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