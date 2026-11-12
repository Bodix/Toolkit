using System;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace Toolkit.Mcp
{
	/// <summary>
	/// Errors of the editor console in one short answer: reading the console through Unity MCP returns the whole
	/// stack trace of every message. Uses the internal API of the editor: if a Unity update breaks it,
	/// read one to three console entries through Unity MCP instead.
	/// </summary>
	public static class McpConsole
	{
		public const string NoErrors = "No errors in the console";

		private const string ApiNotFound = "Console API not found";
		private const BindingFlags Flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		// Console entry modes that mean an error: Error, Assert, Fatal, AssetImportError, ScriptingError,
		// ScriptCompileError, ScriptingException, GraphCompileError, ScriptingAssertion (UnityEditor.ConsoleWindow.Mode).
		private const int ErrorModes = (1 << 0) | (1 << 1) | (1 << 4) | (1 << 6) | (1 << 8) | (1 << 11) | (1 << 17)
			| (1 << 20) | (1 << 21);

		/// <summary>
		/// The first error in the console with the first lines of its stack trace, and the number of errors.
		/// Errors after the first one are usually its consequences.
		/// </summary>
		public static string FirstError(int stackLines = 3)
		{
			Type entries = typeof(EditorApplication).Assembly.GetType("UnityEditor.LogEntries");
			Type entryType = typeof(EditorApplication).Assembly.GetType("UnityEditor.LogEntry");

			if (entries == null || entryType == null)
				return ApiNotFound;

			object entry = Activator.CreateInstance(entryType);
			FieldInfo messageField = entryType.GetField("message");
			FieldInfo modeField = entryType.GetField("mode");
			MethodInfo getEntry = entries.GetMethod("GetEntryInternal", Flags);
			int count = (int)entries.GetMethod("StartGettingEntries", Flags).Invoke(null, null);
			string first = null;
			int errors = 0;

			try
			{
				for (int i = 0; i < count; i++)
				{
					getEntry.Invoke(null, new[] { i, entry });

					if ((Convert.ToInt32(modeField.GetValue(entry)) & ErrorModes) == 0)
						continue;

					errors++;
					first ??= (string)messageField.GetValue(entry);
				}
			}
			finally
			{
				entries.GetMethod("EndGettingEntries", Flags).Invoke(null, null);
			}

			if (first == null)
				return NoErrors;

			string text = string.Join("\n", first.Split('\n').Take(1 + stackLines)).TrimEnd();
			return $"Errors: {errors}. First:\n{text}";
		}

		/// <summary>Clears the console, so <see cref="FirstError" /> then shows only new errors. Compile errors stay.</summary>
		public static string Clear()
		{
			Type entries = typeof(EditorApplication).Assembly.GetType("UnityEditor.LogEntries");
			MethodInfo clear = entries?.GetMethod("Clear", Flags);

			if (clear == null)
				return ApiNotFound;

			clear.Invoke(null, null);
			return "Console cleared";
		}
	}
}
