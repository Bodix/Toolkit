using System;
using Bodix.Evolunity.Patterns;
using UnityEngine;

namespace Toolkit.Quests.Example
{
	public class LogQuestObjective : IQuestObjective
	{
		public bool IsCompleted { get; private set; }
		public float Progress => IsCompleted ? 1f : 0f;

		private readonly LogQuestObjectiveConfig _config;
		private Action _onProgressUpdated;

		public LogQuestObjective(LogQuestObjectiveConfig config)
		{
			_config = config;
		}

		public void Initialize(IEventBus eventBus, Action onProgressUpdated)
		{
			_onProgressUpdated = onProgressUpdated;
			IsCompleted = false;

			Debug.Log($"[Quest Objective] Initialized: {_config.Log}");
		}

		public void CompleteManually()
		{
			if (IsCompleted)
				return;

			IsCompleted = true;
			_onProgressUpdated?.Invoke();

			Debug.Log($"[Quest Objective] Completed: {_config.Log}");
		}

		public void Dispose()
		{
			Debug.Log($"[Quest Objective] Disposed: {_config.Log}");
		}

		public string GetSerializedState()
		{
			return IsCompleted.ToString();
		}

		public void RestoreState(string state)
		{
			if (bool.TryParse(state, out bool isCompleted))
				IsCompleted = isCompleted;
		}
	}
}