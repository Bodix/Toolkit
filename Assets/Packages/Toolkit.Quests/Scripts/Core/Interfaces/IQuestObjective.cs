using System;
using Bodix.Evolunity.Patterns;

namespace Toolkit.Quests
{
	/// <summary>
	/// Defines the base contract for any quest objective.
	/// </summary>
	public interface IQuestObjective
	{
		bool IsCompleted { get; }

		void Initialize(IEventBus eventBus, Action onProgressUpdated);
		void Dispose();
	}
}
