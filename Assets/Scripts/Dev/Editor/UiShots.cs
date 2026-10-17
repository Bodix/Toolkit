using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Dev.Editor
{
	/// <summary>
	/// Screenshots of the Game view at several resolutions, for checking UI. Called from Unity MCP.
	/// Capture works only in Play Mode: it sets each size, waits a few frames and saves a PNG.
	/// Files go to Screenshots/UiShots in the project root (ignored by Git).
	/// </summary>
	public static class UiShots
	{
		public const string Folder = "Screenshots/UiShots";
		private const int WaitFrames = 6;

		private static readonly Queue<(string path, int width, int height)> _queue = new Queue<(string, int, int)>();
		private static (string path, int width, int height) _current;
		private static int _stepFrame;
		private static int _stage;
		private static readonly List<string> _done = new List<string>();

		/// <summary>Sets a fixed Game view resolution. Adds it to the list of sizes if it is missing.</summary>
		public static string SetGameViewSize(int width, int height)
		{
			Assembly editor = typeof(UnityEditor.Editor).Assembly;
			Type sizesType = editor.GetType("UnityEditor.GameViewSizes");
			Type singleton = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
			object sizes = singleton.GetProperty("instance").GetValue(null);
			object group = sizesType.GetProperty("currentGroup").GetValue(sizes);
			Type groupType = group.GetType();
			Type sizeType = editor.GetType("UnityEditor.GameViewSize");
			Type sizeKind = editor.GetType("UnityEditor.GameViewSizeType");

			int count = (int)groupType.GetMethod("GetTotalCount").Invoke(group, null);
			int index = -1;
			for (int i = 0; i < count; i++)
			{
				object size = groupType.GetMethod("GetGameViewSize").Invoke(group, new object[] { i });
				int w = (int)sizeType.GetProperty("width").GetValue(size);
				int h = (int)sizeType.GetProperty("height").GetValue(size);
				int kind = Convert.ToInt32(sizeType.GetProperty("sizeType").GetValue(size));
				if (w == width && h == height && kind == 1)
				{
					index = i;
					break;
				}
			}

			if (index < 0)
			{
				object fixedKind = Enum.ToObject(sizeKind, 1);
				object newSize = Activator.CreateInstance(sizeType, fixedKind, width, height, $"{width}x{height}");
				groupType.GetMethod("AddCustomSize").Invoke(group, new[] { newSize });
				index = count;
			}

			Type gameViewType = editor.GetType("UnityEditor.GameView");
			EditorWindow gameView = EditorWindow.GetWindow(gameViewType, false, null, false);
			PropertyInfo selected = gameViewType.GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (selected != null && selected.CanWrite)
				selected.SetValue(gameView, index);
			else
				gameViewType.GetMethod("SizeSelectionCallback", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
					.Invoke(gameView, new object[] { index, null });

			gameView.Repaint();
			return $"Game view: {width}x{height}";
		}

		/// <summary>
		/// Starts screenshots of the Game view at each size, e.g. "2520x1080,1440x1080".
		/// Files: Screenshots/UiShots/name_WxH.png. Poll <see cref="Status"/> until it says done.
		/// </summary>
		public static string Capture(string name, string sizes = "2520x1080,1440x1080")
		{
			if (!Application.isPlaying)
				return "Capture needs Play Mode";

			Application.runInBackground = true;
			Directory.CreateDirectory(Folder);
			foreach (string size in sizes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
			{
				string[] parts = size.Trim().Split('x');
				int width = int.Parse(parts[0]);
				int height = int.Parse(parts[1]);
				_queue.Enqueue(($"{Folder}/{name}_{width}x{height}.png", width, height));
			}

			if (_stage == 0)
			{
				_done.Clear();
				_stage = 1;
				EditorApplication.update -= Tick;
				EditorApplication.update += Tick;
			}

			return $"Capture queued: {_queue.Count}";
		}

		/// <summary>"busy N left" or "done" with the saved files.</summary>
		public static string Status()
		{
			if (_stage != 0)
				return $"busy {_queue.Count + 1} left";
			return "done: " + string.Join(", ", _done);
		}

		private static void Tick()
		{
			if (!Application.isPlaying)
			{
				_queue.Clear();
				Stop();
				return;
			}

			switch (_stage)
			{
				case 1:
					if (_queue.Count == 0)
					{
						Stop();
						return;
					}

					_current = _queue.Dequeue();
					if (File.Exists(_current.path))
						File.Delete(_current.path);
					SetGameViewSize(_current.width, _current.height);
					_stepFrame = Time.frameCount;
					_stage = 2;
					break;
				case 2:
					if (Time.frameCount - _stepFrame < WaitFrames)
						return;
					ScreenCapture.CaptureScreenshot(_current.path);
					_stepFrame = Time.frameCount;
					_stage = 3;
					break;
				case 3:
					if (!File.Exists(_current.path) && Time.frameCount - _stepFrame < 60)
						return;
					_done.Add(Path.GetFileName(_current.path) + $" (screen {Screen.width}x{Screen.height})");
					_stage = 1;
					break;
			}
		}

		private static void Stop()
		{
			_stage = 0;
			EditorApplication.update -= Tick;
		}
	}
}
