using System;
using System.Collections.Generic;
using PerfectCore.PerfectFoundation;
using PerfectCore.PerfectInventory;
using PerfectCore.PerfectQuests;
using PerfectCore.PerfectQuests.Inventory;
using PerfectCore.PerfectQuests.UI;
using PerfectCore.PerfectUI;
using PerfectCore.PerfectUI.uLayout;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using static Toolkit.Dev.Editor.UiBuild;
using ULayout = PerfectCore.PerfectUI.uLayout.Layout;

namespace Toolkit.Dev.Editor
{
	/// <summary>
	/// Scenes for the UI sheet (D-112): every element in all its states. NewScene() in Edit Mode, then Play Mode,
	/// then a scene method, then <see cref="UiShots.Capture"/>. Called from Unity MCP.
	/// </summary>
	public static partial class UiSheetScenes
	{
		private const string ExampleConfigs = QuestFolder + "Example/Configs/";

		/// <summary>A new unsaved scene with a canvas like in a game: 1920 x 1080, fitted by height.</summary>
		public static string NewScene()
		{
			var active = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
			// An untitled scene is an earlier sheet scene: it is never saved.
			if (active.isDirty && !string.IsNullOrEmpty(active.path))
				return $"Scene {active.path} has unsaved changes";

			EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
			var canvasObject = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
			canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
			var scaler = canvasObject.GetComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.referenceResolution = new Vector2(1920, 1080);
			scaler.matchWidthOrHeight = 1;

			GameObject backdrop = Child("Backdrop", canvasObject.transform);
			Stretch(backdrop);
			Image(backdrop, null, Hex("2B3A44"));

			new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
			return "Sheet scene ready";
		}

		/// <summary>Play Mode: rewards and labels in all states, three columns in a window.</summary>
		public static string BlocksRewards()
		{
			Transform columns = Columns(OpenWindow("Quest blocks: rewards and labels"));

			Transform rewards = Card(columns, "Rewards");
			ShowRewards(rewards, "Rewards", Gold(250), ItemReward("ExampleItem2", 3), ItemReward("ExampleItem1", 12000),
				TextReward("Reputation with the Merchants Guild +10"), TextReward("Map fragment"));

			Transform many = Card(columns, "Many rewards");
			var items = new List<QuestReward>();
			for (int i = 1; i <= 9; i++)
				items.Add(ItemReward(i % 2 == 0 ? "ExampleItem2" : "ExampleItem1", i * 15));
			ShowRewards(many, "Rewards", items.ToArray());
			ShowRewards(many, "Bonus", TextReward("Reputation +5"));
			ShowRewards(many, "Hidden when empty");

			Transform labels = Card(columns, "Labels");
			GameObject row = Child("Labels", labels);
			Wrap(Row(row, 12, SizingMode.Grow, SizingMode.FitContent), 12);
			Label(row.transform, "Label");
			Label(row.transform, "Optional");
			Label(row.transform, "Failed").transform.Find("Background").GetComponent<Image>().color = Red;
			Label(row.transform, "A long label is cut with an ellipsis at its maximum width");
			return "Rewards shown";
		}

		/// <summary>Play Mode: objectives in all states, three columns in a window.</summary>
		public static string BlocksObjectives()
		{
			Transform columns = Columns(OpenWindow("Quest blocks: objectives"));

			var progress = new QuestDefinition { Title = "Progress" };
			var progressStates = new List<QuestObjectiveState>();
			AddObjective(progress, progressStates, "Sink 3 pirate ships", 1 / 3f, "1/3");
			AddObjective(progress, progressStates, "Time left", 0.6f, "01:30");
			AddObjective(progress, progressStates, "Find the lost map fragments hidden in the shipwrecks along the northern coast", 0.4f, "2/5");
			ShowObjectives(Card(columns, "In progress"), progress, progressStates);

			var states = new QuestDefinition { Title = "States" };
			var stateList = new List<QuestObjectiveState>();
			AddObjective(states, stateList, "Deliver 20 crates of rum", 1f, "20/20", completed: true);
			AddObjective(states, stateList, "Escort the convoy", 0.4f, "2/5", failed: true);
			AddObjective(states, stateList, "Keep at least half of the hull", 0.5f, "50%", optional: true, bonus: new QuestReward[] { Gold(50) });
			ShowObjectives(Card(columns, "Done, failed, optional"), states, stateList);

			var offer = new QuestDefinition { Title = "Offer" };
			AddObjective(offer, null, "Sink 3 pirate ships", 0, "");
			AddObjective(offer, null, "Keep at least half of the hull", 0, "", optional: true, bonus: new QuestReward[] { Gold(50) });
			ShowObjectives(Card(columns, "In an offer"), offer, null);
			return "Objectives shown";
		}

		private static Transform Columns(Transform content)
		{
			GameObject columns = Child("Columns", content);
			Stretch(columns);
			Row(columns, 30, SizingMode.Fixed, SizingMode.Fixed, ULayout.Alignment.Start);
			return columns.transform;
		}

		private static void ShowObjectives(Transform card, QuestDefinition definition, List<QuestObjectiveState> states)
		{
			var list = Nested(QuestUiPrefabs.ObjectiveList, card).GetComponent<UiQuestObjectiveList>();
			if (states != null)
				list.Show(new QuestState { Definition = definition, ObjectiveStates = states });
			else
				list.Show(definition);
		}

		private static Transform OpenWindow(string title)
		{
			Transform canvas = Object.FindObjectOfType<Canvas>().transform;
			for (int i = canvas.childCount - 1; i >= 0; i--)
				if (canvas.GetChild(i).name != "Backdrop")
					Object.Destroy(canvas.GetChild(i).gameObject);

			GameObject windowObject = Nested(UiPrefabs + "Window.prefab", canvas);
			var window = windowObject.GetComponent<UiWindow>();
			window.Title = title;
			return windowObject.transform.Find("Panel/Content");
		}

		private static Transform Card(Transform parent, string title)
		{
			GameObject card = Child(title, parent);
			Layout column = Column(card, 24, SizingMode.Grow, SizingMode.FitContent);
			Padding(column, 30);
			Background(card.transform, Dark, ppu: 7);
			GameObject header = Child("Title", card.transform);
			Text(header, title, 40, White);
			Item<LayoutText>(header, SizingMode.Grow, SizingMode.FitContent);
			return card.transform;
		}

		private static void ShowRewards(Transform parent, string title, params QuestReward[] rewards)
		{
			GameObject list = Nested(QuestUiPrefabs.RewardList, parent);
			list.transform.Find("Title Text").GetComponent<TMP_Text>().text = title;
			list.GetComponent<UiQuestRewardList>().Show(rewards);
		}

		private static void AddObjective(QuestDefinition definition, List<QuestObjectiveState> states, string text, float progress,
			string progressText, bool completed = false, bool failed = false, bool optional = false, QuestReward[] bonus = null)
		{
			definition.Objectives.Add(new SheetObjective
			{
				Text = text,
				Progress = progress,
				ProgressText = progressText,
				Optional = optional,
				BonusRewards = new List<QuestReward>(bonus ?? Array.Empty<QuestReward>())
			});
			states?.Add(new QuestObjectiveState { IsCompleted = completed, IsFailed = failed });
		}

		private static GameObject Label(Transform parent, string text)
		{
			GameObject label = Nested(UiPrefabs + "Label.prefab", parent);
			label.GetComponent<UiText>().Text.text = text;
			return label;
		}

		private static QuestReward Gold(int amount) => ItemReward("ExampleItem1", amount);

		private static QuestReward ItemReward(string config, int amount) =>
			new GrantItemReward { Item = Load<ItemConfig>(ExampleConfigs + config + ".asset"), Amount = amount };

		private static QuestReward TextReward(string text) => new SheetReward { Text = text };
	}

	[Serializable]
	[TypeId("objective.ui_sheet")]
	public class SheetObjective : QuestObjective
	{
		public string Text;
		public float Progress;
		public string ProgressText;
		/// <summary>For the pages with a quest service: the state of the objective there.</summary>
		public bool Done;
		public bool Failed;

		public override IQuestObjectiveTracker CreateTracker() => null;
		public override string GetDescription() => Text;
		public override float GetProgress(QuestObjectiveState state) => Progress;
		public override string GetProgressText(QuestObjectiveState state) => ProgressText;
	}

	[Serializable]
	[TypeId("reward.ui_sheet")]
	public class SheetReward : QuestReward
	{
		public string Text;

		public override IQuestRewardApplier CreateApplier() => null;
		public override string GetDescription() => Text;
	}
}
