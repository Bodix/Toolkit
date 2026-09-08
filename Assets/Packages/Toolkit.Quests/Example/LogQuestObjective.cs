using System;
using Bodix.Evolunity.Patterns;
using UnityEngine;

namespace Toolkit.Quests.Example
{
	public class LogQuestObjective : IQuestObjective
	{
		public QuestObjectiveState State { get; private set; }

		private readonly LogQuestObjectiveConfig _config;
		private Action _onProgressUpdated;

		public LogQuestObjective(LogQuestObjectiveConfig config)
		{
			_config = config;
		}

		public void Initialize(IEventBus eventBus, QuestObjectiveState state, Action onProgressUpdated)
		{
			State = state;
			_onProgressUpdated = onProgressUpdated;

			Debug.Log($"[Quest Objective] Initialized: {_config.Log}");
		}

		public void CompleteManually()
		{
			if (State.IsCompleted)
				return;

			State.IsCompleted = true;
			_onProgressUpdated?.Invoke();

			Debug.Log($"[Quest Objective] Completed: {_config.Log}");
		}

		public void Dispose()
		{
			Debug.Log($"[Quest Objective] Disposed: {_config.Log}");
		}
	}
}