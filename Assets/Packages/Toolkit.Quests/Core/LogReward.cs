namespace Toolkit.Quests
{
	public class LogReward : QuestRewardConfig
	{
		public void GrantReward()
		{
			UnityEngine.Debug.Log("rewards");
		}
	}
}