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
	/// Same order as the other pages: NewScene() in Edit Mode, then Play Mode, then a page, then UiShots.Capture.
	/// </summary>
	public static partial class UiSheetScenes
	{
		private const string LockedReason = "Up to 3 quests at once";

		private static readonly Queue<string> _pages = new Queue<string>();
		private static readonly List<string> _shot = new List<string>();

		/// <summary>
		/// Play Mode: opens each page and captures it, one after another, e.g. "blocks_rewards,screens_board".
		/// The name of a page is the name of its files. Poll <see cref="CaptureAllStatus" /> until it says done.
		/// </summary>
		public static string CaptureAll(string pages)
		{
			if (!Application.isPlaying)
				return "CaptureAll needs Play Mode";

			_pages.Clear();
			_shot.Clear();
			foreach (string page in pages.Split(','))
				if (OpenPage(page.Trim()) != null)
					_pages.Enqueue(page.Trim());

			EditorApplicationUpdate(true);
			return $"Pages queued: {_pages.Count}";
		}

		/// <summary>"busy N left" or "done" with the captured pages.</summary>
		public static string CaptureAllStatus()
		{
			return _pages.Count > 0 || UiShots.IsBusy ? $"busy {_pages.Count} left" : "done: " + string.Join(", ", _shot);
		}

		/// <summary>Opens a page by the name of its files, or checks the name when only checking. Null for an unknown name.</summary>
		private static string OpenPage(string page, bool open = false)
		{
			Func<string> action;
			switch (page)
			{
				case "blocks_rewards": action = BlocksRewards; break;
				case "blocks_objectives": action = BlocksObjectives; break;
				case "screens_cards": action = Cards; break;
				case "screens_tracker": action = Tracker; break;
				case "screens_journal": action = () => Journal(); break;
				case "screens_journal_empty": action = () => Journal(true); break;
				case "screens_board": action = () => Board(); break;
				case "screens_board_empty": action = () => Board(true); break;
				default: return null;
			}

			return open ? action() : page;
		}

		private static void EditorApplicationUpdate(bool subscribe)
		{
			UnityEditor.EditorApplication.update -= NextPage;
			if (subscribe)
				UnityEditor.EditorApplication.update += NextPage;
		}

		private static void NextPage()
		{
			if (!Application.isPlaying)
			{
				_pages.Clear();
				EditorApplicationUpdate(false);
				return;
			}

			if (UiShots.IsBusy)
				return;
			if (_pages.Count == 0)
			{
				EditorApplicationUpdate(false);
				return;
			}

			string page = _pages.Dequeue();
			OpenPage(page, true);
			UiShots.Capture(page);
			_shot.Add(page);
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
