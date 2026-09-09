using System;
using Bodix.Evolunity.Patterns;

namespace Toolkit.Quests
{
	/// <summary>
	/// Base class for quest objectives that provides strongly-typed access to its state.
	/// </summary>
	public abstract class QuestObjective<TState> : IQuestObjective where TState : QuestObjectiveState
	{
		private Action _onProgressUpdated;

		// Explicit interface implementation to hide the non-generic state from the inheritor.
		QuestObjectiveState IQuestObjective.State => State;
		public TState State { get; private set; }
		protected IEventBus EventBus { get; private set; }

		public void Initialize(IEventBus eventBus, QuestObjectiveState state, Action onProgressUpdated)
		{
			EventBus = eventBus;
			State = (TState)state;
			_onProgressUpdated = onProgressUpdated;

			OnInit();
		}

		/// <summary>
		/// Called after the objective has been initialized and the state is set.
		/// </summary>
		protected virtual void OnInit() { }

		public abstract void Dispose();

		/// <summary>
		/// Notifies the quest system that the state has changed (e.g. progress increased or completed).
		/// </summary>
		protected void NotifyProgressUpdated()
		{
			_onProgressUpdated?.Invoke();
		}
	}
}