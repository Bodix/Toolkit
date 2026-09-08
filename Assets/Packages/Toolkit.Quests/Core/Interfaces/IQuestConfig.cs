namespace Toolkit.Quests
{
	public interface IQuestConfig
	{
		string Id { get; }
		string Title { get; }
		string Description { get; }
		QuestObjectiveConfig[] Objectives { get; }
		QuestRewardConfig[] Rewards { get; }
	}
}