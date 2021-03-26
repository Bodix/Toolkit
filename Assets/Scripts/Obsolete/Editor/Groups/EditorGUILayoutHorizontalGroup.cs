// Copyright © 2020 Bogdan Nikolayev <bodix321@gmail.com>
// All Rights Reserved

using System;
using UnityEditor;
using UnityEngine;

namespace Obsolete.Editor.Groups
{
    [Obsolete("Use EditorGUILayout.HorizontalScope instead.")]
    public class EditorGUILayoutHorizontalGroup : IDisposable
    {
        private readonly Rect rect;
        
        public EditorGUILayoutHorizontalGroup(params GUILayoutOption[] options)
        {
            rect = EditorGUILayout.BeginHorizontal(options);
        }

        public EditorGUILayoutHorizontalGroup(GUIStyle style, params GUILayoutOption[] options)
        {
            rect = EditorGUILayout.BeginHorizontal(style, options);
        }

        public void Dispose()
        {
            EditorGUILayout.EndHorizontal();
        }

        public static implicit operator Rect(EditorGUILayoutHorizontalGroup group) => group.rect;
    }
}