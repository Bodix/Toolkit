/*
 * Copyright (C) 2020 by Evolutex - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Bogdan Nikolaev <bodix321@gmail.com>
*/

using System;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Common.Editor.Groups
{
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