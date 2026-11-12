using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Toolkit.Mcp
{
	/// <summary>
	/// Plays steps in the editor one after another and saves a report to a file: one Unity MCP command and one wait
	/// in the shell instead of a command per action. A run goes on through compilation and the start of Play Mode.
	/// Steps are separated by ";":
	/// "Compile" clears the console, refreshes assets, waits for compilation and stops on its errors. Edit Mode only.
	/// "NoErrors" stops the run when the console has errors, e.g. after a step that builds assets.
	/// "Play" and "Stop" start and stop Play Mode and wait for it.
	/// "Do:Method(a, b)" calls a public static method of the tools class, missing arguments take their defaults.
	/// "Do:Type.Method" calls a method of another class from the namespace and assembly of the tools class.
	/// A string the method returns goes to the report, an exception stops the run.
	/// "Delay:1.5" waits seconds. "Size:1440x1080" sets the Game view. "Wait:Name" waits for an active UI object.
	/// "Click:Name" and "Click:Name#1" (number among objects with this name) wait until the button or toggle
	/// is active and enabled, then click it. "Shot:name" saves the screenshot name.png. Both only in Play Mode.
	/// In Play Mode the run waits a few frames after every step, so a screenshot shows the laid out page.
	/// A step waits up to 30 seconds, "Play" and "Stop" up to 3 minutes, compilation up to 10 minutes.
	/// The report is saved as shots.txt in the folder of the run when the run ends or a step fails: the state,
	/// the screenshots and what the methods returned. Wait for that file in the shell of the computer.
	/// </summary>
	[InitializeOnLoad]
	public static class McpScenario
	{
		public const string ReportName = "shots.txt";

		// SessionState survives the domain reloads of compilation and of entering Play Mode, unlike a static field.
		private const string StateKey = "Toolkit.Mcp.McpScenario";
		private const double StepTimeout = 30;
		private const double PlayTimeout = 180;
		private const double CompileTimeout = 600;
		// Compilation starts a moment after the refresh that found changed scripts.
		private const double CompileStartDelay = 1;

		/// <summary>
		/// A game that loads after Play Mode starts tells here when it is ready: the first step in Play Mode waits for it.
		/// Set on every domain load.
		/// </summary>
		public static Func<bool> PlayReady;

		/// <summary>
		/// A game that prepares something before Play Mode, e.g. a backup of saves, starts Play Mode here itself.
		/// Set on every domain load.
		/// </summary>
		public static Action StartPlay;

		private static RunState _run;
		private static int _waitUntilFrame;
		private static bool _isPlayReady;
		private static bool _isInvoking;

		static McpScenario()
		{
			EditorApplication.playModeStateChanged += _ =>
			{
				_waitUntilFrame = 0;
				_isPlayReady = false;
			};

			string saved = SessionState.GetString(StateKey, string.Empty);

			if (saved.Length == 0)
				return;

			_run = JsonUtility.FromJson<RunState>(saved);

			if (!_run.IsDone)
				EditorApplication.update += Tick;
		}

		/// <summary>
		/// Starts a run. Tools is the class of the "Do" steps. Screenshots and the report go to the folder,
		/// a path from the project root. A scale below 1 shrinks the screenshots. Settle frames is how many frames
		/// of Play Mode the run waits after a step. An unknown step or a step in a wrong mode is told at once.
		/// </summary>
		public static string Run(string steps, Type tools, string folder, float scale = 1f, int settleFrames = 3)
		{
			if (_isInvoking)
				return "A step cannot start a run";

			List<string> list = steps.Split(';').Select(step => step.Trim()).Where(step => step.Length > 0).ToList();
			string problem = list.Count == 0 ? "No steps" : FindWrongStep(list);

			if (problem != null)
				return problem;

			Finish("replaced by a new run");
			Directory.CreateDirectory(folder);
			File.Delete(Path.Combine(folder, ReportName));

			// The editor has no focus while an agent works, and Play Mode stands still without this.
			if (EditorApplication.isPlaying)
				Application.runInBackground = true;

			_run = new RunState
			{
				Steps = list,
				Tools = tools.AssemblyQualifiedName,
				Folder = folder,
				Scale = scale,
				SettleFrames = settleFrames,
				StepStart = EditorApplication.timeSinceStartup
			};
			Save();
			EditorApplication.update -= Tick;
			EditorApplication.update += Tick;
			return $"Shots started: {list.Count} steps, report in {folder}/{ReportName}";
		}

		/// <summary>The report of the current or the last run.</summary>
		public static string Status()
		{
			return _run == null ? "No shots run" : Report(_run);
		}

		/// <summary>Ends the run and returns its report.</summary>
		public static string Stop()
		{
			Finish("stopped");
			return Status();
		}

		// A mistake found before the run costs no wait for the report.
		private static string FindWrongStep(List<string> steps)
		{
			bool playing = EditorApplication.isPlayingOrWillChangePlaymode;

			for (int i = 0; i < steps.Count; i++)
			{
				Split(steps[i], out string verb, out _);

				switch (verb)
				{
					case "Play":
						playing = true;
						break;
					case "Stop":
						playing = false;
						break;
					case "Compile":
						if (playing)
							return $"Step {i + 1} \"{steps[i]}\" needs Edit Mode: put Stop before it";
						break;
					case "Click":
					case "Shot":
						if (!playing)
							return $"Step {i + 1} \"{steps[i]}\" needs Play Mode: put Play before it";
						break;
					case "NoErrors":
					case "Do":
					case "Delay":
					case "Size":
					case "Wait":
						break;
					default:
						return $"Unknown step {i + 1} \"{steps[i]}\"";
				}
			}

			return null;
		}

		private static void Tick()
		{
			RunState run = _run;

			if (run == null || run.IsDone)
			{
				EditorApplication.update -= Tick;
				return;
			}

			if (EditorApplication.isCompiling || EditorApplication.isUpdating)
			{
				if (TimedOut(run, CompileTimeout))
					Finish("compilation did not end");

				return;
			}

			if (EditorApplication.isPlaying && Time.frameCount < _waitUntilFrame)
				return;

			try
			{
				if (!string.IsNullOrEmpty(run.PendingShot))
					SaveShot(run);
				else if (run.Index >= run.Steps.Count)
					Finish(null);
				else
					PlayStep(run);
			}
			catch (Exception exception)
			{
				Exception cause = exception is TargetInvocationException && exception.InnerException != null
					? exception.InnerException
					: exception;
				Debug.LogException(cause);
				Finish($"{cause.GetType().Name}: {cause.Message}");
			}
		}

		private static void PlayStep(RunState run)
		{
			Split(run.Steps[run.Index], out string verb, out string argument);
			bool playing = EditorApplication.isPlaying;
			// Play Mode starts or stops right now.
			bool changingMode = EditorApplication.isPlayingOrWillChangePlaymode != playing;

			if (verb != "Play" && verb != "Stop")
			{
				if (changingMode)
				{
					if (TimedOut(run, PlayTimeout))
						Finish("Play Mode did not start or stop");

					return;
				}

				if (playing && !_isPlayReady)
				{
					if (PlayReady != null && !PlayReady())
					{
						if (TimedOut(run, StepTimeout))
							Finish("the game did not load");

						return;
					}

					_isPlayReady = true;
				}
			}

			switch (verb)
			{
				case "Compile":
					if (playing)
					{
						Finish("Compile needs Edit Mode");
						return;
					}

					if (!run.StepStarted)
					{
						McpConsole.Clear();
						run.StepStarted = true;
						Hold(run, 0);
						// Changed scripts start compilation, and the domain reload after it restarts this class.
						AssetDatabase.Refresh();
						return;
					}

					if (StepTime(run) < CompileStartDelay)
						return;

					if (EditorUtility.scriptCompilationFailed)
						Finish(McpConsole.FirstError());
					else
						Next(run);

					return;

				case "NoErrors":
					string errors = McpConsole.FirstError();

					if (errors == McpConsole.NoErrors)
						Next(run);
					else
						Finish(errors);

					return;

				case "Play":
					if (playing && !changingMode)
					{
						// The editor has no focus while an agent works, and Play Mode stands still without this.
						Application.runInBackground = true;
						Next(run);
					}
					else if (!playing && !changingMode && !run.StepStarted)
					{
						if (EditorUtility.scriptCompilationFailed)
						{
							Finish($"Play Mode cannot start. {McpConsole.FirstError()}");
							return;
						}

						run.StepStarted = true;
						Hold(run, 0);

						if (StartPlay != null)
							StartPlay();
						else
							EditorApplication.isPlaying = true;
					}
					else if (TimedOut(run, PlayTimeout))
					{
						Finish("Play Mode did not start");
					}

					return;

				case "Stop":
					if (!playing && !changingMode)
						Next(run);
					else if (playing && !changingMode)
						EditorApplication.isPlaying = false;
					else if (TimedOut(run, PlayTimeout))
						Finish("Play Mode did not stop");

					return;

				case "Do":
					string result = Invoke(run, argument);

					if (!string.IsNullOrEmpty(result))
						run.Results.Add(result);

					Next(run);
					return;

				case "Delay":
					if (StepTime(run) >= double.Parse(argument, CultureInfo.InvariantCulture))
						Next(run);

					return;

				case "Size":
					string[] size = argument.Split('x');
					McpGameView.SetSize(int.Parse(size[0], CultureInfo.InvariantCulture), int.Parse(size[1], CultureInfo.InvariantCulture));
					Next(run);
					return;

				case "Wait":
					bool found = Object.FindObjectsByType<RectTransform>(FindObjectsSortMode.None).Any(rect => rect.name == argument);

					if (found)
						Next(run);
					else if (TimedOut(run, StepTimeout))
						Finish($"no active \"{argument}\"");

					return;

				case "Click":
					if (!playing)
					{
						Finish("Click needs Play Mode");
						return;
					}

					int hash = argument.LastIndexOf('#');
					string name = hash < 0 ? argument : argument.Substring(0, hash);
					int index = hash < 0 ? 0 : int.Parse(argument.Substring(hash + 1), CultureInfo.InvariantCulture);
					List<Selectable> buttons = McpUi.ActiveSelectables(name);

					// After Play Mode starts or a screen changes, the button appears a few seconds later.
					if (buttons.Count <= index || !buttons[index].IsInteractable())
					{
						if (TimedOut(run, StepTimeout))
							Finish(buttons.Count <= index
								? $"no active button or toggle \"{name}\" number {index}"
								: $"\"{name}\" is disabled");

						return;
					}

					string clicked = McpUi.Click(name, index);

					if (clicked.StartsWith("Clicked"))
						Next(run);
					else
						Finish(clicked);

					return;

				case "Shot":
					if (!playing)
					{
						Finish("Shot needs Play Mode");
						return;
					}

					string path = $"{run.Folder}/{argument}.png";
					File.Delete(path);
					ScreenCapture.CaptureScreenshot(path);
					run.PendingShot = path;
					// The file is written at the end of the frame.
					Hold(run, 1);
					return;

				default:
					Finish("unknown step");
					return;
			}
		}

		private static void SaveShot(RunState run)
		{
			if (!File.Exists(run.PendingShot))
			{
				if (TimedOut(run, StepTimeout))
					Finish($"screenshot {run.PendingShot} was not saved");

				return;
			}

			if (run.Scale < 1f)
				McpGameView.ShrinkImage(run.PendingShot, run.Scale);

			run.Files.Add(Path.GetFileName(run.PendingShot));
			run.PendingShot = string.Empty;
			run.Index++;
			Hold(run, 0);
		}

		// Calls "LogbookTest(pincontract, 0)" of the tools class or "QuestPrefabs.Build" of a class next to it.
		private static string Invoke(RunState run, string call)
		{
			int open = call.IndexOf('(');
			string name = open < 0 ? call : call.Substring(0, open).Trim();
			string[] arguments = open < 0
				? new string[0]
				: call.Substring(open + 1).TrimEnd(')').Split(',').Select(argument => argument.Trim().Trim('"')).Where(argument => argument.Length > 0).ToArray();

			Type type = Type.GetType(run.Tools);

			if (type == null)
				throw new InvalidOperationException($"no tools class {run.Tools}");

			int dot = name.LastIndexOf('.');

			if (dot >= 0)
			{
				string typeName = $"{type.Namespace}.{name.Substring(0, dot)}";
				type = type.Assembly.GetType(typeName);
				name = name.Substring(dot + 1);

				if (type == null)
					throw new InvalidOperationException($"no class {typeName}");
			}

			MethodInfo method = type.GetMethods(BindingFlags.Public | BindingFlags.Static).FirstOrDefault(candidate =>
				candidate.Name == name && candidate.GetParameters().Length >= arguments.Length &&
				candidate.GetParameters().Skip(arguments.Length).All(parameter => parameter.HasDefaultValue));

			if (method == null)
				throw new InvalidOperationException($"no method {name} with {arguments.Length} arguments in {type.Name}");

			object[] values = method.GetParameters()
				.Select((parameter, i) => i < arguments.Length
					? Convert.ChangeType(arguments[i], parameter.ParameterType, CultureInfo.InvariantCulture)
					: parameter.DefaultValue)
				.ToArray();

			_isInvoking = true;

			try
			{
				return method.Invoke(null, values) as string;
			}
			finally
			{
				_isInvoking = false;
			}
		}

		private static void Split(string step, out string verb, out string argument)
		{
			int colon = step.IndexOf(':');
			verb = colon < 0 ? step : step.Substring(0, colon).Trim();
			argument = colon < 0 ? string.Empty : step.Substring(colon + 1).Trim();
		}

		private static void Next(RunState run)
		{
			run.Index++;
			run.StepStarted = false;
			Hold(run, run.SettleFrames);
		}

		// Restarts the timer of the step and waits this many frames of Play Mode.
		private static void Hold(RunState run, int frames)
		{
			run.StepStart = EditorApplication.timeSinceStartup;
			_waitUntilFrame = EditorApplication.isPlaying ? Time.frameCount + frames : 0;
			Save();
		}

		private static double StepTime(RunState run)
		{
			return EditorApplication.timeSinceStartup - run.StepStart;
		}

		private static bool TimedOut(RunState run, double seconds)
		{
			return StepTime(run) > seconds;
		}

		private static void Finish(string problem)
		{
			RunState run = _run;

			if (run == null || run.IsDone)
				return;

			run.IsDone = true;
			run.Problem = problem;
			Save();
			EditorApplication.update -= Tick;
			File.WriteAllText(Path.Combine(run.Folder, ReportName), Report(run));
		}

		private static void Save()
		{
			SessionState.SetString(StateKey, JsonUtility.ToJson(_run));
		}

		private static string Report(RunState run)
		{
			string step = run.Index < run.Steps.Count ? run.Steps[run.Index] : "end";
			string state = run.IsDone
				? string.IsNullOrEmpty(run.Problem) ? "done" : $"stopped at step {run.Index + 1} \"{step}\": {run.Problem}"
				: $"at step {run.Index + 1}/{run.Steps.Count} \"{step}\"";
			string results = run.Results.Count > 0 ? $" Results: {string.Join("; ", run.Results)}" : string.Empty;
			return $"Shots {state}. Files in {run.Folder}: {string.Join(", ", run.Files)}.{results}";
		}

		[Serializable]
		private class RunState
		{
			public List<string> Steps;
			public string Tools;
			public string Folder;
			public float Scale;
			public int SettleFrames;
			public int Index;
			public double StepStart;
			// The step has done what it does once, e.g. asked for compilation, and now waits for the result.
			public bool StepStarted;
			public string PendingShot;
			public string Problem;
			public bool IsDone;
			public List<string> Files = new();
			public List<string> Results = new();
		}
	}
}
