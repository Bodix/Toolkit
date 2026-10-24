using PerfectCore.PerfectInventory.UI;
using PerfectCore.PerfectQuests.Inventory.UI;
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
	/// <summary>Builders of Perfect Quests prefabs in the Perfect UI style (D-112). Called from Unity MCP.</summary>
	public static class QuestUiPrefabs
	{
		public const string InventoryPrefabs = "Assets/Packages/PerfectCore.PerfectInventory/Prefabs/";
		public const string RewardText = QuestPrefabs + "Quest Reward (Text).prefab";
		public const string RewardItem = QuestPrefabs + "Inventory/Quest Reward (Item).prefab";
		public const string RewardList = QuestPrefabs + "Quest Reward List.prefab";
		public const string Objective = QuestPrefabs + "Quest Objective.prefab";
		public const string ObjectiveList = QuestPrefabs + "Quest Objective List.prefab";

		/// <summary>Builds the reward and objective blocks. They nest Perfect UI prefabs: see <see cref="PerfectUiPrefabs.BuildAll" />.</summary>
		public static string BuildBlocks()
		{
			try
			{
				BuildRewardText();
				BuildRewardItem();
				BuildRewardList();
				BuildObjective();
				BuildObjectiveList();
			}
			finally
			{
				CloseScene();
			}

			return "Quest blocks: rewards, reward list, objective, objective list";
		}

		private static void BuildRewardText()
		{
			// A line of its own that wraps: a label cannot shrink to a narrow column.
			GameObject root = Root("Quest Reward (Text)");
			TextMeshProUGUI text = Text(root, "Reward", 32, White, wrap: true, overflow: TextOverflowModes.Overflow);
			Item<LayoutText>(root, SizingMode.Grow, SizingMode.FitContent);
			var label = root.AddComponent<UiText>();
			Set(label, "text", text);
			var view = root.AddComponent<UiQuestRewardText>();
			Set(view, "_text", label);
			Save(root, RewardText);
		}

		private static void BuildRewardItem()
		{
			GameObject root = VariantRoot(InventoryPrefabs + "Item Cell.prefab", "Quest Reward (Item)");
			Size(root, 100, 100);
			Item<LayoutItem>(root, SizingMode.Fixed, SizingMode.Fixed);
			foreach (TMP_Text text in root.GetComponentsInChildren<TMP_Text>(true))
				text.font = RubikFont;
			var view = root.AddComponent<UiQuestItemReward>();
			Set(view, "_item", root.GetComponent<UiItem>());
			Save(root, RewardItem);
		}

		private static void BuildRewardList()
		{
			GameObject root = Root("Quest Reward List");
			Column(root, 12, SizingMode.Grow, SizingMode.FitContent);

			GameObject title = Child("Title Text", root.transform);
			Text(title, "Rewards", 32, Muted);
			Item<LayoutText>(title, SizingMode.Grow, SizingMode.FitContent);

			GameObject rewards = Child("Rewards", root.transform);
			Wrap(Row(rewards, 12, SizingMode.Grow, SizingMode.FitContent), 12);

			var list = root.AddComponent<UiQuestRewardList>();
			Set(list, "_container", rewards.transform);
			SetList(list, "_viewPrefabs",
				Load<GameObject>(RewardItem).GetComponent<UiQuestItemReward>(),
				Load<GameObject>(RewardText).GetComponent<UiQuestRewardText>());
			Set(list, "_group", root);
			Save(root, RewardList);
		}

		/// <summary>Sizes of an objective: the full one for cards and the compact one for the tracker.</summary>
		public sealed class ObjectiveStyle
		{
			/// <summary>The height of the status and of the first line: the status sits in the middle of it.</summary>
			public float StatusSize;
			public float DotSize;
			public float TextSize;
			/// <summary>Above and below the description, so one line of it is as high as the status.</summary>
			public float DescriptionMargin;
			public float Gap;
			public float BarHeight;
			public float ProgressTextSize;
			/// <summary>The progress text at the end of the first line and no bar: one line per objective.</summary>
			public bool InlineProgress;
			public bool BonusRewards;
		}

		// A line of Rubik 36 is 42.7 high, of Rubik 28 is 33.2. A label is 49.2 high.
		public static readonly ObjectiveStyle FullObjective = new ObjectiveStyle
		{
			StatusSize = 50, DotSize = 16, TextSize = 36, DescriptionMargin = 4, Gap = 12, BarHeight = 20,
			ProgressTextSize = 32, BonusRewards = true
		};

		public static readonly ObjectiveStyle CompactObjective = new ObjectiveStyle
		{
			StatusSize = 44, DotSize = 12, TextSize = 28, DescriptionMargin = 5, Gap = 8, BarHeight = 14,
			ProgressTextSize = 26, InlineProgress = true, BonusRewards = false
		};

		private static void BuildObjective() => BuildObjective("Quest Objective", Objective, FullObjective);

		public static void BuildObjective(string name, string path, ObjectiveStyle style)
		{
			GameObject root = Root(name);
			Row(root, 16, SizingMode.Grow, SizingMode.FitContent, ULayout.Alignment.Start);

			// The status keeps one place in every state, so the texts of all objectives start on one line.
			GameObject status = Child("Status", root.transform);
			Size(status, style.StatusSize, style.StatusSize);
			Item<LayoutItem>(status, SizingMode.Fixed, SizingMode.Fixed);

			GameObject pending = Child("Pending Mark", status.transform);
			Size(pending, style.DotSize, style.DotSize);
			Image(pending, Load<Sprite>(UiFolder + "Textures/Cirlces/Circle_64px.png"), Muted).type = UnityEngine.UI.Image.Type.Simple;
			GameObject completed = Mark(status, "Completed Mark", "check-mark-colored");
			GameObject failed = Mark(status, "Failed Mark", "close-colored");

			// Everything but the status fades when the objective is finished. With the progress inline,
			// the body is a row: the text and the progress text at the right edge.
			GameObject body = Child("Body", root.transform);
			if (style.InlineProgress)
				Row(body, 16, SizingMode.Grow, SizingMode.FitContent, ULayout.Alignment.Start);
			else
				Column(body, style.Gap, SizingMode.Grow, SizingMode.FitContent);
			var finished = body.AddComponent<CanvasGroup>();
			finished.interactable = false;
			finished.blocksRaycasts = false;

			// The optional mark follows a short text on its line and wraps to the next line under a long one.
			// A text that shares a line with the mark is one line high, so both sit in the middle of the line.
			GameObject header = Child("Header", body.transform);
			Layout headerRow = Row(header, 16, SizingMode.Grow, SizingMode.FitContent);
			Wrap(headerRow, 8);
			Limits(headerRow, minHeight: style.StatusSize);

			GameObject description = Child("Description Text", header.transform);
			TextMeshProUGUI descriptionText = Text(description, "Objective", style.TextSize, White, wrap: true, overflow: TextOverflowModes.Overflow);
			var descriptionItem = Item<LayoutText>(description, SizingMode.FitContent, SizingMode.FitContent);
			UiBuild.Margins(descriptionItem, 0, 0, style.DescriptionMargin, style.DescriptionMargin);
			Set(descriptionItem, "m_fitAvailableWidth", true);
			var descriptionView = description.AddComponent<UiText>();
			Set(descriptionView, "text", descriptionText);

			GameObject optional = Nested(UiPrefabs + "Label.prefab", header.transform, "Optional Mark");
			optional.GetComponentInChildren<TMP_Text>().text = "Optional";

			var view = root.AddComponent<UiQuestObjective>();
			Set(view, "_descriptionText", descriptionView);
			Set(view, "_optionalMark", optional);
			Set(view, "_pendingMark", pending);
			Set(view, "_completedMark", completed);
			Set(view, "_failedMark", failed);
			Set(view, "_finishedGroup", finished);

			if (style.InlineProgress)
			{
				// On the first line, at the right edge: as high as the description line.
				GameObject inlineText = Child("Progress Text", body.transform);
				TextMeshProUGUI inlineTmp = Text(inlineText, "0/0", style.ProgressTextSize, Muted, TextAlignmentOptions.Right);
				var inlineItem = Item<LayoutText>(inlineText, SizingMode.FitContent, SizingMode.FitContent);
				float lineHeight = style.TextSize * 42.66f / 36f;
				UiBuild.Margins(inlineItem, 0, 0, style.DescriptionMargin + (lineHeight - style.ProgressTextSize * 42.66f / 36f) / 2, 0);
				var inlineView = inlineText.AddComponent<UiText>();
				Set(inlineView, "text", inlineTmp);
				Set(view, "_progressText", inlineView);
				Save(root, path);
				return;
			}

			GameObject progress = Child("Progress", body.transform);
			Row(progress, 16, SizingMode.Grow, SizingMode.FitContent);

			GameObject bar = Nested(UiPrefabs + "Progress Bar.prefab", progress.transform, "Progress Bar");
			Size(bar, 100, style.BarHeight);
			Item<LayoutItem>(bar, SizingMode.Grow, SizingMode.Fixed);
			// Sliced circles: the corner radius is half the height, so the pixels per unit follow the height.
			var barBackground = bar.transform.Find("Background").GetComponent<Image>();
			barBackground.color = Dark;
			barBackground.raycastTarget = false;
			barBackground.pixelsPerUnitMultiplier = 512f / style.BarHeight;
			var barFill = bar.transform.Find("Background/Fill Image").GetComponent<Image>();
			float fillHeight = style.BarHeight * 0.6f;
			barFill.rectTransform.sizeDelta = new Vector2(fillHeight - style.BarHeight, fillHeight - style.BarHeight);
			barFill.pixelsPerUnitMultiplier = 512f / fillHeight;
			barFill.raycastTarget = false;

			GameObject progressTextObject = Child("Progress Text", progress.transform);
			TextMeshProUGUI progressText = Text(progressTextObject, "0/0", style.ProgressTextSize, Muted, TextAlignmentOptions.Right);
			Item<LayoutText>(progressTextObject, SizingMode.FitContent, SizingMode.FitContent);
			var progressView = progressTextObject.AddComponent<UiText>();
			Set(progressView, "text", progressText);

			if (style.BonusRewards)
			{
				GameObject bonus = Nested(RewardList, body.transform, "Bonus Rewards");
				bonus.transform.Find("Title Text").GetComponent<TMP_Text>().text = "Bonus";
				Set(view, "_bonusRewards", bonus.GetComponent<UiQuestRewardList>());
			}

			Set(view, "_progressText", progressView);
			Set(view, "_progressBar", bar.GetComponent<UiProgressBar>());
			Set(view, "_progressGroup", progress);
			Save(root, path);
		}

		private static GameObject Mark(GameObject status, string name, string icon)
		{
			GameObject mark = Child(name, status.transform);
			Stretch(mark, 3, 3, 3, 3);
			Image(mark, Icon(icon), White).preserveAspect = true;
			return mark;
		}

		private static void BuildObjectiveList() => BuildObjectiveList("Quest Objective List", ObjectiveList, Objective, 24);

		public static void BuildObjectiveList(string name, string path, string objectivePath, float gap)
		{
			GameObject root = Root(name);
			Column(root, gap, SizingMode.Grow, SizingMode.FitContent);
			var list = root.AddComponent<UiQuestObjectiveList>();
			Set(list, "_container", root.transform);
			Set(list, "_objectivePrefab", Load<GameObject>(objectivePath).GetComponent<UiQuestObjective>());
			Save(root, path);
		}
	}
}
