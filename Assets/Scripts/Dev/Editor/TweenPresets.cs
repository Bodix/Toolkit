using System;
using System.IO;
using System.Linq;
using DG.Tweening;
using Toolkit.Tweens;
using Toolkit.Tweens.Animations;
using Toolkit.Tweens.Transitions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using static Toolkit.Dev.Editor.UiBuild;

namespace Toolkit.Dev.Editor
{
	/// <summary>
	/// Builds the presets of Toolkit.Tweens: prefabs that animate the object they lie on. A simple preset holds
	/// one transition, a composite one holds a few transitions and a composite transition over them.
	/// The values are for a UI canvas 1080 high: a show slows down to its end, a hide is shorter and speeds up,
	/// distances are small and overshoots are soft. Called from Unity MCP.
	/// Rebuilding keeps the ids of the objects, so instances and variants keep their links and overrides.
	/// </summary>
	public static class TweenPresets
	{
		public const string Folder = "Assets/Packages/Toolkit.Tweens/Presets/";
		public const string Simple = Folder + "Simple/";
		public const string Composite = Folder + "Composite/";

		// Far enough to come from beyond any edge of a canvas 1080 high, even a 7:3 one.
		private const float OffScreenY = 1100;
		private const float OffScreenX = 2600;

		private static Scene _scene;

		public static string BuildAll()
		{
			Directory.CreateDirectory(Simple);
			Directory.CreateDirectory(Composite);

			try
			{
				// A base goes before its variants.
				// A base goes before its variants.
				Build(Simple + "Fade", null, go => Fade(go));
				Build(Simple + "Pop", null, go => Pop(go));
				Build(Simple + "Pop From Zero", Simple + "Pop", go => FromZero(go.GetComponent<ScaleTransition>()));
				Build(Simple + "Unfold Vertical", null, go => UnfoldVertical(go));
				Build(Simple + "Unfold Horizontal", Simple + "Unfold Vertical",
					go => go.GetComponent<ScaleTransition>().InitialScale = new Vector3(0, 1, 1));
				Build(Simple + "Rise", null, go => Rise(go));
				Build(Simple + "Slide From Top", null, go => SlideFromTop(go));
				Build(Simple + "Slide From Bottom", Simple + "Slide From Top", go => Offset(go, new Vector3(0, -OffScreenY)));
				Build(Simple + "Slide From Left", Simple + "Slide From Top", go => Offset(go, new Vector3(-OffScreenX, 0)));
				Build(Simple + "Slide From Right", Simple + "Slide From Top", go => Offset(go, new Vector3(OffScreenX, 0)));
				Build(Simple + "Drop", Simple + "Slide From Top", go => Drop(go.GetComponent<MoveTransition>()));
				Build(Simple + "Swing", null, go => Swing(go));
				Build(Simple + "Spin In", null, go => SpinIn(go));
				Build(Simple + "Draw", null, go => Draw(go));
				Build(Simple + "Letters Converge", null, go => LettersConverge(go));
				Build(Simple + "Shine", null, go =>
				{
					var sweep = Get<SweepAnimation>(go);
					Set(sweep, "_targetSource", (int)TweenTargetSource.Parent);
					sweep.IgnoreTimeScale = true;
					sweep.Duration = 0.7f;
					sweep.Ease = new CustomizableEase(Ease.InOutSine);
				});

				// The parts take the values of the simple presets.
				BuildComposite("Pop With Fade", go => new TweenTransition[] { Pop(go), Fade(go, 0.2f) });
				BuildComposite("Rise With Fade", go => new TweenTransition[] { Rise(go), Fade(go) });
				BuildComposite("Unfold With Fade", go => new TweenTransition[] { UnfoldVertical(go), Fade(go) });
				BuildComposite("Letters Converge With Fade", go => new TweenTransition[] { LettersConverge(go), TextFade(go) });
				BuildComposite("Pop And Spin", go => new TweenTransition[] { FromZero(Pop(go)), SpinIn(go) });
				// It leaves straight up: a swing on the way out would only blur the move.
				BuildComposite("Drop And Swing", go => new TweenTransition[] { Drop(SlideFromTop(go)), Swing(go) },
					(composite, parts) => composite.OutTransitions = new[] { new CompositeTransitionPart { Transition = parts[0] } });
			}
			finally
			{
				if (_scene.IsValid())
					EditorSceneManager.ClosePreviewScene(_scene);
				_scene = default;
			}

			AssetDatabase.SaveAssets();
			return "Tween presets built in " + Folder;
		}

		// A composite preset is not a variant of a simple one: its composite transition goes first,
		// so a preset dropped into a field of an element gives the field this transition and not a part.
		private static void BuildComposite(string name, Func<GameObject, TweenTransition[]> makeParts,
			Action<CompositeTransition, TweenTransition[]> setUp = null)
		{
			Build(Composite + name, null, go =>
			{
				var composite = Get<CompositeTransition>(go);
				composite.IgnoreTimeScale = true;
				TweenTransition[] parts = makeParts(go);
				composite.InTransitions = parts.Select(part => new CompositeTransitionPart { Transition = part }).ToArray();
				setUp?.Invoke(composite, parts);
			});
		}

		// An existing preset is edited in place: a rebuild keeps the ids of its components and the links to them.
		private static void Build(string name, string baseName, Action<GameObject> setUp)
		{
			string path = name + ".prefab";
			bool exists = File.Exists(path);
			GameObject root = exists ? PrefabUtility.LoadPrefabContents(path) : NewRoot(path, baseName);

			try
			{
				setUp(root);
				PrefabUtility.SaveAsPrefabAsset(root, path);
			}
			finally
			{
				if (exists)
					PrefabUtility.UnloadPrefabContents(root);
				else
					Object.DestroyImmediate(root);
			}
		}

		// A preset is a plain object without a RectTransform, so layouts of the element skip it.
		private static GameObject NewRoot(string path, string baseName)
		{
			if (!_scene.IsValid())
				_scene = EditorSceneManager.NewPreviewScene();

			GameObject root;
			if (baseName == null)
			{
				root = new GameObject();
				SceneManager.MoveGameObjectToScene(root, _scene);
			}
			else
			{
				root = (GameObject)PrefabUtility.InstantiatePrefab(Load<GameObject>(baseName + ".prefab"), _scene);
			}

			root.name = Path.GetFileNameWithoutExtension(path);
			return root;
		}

		private static T Get<T>(GameObject go) where T : Component
		{
			return go.TryGetComponent(out T component) ? component : go.AddComponent<T>();
		}

		// A transition of a preset animates the parent and plays during a pause, as UI usually should.
		private static T Preset<T>(GameObject go) where T : TweenTransition
		{
			var transition = Get<T>(go);
			Set(transition, "_targetSource", (int)TweenTargetSource.Parent);
			transition.IgnoreTimeScale = true;
			return transition;
		}

		private static CanvasGroupFadeTransition Fade(GameObject go, float inDuration = 0.25f)
		{
			CanvasGroupFadeTransition fade = Preset<CanvasGroupFadeTransition>(go);
			fade.InitialAlpha = 0;
			fade.TargetAlpha = 1;
			fade.InDuration = inDuration;
			fade.InEase = Ease.OutQuad;
			fade.OutDuration = 0.2f;
			fade.OutEase = Ease.InQuad;
			return fade;
		}

		// A text fades by its own color: it needs no CanvasGroup.
		private static GraphicFadeTransition TextFade(GameObject go)
		{
			GraphicFadeTransition fade = Preset<GraphicFadeTransition>(go);
			fade.InitialAlpha = 0;
			fade.TargetAlpha = 1;
			fade.InDuration = 0.4f;
			fade.InEase = Ease.OutQuad;
			fade.OutDuration = 0.2f;
			fade.OutEase = Ease.InQuad;
			return fade;
		}

		private static ScaleTransition Scale(GameObject go, Vector3 initial, float inDuration, Ease inEase, float outDuration, Ease outEase)
		{
			ScaleTransition scale = Preset<ScaleTransition>(go);
			scale.InitialScale = initial;
			scale.TargetScale = Vector3.one;
			scale.InDuration = inDuration;
			scale.InEase = inEase;
			scale.OutDuration = outDuration;
			scale.OutEase = outEase;
			return scale;
		}

		private static ScaleTransition Pop(GameObject go)
		{
			return Scale(go, new Vector3(0.85f, 0.85f, 1), 0.35f, Ease.OutBack, 0.2f, Ease.InCubic);
		}

		private static ScaleTransition FromZero(ScaleTransition pop)
		{
			pop.InitialScale = Vector3.zero;
			pop.InDuration = 0.4f;
			pop.OutDuration = 0.25f;
			pop.OutEase = Ease.InBack;
			return pop;
		}

		private static ScaleTransition UnfoldVertical(GameObject go)
		{
			return Scale(go, new Vector3(1, 0, 1), 0.35f, Ease.OutCubic, 0.25f, Ease.InCubic);
		}

		private static MoveTransition Move(GameObject go, Vector3 offset, float inDuration, float outDuration)
		{
			MoveTransition move = Preset<MoveTransition>(go);
			move.InOffset = offset;
			move.OutOffset = offset;
			move.InDuration = inDuration;
			move.InEase = new CustomizableEase(Ease.OutCubic);
			move.OutDuration = outDuration;
			move.OutEase = new CustomizableEase(Ease.InCubic);
			return move;
		}

		private static MoveTransition Rise(GameObject go)
		{
			return Move(go, new Vector3(0, -40), 0.35f, 0.25f);
		}

		private static MoveTransition SlideFromTop(GameObject go)
		{
			return Move(go, new Vector3(0, OffScreenY), 0.5f, 0.35f);
		}

		private static void Offset(GameObject go, Vector3 offset)
		{
			var move = go.GetComponent<MoveTransition>();
			move.InOffset = offset;
			move.OutOffset = offset;
		}

		private static MoveTransition Drop(MoveTransition slide)
		{
			slide.InDuration = 0.9f;
			slide.InEase = new CustomizableEase(DropCurve());
			slide.OutDuration = 0.45f;
			slide.OutEase = new CustomizableEase(LeaveCurve(0.02f));
			return slide;
		}

		private static RotateTransition Swing(GameObject go)
		{
			RotateTransition swing = Preset<RotateTransition>(go);
			swing.InitialRotation = new Vector3(0, 0, 8);
			swing.TargetRotation = Vector3.zero;
			swing.InDuration = 1;
			swing.InEase = new CustomizableEase(SwingCurve());
			swing.OutDuration = 0.3f;
			swing.OutEase = new CustomizableEase(Ease.InCubic);
			return swing;
		}

		private static RotateTransition SpinIn(GameObject go)
		{
			RotateTransition spin = Preset<RotateTransition>(go);
			spin.InitialRotation = new Vector3(0, 0, 45);
			spin.TargetRotation = Vector3.zero;
			spin.InDuration = 0.4f;
			spin.InEase = new CustomizableEase(Ease.OutBack);
			spin.OutDuration = 0.25f;
			spin.OutEase = new CustomizableEase(Ease.InBack);
			return spin;
		}

		private static FillTransition Draw(GameObject go)
		{
			FillTransition fill = Preset<FillTransition>(go);
			fill.InitialFill = 0;
			fill.TargetFill = 1;
			fill.InDuration = 0.35f;
			fill.InEase = new CustomizableEase(Ease.OutCubic);
			fill.OutDuration = 0.25f;
			fill.OutEase = new CustomizableEase(Ease.InCubic);
			return fill;
		}

		private static TextSpacingTransition LettersConverge(GameObject go)
		{
			TextSpacingTransition spacing = Preset<TextSpacingTransition>(go);
			spacing.InitialSpacing = 40;
			spacing.TargetSpacing = 0;
			spacing.InDuration = 0.6f;
			spacing.InEase = new CustomizableEase(Ease.OutQuart);
			spacing.OutDuration = 0.3f;
			spacing.OutEase = new CustomizableEase(Ease.InCubic);
			return spacing;
		}

		// Falls past its place, bounces back above it twice, each time less, and settles.
		public static AnimationCurve DropCurve()
		{
			return new AnimationCurve(
				new Keyframe(0, 0, 0, 4.1f),
				new Keyframe(0.5f, 1.0275f, 0, 0),
				new Keyframe(0.68f, 0.99f, 0, 0),
				new Keyframe(0.84f, 1.004f, 0, 0),
				new Keyframe(1, 1, 0, 0));
		}

		// Swings past the target angle and back, each time less, and settles.
		public static AnimationCurve SwingCurve()
		{
			return new AnimationCurve(
				new Keyframe(0, 0, 0, 6),
				new Keyframe(0.5f, 1.52f, 0, 0),
				new Keyframe(0.68f, 0.7f, 0, 0),
				new Keyframe(0.84f, 1.14f, 0, 0),
				new Keyframe(1, 1, 0, 0));
		}

		// Steps back by a share of the way first, then leaves faster and faster.
		public static AnimationCurve LeaveCurve(float stepBack)
		{
			return new AnimationCurve(
				new Keyframe(0, 0, 0, 0),
				new Keyframe(0.25f, -stepBack, 0, 0),
				new Keyframe(1, 1, 2.7f, 0));
		}
	}
}
