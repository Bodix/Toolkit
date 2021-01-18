using System.IO;
using UnityEngine;

namespace Evolutex.Evolunity.Editor.ProjectWindow
{
	/// <summary>
	/// Draws the file suffix of assets into the project window.
	/// </summary>
	public class FileSuffixDetail : ProjectWindowDetailBase
	{
		public FileSuffixDetail()
		{
			Name = "File Suffix";
			ColumnWidth = 80;
		}

		public override string GetLabel(string guid, string assetPath, Object asset)
		{
			return Path.GetExtension(assetPath);
		}
	}
}
