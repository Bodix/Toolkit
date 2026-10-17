using PerfectCore.PerfectInventory.UI;
using PerfectCore.PerfectQuests.Inventory.UI;
using PerfectCore.PerfectQuests.UI;
using PerfectCore.PerfectUI;
using PerfectCore.PerfectUI.uLayout;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Toolkit.Dev.Editor.UiBuild;

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

		private static void BuildObjective()
		{
			GameObject root = Root("Quest Objective");
			Column(root, 12, SizingMode.Grow, SizingMode.FitContent);

			GameObject header = Child("Header", root.transform);
			Row(header, 16, SizingMode.Grow, SizingMode.FitContent);

			GameObject completed = Child("Completed Mark", header.transform);
			Size(completed, 44, 44);
			Image(completed, Icon("check-mark-colored"), White).preserveAspect = true;
			Item<LayoutItem>(completed, SizingMode.Fixed, SizingMode.Fixed);

			GameObject description = Child("Description Text", header.transform);
			TextMeshProUGUI descriptionText = Text(description, "Objective", 36, White, wrap: true, overflow: TextOverflowModes.Overflow);
			Item<LayoutText>(description, SizingMode.Grow, SizingMode.FitContent);
			var descriptionView = description.AddComponent<UiText>();
			Set(descriptionView, "text", descriptionText);

			GameObject optional = Nested(UiPrefabs + "Label.prefab", header.transform, "Optional Mark");
			optional.GetComponentInChildren<TMP_Text>().text = "Optional";

			GameObject failed = Nested(UiPrefabs + "Label.prefab", header.transform, "Failed Mark");
			failed.GetComponentInChildren<TMP_Text>().text = "Failed";
			failed.transform.Find("Background").GetComponent<Image>().color = Red;

			GameObject progress = Child("Progress", root.transform);
			Row(progress, 16, SizingMode.Grow, SizingMode.FitContent);

			GameObject bar = Nested(UiPrefabs + "Progress Bar.prefab", progress.transform, "Progress Bar");
			Size(bar, 100, 20);
			Item<LayoutItem>(bar, SizingMode.Grow, SizingMode.Fixed);
			var barBackground = bar.transform.Find("Background").GetComponent<Image>();
			barBackground.color = Dark;
			barBackground.raycastTarget = false;
			barBackground.pixelsPerUnitMultiplier = 25.6f;
			var barFill = bar.transform.Find("Background/Fill Image").GetComponent<Image>();
			barFill.rectTransform.sizeDelta = new Vector2(-8, -8);
			barFill.pixelsPerUnitMultiplier = 42.7f;
			barFill.raycastTarget = false;

			GameObject progressTextObject = Child("Progress Text", progress.transform);
			TextMeshProUGUI progressText = Text(progressTextObject, "0/0", 32, Muted, TextAlignmentOptions.Right);
			Item<LayoutText>(progressTextObject, SizingMode.FitContent, SizingMode.FitContent);
			var progressView = progressTextObject.AddComponent<UiText>();
			Set(progressView, "text", progressText);

			GameObject bonus = Nested(RewardList, root.transform, "Bonus Rewards");
			bonus.transform.Find("Title Text").GetComponent<TMP_Text>().text = "Bonus";

			var view = root.AddComponent<UiQuestObjective>();
			Set(view, "_descriptionText", descriptionView);
			Set(view, "_progressText", progressView);
			Set(view, "_progressBar", bar.GetComponent<UiProgressBar>());
			Set(view, "_progressGroup", progress);
			Set(view, "_optionalMark", optional);
			Set(view, "_failedMark", failed);
			Set(view, "_completedMark", completed);
			Set(view, "_bonusRewards", bonus.GetComponent<UiQuestRewardList>());
			Save(root, Objective);
		}

		private static void BuildObjectiveList()
		{
			GameObject root = Root("Quest Objective List");
			Column(root, 24, SizingMode.Grow, SizingMode.FitContent);
			var list = root.AddComponent<UiQuestObjectiveList>();
			Set(list, "_container", root.transform);
			Set(list, "_objectivePrefab", Load<GameObject>(Objective).GetComponent<UiQuestObjective>());
			Save(root, ObjectiveList);
		}
	}
}
