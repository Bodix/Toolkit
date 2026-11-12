using PerfectCore.PerfectUI;
using UnityEngine;
using static Toolkit.Dev.Editor.UiBuild;

namespace Toolkit.Dev.Editor
{
	/// <summary>
	/// Pages of the sheet for the announcements of Perfect UI: every kind with a short and a long text.
	/// </summary>
	public static partial class UiSheetScenes
	{
		private const string AnnouncementTitle = "Quest Completed";
		private const string LongAnnouncementMessage =
			"Deliver Forty Barrels of the Finest Caribbean Rum to the Governor of Port Royal Before the Storm Season Begins";

		/// <summary>
		/// Play Mode: one announcement shown at once, without the announcer.
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
	}
}
