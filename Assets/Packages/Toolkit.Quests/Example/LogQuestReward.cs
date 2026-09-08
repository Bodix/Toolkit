using UnityEngine;

namespace Toolkit.Quests.Example
{
	public class LogQuestReward : IQuestReward
	{
		private readonly LogQuestRewardConfig _config;

		public LogQuestReward(LogQuestRewardConfig config)
		{
			_config = config;
		}

		public void GrantReward()
		{
			Debug.Log($"[Quest Reward] Rewarded: {_config.RewardName}");
		}
	}
}