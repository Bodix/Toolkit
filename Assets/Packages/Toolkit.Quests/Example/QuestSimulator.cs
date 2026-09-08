using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using VContainer;

namespace Toolkit.Quests.Example
{
	public class QuestSimulator : MonoBehaviour
	{
		[SerializeField]
		private QuestConfig _testQuest;

		private QuestService _questService;

		[Inject]
		public void Construct(QuestService questService)
		{
			_questService = questService;
		}

		[Button("1. Accept Quest")]
		public void AcceptQuest()
		{
			if (_testQuest == null)
			{
				Debug.LogError("[Quest Simulator] Assign a QuestConfig to Test Quest field first!");
				return;
			}
			
			if (_questService.ActiveQuests.Any(q => q.Config == _testQuest))
			{
				Debug.LogWarning("[Quest Simulator] Quest is already active.");
				return;
			}

			_questService.AcceptQuest(_testQuest);
			Debug.Log($"[Quest Simulator] Accepted Quest: {_testQuest.Title}");
		}

		[Button("2. Complete First Pending Objective")]
		public void CompleteFirstObjective()
		{
			if (_testQuest == null) return;

			var instance = _questService.ActiveQuests.FirstOrDefault(q => q.Config == _testQuest);
			if (instance == null)
			{
				Debug.LogWarning("[Quest Simulator] Quest is not active.");
				return;
			}

			var objective = instance.Objectives
				.OfType<LogQuestObjective>()
				.FirstOrDefault(o => !o.State.IsCompleted);

			if (objective != null)
			{
				objective.CompleteManually();
			}
			else
			{
				Debug.LogWarning("[Quest Simulator] No incomplete DebugQuestObjective found.");
			}
		}

		[Button("3. Cancel Quest")]
		public void CancelQuest()
		{
			if (_testQuest == null) return;

			var instance = _questService.ActiveQuests.FirstOrDefault(q => q.Config == _testQuest);
			if (instance != null)
			{
				_questService.CancelQuest(instance);
				Debug.Log($"[Quest Simulator] Canceled Quest: {_testQuest.Title}");
			}
			else
			{
				Debug.LogWarning("[Quest Simulator] Quest is not active.");
			}
		}

		[Button("4. Dispose All Quests")]
		public void DisposeAll()
		{
			_questService.DisposeAll();
			Debug.Log("[Quest Simulator] Disposed all active quests.");
		}
	}
}