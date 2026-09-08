using Bodix.Evolunity.Patterns;
using Toolkit.Quests.VContainer;
using VContainer;
using VContainer.Unity;

namespace Toolkit.Quests.Example
{
	public class QuestSimulationScope : LifetimeScope
	{
		protected override void Configure(IContainerBuilder builder)
		{
			// Register a dummy event bus or you can use your actual event bus
			builder.Register<IEventBus, DummyEventBus>(Lifetime.Singleton);
			
			// Register the QuestFactory implementation
			builder.Register<IQuestFactory, QuestFactory>(Lifetime.Singleton);
			
			// Register the QuestService
			builder.Register<QuestService>(Lifetime.Singleton);

			// Inject into our simulator
			builder.RegisterComponentInHierarchy<QuestSimulator>();
		}
	}
}