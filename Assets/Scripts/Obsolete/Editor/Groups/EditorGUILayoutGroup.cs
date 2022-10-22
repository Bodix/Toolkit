// Copyright © 2020 Bogdan Nikolayev <bodix321@gmail.com>
// All Rights Reserved

using System;
using UnityEngine;

namespace Obsolete.Editor.Groups
{
    // TODO: Add groups:
    // 1. Fade.
    // 2. ScrollView.
    // 3. Toggle.

    /// <summary>
    /// Use these methods in the "using" block.
    /// </summary>
    [Obsolete("Use EditorGUILayout scopes instead.")]
    public static class EditorGUILayoutGroup
    {
        public static EditorGUILayoutHorizontalGroup Horizontal(params GUILayoutOption[] options) =>
            new EditorGUILayoutHorizontalGroup(options);

        public static EditorGUILayoutHorizontalGroup Horizontal(GUIStyle style, params GUILayoutOption[] options) =>
            new EditorGUILayoutHorizontalGroup(style, options);

        public static EditorGUILayoutVerticalGroup Vertical(params GUILayoutOption[] options) =>
            new EditorGUILayoutVerticalGroup(options);

        public static EditorGUILayoutVerticalGroup Vertical(GUIStyle style, params GUILayoutOption[] options) =>
            new EditorGUILayoutVerticalGroup(style, options);
    }
}