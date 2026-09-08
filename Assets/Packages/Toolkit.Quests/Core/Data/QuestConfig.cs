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
		private QuestObjectiveConfig[] _objectives;
		[SerializeField]
		private QuestRewardConfig[] _rewards;

		public string Title => _title;
		public string Description => _description;
		/// <summary>
		/// List of objectives that the factory will build.
		/// </summary>
		public QuestObjectiveConfig[] Objectives => _objectives;
		/// <summary>
		/// List of objectives that the factory will build.
		/// </summary>
		public QuestRewardConfig[] Rewards => _rewards;
	}
}