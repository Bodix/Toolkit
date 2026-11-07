using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Toolkit.Dev.Editor
{
	// Saving over an existing prefab, Unity keeps the file ids of objects by their names. Where names repeat, in the prefab,
	// in its base or inside nested prefabs, objects get each other's ids or new ones, a whole nested prefab included,
	// and the overrides of variants and the links to these objects break. So for the save every object of the old prefab
	// and of the new one gets its path as the name, and the names come back after the save.
	// If an object or a component at the same path still gets another id, the old file comes back and the save fails.
	public static partial class UiBuild
	{
		private const string PathSeparator = " > ";

		public static void Save(GameObject root, string path)
		{
			try
			{
				RebuildLayout(root);

				if (File.Exists(path))
				{
					SaveOver(root, path);
				}
				else
				{
					// A new prefab needs no matching, but the next save does: the names are checked now.
					PathNames(root);
					SaveAsset(root, path);
				}
			}
			finally
			{
				Object.DestroyImmediate(root);
			}
		}

		private static void SaveOver(GameObject root, string path)
		{
			byte[] oldFile = File.ReadAllBytes(path);
			Dictionary<string, long> oldIds = Ids(path);
			List<(GameObject go, string name)> pathNames = PathNames(root);
			HashSet<string> withoutNameOverride = WithoutNameOverride(pathNames);
			Dictionary<string, string> names = ToPathNames(pathNames);

			try
			{
				EditSaved(path, contents => ToPathNames(PathNames(contents)));
				SaveAsset(root, path);
				EditSaved(path, contents => ToShortNames(contents, names, withoutNameOverride));
				CheckIds(path, oldIds);
			}
			catch
			{
				File.WriteAllBytes(path, oldFile);
				AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
				throw;
			}
		}

		/// <summary>Gives the objects their path names. Returns the short names by the path names.</summary>
		private static Dictionary<string, string> ToPathNames(List<(GameObject go, string name)> pathNames)
		{
			var names = new Dictionary<string, string>();

			foreach ((GameObject go, string name) in pathNames)
			{
				names.Add(name, go.name);
				Rename(go, name);
			}

			return names;
		}

		/// <summary>The path names of all objects but the root.</summary>
		private static List<(GameObject go, string name)> PathNames(GameObject root)
		{
			var names = new List<(GameObject go, string name)>();
			CheckComponents(root, root);

			foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
			{
				if (transform == root.transform)
					continue;

				bool own = !PrefabUtility.IsPartOfPrefabInstance(transform) || PrefabUtility.IsOutermostPrefabInstanceRoot(transform.gameObject);
				// Inside a nested prefab the order of objects is fixed, so an index tells siblings of one name apart.
				// An own object can move, and its index can go to another object.
				if (own && SameNamedSiblings(transform) > 1)
					throw new InvalidOperationException($"Two objects at {PathOf(transform, root.transform)} in {root.name}: the save keeps ids by paths, give one of them another name.");

				CheckComponents(transform.gameObject, root);
				names.Add((transform.gameObject, PathSeparator + PathOf(transform, root.transform)));
			}

			return names;
		}

		/// <summary>Unity matches the components of an object by their types, so two own components of one type swap their ids.</summary>
		private static void CheckComponents(GameObject go, GameObject root)
		{
			Component[] components = go.GetComponents<Component>().Where(component => component != null).ToArray();

			foreach (Component component in components)
			{
				bool own = !PrefabUtility.IsPartOfPrefabInstance(component);
				if (own && components.Count(other => other.GetType() == component.GetType()) > 1)
					throw new InvalidOperationException($"Two {component.GetType().Name} components on {go.name} in {root.name}: the save cannot keep their ids.");
			}
		}

		/// <summary>
		/// The path names of the objects inside nested prefabs whose names are the ones of the nested prefab.
		/// The root of a nested prefab keeps its name override, as after a usual save.
		/// </summary>
		private static HashSet<string> WithoutNameOverride(List<(GameObject go, string name)> pathNames)
		{
			var paths = new HashSet<string>();

			foreach ((GameObject go, string name) in pathNames)
			{
				GameObject source = PrefabUtility.GetCorrespondingObjectFromSource(go);
				if (source != null && source.name == go.name && !PrefabUtility.IsOutermostPrefabInstanceRoot(go))
					paths.Add(name);
			}

			return paths;
		}

		private static void ToShortNames(GameObject root, Dictionary<string, string> names, HashSet<string> withoutNameOverride)
		{
			List<GameObject> named = root.GetComponentsInChildren<Transform>(true).Select(transform => transform.gameObject)
				.Where(go => names.ContainsKey(go.name)).ToList();

			// A revert syncs the whole nested prefab with its recorded overrides, so it goes before the other names change.
			foreach (GameObject go in named.Where(go => withoutNameOverride.Contains(go.name)))
				PrefabUtility.RevertPropertyOverride(new SerializedObject(go).FindProperty("m_Name"), InteractionMode.AutomatedAction);

			foreach (GameObject go in named.Where(go => names.ContainsKey(go.name)))
				Rename(go, names[go.name]);
		}

		private static void Rename(GameObject go, string name)
		{
			go.name = name;
			// A change by code becomes an override of a nested prefab when recorded: a sync of the nested prefab drops it before.
			if (PrefabUtility.IsPartOfPrefabInstance(go))
				PrefabUtility.RecordPrefabInstancePropertyModifications(go);
		}

		/// <summary>The path of an object, with its index among siblings of the same name, if any.</summary>
		private static string PathOf(Transform transform, Transform root)
		{
			int index = 0;
			int count = 0;
			foreach (Transform sibling in transform.parent)
			{
				if (sibling == transform)
					index = count;
				if (sibling.name == transform.name)
					count++;
			}

			string name = count > 1 ? $"{transform.name} [{index}]" : transform.name;
			return transform.parent == root ? name : PathOf(transform.parent, root) + PathSeparator + name;
		}

		private static int SameNamedSiblings(Transform transform)
		{
			return transform.parent.Cast<Transform>().Count(sibling => sibling.name == transform.name);
		}

		/// <summary>Changes the saved prefab in place, so its objects keep their ids.</summary>
		private static void EditSaved(string path, Action<GameObject> edit)
		{
			GameObject contents = PrefabUtility.LoadPrefabContents(path);

			try
			{
				edit(contents);
				SaveAsset(contents, path);
			}
			finally
			{
				PrefabUtility.UnloadPrefabContents(contents);
			}
		}

		private static void SaveAsset(GameObject root, string path)
		{
			PrefabUtility.SaveAsPrefabAsset(root, path, out bool success);
			if (!success)
				throw new Exception($"Prefab not saved: {path}");
		}

		private static void CheckIds(string path, Dictionary<string, long> oldIds)
		{
			Dictionary<string, long> newIds = Ids(path);

			List<string> changed = oldIds.Where(old => newIds.TryGetValue(old.Key, out long id) && id != old.Value).Select(old => old.Key).ToList();
			if (changed.Count > 0)
				throw new InvalidOperationException($"{path} not saved: these objects would get new file ids, and variants would lose their overrides:\n{string.Join("\n", changed)}");

			List<string> removed = oldIds.Keys.Where(key => !newIds.ContainsKey(key)).ToList();
			if (removed.Count > 0)
				Debug.LogWarning($"{path}: removed or renamed, variants lose their overrides of them:\n{string.Join("\n", removed)}");
		}

		/// <summary>The file ids of the objects and components of a saved prefab by their paths.</summary>
		private static Dictionary<string, long> Ids(string path)
		{
			var ids = new Dictionary<string, long>();
			Transform root = AssetDatabase.LoadAssetAtPath<GameObject>(path).transform;

			foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
			{
				string key = transform == root ? "" : PathOf(transform, root);
				ids.Add(key, IdOf(transform.gameObject));

				var counts = new Dictionary<Type, int>();
				foreach (Component component in transform.GetComponents<Component>().Where(component => component != null))
				{
					counts.TryGetValue(component.GetType(), out int count);
					counts[component.GetType()] = count + 1;
					ids.Add($"{key} #{component.GetType().Name} {count}", IdOf(component));
				}
			}

			return ids;
		}

		private static long IdOf(Object asset)
		{
			AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out string _, out long id);
			return id;
		}
	}
}
