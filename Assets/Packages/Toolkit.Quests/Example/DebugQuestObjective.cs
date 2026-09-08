using System;
using Bodix.Evolunity.Patterns;
using UnityEngine;

namespace Toolkit.Quests.Example
{
	public class DebugQuestObjective : IQuestObjective
	{
		public string Id { get; }
		public bool IsCompleted { get; private set; }

		private readonly string _targetActionName;
		private Action _onProgressUpdated;

		public DebugQuestObjective(string id, string targetActionName)
		{
			Id = id;
			_targetActionName = targetActionName;
		}

		public void Initialize(IEventBus eventBus, Action onProgressUpdated)
		{
			_onProgressUpdated = onProgressUpdated;
			IsCompleted = false;
			Debug.Log($"[Quest Objective] Initialized: {_targetActionName}");
		}

		public void CompleteManually()
		{
			if (IsCompleted) return;
			
			IsCompleted = true;
			Debug.Log($"[Quest Objective] Completed: {_targetActionName}");
			_onProgressUpdated?.Invoke();
		}

		public void Dispose()
		{
			Debug.Log($"[Quest Objective] Disposed: {_targetActionName}");
		}
	}
}