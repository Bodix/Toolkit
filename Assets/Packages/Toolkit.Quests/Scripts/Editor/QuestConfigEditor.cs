using System;
using System.Collections.Generic;
using System.Linq;
using Bodix.Evolunity.Collections;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Quests.Editor
{
	[CustomEditor(typeof(QuestConfig))]
	public class QuestConfigEditor : UnityEditor.Editor
	{
		private Type[] _objectiveTypes;
		private string[] _objectiveNames;
		private int _selectedObjectiveIndex;

		private Type[] _rewardTypes;
		private string[] _rewardNames;
		private int _selectedRewardIndex;

		// Stores foldout states for each sub-asset.
		private Dictionary<DataAsset, bool> _foldoutStates = new Dictionary<DataAsset, bool>();

		private void OnEnable()
		{
			_objectiveTypes = GetSubAssetTypes<QuestObjectiveConfig>();
			_objectiveNames = _objectiveTypes.Select(t => t.Name).ToArray();

			_rewardTypes = GetSubAssetTypes<QuestRewardConfig>();
			_rewardNames = _rewardTypes.Select(t => t.Name).ToArray();
		}

		private Type[] GetSubAssetTypes<T>() where T : DataAsset
		{
			return AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(a => a.GetTypes())
				.Where(t => t.IsSubclassOf(typeof(T)) && !t.IsAbstract)
				.ToArray();
		}

		public override void OnInspectorGUI()
		{
			var quest = (QuestConfig)target;

			if (GUILayout.Button("Regenerate ID", GUILayout.Height(25)))
			{
				quest.GenerateId();
				EditorUtility.SetDirty(quest);
				AssetDatabase.SaveAssets();
			}
			EditorGUILayout.Space(5);

			serializedObject.Update();

			// Draws default fields (Title, Description, etc.) but hides the default lists to prevent duplication.
			DrawPropertiesExcluding(serializedObject, "m_Script", "_objectives", "_rewards");
			serializedObject.ApplyModifiedProperties();

			EditorGUILayout.Space(10);
			DrawSubAssetList("Objectives", quest, quest.Objectives, _objectiveTypes, _objectiveNames, ref _selectedObjectiveIndex);

			EditorGUILayout.Space(10);
			DrawSubAssetList("Rewards", quest, quest.Rewards, _rewardTypes, _rewardNames, ref _selectedRewardIndex);
		}

		private void DrawSubAssetList<T>(string title, QuestConfig quest, List<T> list, Type[] types, string[] typeNames, ref int selectedIndex) where T : DataAsset
		{
			EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

			if (list == null || list.Count == 0)
			{
				EditorGUILayout.HelpBox($"No {title.ToLower()} added yet.", MessageType.Info);
			}
			else
			{
				for (int i = 0; i < list.Count; i++)
				{
					var subAsset = list[i];
					if (subAsset == null) continue;

					if (!_foldoutStates.ContainsKey(subAsset))
						_foldoutStates[subAsset] = true;

					EditorGUILayout.BeginVertical("box");
					EditorGUILayout.BeginHorizontal();

					// Draws the foldout header.
					_foldoutStates[subAsset] = EditorGUILayout.Foldout(_foldoutStates[subAsset], subAsset.GetType().Name, true);

					// Delete button that immediately removes the sub-asset from memory.
					if (GUILayout.Button("X", GUILayout.Width(25)))
					{
						RemoveSubAsset(quest, list, subAsset);
						i--; // Adjusts index after list modification.
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.EndVertical();
						continue;
					}

					EditorGUILayout.EndHorizontal();

					// Draws the actual sub-asset inspector if expanded.
					if (_foldoutStates[subAsset])
					{
						EditorGUI.indentLevel++;

						// Creates or reuses a native Unity Editor for the sub-asset.
						UnityEditor.Editor subAssetEditor = null;
						CreateCachedEditor(subAsset, null, ref subAssetEditor);
						if (subAssetEditor != null)
						{
							subAssetEditor.OnInspectorGUI();
						}

						EditorGUI.indentLevel--;
					}

					EditorGUILayout.EndVertical();
				}
			}

			EditorGUILayout.Space(5);
			EditorGUILayout.LabelField($"Add New {title.TrimEnd('s')}", EditorStyles.boldLabel);
			
			using (new EditorGUILayout.HorizontalScope())
			{
				selectedIndex = EditorGUILayout.Popup(selectedIndex, typeNames);

				if (GUILayout.Button("Add", GUILayout.Width(80)) && types.Length > 0)
				{
					AddSubAsset(quest, list, types[selectedIndex]);
				}
			}
		}

		private void AddSubAsset<T>(QuestConfig quest, List<T> list, Type type) where T : DataAsset
		{
			var subAsset = (T)CreateInstance(type);

			// Generates a unique name to ensure DataAsset creates a unique ID without duplicates.
			string uniqueHash = Guid.NewGuid().ToString().Substring(0, 5);
			subAsset.name = $"{quest.name}_{type.Name}_{uniqueHash}";

			// Natively binds the new DataAsset inside the main Quest file.
			AssetDatabase.AddObjectToAsset(subAsset, quest);
			list.Add(subAsset);

			// Forces your framework to generate the ID immediately.
			subAsset.GenerateId();

			_foldoutStates[subAsset] = true;

			EditorUtility.SetDirty(quest);
			AssetDatabase.SaveAssets();
		}

		private void RemoveSubAsset<T>(QuestConfig quest, List<T> list, T subAsset) where T : DataAsset
		{
			list.Remove(subAsset);

			// Destroys the asset completely, preventing orphan files in the project.
			DestroyImmediate(subAsset, true);

			EditorUtility.SetDirty(quest);
			AssetDatabase.SaveAssets();
		}
	}
}
