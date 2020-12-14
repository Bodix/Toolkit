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