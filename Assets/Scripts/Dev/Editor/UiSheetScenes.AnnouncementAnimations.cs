using System;
using DG.Tweening;
using PerfectCore.PerfectUI;
using PerfectCore.PerfectUI.uLayout;
using TMPro;
using Toolkit.Tweens;
using Toolkit.Tweens.Animations;
using Toolkit.Tweens.Transitions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using static Toolkit.Dev.Editor.UiBuild;

namespace Toolkit.Dev.Editor
{
	/// <summary>
	/// Pages of the sheet for animated announcements: the banner, the plaque and the toast, animated with the presets
	/// of Toolkit.Tweens. A sample of the timing for a game: the show grows with the sound of a completed quest,
	/// the accent comes at its peak at about 0.9 s, and the hide is short.
	/// </summary>
	public static partial class UiSheetScenes
	{
		// The sheet canvas is fitted by height, so a share of the screen height is always the same length.
		private const float ScreenHeight = 1080;

		private static Tween _frame;

		/// <summary>
		/// Play Mode: a still frame of an animated announcement. Kind: "Banner", "Plaque" or "Toast".
		/// A frame of the show at showTime seconds or, with hideTime, a frame of the hide of the shown announcement.
		/// </summary>
		public static string AnnouncementFrame(string kind, float showTime, float hideTime = -1, string message = "Rum Run")
		{
			UiAnnouncement announcement = AnimatedAnnouncement(kind, message);
			announcement.Show(AnnouncementPayload(message), instantly: true);
			TweenTransition transition = ((OneTweenTransition)announcement.Animations).Transition;

			if (_frame != null && _frame.active)
				_frame.Kill();

			bool isHide = hideTime >= 0;
			_frame = isHide ? transition.PlayOut() : transition.PlayIn();
			_frame.Pause();
			_frame.Goto(isHide ? hideTime : showTime);
			return $"{kind} {(isHide ? "hide" : "show")} at {(isHide ? hideTime : showTime)} s";
		}

		/// <summary>Play Mode: an animated announcement shows itself as in a game. <see cref="HideAnnouncement" /> hides it.</summary>
		public static string PlayAnnouncement(string kind, string message = "Rum Run")
		{
			AnimatedAnnouncement(kind, message).Show(AnnouncementPayload(message));
			return kind + " is showing";
		}

		/// <summary>Play Mode: the announcement on the screen hides with its animation.</summary>
		public static string HideAnnouncement()
		{
			Object.FindObjectOfType<UiAnnouncement>().Hide();
			return "The announcement is hiding";
		}

		/// <summary>Play Mode: sets Time.timeScale, 0 pauses the game. Animated announcements play on.</summary>
		public static string SetTimeScale(float scale)
		{
			Time.timeScale = scale;
			return "Time scale " + scale;
		}

		private static UiAnnouncementPayload AnnouncementPayload(string message)
		{
			return new UiAnnouncementPayload { Title = AnnouncementTitle, Message = message };
		}

		private static UiAnnouncement AnimatedAnnouncement(string kind, string message)
		{
			Transform canvas = ClearCanvas();
			var announcement = Nested(UiPrefabs + $"Announcement ({kind}).prefab", canvas).GetComponent<UiAnnouncement>();
			GameObject root = announcement.gameObject;

			// Laid out once, so the animation knows the sizes of the parts, e.g. the height of the tab.
			announcement.Show(AnnouncementPayload(message), instantly: true);
			CompositeTransition animation = kind switch
			{
				"Banner" => BannerAnimation(root),
				"Plaque" => PlaqueAnimation(root),
				"Toast" => ToastAnimation(root),
				_ => throw new ArgumentException($"Unknown kind {kind}")
			};
			announcement.HideInstantly();

			animation.IgnoreTimeScale = true;
			announcement.Animations = new OneTweenTransition { Transition = animation };
			return announcement;
		}

		// The band unfolds by height, its lines grow from the middle, the letters of the title come together,
		// the message rises, and at the accent a shine runs over the band. The hide fades the texts and folds the band.
		private static CompositeTransition BannerAnimation(GameObject root)
		{
			root.AddComponent<CanvasGroup>();
			GameObject title = AnnouncementPart(root, "Title Text");
			GameObject message = AnnouncementPart(root, "Message Text");
			message.AddComponent<CanvasGroup>();

			GameObject band = AddPreset(TweenPresets.Composite + "Unfold With Fade", root);
			var bandScale = band.GetComponent<ScaleTransition>();
			bandScale.InitialScale = new Vector3(1, 0.02f, 1);
			bandScale.InDuration = bandScale.OutDuration = 0.34f;
			var bandFade = band.GetComponent<CanvasGroupFadeTransition>();
			bandFade.InDuration = bandFade.OutDuration = 0.34f;

			ScaleTransition topLine = GrowLine(AnnouncementPart(root, "Top Line"));
			ScaleTransition bottomLine = GrowLine(AnnouncementPart(root, "Bottom Line"));

			GameObject letters = AddPreset(TweenPresets.Composite + "Letters Converge With Fade", title);
			var spacing = letters.GetComponent<TextSpacingTransition>();
			spacing.TargetSpacing = title.GetComponent<TMP_Text>().characterSpacing;
			spacing.InitialSpacing = spacing.TargetSpacing + 46;
			spacing.InDuration = 0.62f;
			var titleFade = letters.GetComponent<GraphicFadeTransition>();
			titleFade.InDuration = 0.62f;

			GameObject rise = AddPreset(TweenPresets.Composite + "Rise With Fade", message);
			var riseMove = rise.GetComponent<MoveTransition>();
			riseMove.InOffset = new Vector3(0, -0.016f * ScreenHeight);
			riseMove.InDuration = 0.4f;
			riseMove.InEase = new CustomizableEase(Ease.OutQuad);
			var messageFade = rise.GetComponent<CanvasGroupFadeTransition>();
			messageFade.InDuration = 0.4f;

			SweepAnimation shine = Shine(root, 500, 0.25f, 0.75f, null);

			var animation = root.AddComponent<CompositeTransition>();
			animation.InTransitions = new[]
			{
				At(band.GetComponent<CompositeTransition>(), 0),
				At(topLine, 0.12f),
				At(bottomLine, 0.12f),
				At(letters.GetComponent<CompositeTransition>(), 0.14f),
				At(rise.GetComponent<CompositeTransition>(), 0.52f)
			};
			animation.InAnimations = new[] { At(shine, 0.86f) };
			animation.OutTransitions = new[]
			{
				At(titleFade, 0),
				At(messageFade, 0),
				At(band.GetComponent<CompositeTransition>(), 0.1f)
			};
			return animation;
		}

		private static ScaleTransition GrowLine(GameObject line)
		{
			var grow = AddPreset(TweenPresets.Simple + "Unfold Horizontal", line).GetComponent<ScaleTransition>();
			grow.InDuration = 0.6f;
			return grow;
		}

		// The sign drops from above and swings, the tab slides out from under the plate with the message,
		// and at the accent the icon pops out with a spin. The sign leaves upwards.
		private static CompositeTransition PlaqueAnimation(GameObject root)
		{
			GameObject message = AnnouncementPart(root, "Message Text");
			message.AddComponent<CanvasGroup>();
			float signHeight = Rt(root).rect.height;
			float plateHeight = Rt(AnnouncementPart(root, "Plate")).rect.height;
			var tabOffset = new Vector3(0, (signHeight - plateHeight) * 1.05f);

			GameObject sign = AddPreset(TweenPresets.Composite + "Drop And Swing", root);
			var drop = sign.GetComponent<MoveTransition>();
			drop.InOffset = new Vector3(0, 0.8f * ScreenHeight);
			drop.OutOffset = new Vector3(0, 0.85f * ScreenHeight);
			drop.InDuration = 0.95f;
			var swing = sign.GetComponent<RotateTransition>();
			swing.InitialRotation = new Vector3(0, 0, 5);
			swing.InDuration = 0.95f;

			// The tab is the background of the whole sign, and the plate covers its top. A move would show that top
			// above the plate, so the tab unrolls downwards from under the plate instead.
			GameObject background = AnnouncementPart(root, "Background");
			RectTransform tabRect = Rt(background);
			Vector2 offsetMin = tabRect.offsetMin;
			Vector2 offsetMax = tabRect.offsetMax;
			tabRect.pivot = new Vector2(0.5f, 1);
			tabRect.offsetMin = offsetMin;
			tabRect.offsetMax = offsetMax;
			var tab = AddPreset(TweenPresets.Simple + "Unfold Vertical", background).GetComponent<ScaleTransition>();
			tab.InitialScale = new Vector3(1, plateHeight / signHeight, 1);
			tab.InDuration = 0.36f;
			tab.InEase = Ease.OutBack;
			MoveTransition messageSlide = SlideFromUnderPlate(message, tabOffset);
			// The message is drawn above the plate, so it shows only when it is out from under it.
			var messageFade = AddPreset(TweenPresets.Simple + "Fade", message).GetComponent<CanvasGroupFadeTransition>();
			messageFade.InDuration = 0.3f;

			GameObject badge = AddPreset(TweenPresets.Composite + "Pop And Spin", AnnouncementPart(root, "Icon"));
			badge.GetComponent<ScaleTransition>().InDuration = 0.42f;
			var spin = badge.GetComponent<RotateTransition>();
			spin.InitialRotation = new Vector3(0, 0, 40);
			spin.InDuration = 0.42f;

			var animation = root.AddComponent<CompositeTransition>();
			animation.InTransitions = new[]
			{
				At(sign.GetComponent<CompositeTransition>(), 0),
				At(tab, 0.5f),
				At(messageSlide, 0.5f),
				At(messageFade, 0.56f),
				At(badge.GetComponent<CompositeTransition>(), 0.78f)
			};
			animation.OutTransitions = new[] { At(sign.GetComponent<CompositeTransition>(), 0) };
			return animation;
		}

		private static MoveTransition SlideFromUnderPlate(GameObject part, Vector3 offset)
		{
			var slide = AddPreset(TweenPresets.Simple + "Slide From Top", part).GetComponent<MoveTransition>();
			slide.InOffset = offset;
			slide.InDuration = 0.36f;
			slide.InEase = new CustomizableEase(Ease.OutBack);
			return slide;
		}

		// The plate slides in from above with a small overshoot. At the accent the icon grows, its check mark draws
		// itself and a shine runs over the plate. The plate leaves upwards.
		private static CompositeTransition ToastAnimation(GameObject root)
		{
			var slide = AddPreset(TweenPresets.Simple + "Slide From Top", root).GetComponent<MoveTransition>();
			slide.InOffset = new Vector3(0, 0.36f * ScreenHeight);
			slide.OutOffset = new Vector3(0, 0.4f * ScreenHeight);
			slide.InEase = new CustomizableEase(new AnimationCurve(
				new Keyframe(0, 0, 0, 3), new Keyframe(0.7f, 1.039f, 0, 0), new Keyframe(1, 1, 0, 0)));
			slide.OutDuration = 0.36f;
			slide.OutEase = new CustomizableEase(TweenPresets.LeaveCurve(0.03f));

			GameObject icon = AnnouncementPart(root, "Icon");
			var grow = AddPreset(TweenPresets.Simple + "Pop", icon).GetComponent<ScaleTransition>();
			grow.InitialScale = new Vector3(0.6f, 0.6f, 1);
			grow.InDuration = 0.4f;

			var check = icon.transform.Find("Image").GetComponent<Image>();
			check.type = UnityEngine.UI.Image.Type.Filled;
			check.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
			check.fillOrigin = (int)UnityEngine.UI.Image.OriginHorizontal.Left;
			var draw = AddPreset(TweenPresets.Simple + "Draw", check.gameObject).GetComponent<FillTransition>();
			draw.InDuration = 0.32f;

			SweepAnimation shine = Shine(root, 160, 0.35f, 0.7f, AnnouncementPart(root, "Background").GetComponent<Image>());

			var animation = root.AddComponent<CompositeTransition>();
			animation.InTransitions = new[] { At(slide, 0), At(grow, 0.74f), At(draw, 0.8f) };
			animation.InAnimations = new[] { At(shine, 0.9f) };
			animation.OutTransitions = new[] { At(slide, 0) };
			return animation;
		}

		// A soft stripe under a mask of the announcement: before and after the sweep it waits outside the mask.
		// The mask takes the shape of the plate, or is a plain rectangle without one.
		private static SweepAnimation Shine(GameObject root, float width, float alpha, float duration, Image shape)
		{
			GameObject mask = Child("Shine Mask", root.transform);
			Stretch(mask);
			Set(Item<LayoutItem>(mask, SizingMode.Fixed, SizingMode.Fixed), "m_ignoreLayout", true);
			if (shape != null)
			{
				Image(mask, shape.sprite, Color.white, shape.pixelsPerUnitMultiplier);
				mask.AddComponent<Mask>().showMaskGraphic = false;
			}
			else
			{
				mask.AddComponent<RectMask2D>();
			}

			GameObject stripe = Child("Shine", mask.transform);
			RectTransform rect = Rt(stripe);
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = new Vector2(0, 1);
			rect.pivot = new Vector2(1, 0.5f);
			rect.sizeDelta = new Vector2(width, 0);
			Image(stripe, PerfectUiPrefabs.EdgeFade, new Color(1, 1, 1, alpha));

			var sweep = AddPreset(TweenPresets.Simple + "Shine", stripe).GetComponent<SweepAnimation>();
			sweep.Duration = duration;
			return sweep;
		}

		// A preset lies on the part it animates.
		private static GameObject AddPreset(string preset, GameObject part)
		{
			return (GameObject)PrefabUtility.InstantiatePrefab(Load<GameObject>(preset + ".prefab"), part.transform);
		}

		private static GameObject AnnouncementPart(GameObject root, string name)
		{
			return root.transform.Find(name).gameObject;
		}

		private static CompositeTransitionPart At(TweenTransition transition, float position)
		{
			return new CompositeTransitionPart { Transition = transition, Position = position };
		}

		private static CompositeAnimationPart At(TweenAnimation animation, float position)
		{
			return new CompositeAnimationPart { Animation = animation, Position = position };
		}
	}
}
