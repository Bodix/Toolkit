using System;

namespace Toolkit.Quests.Example
{
	[Serializable]
	public class LogQuestRewardConfig : QuestRewardConfig
	{
		public string RewardName = "COOL_ITEM";

		public override IQuestReward CreateInstance()
		{
			return new LogQuestReward(this);
		}
	}
}