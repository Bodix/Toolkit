using System.Linq;
using UnityEditor;
using UnityEngine;

namespace _WIP.FindInParent.Editor
{
    [CustomPropertyDrawer(typeof(FindInParentAttribute))]
    public class FindInParentDrawer : AttributePropertyDrawer<FindInParentAttribute>
    {
        protected override SerializedPropertyType[] SupportedTypes => new[]
        {
            SerializedPropertyType.ObjectReference
        };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.objectReferenceValue = ((Component)property.serializedObject.targetObject)
                .GetComponentsInParent(property.GetFieldInfo().FieldType, Attribute.IncludeInactive)
                .FirstOrDefault();

            EditorGUILayout.ObjectField(property, label);
        }
    }
}