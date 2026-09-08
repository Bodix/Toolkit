namespace Toolkit.Quests
{
	public interface IQuestFactory
	{
		IQuestObjective CreateObjective(string objectiveId);
		IQuestReward CreateReward(string rewardId);
	}
}