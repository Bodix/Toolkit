using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    // [CustomPropertyDrawer(typeof(List2))]
    // public class List : PropertyDrawer
    // {
    //     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //     {
    //         List2 serializedComponentResetter = property.GetSerializedValue<List2>();
    //         // Debug.Log("123");
    //     }
    // }
    //
    // [CustomPropertyDrawer(typeof(SerializableContainer))]
    // public class _Test2 : PropertyDrawer
    // {
    //     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //     {
    //         SerializableContainer serializedComponentResetter = property.GetSerializedValue<SerializableContainer>();
    //
    //         // Debug.Log("123");
    //     }
    // }
    //
    // [CustomPropertyDrawer(typeof(List<SerializableContainer>))]
    // public class Testtt2 : PropertyDrawer
    // {
    //     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //     {
    //         List<SerializableContainer> serializedComponentResetter = property.GetSerializedValue<List<SerializableContainer>>();
    //
    //         // Debug.Log("123");
    //     }
    // }
    
    
}