using System;
using Bodix.Evolunity.Patterns;
using UnityEngine;

namespace Toolkit.Quests.Example
{
	public class DebugQuestObjective : IQuestObjective
	{
		public string Id => _config.Id;
		public bool IsCompleted { get; private set; }

		private readonly DebugQuestObjectiveConfig _config;
		private Action _onProgressUpdated;

		public DebugQuestObjective(DebugQuestObjectiveConfig config)
		{
			_config = config;
		}

		public void Initialize(IEventBus eventBus, Action onProgressUpdated)
		{
			_onProgressUpdated = onProgressUpdated;
			IsCompleted = false;
			Debug.Log($"[Quest Objective] Initialized: {_config.TargetActionName}");
		}

		public void CompleteManually()
		{
			if (IsCompleted) return;
			
			IsCompleted = true;
			Debug.Log($"[Quest Objective] Completed: {_config.TargetActionName}");
			_onProgressUpdated?.Invoke();
		}

		public void Dispose()
		{
			Debug.Log($"[Quest Objective] Disposed: {_config.TargetActionName}");
		}
	}
}