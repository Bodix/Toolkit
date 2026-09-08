using Bodix.Evolunity.Collections;
using UnityEngine;

namespace Toolkit.Quests
{
	/// <summary>
	/// Contains static data for a quest template.
	/// </summary>
	[CreateAssetMenu(fileName = "NewQuest", menuName = "Configs/Quest")]
	public class QuestConfig : DataAsset, IQuestConfig
	{
		[SerializeField]
		private string _title;
		[TextArea]
		[SerializeField]
		private string _description;
		[SerializeField]
		private string[] _objectiveIds;
		[SerializeField]
		private string[] _rewardIds;

		public string Title => _title;
		public string Description => _description;
		/// <summary>
		/// List of objective IDs that the factory will build.
		/// </summary>
		public string[] ObjectiveIds => _objectiveIds;
		/// <summary>
		/// List of objective IDs that the factory will build.
		/// </summary>
		public string[] RewardIds => _rewardIds;
	}
}