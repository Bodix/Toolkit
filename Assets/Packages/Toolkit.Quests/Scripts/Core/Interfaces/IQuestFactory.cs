namespace Toolkit.Quests
{
	public interface IQuestFactory
	{
		IQuestObjective CreateObjective(QuestObjectiveConfig config);
		IQuestReward CreateReward(QuestRewardConfig config);
	}
}
