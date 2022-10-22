// Copyright © 2020 Bogdan Nikolayev <bodix321@gmail.com>
// All Rights Reserved

using System;
using UnityEditor;
using UnityEngine;

namespace Obsolete.Editor.Groups
{
    [Obsolete("Use EditorGUILayout.VerticalScope instead.")]
    public class EditorGUILayoutVerticalGroup : IDisposable
    {
        private readonly Rect rect;

        public EditorGUILayoutVerticalGroup(params GUILayoutOption[] options)
        {
            rect = EditorGUILayout.BeginVertical(options);
        }

        public EditorGUILayoutVerticalGroup(GUIStyle style, params GUILayoutOption[] options)
        {
            rect = EditorGUILayout.BeginVertical(style, options);
        }

        public void Dispose()
        {
            EditorGUILayout.EndVertical();
        }

        public static implicit operator Rect(EditorGUILayoutVerticalGroup group) => group.rect;
    }
}