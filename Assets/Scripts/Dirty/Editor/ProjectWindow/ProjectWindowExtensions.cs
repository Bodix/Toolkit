// Evolunity for Unity
// Copyright © 2020 Bogdan Nikolayev <bodix321@gmail.com>
// All Rights Reserved

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Evolutex.Evolunity.Editor
{
    [InitializeOnLoad]
    internal class ProjectWindowExtensions
    {
        private static readonly Color LabelColor = new Color(0.75f, 0.75f, 0.75f, 1.0f);

        static ProjectWindowExtensions()
        {
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
        }
        
        // public static bool Enabled
        // {
        //     // get => EditorPlayerPrefs.
        // }

        private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            // string path = AssetDatabase.GUIDToAssetPath(guid);
            // string extension = Path.GetExtension(path);
            //
            // bool icons = selectionRect.height > 20;
            // if (icons || path.Length == 0)
            //     return;
            //
            // GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            // Vector2 labelSize = labelStyle.CalcSize(new GUIContent(extension));
            //
            // Rect newRect = selectionRect;
            // newRect.width += selectionRect.x;
            // newRect.x = newRect.width - labelSize.x - 4;
            //
            // Color prevGuiColor = GUI.color;
            // GUI.color = LabelColor;
            // GUI.Label(newRect, extension, labelStyle);
            // GUI.color = prevGuiColor;
        }
    }
}