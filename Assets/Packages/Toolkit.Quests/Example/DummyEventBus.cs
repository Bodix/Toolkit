using System;
using Bodix.Evolunity.Patterns;

namespace Toolkit.Quests.Example
{
	/// <summary>
	/// Dummy implementation of IEventBus for simulation purposes.
	/// </summary>
	public class DummyEventBus : IEventBus
	{
		public void Subscribe<T>(Action<T> handler)
		{
		}

		public void Unsubscribe<T>(Action<T> handler)
		{
		}
	}
}