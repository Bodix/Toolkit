using PerfectCore.PerfectQuests.UI;
using PerfectCore.PerfectUI;
using PerfectCore.PerfectUI.uLayout;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Toolkit.Dev.Editor.UiBuild;
using ULayout = PerfectCore.PerfectUI.uLayout.Layout;

namespace Toolkit.Dev.Editor
{
	/// <summary>
	/// Builders of the Perfect Quests cards and screens in the Perfect UI style (D-112): cards, tracker, journal,
	/// offer and board. They nest the blocks, so run <see cref="QuestUiPrefabs.BuildBlocks" /> first. Called from Unity MCP.
	/// </summary>
	public static class QuestScreenPrefabs
	{
		public const string ObjectiveCompact = QuestPrefabs + "Quest Objective (Compact).prefab";
		public const string ObjectiveListCompact = QuestPrefabs + "Quest Objective List (Compact).prefab";
		public const string Card = QuestPrefabs + "Quest Card.prefab";
		public const string CardCompact = QuestPrefabs + "Quest Card (Compact).prefab";
		public const string Tracker = QuestPrefabs + "Quest Tracker.prefab";
		public const string JournalEntry = QuestPrefabs + "Quest Journal Entry.prefab";
		public const string Journal = QuestPrefabs + "Quest Journal.prefab";
		public const string Offer = QuestPrefabs + "Quest Offer.prefab";
		public const string Board = QuestPrefabs + "Quest Board.prefab";

		private const float CardPadding = 30;
		private const float CardGap = 24;
		private const float ListGap = 30;

		/// <summary>The panel of the tracker over the game: darker than a card, so it reads on a bright sea.</summary>
		private static readonly Color Shade = Hex("00000080");
		/// <summary>The colors of a Perfect UI tab: a gray plate, a white one when selected, dark text on both.</summary>
		private static readonly Color TabPlate = Hex("999999");
		private static readonly Color TabText = Hex("4C4C4C");

		/// <summary>Builds the compact objective, the cards, tracker, journal, offer and board.</summary>
		public static string BuildScreens()
		{
			try
			{
				QuestUiPrefabs.BuildObjective("Quest Objective (Compact)", ObjectiveCompact, QuestUiPrefabs.CompactObjective);
				QuestUiPrefabs.BuildObjectiveList("Quest Objective List (Compact)", ObjectiveListCompact, ObjectiveCompact, 12);
				BuildCard();
				BuildCardCompact();
				BuildTracker();
				BuildJournalEntry();
				BuildJournal();
				BuildOffer();
				BuildBoard();
			}
			finally
			{
				CloseScene();
			}

			return "Quest screens: compact objective, cards, tracker, journal entry, journal, offer, board";
		}

		private static void BuildCard()
		{
			GameObject root = Root("Quest Card");
			Size(root, 800, 600);
			Padding(Column(root, CardGap, SizingMode.Grow, SizingMode.FitContent), CardPadding);
			Background(root.transform, Dark, ppu: 7);

			UiText title = TextBlock(root.transform, "Title Text", "Quest title", 44, White);
			UiText description = TextBlock(root.transform, "Description Text", "Description", 32, Muted);
			UiQuestObjectiveList objectives = ObjectiveSection(root.transform);
			GameObject rewards = Nested(QuestUiPrefabs.RewardList, root.transform, "Rewards");

			var view = root.AddComponent<UiQuestView>();
			Set(view, "_titleText", title);
			Set(view, "_descriptionText", description);
			Set(view, "_objectives", objectives);
			Set(view, "_rewards", rewards.GetComponent<UiQuestRewardList>());
			Save(root, Card);
		}

		private static void BuildCardCompact()
		{
			GameObject root = Root("Quest Card (Compact)");
			Size(root, 500, 200);
			Column(root, 12, SizingMode.Grow, SizingMode.FitContent);

			// One line: the tracker is narrow, and the journal has the full title.
			GameObject titleObject = Child("Title Text", root.transform);
			TextMeshProUGUI titleText = Text(titleObject, "Quest title", 32, White);
			Item<LayoutText>(titleObject, SizingMode.Grow, SizingMode.FitContent);
			var title = titleObject.AddComponent<UiText>();
			Set(title, "text", titleText);

			GameObject objectives = Nested(ObjectiveListCompact, root.transform, "Objectives");

			var view = root.AddComponent<UiQuestView>();
			Set(view, "_titleText", title);
			Set(view, "_objectives", objectives.GetComponent<UiQuestObjectiveList>());
			Save(root, CardCompact);
		}

		private static void BuildTracker()
		{
			// The panel hangs from the top of the root and grows down with the quests.
			GameObject root = Root("Quest Tracker");
			Size(root, 480, 600);
			GameObject panel = Child("Panel", root.transform);
			RectTransform panelRect = Rt(panel);
			panelRect.anchorMin = new Vector2(0, 1);
			panelRect.anchorMax = new Vector2(1, 1);
			panelRect.pivot = new Vector2(0.5f, 1);
			panelRect.anchoredPosition = Vector2.zero;
			panelRect.sizeDelta = new Vector2(0, 200);
			Padding(Column(panel, ListGap, SizingMode.Grow, SizingMode.FitContent), 24);
			Background(panel.transform, Shade, ppu: 7);

			var tracker = root.AddComponent<UiQuestTracker>();
			Set(tracker, "_container", panel.transform);
			Set(tracker, "_entryPrefab", Load<GameObject>(CardCompact).GetComponent<UiQuestView>());
			Set(tracker, "_maxEntries", 3);
			Set(tracker, "_group", panel);
			Save(root, Tracker);
		}

		private static void BuildJournalEntry()
		{
			GameObject root = Root("Quest Journal Entry");
			Size(root, 500, 90);
			Padding(Row(root, 16, SizingMode.Grow, SizingMode.FitContent), 30, 30, 22, 22);
			// Background ends up first: each call puts its plate at the back.
			Image selected = Background(root.transform, White, name: "Selected Mark");
			Image plate = Background(root.transform, TabPlate, raycast: true);

			GameObject titleObject = Child("Title Text", root.transform);
			TextMeshProUGUI titleText = Text(titleObject, "Quest title", 32, TabText);
			Item<LayoutText>(titleObject, SizingMode.Grow, SizingMode.FitContent);
			var title = titleObject.AddComponent<UiText>();
			Set(title, "text", titleText);

			GameObject progressObject = Child("Progress Text", root.transform);
			TextMeshProUGUI progressText = Text(progressObject, "0/0", 28, TabText, TextAlignmentOptions.Right);
			Item<LayoutText>(progressObject, SizingMode.FitContent, SizingMode.FitContent);
			var progress = progressObject.AddComponent<UiText>();
			Set(progress, "text", progressText);

			var button = root.AddComponent<ObservableButton>();
			button.targetGraphic = plate;
			button.navigation = new Navigation { mode = Navigation.Mode.None };
			var uiButton = root.AddComponent<UiButton>();
			Set(uiButton, "button", button);
			Set(uiButton, "background", plate);

			var entry = root.AddComponent<UiQuestJournalEntry>();
			Set(entry, "_button", uiButton);
			Set(entry, "_titleText", title);
			Set(entry, "_progressText", progress);
			Set(entry, "_selectedMark", selected.gameObject);
			Save(root, JournalEntry);
		}

		private static void BuildJournal()
		{
			GameObject root = VariantRoot(UiPrefabs + "Window.prefab", "Quest Journal");
			root.transform.Find("Panel/Header/Title Text").GetComponent<TMP_Text>().text = "Journal";
			Transform content = root.transform.Find("Panel/Content");

			// The list and the card share the width: the list takes more of a narrow screen, so titles stay readable.
			GameObject list = ScrollView(content, "List Scroll View");
			RectInterpolator(list, rt => Split(rt, 0, 0.3f, 0, -ListGap / 2), rt => Split(rt, 0, 0.4f, 0, -ListGap / 2));
			GameObject entries = list.transform.Find("Viewport/Content").gameObject;
			Padding(Column(entries, 12, SizingMode.Fixed, SizingMode.FitContent), 0, ListGap, 0, 0);

			GameObject details = Child("Details", content);
			RectInterpolator(details, rt => Split(rt, 0.3f, 1, ListGap / 2, 0), rt => Split(rt, 0.4f, 1, ListGap / 2, 0));
			Column(details, CardGap, SizingMode.Fixed, SizingMode.Fixed);
			GameObject cardScroll = ScrollView(details.transform, "Card Scroll View");
			Item<LayoutItem>(cardScroll, SizingMode.Grow, SizingMode.Grow);
			GameObject cardContent = cardScroll.transform.Find("Viewport/Content").gameObject;
			Padding(Column(cardContent, 0, SizingMode.Fixed, SizingMode.FitContent), 0, ListGap, 0, 0);
			GameObject card = Nested(Card, cardContent.transform, "Quest Card");

			GameObject buttons = Child("Buttons", details.transform);
			Row(buttons, 20, SizingMode.Grow, SizingMode.FitContent, justify: ULayout.Justification.End);
			GameObject abandon = IconTextButton(buttons.transform, "Abandon Button", "Abandon", "close-colored");
			Size(abandon, 360, 100);
			Item<LayoutItem>(abandon, SizingMode.Fixed, SizingMode.Fixed);

			GameObject empty = EmptyState(content, "No active quests\nAccept one on the quest board");

			var journal = root.AddComponent<UiQuestJournal>();
			Set(journal, "_entriesContainer", entries.transform);
			Set(journal, "_entryPrefab", Load<GameObject>(JournalEntry).GetComponent<UiQuestJournalEntry>());
			Set(journal, "_emptyState", empty);
			Set(journal, "_details", card.GetComponent<UiQuestView>());
			Set(journal, "_abandonButton", abandon.GetComponent<UiButton>());
			Save(root, Journal);
		}

		private static void BuildOffer()
		{
			// The board sets the width of an offer, see LayoutColumns. The status plate sits in the middle.
			GameObject root = Root("Quest Offer");
			Size(root, 600, 800);
			Padding(Column(root, CardGap, SizingMode.Fixed, SizingMode.FitContent, ULayout.Alignment.Center), CardPadding);
			Background(root.transform, Dark, ppu: 7);

			UiText title = TextBlock(root.transform, "Title Text", "Quest title", 44, White, maxLines: 2);
			UiText description = TextBlock(root.transform, "Description Text", "Description", 32, Muted);
			UiQuestObjectiveList objectives = ObjectiveSection(root.transform);
			GameObject rewards = Nested(QuestUiPrefabs.RewardList, root.transform, "Rewards");

			GameObject footer = Child("Footer", root.transform);
			Column(footer, 12, SizingMode.Grow, SizingMode.FitContent, ULayout.Alignment.Center);
			// Side by side while each button has room for its label, one under the other in a narrow card.
			GameObject buttons = Child("Buttons", footer.transform);
			Wrap(Row(buttons, 20, SizingMode.Grow, SizingMode.FitContent), 16);
			var buttonColumns = buttons.AddComponent<LayoutColumns>();
			Set(buttonColumns, "minColumnWidth", 240f);
			Set(buttonColumns, "maxColumns", 2);
			GameObject decline = OfferButton(buttons.transform, "Decline Button", "Decline", "close-colored");
			GameObject accept = OfferButton(buttons.transform, "Accept Button", "Accept", "check-mark-colored");
			UiText unavailable = TextBlock(footer.transform, "Unavailable Text", "Why the quest cannot be accepted", 28, Muted);
			unavailable.Text.alignment = TextAlignmentOptions.Center;
			unavailable.gameObject.SetActive(false);

			// As high as the buttons, so the card keeps its height when the plate replaces them.
			GameObject status = Nested(UiPrefabs + "Label.prefab", root.transform, "Status Plate");
			status.GetComponentInChildren<TMP_Text>().text = "Quest accepted";
			UiBuild.Margins(status.GetComponent<ULayout>(), 0, 0, 25, 26);
			status.SetActive(false);

			var offer = root.AddComponent<UiQuestOffer>();
			Set(offer, "_titleText", title);
			Set(offer, "_descriptionText", description);
			Set(offer, "_objectives", objectives);
			Set(offer, "_rewards", rewards.GetComponent<UiQuestRewardList>());
			Set(offer, "_buttonsGroup", footer);
			Set(offer, "_acceptButton", accept.GetComponent<UiButton>());
			Set(offer, "_cancelButton", decline.GetComponent<UiButton>());
			Set(offer, "_unavailableText", unavailable);
			Set(offer, "_statusPlate", status.GetComponent<UiText>());
			Save(root, Offer);
		}

		private static void BuildBoard()
		{
			GameObject root = VariantRoot(UiPrefabs + "Window.prefab", "Quest Board");
			root.transform.Find("Panel/Header/Title Text").GetComponent<TMP_Text>().text = "Quest Board";
			Transform content = root.transform.Find("Panel/Content");

			// Equal columns: as many offers in a row as fit at the minimum width.
			GameObject scroll = ScrollView(content, "Offers Scroll View");
			GameObject offers = scroll.transform.Find("Viewport/Content").gameObject;
			Layout row = Row(offers, ListGap, SizingMode.Fixed, SizingMode.FitContent, ULayout.Alignment.Start);
			Wrap(row, ListGap);
			Padding(row, 0, ListGap, 0, 0);
			var columns = offers.AddComponent<LayoutColumns>();
			// Four offers in a row at 7:3, three at 16:9, two at 4:3.
			Set(columns, "minColumnWidth", 480f);
			Set(columns, "maxColumns", 4);

			GameObject empty = EmptyState(content, "No quests on the board\nCome back later");

			var board = root.AddComponent<UiQuestBoard>();
			Set(board, "_offersContainer", offers.transform);
			Set(board, "_offerPrefab", Load<GameObject>(Offer).GetComponent<UiQuestOffer>());
			Set(board, "_emptyState", empty);
			Save(root, Board);
		}

		/// <summary>A text that wraps. With a line limit, the last line ends with an ellipsis.</summary>
		private static UiText TextBlock(Transform parent, string name, string text, float size, Color color, int maxLines = 0)
		{
			GameObject go = Child(name, parent);
			TextMeshProUGUI tmp = Text(go, text, size, color, wrap: true,
				overflow: maxLines > 0 ? TextOverflowModes.Ellipsis : TextOverflowModes.Overflow);
			var item = Item<LayoutText>(go, SizingMode.Grow, SizingMode.FitContent);
			if (maxLines > 0)
				Limits(item, maxHeight: Mathf.Ceil(LineHeight(size) * maxLines));
			var view = go.AddComponent<UiText>();
			Set(view, "text", tmp);
			return view;
		}

		/// <summary>The height of a line of Rubik: 42.66 at size 36.</summary>
		private static float LineHeight(float size) => size * 42.66f / 36f;

		private static UiQuestObjectiveList ObjectiveSection(Transform parent)
		{
			GameObject section = Child("Objectives", parent);
			Column(section, 12, SizingMode.Grow, SizingMode.FitContent);
			GameObject title = Child("Title Text", section.transform);
			Text(title, "Objectives", 32, Muted);
			Item<LayoutText>(title, SizingMode.Grow, SizingMode.FitContent);
			return Nested(QuestUiPrefabs.ObjectiveList, section.transform, "Objective List").GetComponent<UiQuestObjectiveList>();
		}

		private static GameObject IconTextButton(Transform parent, string name, string text, string icon)
		{
			GameObject button = Nested(UiPrefabs + "Icon Text Button (Centered).prefab", parent, name);
			TMP_Text label = button.transform.Find("Content/Text").GetComponent<TMP_Text>();
			label.text = text;
			AutoSize(label, 24, 40);
			Image image = button.transform.Find("Content/Icon").GetComponent<Image>();
			image.sprite = Icon(icon);
			image.color = White;
			return button;
		}

		private static GameObject OfferButton(Transform parent, string name, string text, string icon)
		{
			GameObject button = IconTextButton(parent, name, text, icon);
			Size(button, 240, 100);
			Item<LayoutItem>(button, SizingMode.Fixed, SizingMode.Fixed);
			return button;
		}

		/// <summary>A Perfect UI scroll view that fills its parent and scrolls only down.</summary>
		private static GameObject ScrollView(Transform parent, string name)
		{
			GameObject scroll = Nested(UiPrefabs + "Scroll View.prefab", parent, name);
			Stretch(scroll);
			var scrollRect = scroll.GetComponent<ScrollRect>();
			scrollRect.horizontal = false;
			scrollRect.horizontalScrollbar = null;
			scroll.transform.Find("Scrollbar Horizontal").gameObject.SetActive(false);
			// The scroll rect sizes the vertical scrollbar only together with a horizontal one, so it gets the full height here.
			RectTransform scrollbar = Rt(scroll.transform.Find("Scrollbar Vertical").gameObject);
			scrollbar.anchorMin = new Vector2(1, 0);
			scrollbar.anchorMax = new Vector2(1, 1);
			scrollbar.pivot = new Vector2(1, 1);
			scrollbar.anchoredPosition = Vector2.zero;
			scrollbar.sizeDelta = new Vector2(20, 0);
			// The layout of the content sets its height.
			Object.DestroyImmediate(scroll.transform.Find("Viewport/Content").GetComponent<ContentSizeFitter>());
			return scroll;
		}

		private static GameObject EmptyState(Transform parent, string text)
		{
			GameObject empty = Child("Empty State", parent);
			Stretch(empty);
			Text(empty, text, 36, Muted, TextAlignmentOptions.Center, wrap: true, overflow: TextOverflowModes.Overflow);
			return empty;
		}

		/// <summary>A part of the width of the parent, from one share to another, with offsets for a gap.</summary>
		private static void Split(RectTransform rt, float from, float to, float left, float right)
		{
			rt.anchorMin = new Vector2(from, 0);
			rt.anchorMax = new Vector2(to, 1);
			rt.pivot = new Vector2(0.5f, 0.5f);
			rt.offsetMin = new Vector2(left, 0);
			rt.offsetMax = new Vector2(right, 0);
		}
	}
}
