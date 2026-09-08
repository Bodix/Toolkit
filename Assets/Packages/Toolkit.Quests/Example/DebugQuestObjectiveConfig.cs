using System;

namespace Toolkit.Quests.Example
{
	[Serializable]
	public class DebugQuestObjectiveConfig : QuestObjectiveConfig
	{
		public string TargetActionName = "Click the button";

		public override IQuestObjective CreateInstance()
		{
			return new DebugQuestObjective(this);
		}
	}
}