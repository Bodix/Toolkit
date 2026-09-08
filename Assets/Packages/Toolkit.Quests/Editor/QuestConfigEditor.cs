using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Quests.Editor
{
	[CustomEditor(typeof(QuestConfig))]
	public class QuestConfigEditor : UnityEditor.Editor
	{
		private Type[] _objectiveTypes;
		private string[] _objectiveNames;
		private int _selectedIndex;

		// Stores foldout states for each sub-asset.
		private Dictionary<QuestObjectiveConfig, bool> _foldoutStates = new Dictionary<QuestObjectiveConfig, bool>();

		private void OnEnable()
		{
			// Finds all valid subclasses of QuestObjectiveDefinition via reflection.
			_objectiveTypes = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(a => a.GetTypes())
				.Where(t => t.IsSubclassOf(typeof(QuestObjectiveConfig)) && !t.IsAbstract)
				.ToArray();

			_objectiveNames = _objectiveTypes.Select(t => t.Name).ToArray();
		}

		public override void OnInspectorGUI()
		{
			var quest = (QuestConfig)target;

			serializedObject.Update();

			// Draws default fields (Title, Description, etc.) but hides the default Objectives list.
			DrawPropertiesExcluding(serializedObject, "Objectives");
			serializedObject.ApplyModifiedProperties();

			EditorGUILayout.Space(10);
			DrawObjectivesList(quest);

			EditorGUILayout.Space(10);
			DrawSubAssetBuilder(quest);
		}

		private void DrawObjectivesList(QuestConfig quest)
		{
			EditorGUILayout.LabelField("Objectives", EditorStyles.boldLabel);

			if (quest.Objectives == null || quest.Objectives.Count == 0)
			{
				EditorGUILayout.HelpBox("No objectives added yet.", MessageType.Info);
				return;
			}

			for (int i = 0; i < quest.Objectives.Count; i++)
			{
				var objective = quest.Objectives[i];
				if (objective == null) continue;

				if (!_foldoutStates.ContainsKey(objective))
					_foldoutStates[objective] = true;

				EditorGUILayout.BeginVertical("box");
				EditorGUILayout.BeginHorizontal();

				// Draws the foldout header.
				_foldoutStates[objective] = EditorGUILayout.Foldout(_foldoutStates[objective], objective.GetType().Name, true);

				// Delete button that immediately removes the sub-asset from memory.
				if (GUILayout.Button("X", GUILayout.Width(25)))
				{
					RemoveSubAsset(quest, objective);
					i--; // Adjusts index after list modification.
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndVertical();
					continue;
				}

				EditorGUILayout.EndHorizontal();

				// Draws the actual sub-asset inspector if expanded.
				if (_foldoutStates[objective])
				{
					EditorGUI.indentLevel++;

					// Creates or reuses a native Unity Editor for the sub-asset.
					UnityEditor.Editor objectiveEditor = null;
					CreateCachedEditor(objective, null, ref objectiveEditor);
					objectiveEditor.OnInspectorGUI();

					EditorGUI.indentLevel--;
				}

				EditorGUILayout.EndVertical();
			}
		}

		private void DrawSubAssetBuilder(QuestConfig quest)
		{
			EditorGUILayout.LabelField("Add New Objective", EditorStyles.boldLabel);

			using (new EditorGUILayout.HorizontalScope())
			{
				_selectedIndex = EditorGUILayout.Popup(_selectedIndex, _objectiveNames);

				if (GUILayout.Button("Add", GUILayout.Width(80)))
				{
					AddSubAsset(quest, _objectiveTypes[_selectedIndex]);
				}
			}
		}

		private void AddSubAsset(QuestConfig quest, Type type)
		{
			var subAsset = (QuestObjectiveConfig)CreateInstance(type);

			// Generates a unique name to ensure DataAsset creates a unique ID without duplicates.
			string uniqueHash = Guid.NewGuid().ToString().Substring(0, 5);
			subAsset.name = $"{quest.name}_{type.Name}_{uniqueHash}";

			// Natively binds the new DataAsset inside the main Quest file.
			AssetDatabase.AddObjectToAsset(subAsset, quest);
			quest.Objectives.Add(subAsset);

			// Forces your framework to generate the ID immediately.
			subAsset.GenerateId();

			_foldoutStates[subAsset] = true;

			EditorUtility.SetDirty(quest);
			AssetDatabase.SaveAssets();
		}

		private void RemoveSubAsset(QuestConfig quest, QuestObjectiveConfig objective)
		{
			quest.Objectives.Remove(objective);

			// Destroys the asset completely, preventing orphan files in the project.
			DestroyImmediate(objective, true);

			EditorUtility.SetDirty(quest);
			AssetDatabase.SaveAssets();
		}
	}
}