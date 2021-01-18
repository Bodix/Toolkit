// Copyright © 2020 Bogdan Nikolayev <bodix321@gmail.com>
// All Rights Reserved

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Obsolete.Attributes;

namespace Obsolete.Editor.Drawers
{
    [Obsolete("Use UnityEngine.LayerMask instead.")]
    [CustomPropertyDrawer(typeof(LayerMaskAttribute))]
    public class LayerMaskDrawer : PropertyDrawer
    {
        private const string TypeErrorMessage = "Use " + nameof(LayerMaskAttribute) + " with int";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Assert.IsTrue(property.propertyType == SerializedPropertyType.Integer, TypeErrorMessage);

            if (property.propertyType == SerializedPropertyType.Integer)
                property.intValue = EditorGUICustom.LayerMaskField(position, label, property.intValue);
            else
                EditorGUI.LabelField(position, TypeErrorMessage);
        }
    }
}