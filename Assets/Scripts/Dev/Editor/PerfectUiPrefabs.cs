using PerfectCore.PerfectUI;
using PerfectCore.PerfectUI.uLayout;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Toolkit.Dev.Editor.UiBuild;

namespace Toolkit.Dev.Editor
{
	/// <summary>Builders of new Perfect UI prefabs (D-112). Called from Unity MCP.</summary>
	public static class PerfectUiPrefabs
	{
		public static string BuildAll()
		{
			try
			{
				Label();
				Window();
			}
			finally
			{
				CloseScene();
			}

			return "Perfect UI prefabs: Label, Window";
		}

		public static void Label()
		{
			GameObject root = Root("Label");
			Layout row = Row(root);
			Padding(row, 20, 20, 8, 8);
			Background(root.transform, Faint);

			GameObject text = Child("Text", root.transform);
			TextMeshProUGUI tmp = Text(text, "Label", 28, White, TextAlignmentOptions.Center);
			Limits(Item<LayoutText>(text, SizingMode.FitContent, SizingMode.FitContent), maxWidth: 360);

			var label = root.AddComponent<UiText>();
			Set(label, "text", tmp);
			Save(root, UiPrefabs + "Label.prefab");
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
