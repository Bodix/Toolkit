using PerfectCore.PerfectUI;
using PerfectCore.PerfectUI.uLayout;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Toolkit.Dev.Editor.UiBuild;
using ULayout = PerfectCore.PerfectUI.uLayout.Layout;

namespace Toolkit.Dev.Editor
{
	/// <summary>Builders of new Perfect UI prefabs. Called from Unity MCP.</summary>
	public static class PerfectUiPrefabs
	{
		public const string AnnouncementPrefab = UiPrefabs + "Announcement.prefab";

		// The gap an announcement keeps to the side edges of the screen.
		private const float AnnouncementScreenMargin = 80;

		private static readonly Color DarkGray = Hex("333333");

		private static Sprite EdgeFade => Load<Sprite>(UiFolder + "Textures/Gradients/Horizontal_Edge_Fade.png");

		/// <summary>
		/// Builds Badge, Window and the announcements. The Perfect Quests builders nest Badge and Window,
		/// so after a change run this first.
		/// </summary>
		public static string BuildAll()
		{
			try
			{
				Badge();
				Window();
				// The base goes first: the others are its variants.
				Announcement();
				AnnouncementBanner();
				AnnouncementPlaque();
				AnnouncementToast();
			}
			finally
			{
				CloseScene();
			}

			return "Perfect UI prefabs: Badge, Window, Announcement, Announcement (Banner), Announcement (Plaque), Announcement (Toast)";
		}

		/// <summary>A plate of a fixed size without layout: whoever puts it into a layout adds one, as <see cref="AddBadgeLayout" /> does.</summary>
		public static void Badge()
		{
			GameObject root = Root("Badge");
			Size(root, 130, 50);

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

			// The background dims the whole screen, the panel stays away from the notch.
			GameObject safeArea = Child("Safe Area", root.transform);
			Stretch(safeArea);
			safeArea.AddComponent<SafeArea>();

			GameObject panel = Child("Panel", safeArea.transform);
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

		/// <summary>
		/// The base of the announcements: a plate at the top of the screen with the parts in a column.
		/// Every part lies right under the root: a variant cannot reorder or nest the parts of its base,
		/// so it only moves and restyles them and adds its own decor.
		/// </summary>
		public static void Announcement()
		{
			GameObject root = Root("Announcement");
			PlaceAtTop(root, 80);
			ULayout column = Column(root, 12, alignItems: ULayout.Alignment.Center);
			Padding(column, 60, 60, 40, 40);
			// Fit Available Width of the texts counts these margins, so a long text ends before the screen edge.
			UiBuild.Margins(column, AnnouncementScreenMargin, AnnouncementScreenMargin, 0, 0);
			Background(root.transform, Gray, ppu: 5);

			GameObject icon = Child("Icon", root.transform);
			Size(icon, 120, 120);
			// Not sliced: a circle of any size.
			Image(icon, Circle, White).type = UnityEngine.UI.Image.Type.Simple;
			Item<LayoutItem>(icon, SizingMode.Fixed, SizingMode.Fixed);
			// In shares of the circle, so a variant resizes only the circle.
			GameObject iconImage = Child("Image", icon.transform);
			RectTransform iconImageRect = Stretch(iconImage);
			iconImageRect.anchorMin = new Vector2(0.22f, 0.22f);
			iconImageRect.anchorMax = new Vector2(0.78f, 0.78f);
			Image glyph = Image(iconImage, Icon("check-mark"), Gray);
			glyph.preserveAspect = true;

			TextMeshProUGUI title = AnnouncementText(root, "Title Text", "Title", 72, White);
			TextMeshProUGUI message = AnnouncementText(root, "Message Text", "Message", 48, Muted);

			// Off, so the empty place adds no gap to the column. Whoever fills it turns it on.
			GameObject content = Child("Content", root.transform);
			Column(content, 12, alignItems: ULayout.Alignment.Center);
			content.SetActive(false);

			var announcement = root.AddComponent<UiAnnouncement>();
			Set(announcement, "titleText", title);
			Set(announcement, "messageText", message);
			Set(announcement, "iconImage", glyph);
			Set(announcement, "content", Rt(content));
			Save(root, AnnouncementPrefab);
		}

		/// <summary>A band across the whole screen that fades out at its sides, with a thin line above and below. No icon.</summary>
		public static void AnnouncementBanner()
		{
			GameObject root = VariantRoot(AnnouncementPrefab, "Announcement (Banner)");
			RectTransform rect = Rt(root);
			rect.anchorMin = new Vector2(0, 0.57f);
			rect.anchorMax = new Vector2(1, 0.57f);
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.anchoredPosition = Vector2.zero;
			rect.sizeDelta = Vector2.zero;

			var column = root.GetComponent<ULayout>();
			Sizing(column, SizingMode.Fixed, SizingMode.FitContent);
			Set(column, "m_innerSpacing", 16f);
			Padding(column, 130, 130, 52, 54);
			UiBuild.Margins(column, 0, 0, 0, 0);

			var background = Part(root, "Background").GetComponent<Image>();
			background.sprite = EdgeFade;
			background.type = UnityEngine.UI.Image.Type.Simple;
			background.color = new Color(Gray.r, Gray.g, Gray.b, 0.9f);

			Part(root, "Icon").SetActive(false);
			RestyleText(root, "Title Text", 108, 1280);
			RestyleText(root, "Message Text", 54, 1280);

			BannerLine(root, "Top Line", 1);
			BannerLine(root, "Bottom Line", 0);
			Save(root, UiPrefabs + "Announcement (Banner).prefab");
		}

		/// <summary>
		/// A plate with the title, a narrower tab with the message that hangs under it and a round icon on the top edge.
		/// The tab is the base background: it lies under the whole sign, and the plate covers its top.
		/// Without a message the sign is as high as the plate, so the tab is hidden.
		/// </summary>
		public static void AnnouncementPlaque()
		{
			const float tabInset = 40;
			const float tabPadding = 54;
			// Not lower than a line of the title: TMP drops a line that does not fit its box.
			const float titleHeight = 116;
			const float titleTop = 80;
			const float titleBottom = 34;

			GameObject root = VariantRoot(AnnouncementPrefab, "Announcement (Plaque)");
			// The icon sticks out above the plate by half.
			PlaceAtTop(root, 185);
			var column = root.GetComponent<ULayout>();
			Set(column, "m_innerSpacing", 0f);
			Padding(column, 0);

			var tab = Part(root, "Background").GetComponent<Image>();
			Stretch(tab.gameObject, tabInset, tabInset);
			tab.color = DarkGray;
			tab.pixelsPerUnitMultiplier = 10;

			// Above the tab and below the icon and the texts.
			GameObject plate = Child("Plate", root.transform);
			plate.transform.SetSiblingIndex(1);
			RectTransform plateRect = Rt(plate);
			plateRect.anchorMin = new Vector2(0, 1);
			plateRect.anchorMax = new Vector2(1, 1);
			plateRect.pivot = new Vector2(0.5f, 1);
			plateRect.anchoredPosition = Vector2.zero;
			plateRect.sizeDelta = new Vector2(0, titleTop + titleHeight + titleBottom);
			Image(plate, Circle, Gray, ppu: 7);
			IgnoreLayout(plate);

			GameObject icon = Part(root, "Icon");
			Set(icon.GetComponent<LayoutItem>(), "m_ignoreLayout", true);
			RectTransform iconRect = Rt(icon);
			iconRect.anchorMin = iconRect.anchorMax = new Vector2(0.5f, 1);
			iconRect.pivot = new Vector2(0.5f, 0.5f);
			iconRect.anchoredPosition = Vector2.zero;
			iconRect.sizeDelta = new Vector2(136, 136);

			// A fixed height of the title keeps the plate height known.
			LayoutText title = RestyleText(root, "Title Text", 96, 1000);
			Sizing(title, SizingMode.FitContent, SizingMode.Fixed);
			Size(title.gameObject, Rt(title.gameObject).sizeDelta.x, titleHeight);
			UiBuild.Margins(title, 86, 86, titleTop, titleBottom);

			LayoutText message = RestyleText(root, "Message Text", 52, 1000);
			UiBuild.Margins(message, tabInset + tabPadding, tabInset + tabPadding, 24, 24);
			UiBuild.Margins(Part(root, "Content").GetComponent<ULayout>(), tabInset + tabPadding, tabInset + tabPadding, 0, 24);
			Save(root, UiPrefabs + "Announcement (Plaque).prefab");
		}

		/// <summary>A compact plate at the top of the screen: a round icon on the left, the title and the message under it on the right.</summary>
		public static void AnnouncementToast()
		{
			const float padding = 30;
			const float iconSize = 150;
			const float iconGap = 36;

			GameObject root = VariantRoot(AnnouncementPrefab, "Announcement (Toast)");
			PlaceAtTop(root, 86);
			var column = root.GetComponent<ULayout>();
			Set(column, "m_innerSpacing", 4f);
			Set(column, "m_alignItems", (int)ULayout.Alignment.Start);
			Set(column, "m_alignContent", (int)ULayout.Alignment.Start);
			Set(column, "m_justifyContent", (int)ULayout.Justification.Center);
			// The icon stands in the left padding, out of the column. The plate is never lower than the icon.
			Padding(column, padding + iconSize + iconGap, 76, padding, padding);
			Limits(column, minHeight: iconSize + padding * 2);
			Part(root, "Background").GetComponent<Image>().pixelsPerUnitMultiplier = 4.75f;

			GameObject icon = Part(root, "Icon");
			Set(icon.GetComponent<LayoutItem>(), "m_ignoreLayout", true);
			RectTransform iconRect = Rt(icon);
			iconRect.anchorMin = iconRect.anchorMax = iconRect.pivot = new Vector2(0, 0.5f);
			iconRect.anchoredPosition = new Vector2(padding, 0);
			iconRect.sizeDelta = new Vector2(iconSize, iconSize);

			RestyleText(root, "Title Text", 72, 1000).GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Left;
			RestyleText(root, "Message Text", 48, 1000).GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Left;
			Save(root, UiPrefabs + "Announcement (Toast).prefab");
		}

		/// <summary>One line that ends with an ellipsis: never wider than its limit and than the room the screen gives it.</summary>
		private static TextMeshProUGUI AnnouncementText(GameObject root, string name, string text, float size, Color color)
		{
			GameObject go = Child(name, root.transform);
			TextMeshProUGUI tmp = Text(go, text, size, color, TextAlignmentOptions.Center);
			var item = Item<LayoutText>(go, SizingMode.FitContent, SizingMode.FitContent);
			Limits(item, maxWidth: 1000);
			Set(item, "m_fitAvailableWidth", true);
			return tmp;
		}

		private static LayoutText RestyleText(GameObject root, string name, float size, float maxWidth)
		{
			GameObject go = Part(root, name);
			go.GetComponent<TMP_Text>().fontSize = size;
			var item = go.GetComponent<LayoutText>();
			Limits(item, maxWidth: maxWidth);
			return item;
		}

		private static void BannerLine(GameObject root, string name, float edge)
		{
			GameObject line = Child(name, root.transform);
			RectTransform rect = Rt(line);
			rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, edge);
			rect.anchoredPosition = Vector2.zero;
			rect.sizeDelta = new Vector2(1240, 4);
			Image(line, EdgeFade, White);
			IgnoreLayout(line);
		}

		private static void IgnoreLayout(GameObject go)
		{
			var item = Item<LayoutItem>(go, SizingMode.Fixed, SizingMode.Fixed);
			Set(item, "m_ignoreLayout", true);
		}

		private static GameObject Part(GameObject root, string name) => root.transform.Find(name).gameObject;

		private static void PlaceAtTop(GameObject root, float offset)
		{
			RectTransform rect = Rt(root);
			rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 1);
			rect.anchoredPosition = new Vector2(0, -offset);
		}
	}
}
