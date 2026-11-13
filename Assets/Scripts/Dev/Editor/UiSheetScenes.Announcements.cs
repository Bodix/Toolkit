using PerfectCore.PerfectQuests;
using PerfectCore.PerfectQuests.UI;
using PerfectCore.PerfectUI;
using UnityEngine;
using static Toolkit.Dev.Editor.UiBuild;

namespace Toolkit.Dev.Editor
{
	/// <summary>
	/// Pages of the sheet for the announcements of Perfect UI: every kind with a short and a long text.
	/// And for the announcer of Perfect Quests.
	/// </summary>
	public static partial class UiSheetScenes
	{
		private const string AnnouncementTitle = "Quest Completed";
		private const string LongAnnouncementMessage =
			"Deliver Forty Barrels of the Finest Caribbean Rum to the Governor of Port Royal Before the Storm Season Begins";

		/// <summary>
		/// Play Mode: one announcement shown at once, without the announcement service.
		/// Kind: "Banner", "Plaque", "Toast" or "" for the base. A null message leaves only the title.
		/// </summary>
		public static string Announcement(string kind, string message = "Rum Run", string title = AnnouncementTitle)
		{
			Transform canvas = ClearCanvas();
			string name = string.IsNullOrEmpty(kind) ? "Announcement" : $"Announcement ({kind})";
			var announcement = Nested(UiPrefabs + name + ".prefab", canvas).GetComponent<UiAnnouncement>();
			announcement.Show(new UiAnnouncementPayload { Title = title, Message = message }, instantly: true);
			return name + " shown";
		}

		/// <summary>
		/// Play Mode: the announcer of Perfect Quests after quests are completed at once, their titles separated by "|".
		/// It announces them one after another. With a filter, only the quests with this word in the title.
		/// </summary>
		public static string QuestAnnouncer(string titles = "Rum Run", string filter = "")
		{
			Transform canvas = ClearCanvas();
			var announcer = Nested(QuestUiPrefabs.Announcer, canvas).GetComponent<UiQuestAnnouncer>();
			if (filter.Length > 0)
				announcer.Filter = quest => quest.Definition.Title.Contains(filter);

			var service = new QuestService(null, new SheetFactory());
			announcer.Bind(service);
			// A quest without objectives is completed as soon as it is accepted.
			foreach (string title in titles.Split('|'))
				service.AcceptQuest(new QuestDefinition { Title = title });
			return "Quest announcer: " + titles;
		}

		/// <summary>Play Mode: the texts of the announcement on the screen now, for the steps that check logic.</summary>
		public static string ShownAnnouncement()
		{
			var announcement = Object.FindObjectOfType<UiAnnouncement>();
			if (announcement == null || announcement.IsHidden)
				return "No announcement";

			string message = announcement.MessageText.gameObject.activeSelf ? announcement.MessageText.text : "";
			return $"Announcement: {announcement.TitleText.text} / {message}";
		}
	}
}
