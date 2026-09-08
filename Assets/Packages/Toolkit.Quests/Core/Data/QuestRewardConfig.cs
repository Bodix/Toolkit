using Bodix.Evolunity.Collections;

namespace Toolkit.Quests
{
	public abstract class QuestRewardConfig : DataAsset
	{
		/// <summary>
		/// Creates the runtime instance of this reward.
		/// </summary>
		public abstract IQuestReward CreateInstance();
	}
}