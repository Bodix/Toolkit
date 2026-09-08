using UnityEngine;

namespace Toolkit.Quests
{
	public class LogReward : QuestRewardConfig
	{
		public string Reward = "COOL_ITEM";

		public void GrantReward()
		{
			Debug.Log("Rewarded " + Reward);
		}
	}
}