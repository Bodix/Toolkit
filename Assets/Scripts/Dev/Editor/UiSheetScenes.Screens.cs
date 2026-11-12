using Toolkit.Mcp;
using System.Linq;
using System;
using System.Collections.Generic;
using PerfectCore.PerfectFoundation;
using PerfectCore.PerfectQuests;
using PerfectCore.PerfectQuests.UI;
using PerfectCore.PerfectUI.uLayout;
using TMPro;
using UnityEngine;
using static Toolkit.Dev.Editor.UiBuild;
using Object = UnityEngine.Object;

namespace Toolkit.Dev.Editor
{
	/// <summary>
	/// Pages of the sheet for the cards and screens of step 7.2.2: cards, tracker, journal and board, each also empty.
	/// Here is also the list of all pages and the command that captures them, <see cref="Check" />.
	/// </summary>
	public static partial class UiSheetScenes
	{
		private const string LockedReason = "Up to 3 quests at once";

		private const string ShotsFolder = "Screenshots/UiShots";
		private static readonly string[] ShotSizes = { "2520x1080", "1440x1080" };
		// Nested uLayout layouts settle one level per frame.
		private const int SettleFrames = 15;
		// Perfect Quests prefabs nest Perfect UI ones, and the screens nest the blocks.
		private const string BuildSteps =
			"Do:PerfectUiPrefabs.BuildAll; Do:QuestUiPrefabs.BuildBlocks; Do:QuestScreenPrefabs.BuildScreens; NoErrors";

		// The name of a page is the name of its files.
		private static readonly (string name, Func<string> open)[] Pages =
		{
			("blocks_rewards", BlocksRewards),
			("blocks_objectives", BlocksObjectives),
			("screens_cards", Cards),
			("screens_tracker", Tracker),
			("screens_journal", () => Journal()),
			("screens_journal_empty", () => Journal(true)),
			("screens_board", () => Board()),
			("screens_board_empty", () => Board(true)),
			("announcement_base", () => Announcement("")),
			("announcement_banner", () => Announcement("Banner")),
			("announcement_banner_long", () => Announcement("Banner", LongAnnouncementMessage)),
			("announcement_plaque", () => Announcement("Plaque")),
			("announcement_plaque_long", () => Announcement("Plaque", LongAnnouncementMessage)),
			("announcement_plaque_title", () => Announcement("Plaque", null)),
			("announcement_toast", () => Announcement("Toast")),
			("announcement_toast_long", () => Announcement("Toast", LongAnnouncementMessage)),
			("announcement_toast_title", () => Announcement("Toast", null))
		};

		/// <summary>
		/// A whole pass over the UI of the packages in one command: compiles the code, rebuilds the prefabs of Perfect UI
		/// and Perfect Quests, opens the sheet scene, starts Play Mode, captures the pages at every size and stops Play Mode.
		/// Pages are names separated by commas, e.g. "blocks_rewards,screens_board", or "all". A wrong name is answered
		/// with the list of names.
		/// Steps are played after the pages, still in Play Mode, to check logic: steps of <see cref="McpScenario" />,
		/// where "Do" calls a method of this class.
		/// Then run in the shell of the computer, from the project folder:
		/// <c>python3 Assets/Scripts/Dev/Sheet~/ui_sheet.py Screenshots/UiShots Screenshots/UiShots/sheet.png "Title"</c>
		/// It waits for the report of the run and prints it: the errors of compilation or of the build, or the screenshots.
		/// From the screenshots of the pages it builds the sheet image. No report after the wait: run it again.
		/// </summary>
		public static string Check(string pages, string steps = "")
		{
			List<string> names = pages.Trim() == "all"
				? Pages.Select(page => page.name).ToList()
				: pages.Split(',').Select(page => page.Trim()).Where(page => page.Length > 0).ToList();
			List<string> unknown = names.Where(name => Pages.All(page => page.name != name)).ToList();

			if (unknown.Count > 0)
				return $"Unknown pages: {string.Join(", ", unknown)}. Pages: {string.Join(", ", Pages.Select(page => page.name))}";

			if (!Application.isPlaying && UnsavedScene() is string unsaved)
				return unsaved;

			var list = new List<string> { "Stop", "Compile", BuildSteps, "Do:NewScene", "Play" };

			foreach (string name in names)
			{
				list.Add($"Do:Page({name})");

				foreach (string size in ShotSizes)
				{
					list.Add($"Size:{size}");
					list.Add($"Shot:{name}_{size}");
				}
			}

			if (steps.Trim().Length > 0)
				list.Add(steps);

			// The console keeps the errors of Play Mode after it stops.
			list.Add("Stop");
			list.Add("NoErrors");
			return McpScenario.Run(string.Join("; ", list), typeof(UiSheetScenes), ShotsFolder, 1f, SettleFrames);
		}

		/// <summary>Play Mode: opens a page of the sheet by its name.</summary>
		public static void Page(string name)
		{
			foreach ((string page, Func<string> open) in Pages)
			{
				if (page != name)
					continue;

				open();
				return;
			}

			throw new ArgumentException($"Unknown page {name}");
		}

		/// <summary>Play Mode: a full card, compact cards and journal entries: normal, selected and with a long title.</summary>
		public static string Cards()
		{
			Transform columns = Columns(OpenWindow("Quest cards"));
			QuestService service = Service(PirateHunt(), RumDelivery(), LostMap(), LongTitleQuest());
			IReadOnlyList<PerfectCore.PerfectQuests.Quest> quests = service.ActiveQuests;

			Transform full = Slot(columns, "Full card");
			Nested(QuestScreenPrefabs.Card, full).GetComponent<UiQuestView>().Show(quests[2]);

			Transform compact = Slot(columns, "Compact cards");
			Nested(QuestScreenPrefabs.CardCompact, compact).GetComponent<UiQuestView>().Show(quests[1]);
			Nested(QuestScreenPrefabs.CardCompact, compact).GetComponent<UiQuestView>().Show(quests[2]);

			Transform entries = Slot(columns, "Journal entries");
			for (int i = 0; i < quests.Count; i++)
			{
				var entry = Nested(QuestScreenPrefabs.JournalEntry, entries).GetComponent<UiQuestJournalEntry>();
				entry.Show(quests[i]);
				entry.SetSelected(i == 1);
			}

			return "Cards shown";
		}

		/// <summary>Play Mode: the tracker on a HUD with four quests, so the limit of three shows.</summary>
		public static string Tracker()
		{
			Transform canvas = ClearCanvas();
			GameObject tracker = Nested(QuestScreenPrefabs.Tracker, canvas);
			var rect = (RectTransform)tracker.transform;
			rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(1, 1);
			rect.anchoredPosition = new Vector2(-40, -40);
			tracker.GetComponent<UiQuestTracker>().Bind(Service(PirateHunt(), RumDelivery(), LostMap(), Smugglers()));
			return "Tracker shown";
		}

		/// <summary>Play Mode: the journal with quests, the second one selected, or empty.</summary>
		public static string Journal(bool empty = false)
		{
			Transform canvas = ClearCanvas();
			var journal = Nested(QuestScreenPrefabs.Journal, canvas).GetComponent<UiQuestJournal>();
			QuestService service = empty ? Service() : Service(PirateHunt(), RumDelivery(), LostMap(), LongTitleQuest());
			journal.Bind(service);
			if (!empty)
				journal.Select(service.ActiveQuests[1]);
			return empty ? "Empty journal shown" : "Journal shown";
		}

		/// <summary>
		/// Play Mode: the board with four offers: one accepted, one that cannot be accepted, one with a long title.
		/// The short ones go first, so their plate and reason are in the picture. Or empty.
		/// </summary>
		public static string Board(bool empty = false)
		{
			Transform canvas = ClearCanvas();
			var board = Nested(QuestScreenPrefabs.Board, canvas).GetComponent<UiQuestBoard>();
			if (empty)
			{
				board.Setup(null);
				return "Empty board shown";
			}

			QuestDefinition locked = LostMap();
			board.UnavailableReason = quest => quest == locked ? LockedReason : null;
			board.Setup(new[] { Smugglers(), locked, LongTitleQuest(), PirateHunt() });

			// Accepts the first offer the way a player does, so the board shows its plate.
			var first = board.GetComponentInChildren<UiQuestOffer>();
			first.transform.Find("Footer/Buttons/Accept Button").GetComponent<PerfectCore.PerfectUI.UiButton>().Button.onClick.Invoke();
			return "Board shown";
		}

		private static Transform ClearCanvas()
		{
			Transform canvas = Object.FindObjectOfType<Canvas>().transform;
			for (int i = canvas.childCount - 1; i >= 0; i--)
				if (canvas.GetChild(i).name != "Backdrop")
					Object.Destroy(canvas.GetChild(i).gameObject);
			return canvas;
		}

		/// <summary>A column of the page with a caption and no plate: the cards bring their own.</summary>
		private static Transform Slot(Transform parent, string caption)
		{
			GameObject slot = Child(caption, parent);
			Column(slot, 24, SizingMode.Grow, SizingMode.FitContent);
			GameObject title = Child("Caption", slot.transform);
			Text(title, caption, 32, Muted);
			Item<LayoutText>(title, SizingMode.Grow, SizingMode.FitContent);
			return slot.transform;
		}

		private static QuestService Service(params QuestDefinition[] definitions)
		{
			var service = new QuestService(null, new SheetFactory());
			foreach (QuestDefinition definition in definitions)
				service.RestoreQuest(SheetStates(definition));
			return service;
		}

		/// <summary>The state of a quest in progress: objectives marked done in the definition are completed.</summary>
		private static QuestState SheetStates(QuestDefinition definition)
		{
			QuestState state = QuestState.Create(definition);
			state.Status = QuestStatus.Active;
			for (int i = 0; i < definition.Objectives.Count; i++)
			{
				if (definition.Objectives[i] is SheetObjective sheet)
				{
					state.ObjectiveStates[i].IsCompleted = sheet.Done;
					state.ObjectiveStates[i].IsFailed = sheet.Failed;
				}
				else if (state.ObjectiveStates[i] is TimeLimitObjectiveState time)
				{
					time.RemainingTime = 90;
				}
			}

			return state;
		}

		private static QuestDefinition PirateHunt()
		{
			var quest = Definition("Pirate Hunt", "Pirates raid the trade route east of the port. Sink them and bring back their flags.",
				Gold(250), TextReward("Reputation with the Merchants Guild +10"));
			quest.Objectives.Add(Objective("Sink 3 pirate ships", 1 / 3f, "1/3"));
			quest.Objectives.Add(new TimeLimitObjective { Duration = 120, Optional = true, BonusRewards = new List<QuestReward> { Gold(50) } });
			return quest;
		}

		private static QuestDefinition RumDelivery()
		{
			var quest = Definition("Rum Delivery", "The tavern of the next port ran dry. Bring the rum before the sailors riot.",
				ItemReward("ExampleItem2", 3), Gold(120));
			quest.Objectives.Add(Objective("Deliver 20 crates of rum", 1, "20/20", done: true));
			quest.Objectives.Add(Objective("Escort the convoy", 0.4f, "2/5"));
			SheetObjective hull = Objective("Keep at least half of the hull", 0.5f, "50%");
			hull.Optional = true;
			hull.Failed = true;
			quest.Objectives.Add(hull);
			return quest;
		}

		private static QuestDefinition LostMap()
		{
			var quest = Definition("The Lost Map", "An old captain sold you a torn map. The other pieces lie in wrecks.",
				TextReward("Map of the Guardian's lair"));
			quest.Objectives.Add(Objective("Find the lost map fragments hidden in the shipwrecks along the northern coast", 0.4f, "2/5"));
			return quest;
		}

		private static QuestDefinition LongTitleQuest()
		{
			var quest = Definition("Escort the Merchant Convoy Through the Stormy Northern Straits Before the Winter Comes",
				"A long title wraps to two lines and ends with an ellipsis.", Gold(400), ItemReward("ExampleItem2", 2),
				ItemReward("ExampleItem1", 12000), TextReward("Reputation +5"));
			quest.Objectives.Add(Objective("Meet the convoy at the lighthouse", 0, "0/1"));
			return quest;
		}

		private static QuestDefinition Smugglers()
		{
			var quest = Definition("Smugglers' Cove", "Hidden in the tracker: it shows three quests at most.", Gold(80));
			quest.Objectives.Add(Objective("Sink the smuggler", 0, "0/1"));
			return quest;
		}

		private static QuestDefinition Definition(string title, string description, params QuestReward[] rewards) =>
			new QuestDefinition { Title = title, Description = description, Rewards = new List<QuestReward>(rewards) };

		private static SheetObjective Objective(string text, float progress, string progressText, bool done = false) =>
			new SheetObjective { Text = text, Progress = progress, ProgressText = progressText, Done = done };

		private sealed class SheetFactory : IQuestFactory
		{
			public IQuestObjectiveTracker CreateTracker(QuestObjective objective) => new SheetTracker();
			public IQuestRewardApplier CreateApplier(QuestReward reward) => null;
		}

		/// <summary>Keeps the state as it is: the sheet shows quests, nothing plays them.</summary>
		private sealed class SheetTracker : IQuestObjectiveTracker
		{
			public QuestObjectiveState State { get; private set; }

			public void Initialize(IEventBus eventBus, QuestObjectiveState state, Action onProgressUpdated) => State = state;

			public void Dispose()
			{
			}
		}
	}
}
