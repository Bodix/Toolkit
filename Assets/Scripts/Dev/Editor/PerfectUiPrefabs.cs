using PerfectCore.PerfectUI;
using PerfectCore.PerfectUI.uLayout;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Toolkit.Dev.Editor.UiBuild;

namespace Toolkit.Dev.Editor
{
	/// <summary>Builders of new Perfect UI prefabs. Called from Unity MCP.</summary>
	public static class PerfectUiPrefabs
	{
		/// <summary>Builds Badge and Window. The Perfect Quests builders nest them, so after a change run this first.</summary>
		public static string BuildAll()
		{
			try
			{
				Badge();
				Window();
			}
			finally
			{
				CloseScene();
			}

			return "Perfect UI prefabs: Badge, Window";
		}

		/// <summary>A plate of a fixed size without layout: whoever puts it into a layout adds one, as <see cref="AddBadgeLayout" /> does.</summary>
		public static void Badge()
		{
			GameObject root = Root("Badge");
			Size(root, 120, 50);

			GameObject plate = Child("Background", root.transform);
			Stretch(plate);
			Image background = Image(plate, Circle, Faint);

			GameObject text = Child("Text", root.transform);
			Stretch(text, 20, 20, 8, 8);
			TextMeshProUGUI tmp = Text(text, "Badge", 28, White, TextAlignmentOptions.Center);

			var badge = root.AddComponent<UiBadge>();
			Set(badge, "text", tmp);
			Set(badge, "background", background);
			Save(root, UiPrefabs + "Badge.prefab");
		}

		/// <summary>uLayout of a nested Badge: the plate fits the text, at most 360 wide and never wider than its container.</summary>
		public static void AddBadgeLayout(GameObject badge)
		{
			Layout row = Row(badge);
			Padding(row, 20, 20, 8, 8);

			var plate = badge.transform.Find("Background").gameObject.AddComponent<LayoutItem>();
			Set(plate, "m_ignoreLayout", true);
			Sizing(plate, SizingMode.Fixed, SizingMode.Fixed);

			// uLayout sizes the text from a point: the stretch of the bare badge would stay in its size.
			var text = (RectTransform)badge.transform.Find("Text");
			text.anchorMin = text.anchorMax = new Vector2(0, 0.5f);
			text.sizeDelta = Vector2.zero;
			text.anchoredPosition = Vector2.zero;
			var textItem = Item<LayoutText>(text.gameObject, SizingMode.FitContent, SizingMode.FitContent);
			Limits(textItem, maxWidth: 360);
			// A narrow column cuts the text with an ellipsis.
			Set(textItem, "m_fitAvailableWidth", true);
		}

		public static void Window()
		{
			GameObject root = Root("Window");
			Stretch(root);
			var window = root.AddComponent<UiWindow>();

			GameObject background = Child("Background Button", root.transform);
			Stretch(background);
			Image dim = Image(background, null, Dim, raycast: true);
			var button = background.AddComponent<Button>();
			button.transition = Selectable.Transition.None;
			button.navigation = new Navigation { mode = Navigation.Mode.None };
			button.targetGraphic = dim;
			var backgroundButton = background.AddComponent<UiButton>();
			Set(backgroundButton, "button", button);

			GameObject panel = Child("Panel", root.transform);
			Stretch(panel, 40, 40, 40, 40);
			Layout column = Column(panel, 30, SizingMode.Fixed, SizingMode.Fixed);
			Padding(column, 40);
			Background(panel.transform, Gray, ppu: 5, raycast: true);
			RectInterpolator(panel,
				widest: rt => { rt.offsetMin = new Vector2(160, 40); rt.offsetMax = new Vector2(-160, -40); },
				narrowest: rt => { rt.offsetMin = new Vector2(40, 40); rt.offsetMax = new Vector2(-40, -40); });

			GameObject header = Child("Header", panel.transform);
			Row(header, 30, SizingMode.Grow, SizingMode.FitContent);

			GameObject title = Child("Title Text", header.transform);
			TextMeshProUGUI titleText = Text(title, "Title", 60, White);
			Item<LayoutText>(title, SizingMode.Grow, SizingMode.FitContent);

			GameObject close = Nested(UiPrefabs + "Icon Button.prefab", header.transform, "Close Button");
			Size(close, 100, 100);
			var closeIcon = close.transform.Find("Icon").GetComponent<Image>();
			closeIcon.sprite = Icon("close");
			closeIcon.color = Gray;
			closeIcon.rectTransform.sizeDelta = new Vector2(-50, -50);

			GameObject content = Child("Content", panel.transform);
			Item<LayoutItem>(content, SizingMode.Grow, SizingMode.Grow);

			Set(window, "titleText", titleText);
			Set(window, "closeButton", close.GetComponent<UiIconButton>());
			Set(window, "backgroundButton", backgroundButton);
			Save(root, UiPrefabs + "Window.prefab");
		}
	}
}
