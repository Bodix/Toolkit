using System.Linq;
using System.Text;
using PerfectCore.PerfectUI.uLayout;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Toolkit.Dev.Editor
{
	/// <summary>Compact text dump of a prefab hierarchy with layout, image and text settings. Called from Unity MCP.</summary>
	public static class PrefabDump
	{
		public static string Dump(string path, int maxDepth = 10)
		{
			GameObject root = PrefabUtility.LoadPrefabContents(path);
			var sb = new StringBuilder();
			try
			{
				Append(sb, root.transform, 0, maxDepth);
			}
			finally
			{
				PrefabUtility.UnloadPrefabContents(root);
			}

			return sb.ToString();
		}

		private static void Append(StringBuilder sb, Transform t, int depth, int maxDepth)
		{
			sb.Append(new string(' ', depth * 2)).Append(t.name);
			if (!t.gameObject.activeSelf)
				sb.Append(" (off)");
			if (depth > 0 && PrefabUtility.IsAnyPrefabInstanceRoot(t.gameObject))
				sb.Append(" <").Append(System.IO.Path.GetFileNameWithoutExtension(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(t.gameObject))).Append('>');

			if (t is RectTransform rt)
			{
				sb.Append($" | a{F(rt.anchorMin)}-{F(rt.anchorMax)} p{F(rt.pivot)} pos{F(rt.anchoredPosition)} sd{F(rt.sizeDelta)}");
			}

			foreach (Component c in t.GetComponents<Component>())
			{
				if (c == null || c is Transform || c is CanvasRenderer)
					continue;
				sb.Append(" | ").Append(Describe(c));
			}

			sb.Append('\n');
			if (depth >= maxDepth)
				return;
			foreach (Transform child in t)
				Append(sb, child, depth + 1, maxDepth);
		}

		private static string Describe(Component c)
		{
			switch (c)
			{
				case Image img:
					return $"Image {(img.sprite ? img.sprite.name : "-")} {img.type} ppu{img.pixelsPerUnitMultiplier:0.##} #{ColorUtility.ToHtmlStringRGBA(img.color)}{(img.raycastTarget ? " ray" : "")}{(img.preserveAspect ? " keepAspect" : "")}";
				case TMP_Text tmp:
					return $"TMP '{Short(tmp.text)}' {(tmp.font ? tmp.font.name.Replace(" SDF", "") : "-")} {tmp.fontSize:0.#}{(tmp.enableAutoSizing ? $" auto{tmp.fontSizeMin:0}-{tmp.fontSizeMax:0}" : "")} {tmp.fontStyle} {tmp.alignment} wrap{(tmp.enableWordWrapping ? 1 : 0)} {tmp.overflowMode} #{ColorUtility.ToHtmlStringRGBA(tmp.color)}{(tmp.raycastTarget ? " ray" : "")}";
				case Layout l:
					return $"Layout {l.Direction} pad{M(l.Padding)} gap{l.InnerSpacing:0.#} just{l.JustifyContent} alignC{l.AlignContent} alignI{l.AlignItems}{(l.Wrap ? " wrap" : "")} {Item(l)}";
				case LayoutItem li:
					return $"{li.GetType().Name} {Item(li)}";
				case HorizontalOrVerticalLayoutGroup g:
					return $"{g.GetType().Name} pad({g.padding.left},{g.padding.right},{g.padding.top},{g.padding.bottom}) gap{g.spacing:0.#} {g.childAlignment} ctrl{(g.childControlWidth ? "W" : "")}{(g.childControlHeight ? "H" : "")} exp{(g.childForceExpandWidth ? "W" : "")}{(g.childForceExpandHeight ? "H" : "")}";
				case ContentSizeFitter f:
					return $"Fitter {f.horizontalFit}/{f.verticalFit}";
				case LayoutElement le:
					return $"LayoutElement min({le.minWidth:0},{le.minHeight:0}) pref({le.preferredWidth:0},{le.preferredHeight:0}) flex({le.flexibleWidth:0.#},{le.flexibleHeight:0.#}){(le.ignoreLayout ? " ignore" : "")}";
				default:
					return c.GetType().Name;
			}
		}

		private static string Item(LayoutItem li)
		{
			var so = new SerializedObject(li);
			string Lim(string use, string val) => so.FindProperty(use).boolValue ? so.FindProperty(val).floatValue.ToString("0") : "_";
			return $"size{li.Sizing.x}/{li.Sizing.y} min({Lim("m_useMinWidth", "m_minWidth")},{Lim("m_useMinHeight", "m_minHeight")}) max({Lim("m_useMaxWidth", "m_maxWidth")},{Lim("m_useMaxHeight", "m_maxHeight")}) flex({li.flexibleWidth:0.#},{li.flexibleHeight:0.#}) m{M(li.Margins)}{(li.IgnoreLayout ? " ignore" : "")}";
		}

		private static string M(Margins m) => $"({m.left:0},{m.right:0},{m.top:0},{m.bottom:0})";
		private static string F(Vector2 v) => $"({v.x:0.##},{v.y:0.##})";
		private static string Short(string s) => s == null ? "" : (s.Length > 18 ? s.Substring(0, 18) + "…" : s).Replace("\n", " ");
	}
}
