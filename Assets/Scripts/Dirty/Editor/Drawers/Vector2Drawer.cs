using Evolutex.Evolunity.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Evolutex.Evolunity.Editor.Drawers.Materials
{
    // TODO:
    // Fix double-lined property view on short width.

    // https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/Inspector/MaterialEditor.cs
    // TextureScaleOffsetProperty
    // VectorProperty

    public class Vector2Drawer : MaterialPropertyDrawer
    {
        private const string TypeErrorMessage =
            "You can use [Vector2] attribute only with a property with Vector type";

        private const string labelOffset = "                       ";

        public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            Assert.IsTrue(prop.type == MaterialProperty.PropType.Vector, TypeErrorMessage);

            // EditorGUI.BeginChangeCheck();
            // EditorGUI.showMixedValue = prop.hasMixedValue;

            // float labelWidth = EditorGUIUtility.labelWidth;
            // Rect labelRect = new Rect(position.x + EditorGUI.Inde, position.y, labelWidth,  position.height);
            // Rect valueRect = new Rect(controlStartX, position.y, position.width - labelWidth, position.height);

            Rect fixedPosition = position.TranslateX(-93).AddWidth(173);
            Vector4 value = prop.vectorValue;
            // EditorGUIUtility.labelWidth = 0f;

            value = EditorGUI.Vector2Field(fixedPosition, labelOffset + label, value);
            // EditorGUIUtility.labelWidth = labelWidth;

            // EditorGUI.showMixedValue = false;
            // if (EditorGUI.EndChangeCheck())
            // {
            prop.vectorValue = value;
            // }
        }
    }
}