using UnityEngine;

namespace Toolkit.Quests.Example
{
	public class LogQuestObjective : QuestObjective<QuestObjectiveState>
	{
		private readonly LogQuestObjectiveConfig _config;

		public LogQuestObjective(LogQuestObjectiveConfig config)
		{
			_config = config;
		}

		protected override void OnInit()
		{
			Debug.Log($"[Quest Objective] Initialized: {_config.Log}");
		}

		public void CompleteManually()
		{
			if (State.IsCompleted)
				return;

			State.IsCompleted = true;
			NotifyProgressUpdated();

			Debug.Log($"[Quest Objective] Completed: {_config.Log}");
		}

		public override void Dispose()
		{
			Debug.Log($"[Quest Objective] Disposed: {_config.Log}");
		}
	}
}