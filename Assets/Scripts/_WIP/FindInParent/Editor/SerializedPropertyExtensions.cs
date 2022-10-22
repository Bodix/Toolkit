using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace _WIP.FindInParent.Editor
{
    public static class SerializedPropertyExtensions
    {
        // Also may be useful:
        // https://gist.github.com/aholkner/214628a05b15f0bb169660945ac7923b

        public static T GetSerializedValue<T>(this SerializedProperty property)
        {
            object @object = GetSerializedValue(property);

            if (@object.GetType().GetInterfaces().Contains(typeof(IList<T>)))
            {
                int propertyIndex = int.Parse(property.propertyPath[property.propertyPath.Length - 2].ToString());

                return ((IList<T>)@object)[propertyIndex];
            }
            else return (T)@object;
        }

        public static FieldInfo GetFieldInfo(this SerializedProperty property)
        {
            string[] propertyNames = property.GetPropertyNames();

            object @object = property.serializedObject.targetObject;
            foreach (string path in propertyNames.Take(propertyNames.Length - 1))
            {
                @object = @object.GetType()
                    .GetField(path, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .GetValue(@object);
            }

            return @object.GetType()
                .GetField(
                    propertyNames[propertyNames.Length - 1],
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        }

        public static object GetSerializedValue(this SerializedProperty property)
        {
            object @object = property.serializedObject.targetObject;
            string[] propertyNames = property.GetPropertyNames();

            // Get the last object of the property path.
            foreach (string path in propertyNames)
            {
                @object = @object.GetType()
                    .GetField(path, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .GetValue(@object);
            }

            return @object;
        }

        private static string[] GetPropertyNames(this SerializedProperty property)
        {
            string[] propertyNames = property.propertyPath.Split('.');

            // Clear the property path from "Array" and "data[i]".
            if (propertyNames.Length >= 3 && propertyNames[propertyNames.Length - 2] == "Array")
                propertyNames = propertyNames.Take(propertyNames.Length - 2).ToArray();

            return propertyNames;
        }

        public static void ForEachArrayElement(this SerializedProperty serializedProperty,
            Action<SerializedProperty> action)
        {
            for (int i = 0; i < serializedProperty.arraySize; i++)
                action.Invoke(serializedProperty.GetArrayElementAtIndex(i));
        }

        // https://forum.unity.com/threads/loop-through-serializedproperty-children.435119/
        // https://answers.unity.com/questions/1147569/serializedproperty-with-children-class-fields.html
        public static IEnumerable<SerializedProperty> GetChildrenCopies(this SerializedProperty serializedProperty)
        {
            SerializedProperty propertyCopy = serializedProperty.Copy();

            if (propertyCopy.Next(true))
            {
                do
                {
                    yield return propertyCopy.Copy();
                }
                while (propertyCopy.Next(false));
            }
        }

        public static IEnumerable<SerializedProperty> GetVisibleChildrenCopies(
            this SerializedProperty serializedProperty)
        {
            SerializedProperty propertyCopy = serializedProperty.Copy();

            if (propertyCopy.NextVisible(true))
            {
                do
                {
                    yield return propertyCopy.Copy();
                }
                while (propertyCopy.NextVisible(false));
            }
        }
    }
}