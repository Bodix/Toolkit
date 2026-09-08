using System;
using Bodix.Evolunity.Patterns;

namespace Toolkit.Quests
{
	/// <summary>
	/// Defines the base contract for any quest objective logic.
	/// </summary>
	public interface IQuestObjective
	{
		QuestObjectiveState State { get; }

		void Initialize(IEventBus eventBus, QuestObjectiveState state, Action onProgressUpdated);
		void Dispose();
	}
}
