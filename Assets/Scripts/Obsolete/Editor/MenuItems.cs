// Copyright © 2020 Bogdan Nikolayev <bodix321@gmail.com>
// All Rights Reserved

using System;
using UnityEditor;
using UnityEngine;

namespace Obsolete.Editor
{
    public static class MenuItems
    {
        // [MenuItem("Edit/Clear PlayerPrefs", priority = 270)]
        // [MenuItem("Tools/Evolunity/Clear PlayerPrefs")]
        [Obsolete("Use \"Edit/Clear All PlayerPrefs\" instead.")]
        public static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            EditorUtility.DisplayDialog("Clear PlayerPrefs", "PlayerPrefs was successfully cleared", "OK");
        }
    }
}