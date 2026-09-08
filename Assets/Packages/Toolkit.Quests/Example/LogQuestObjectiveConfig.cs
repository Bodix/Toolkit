using System;

namespace Toolkit.Quests.Example
{
	[Serializable]
	public class LogQuestObjectiveConfig : QuestObjectiveConfig
	{
		public string Log = "Click the button";

		public override IQuestObjective CreateInstance()
		{
			return new LogQuestObjective(this);
		}
	}
}