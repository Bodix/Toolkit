using UnityEngine;

namespace Toolkit.Quests.Example
{
	public class LogQuestReward : IQuestReward
	{
		private readonly string _rewardName;

		public LogQuestReward(string rewardName)
		{
			_rewardName = rewardName;
		}

		public void GrantReward()
		{
			Debug.Log($"[Quest Reward] Rewarded: {_rewardName}");
		}
	}
}