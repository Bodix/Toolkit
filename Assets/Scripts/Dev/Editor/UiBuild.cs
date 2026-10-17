using System;
using PerfectCore.PerfectUI.uLayout;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Toolkit.Dev.Editor
{
	/// <summary>
	/// Helpers for prefab builders: objects, images, texts, uLayout and saving.
	/// Builders create a prefab from scratch in a preview scene and save it over the old one (the GUID stays).
	/// </summary>
	public static class UiBuild
	{
		public const string UiFolder = "Packages/net.perfectcore.perfectui/";
		public const string UiPrefabs = UiFolder + "Prefabs/";
		public const string QuestFolder = "Assets/Packages/PerfectCore.PerfectQuests/";
		public const string QuestPrefabs = QuestFolder + "Prefabs/";

		public static readonly Color Gray = Hex("4C4C4C");
		public static readonly Color Dim = Hex("4C4C4C80");
		public static readonly Color White = Color.white;
		public static readonly Color Faint = Hex("FFFFFF26");
		public static readonly Color Muted = Hex("FFFFFFA6");
		public static readonly Color Dark = Hex("00000040");
		public static readonly Color Red = Hex("E5534B");

		private static Scene _scene;
		private static Transform _holder;

		public static Sprite Circle => Load<Sprite>(UiFolder + "Textures/Cirlces/Circle_512px.png");
		public static Sprite Icon(string name) => Load<Sprite>(UiFolder + $"Textures/Icons/{name}.png");
		public static TMP_FontAsset RubikFont => Load<TMP_FontAsset>(UiFolder + "Fonts/Rubik/Rubik-Regular SDF.asset");
		public static Object Bounds => Load<Object>(UiPrefabs + "Aspect Ratio Bounds.asset");

		public static T Load<T>(string path) where T : Object
		{
			var asset = AssetDatabase.LoadAssetAtPath<T>(path);
			if (asset == null)
				throw new Exception($"Missing asset: {path}");
			return asset;
		}

		public static Color Hex(string hex)
		{
			ColorUtility.TryParseHtmlString("#" + hex, out Color color);
			return color;
		}

		/// <summary>
		/// A new prefab root. It lives under a holder RectTransform, because uLayout complains about a layout without a parent.
		/// </summary>
		public static GameObject Root(string name)
		{
			var go = new GameObject(name, typeof(RectTransform));
			go.transform.SetParent(Holder(), false);
			return go;
		}

		private static Transform Holder()
		{
			if (!_scene.IsValid())
				_scene = EditorSceneManager.NewPreviewScene();
			if (_holder == null)
			{
				var holder = new GameObject("Holder", typeof(RectTransform));
				SceneManager.MoveGameObjectToScene(holder, _scene);
				((RectTransform)holder.transform).sizeDelta = new Vector2(1920, 1080);
				_holder = holder.transform;
			}

			return _holder;
		}

		public static GameObject Child(string name, Transform parent)
		{
			var go = new GameObject(name, typeof(RectTransform));
			go.transform.SetParent(parent, false);
			return go;
		}

		public static GameObject Nested(string prefabPath, Transform parent, string name = null)
		{
			var prefab = Load<GameObject>(prefabPath);
			var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
			if (name != null)
				go.name = name;
			return go;
		}

		/// <summary>An instance of a prefab as the root of a new prefab: saving it makes a prefab variant.</summary>
		public static GameObject VariantRoot(string basePrefabPath, string name)
		{
			var go = (GameObject)PrefabUtility.InstantiatePrefab(Load<GameObject>(basePrefabPath), Holder());
			go.name = name;
			return go;
		}

		public static void Save(GameObject root, string path)
		{
			RebuildLayout(root);
			PrefabUtility.SaveAsPrefabAsset(root, path, out bool success);
			Object.DestroyImmediate(root);
			if (!success)
				throw new Exception($"Prefab not saved: {path}");
		}

		/// <summary>Runs uLayout and TMP in the editor, so the saved prefab has its real sizes.</summary>
		public static void RebuildLayout(GameObject root)
		{
			for (int pass = 0; pass < 3; pass++)
			{
				foreach (TMP_Text text in root.GetComponentsInChildren<TMP_Text>(true))
					text.ForceMeshUpdate();
				LayoutItem[] items = root.GetComponentsInChildren<LayoutItem>(true);
				for (int i = items.Length - 1; i >= 0; i--)
					items[i].Update();
				LayoutRebuilder.ForceRebuildLayoutImmediate(Rt(root));
			}
		}

		public static void CloseScene()
		{
			if (_scene.IsValid())
				EditorSceneManager.ClosePreviewScene(_scene);
			_scene = default;
			_holder = null;
		}

		public static RectTransform Rt(GameObject go) => (RectTransform)go.transform;

		public static RectTransform Stretch(GameObject go, float left = 0, float right = 0, float top = 0, float bottom = 0)
		{
			RectTransform rt = Rt(go);
			rt.anchorMin = Vector2.zero;
			rt.anchorMax = Vector2.one;
			rt.pivot = new Vector2(0.5f, 0.5f);
			rt.offsetMin = new Vector2(left, bottom);
			rt.offsetMax = new Vector2(-right, -top);
			return rt;
		}

		public static RectTransform Size(GameObject go, float width, float height)
		{
			RectTransform rt = Rt(go);
			rt.sizeDelta = new Vector2(width, height);
			return rt;
		}

		public static Image Image(GameObject go, Sprite sprite, Color color, float ppu = 10, bool raycast = false)
		{
			var image = go.AddComponent<Image>();
			image.sprite = sprite;
			image.color = color;
			image.raycastTarget = raycast;
			if (sprite != null && sprite.border != Vector4.zero)
			{
				image.type = UnityEngine.UI.Image.Type.Sliced;
				image.pixelsPerUnitMultiplier = ppu;
			}

			return image;
		}

		/// <summary>An image behind the layout content: stretched and ignored by the layout.</summary>
		public static Image Background(Transform parent, Color color, float ppu = 10, bool raycast = false, string name = "Background")
		{
			GameObject go = Child(name, parent);
			go.transform.SetAsFirstSibling();
			Stretch(go);
			Image image = Image(go, Circle, color, ppu, raycast);
			LayoutItem item = go.AddComponent<LayoutItem>();
			Set(item, "m_ignoreLayout", true);
			Set(item, "m_sizing.x", (int)SizingMode.Fixed);
			Set(item, "m_sizing.y", (int)SizingMode.Fixed);
			return image;
		}

		public static TextMeshProUGUI Text(GameObject go, string text, float size, Color color,
			TextAlignmentOptions alignment = TextAlignmentOptions.Left, bool wrap = false,
			TextOverflowModes overflow = TextOverflowModes.Ellipsis)
		{
			var tmp = go.AddComponent<TextMeshProUGUI>();
			tmp.font = RubikFont;
			tmp.text = text;
			tmp.fontSize = size;
			tmp.color = color;
			tmp.alignment = alignment;
			tmp.enableWordWrapping = wrap;
			tmp.overflowMode = overflow;
			tmp.raycastTarget = false;
			tmp.margin = Vector4.zero;
			return tmp;
		}

		public static void AutoSize(TMP_Text tmp, float min, float max)
		{
			tmp.enableAutoSizing = true;
			tmp.fontSizeMin = min;
			tmp.fontSizeMax = max;
		}

		public static Layout Layout(GameObject go, Layout.LayoutDirection direction, float gap = 0,
			Layout.Justification justify = PerfectCore.PerfectUI.uLayout.Layout.Justification.Start,
			Layout.Alignment alignItems = PerfectCore.PerfectUI.uLayout.Layout.Alignment.Start,
			SizingMode x = SizingMode.FitContent, SizingMode y = SizingMode.FitContent)
		{
			var layout = go.AddComponent<Layout>();
			Set(layout, "m_direction", (int)direction);
			Set(layout, "m_innerSpacing", gap);
			Set(layout, "m_justifyContent", (int)justify);
			Set(layout, "m_alignContent", (int)alignItems);
			Set(layout, "m_alignItems", (int)alignItems);
			Sizing(layout, x, y);
			return layout;
		}

		public static Layout Row(GameObject go, float gap = 0, SizingMode x = SizingMode.FitContent, SizingMode y = SizingMode.FitContent,
			Layout.Alignment alignItems = PerfectCore.PerfectUI.uLayout.Layout.Alignment.Center,
			Layout.Justification justify = PerfectCore.PerfectUI.uLayout.Layout.Justification.Start)
			=> Layout(go, PerfectCore.PerfectUI.uLayout.Layout.LayoutDirection.Row, gap, justify, alignItems, x, y);

		public static Layout Column(GameObject go, float gap = 0, SizingMode x = SizingMode.FitContent, SizingMode y = SizingMode.FitContent,
			Layout.Alignment alignItems = PerfectCore.PerfectUI.uLayout.Layout.Alignment.Start,
			Layout.Justification justify = PerfectCore.PerfectUI.uLayout.Layout.Justification.Start)
			=> Layout(go, PerfectCore.PerfectUI.uLayout.Layout.LayoutDirection.Column, gap, justify, alignItems, x, y);

		public static void Padding(Layout layout, float left, float right, float top, float bottom)
		{
			Set(layout, "m_padding.left", left);
			Set(layout, "m_padding.right", right);
			Set(layout, "m_padding.top", top);
			Set(layout, "m_padding.bottom", bottom);
		}

		public static void Padding(Layout layout, float all) => Padding(layout, all, all, all, all);

		public static void Wrap(Layout layout, float lineSpacing)
		{
			Set(layout, "m_wrap", true);
			Set(layout, "m_lineSpacing", lineSpacing);
		}

		public static T Item<T>(GameObject go, SizingMode x, SizingMode y) where T : LayoutItem
		{
			var item = go.AddComponent<T>();
			Sizing(item, x, y);
			return item;
		}

		public static void Sizing(LayoutItem item, SizingMode x, SizingMode y)
		{
			Set(item, "m_sizing.x", (int)x);
			Set(item, "m_sizing.y", (int)y);
		}

		public static void Limits(LayoutItem item, float minWidth = -1, float maxWidth = -1, float minHeight = -1, float maxHeight = -1)
		{
			if (minWidth >= 0) { Set(item, "m_useMinWidth", true); Set(item, "m_minWidth", minWidth); }
			if (maxWidth >= 0) { Set(item, "m_useMaxWidth", true); Set(item, "m_maxWidth", maxWidth); }
			if (minHeight >= 0) { Set(item, "m_useMinHeight", true); Set(item, "m_minHeight", minHeight); }
			if (maxHeight >= 0) { Set(item, "m_useMaxHeight", true); Set(item, "m_maxHeight", maxHeight); }
		}

		public static void Margins(LayoutItem item, float left, float right, float top, float bottom)
		{
			Set(item, "m_margins.left", left);
			Set(item, "m_margins.right", right);
			Set(item, "m_margins.top", top);
			Set(item, "m_margins.bottom", bottom);
		}

		/// <summary>Sets a serialized field by its property path. Value: bool, int (also enums), float, string, Color, Vector2/3, Object.</summary>
		public static void Set(Object target, string path, object value)
		{
			var so = new SerializedObject(target);
			SerializedProperty p = so.FindProperty(path);
			if (p == null)
				throw new Exception($"{target.GetType().Name} has no field {path}");
			switch (value)
			{
				case bool b: p.boolValue = b; break;
				case int i when p.propertyType == SerializedPropertyType.Enum: p.enumValueIndex = i; break;
				case int i: p.intValue = i; break;
				case float f: p.floatValue = f; break;
				case string s: p.stringValue = s; break;
				case Color c: p.colorValue = c; break;
				case Vector2 v2: p.vector2Value = v2; break;
				case Vector3 v3: p.vector3Value = v3; break;
				case Object o: p.objectReferenceValue = o; break;
				case null: p.objectReferenceValue = null; break;
				default: throw new Exception($"Unsupported value {value.GetType().Name} for {path}");
			}

			so.ApplyModifiedPropertiesWithoutUndo();
		}

		public static void SetList(Object target, string path, params Object[] values)
		{
			var so = new SerializedObject(target);
			SerializedProperty p = so.FindProperty(path);
			p.arraySize = values.Length;
			for (int i = 0; i < values.Length; i++)
				p.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
			so.ApplyModifiedPropertiesWithoutUndo();
		}

		/// <summary>Fills widest and narrowest data of a RectTransform interpolator and applies the widest one.</summary>
		public static void RectInterpolator(GameObject go, Action<RectTransform> widest, Action<RectTransform> narrowest)
		{
			RectTransform rt = Rt(go);
			var interpolator = go.AddComponent<PerfectCore.PerfectUI.RectTransformInterpolator>();
			Set(interpolator, "aspectRatioBounds", Bounds);
			Set(interpolator, "Target", rt);
			narrowest(rt);
			SaveRect(interpolator, "NarrowestData", rt);
			widest(rt);
			SaveRect(interpolator, "WidestData", rt);
		}

		private static void SaveRect(Object interpolator, string field, RectTransform rt)
		{
			Set(interpolator, field + ".AnchoredPosition", (Vector3)rt.anchoredPosition);
			Set(interpolator, field + ".SizeDelta", rt.sizeDelta);
			Set(interpolator, field + ".AnchorMin", rt.anchorMin);
			Set(interpolator, field + ".AnchorMax", rt.anchorMax);
			Set(interpolator, field + ".Pivot", rt.pivot);
		}
	}
}
