// Copyright © 2020 Bogdan Nikolayev <bodix321@gmail.com>
// All Rights Reserved

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Obsolete.Editor
{
    public static class EditorGUICustom
    {
        private const int MaxLayersCount = 32;

        public static LayerMask LayerMaskField(Rect position, GUIContent label, LayerMask mask)
        {
            List<string> layers = new List<string>(MaxLayersCount);
            for (int i = 0; i < MaxLayersCount; i++)
            {
                string layerName = LayerMask.LayerToName(i);

                if (layerName != "")
                    layers.Add(layerName);
            }

            mask.value = EditorGUI.MaskField(position, label, mask.value, layers.ToArray());

            return mask;
        }
    }
}