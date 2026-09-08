using VContainer;

namespace Toolkit.Quests.Integrations
{
	public class QuestFactory : IQuestFactory
	{
		private readonly IObjectResolver _resolver;

		public QuestFactory(IObjectResolver resolver)
		{
			_resolver = resolver;
		}

		public IQuestObjective CreateObjective(QuestObjectiveConfig config)
		{
			// if (definition is SinkShipObjectiveDefinition sinkObjective)
			// {
			// 	// Let VContainer inject event buses or other dependencies.
			// 	var objective = _resolver.Resolve<SinkShipRuntimeObjective>();
			// 	objective.SetupData(sinkObjective);
			// 	return objective;
			// }

			return null;
		}

		public IQuestReward CreateReward(QuestRewardConfig config)
		{
			// Pattern matching to create the specific runtime reward logic.
			// if (definition is ItemRewardDefinition itemReward)
			// {
			// 	// Passing the data directly to the runtime object.
			// 	return new ItemQuestReward(itemReward.Item, itemReward.Count);
			// }

			return null;
		}
	}
}