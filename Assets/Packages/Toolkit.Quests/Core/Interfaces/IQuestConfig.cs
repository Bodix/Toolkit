namespace Toolkit.Quests
{
	public interface IQuestConfig
	{
		string Id { get; }
		string Title { get; }
		string Description { get; }
		string[] ObjectiveIds { get; }
		string[] RewardIds { get; }
	}
}