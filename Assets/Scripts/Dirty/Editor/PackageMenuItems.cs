// Evolunity for Unity
// Copyright © 2020 Bogdan Nikolayev <bodix321@gmail.com>
// All Rights Reserved

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Dirty.Editor
{
	/// <summary>
	/// Packs the selected packages into tarballs with <see cref="Client.Pack"/>, the same call
	/// the Asset Store UPM uploader uses. The tarballs go to the "_packages" folder next to "Assets".
	/// </summary>
	public static class PackageMenuItems
	{
		private const string OutputFolderName = "_packages";
		private const string LogPrefix = "[Pack Package]";

		[MenuItem("Assets/Pack Package", priority = 113)]
		public static void PackSelectedPackages()
		{
			string projectFolder = Directory.GetParent(Application.dataPath).FullName;
			string outputFolder = Path.Combine(projectFolder, OutputFolderName);
			Directory.CreateDirectory(outputFolder);

			foreach (PackageInfo package in GetSelectedPackages())
				Pack(package, outputFolder);
		}

		[MenuItem("Assets/Pack Package", true)]
		private static bool CanPackSelectedPackages()
		{
			return GetSelectedPackages().Count > 0;
		}

		private static void Pack(PackageInfo package, string outputFolder)
		{
			Debug.Log($"{LogPrefix} Packing {package.name} {package.version}...");

			PackRequest request = Client.Pack(package.resolvedPath, outputFolder);

			void WaitForCompletion()
			{
				if (!request.IsCompleted)
					return;

				EditorApplication.update -= WaitForCompletion;

				if (request.Status == StatusCode.Success)
					Debug.Log($"{LogPrefix} {package.name} {package.version} is packed to {request.Result.tarballPath}");
				else
					Debug.LogError($"{LogPrefix} {package.name} could not be packed: {request.Error.message}");
			}

			EditorApplication.update += WaitForCompletion;
		}

		// Only packages whose source files are on disk can be packed: embedded ones (in the "Packages" folder)
		// and local ones (referenced with "file:"). Packages from a registry or Git live in the read-only cache.
		private static List<PackageInfo> GetSelectedPackages()
		{
			List<PackageInfo> packages = new List<PackageInfo>();

			foreach (string guid in Selection.assetGUIDs)
			{
				PackageInfo package = PackageInfo.FindForAssetPath(AssetDatabase.GUIDToAssetPath(guid));

				if (package == null)
					continue;

				if (package.source != PackageSource.Embedded && package.source != PackageSource.Local)
					continue;

				if (!packages.Exists(other => other.name == package.name))
					packages.Add(package);
			}

			return packages;
		}
	}
}