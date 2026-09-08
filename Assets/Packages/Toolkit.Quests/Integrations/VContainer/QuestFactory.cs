using Toolkit.Quests.Example;
using VContainer;

namespace Toolkit.Quests.VContainer
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
			if (config is DebugQuestObjectiveConfig debugObjective)
			{
				return new DebugQuestObjective(debugObjective.Id, debugObjective.TargetActionName);
			}

			// Example with VContainer injection:
			// if (config is SinkShipObjectiveConfig sinkObjective)
			// {
			// 	var objective = _resolver.Resolve<SinkShipRuntimeObjective>();
			// 	objective.SetupData(sinkObjective);
			// 	return objective;
			// }

			return null;
		}

		public IQuestReward CreateReward(QuestRewardConfig config)
		{
			if (config is LogQuestRewardConfig logReward)
			{
				return new LogQuestReward(logReward.RewardName);
			}

			// Example with VContainer injection:
			// if (config is ItemRewardConfig itemReward)
			// {
			// 	var reward = _resolver.Resolve<ItemQuestReward>();
			// 	reward.SetupData(itemReward);
			// 	return reward;
			// }

			return null;
		}
	}
}