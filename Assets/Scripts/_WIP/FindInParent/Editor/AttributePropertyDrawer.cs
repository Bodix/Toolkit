using System.Linq;
using Evolutex.Evolunity.Extensions;
using UnityEditor;
using UnityEngine;

namespace _WIP.FindInParent.Editor
{
    public abstract class AttributePropertyDrawer<T> : PropertyDrawer where T : PropertyAttribute
    {
        protected abstract SerializedPropertyType[] SupportedTypes { get; }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return -2f;
        }

        protected T Attribute => (T)attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (IsSupported(property))
            {
                OnValidatedGUI(position, property, label);
            }
            else
            {
                string typeErrorText = "Type " + property.propertyType
                    + " is not supported with this property attribute.\n\n"
                    + "Supported property types:\n"
                    + SupportedTypes.AsString(x => "- " + x, "\n");

                EditorGUI.HelpBox(GUILayoutUtility.GetRect(new GUIContent(typeErrorText), EditorStyles.helpBox),
                    typeErrorText, MessageType.Error);
            }
        }

        protected virtual bool IsSupported(SerializedProperty property)
        {
            return SupportedTypes.Any(x => x == property.propertyType);
        }

        protected virtual void OnValidatedGUI(Rect position, SerializedProperty property, GUIContent label) { }
    }
}