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
			if (config == null)
				return null;

			IQuestObjective objective = config.CreateInstance();
			if (objective != null)
				_resolver.Inject(objective);

			return objective;
		}

		public IQuestReward CreateReward(QuestRewardConfig config)
		{
			if (config == null)
				return null;

			IQuestReward reward = config.CreateInstance();
			if (reward != null)
				_resolver.Inject(reward);

			return reward;
		}
	}
}
