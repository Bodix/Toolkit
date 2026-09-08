using System.Collections.Generic;

namespace Toolkit.Quests
{
	public interface IQuestConfig
	{
		string Id { get; }
		string Title { get; }
		string Description { get; }
		List<QuestObjectiveConfig> Objectives { get; }
		QuestRewardConfig[] Rewards { get; }
	}
}