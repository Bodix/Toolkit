using System.Collections.Generic;
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
		private List<QuestObjectiveConfig> _objectives = new List<QuestObjectiveConfig>();
		[SerializeField]
		private List<QuestRewardConfig> _rewards = new List<QuestRewardConfig>();

		public string Title => _title;
		public string Description => _description;
		/// <summary>
		/// List of objectives that the factory will build.
		/// </summary>
		public List<QuestObjectiveConfig> Objectives => _objectives;
		/// <summary>
		/// List of rewards that the factory will build.
		/// </summary>
		public List<QuestRewardConfig> Rewards => _rewards;
	}
}
