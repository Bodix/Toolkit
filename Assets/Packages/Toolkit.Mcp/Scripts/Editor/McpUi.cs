using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Toolkit.Mcp
{
	/// <summary>Acts on uGUI as a player would, by the names of objects.</summary>
	public static class McpUi
	{
		/// <summary>
		/// Clicks an active button or toggle by the name of its object, as a player would: a disabled one is not pressed.
		/// Index picks one of several objects with this name, in hierarchy order. Only in Play Mode.
		/// </summary>
		public static string Click(string name, int index = 0)
		{
			if (!Application.isPlaying)
				return "Click needs Play Mode";

			List<Selectable> found = ActiveSelectables(name);

			if (index < 0 || index >= found.Count)
				return $"No active button or toggle \"{name}\" number {index}: {found.Count} found";

			Selectable target = found[index];

			if (!target.IsInteractable())
				return $"\"{name}\" is disabled";

			ExecuteEvents.Execute(target.gameObject,
				new PointerEventData(EventSystem.current),
				ExecuteEvents.pointerClickHandler);

			string state = target is Toggle toggle ? toggle.isOn ? ", now on" : ", now off" : string.Empty;
			return $"Clicked \"{name}\" {index + 1} of {found.Count}{state}";
		}

		/// <summary>Active buttons and toggles with this name, in hierarchy order.</summary>
		public static List<Selectable> ActiveSelectables(string name)
		{
			return Object.FindObjectsByType<Selectable>(FindObjectsSortMode.None)
				.Where(selectable => selectable.name == name && selectable.isActiveAndEnabled)
				.OrderBy(selectable => HierarchyKey(selectable.transform), StringComparer.Ordinal)
				.ToList();
		}

		/// <summary>Sibling indices from the root: sorting by the key follows the hierarchy.</summary>
		public static string HierarchyKey(Transform current)
		{
			StringBuilder key = new();

			for (; current != null; current = current.parent)
				key.Insert(0, current.GetSiblingIndex().ToString("D4") + "/");

			return key.ToString();
		}
	}
}
