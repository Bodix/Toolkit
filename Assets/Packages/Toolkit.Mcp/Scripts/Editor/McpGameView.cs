using System.IO;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Mcp
{
	/// <summary>The size of the Game view and the files of its screenshots.</summary>
	public static class McpGameView
	{
		/// <summary>Sets a custom Game view resolution, for example 1920 x 1080.</summary>
		public static string SetSize(int width, int height)
		{
			PlayModeWindow.SetCustomRenderingResolution((uint)width, (uint)height, "MCP");
			return $"Game view: {width}x{height}";
		}

		/// <summary>Shrinks a saved screenshot by the scale: a smaller picture costs an AI agent fewer tokens.</summary>
		public static void ShrinkImage(string path, float scale)
		{
			Texture2D source = new(2, 2);
			source.LoadImage(File.ReadAllBytes(path));
			int width = Mathf.Max(1, Mathf.RoundToInt(source.width * scale));
			int height = Mathf.Max(1, Mathf.RoundToInt(source.height * scale));

			RenderTexture target = RenderTexture.GetTemporary(width, height);
			Graphics.Blit(source, target);
			RenderTexture previous = RenderTexture.active;
			RenderTexture.active = target;
			Texture2D small = new(width, height, TextureFormat.RGB24, false);
			small.ReadPixels(new Rect(0, 0, width, height), 0, 0);
			RenderTexture.active = previous;
			RenderTexture.ReleaseTemporary(target);

			File.WriteAllBytes(path, small.EncodeToPNG());
			Object.DestroyImmediate(source);
			Object.DestroyImmediate(small);
		}
	}
}
