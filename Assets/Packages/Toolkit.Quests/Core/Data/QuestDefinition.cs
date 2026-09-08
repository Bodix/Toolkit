using UnityEngine;

namespace Toolkit.Quests
{
	/// <summary>
	/// Contains static data for a quest template.
	/// </summary>
	[CreateAssetMenu(fileName = "NewQuest", menuName = "Universal Quests/Quest Definition")]
	public class QuestDefinition : ScriptableObject
	{
		public string Id;
		public string Title;
		[TextArea]
		public string Description;

		/// <summary>
		/// List of objective IDs that the factory will build.
		/// </summary>
		public string[] ObjectiveIds;

		/// <summary>
		/// List of objective IDs that the factory will build.
		/// </summary>
		public string[] RewardIds;
	}
}