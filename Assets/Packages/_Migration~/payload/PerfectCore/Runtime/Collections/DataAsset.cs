// Perfect Core for Unity
// Copyright © 2026 Bogdan Nikolayev <bodix321@gmail.com>
// All Rights Reserved.

using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PerfectCore
{
	/// <summary>
	/// Base class for all data assets with an auto-generated unique identifier.
	/// </summary>
	public abstract class DataAsset : ScriptableObject
	{
		[Tooltip("Unique string ID in 'namespace:name' format")]
		[SerializeField]
		private string _id;

		public string Id => _id;

#if UNITY_EDITOR
		protected virtual void OnValidate()
		{
			if (string.IsNullOrWhiteSpace(_id))
				GenerateId();
		}

		/// <summary>
		/// Rebuilds the ID from the asset name and its folder.
		/// Available from the inspector's context menu (the three dots in the header).
		/// </summary>
		[ContextMenu("Regenerate ID")]
		public void GenerateId()
		{
			string expectedName = ReplaceNonAlphanumerics(name.ToLower());
			string assetPath = AssetDatabase.GetAssetPath(this);

			if (!string.IsNullOrEmpty(assetPath))
			{
				string parentFolder = Path.GetFileName(Path.GetDirectoryName(assetPath))?.ToLower();
				parentFolder = ReplaceNonAlphanumerics(parentFolder ?? string.Empty);

				_id = $"{parentFolder}:{expectedName}";
			}
			else
			{
				_id = expectedName;
			}

			EditorUtility.SetDirty(this);
		}

		/// <summary>
		/// Replace any non-alphanumeric characters with an underscore.
		/// </summary>
		private string ReplaceNonAlphanumerics(string text)
		{
			return Regex.Replace(text, "[^a-z0-9]+", "_").Trim('_');
		}
#endif
	}
}
