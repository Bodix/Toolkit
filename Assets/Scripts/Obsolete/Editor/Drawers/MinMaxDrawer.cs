// Copyright © 2020 Bogdan Nikolayev <bodix321@gmail.com>
// All Rights Reserved

using System;
using Obsolete.Attributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Obsolete.Editor.Drawers
{
    [Obsolete("Use NaughtyAttributes.MinMaxSlider instead.")]
    [CustomPropertyDrawer(typeof(MinMaxAttribute))]
    public class MinMaxDrawer : PropertyDrawer
    {
        private const string ErrorMessage = "You can use MinMax attribute only on a Vector2";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Assert.IsTrue(property.propertyType == SerializedPropertyType.Vector2, ErrorMessage);

            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                MinMaxAttribute minMaxAttribute = attribute as MinMaxAttribute;

                // The variable name on the left.
                Rect totalValueRect = EditorGUI.PrefixLabel(position, label);

                // The left value, after the variable name.
                Rect leftRect = new Rect(totalValueRect.x, totalValueRect.y, 50, totalValueRect.height);

                // Rect of the slider.
                Rect valueRect = new Rect(leftRect.xMax, totalValueRect.y,
                    totalValueRect.width - leftRect.width * 2 - 4, totalValueRect.height);

                // The right value.
                Rect rightRect = new Rect(totalValueRect.xMax - leftRect.width - 2, totalValueRect.y,
                    leftRect.width, totalValueRect.height);

                // Current x.
                float minValue = property.vector2Value.x;
                // Current y.
                float maxValue = property.vector2Value.y;

                EditorGUI.MinMaxSlider(valueRect, ref minValue, ref maxValue, minMaxAttribute.Min, minMaxAttribute.Max);

                // Assigns the value to the property.
                property.vector2Value = new Vector2(minValue, maxValue);

                // Writes the value on the left.
                EditorGUI.LabelField(leftRect, minValue.ToString("F3"));
                // Writes the value on the right.
                EditorGUI.LabelField(rightRect, maxValue.ToString("F3"));
            }
            else
            {
                GUI.Label(position, ErrorMessage);
            }
        }
    }
}