// Copyright © 2020 Bogdan Nikolayev <bodix321@gmail.com>
// All Rights Reserved

using UnityEngine;

namespace Extensions
{
    public static class OtherExtensions
    {
        // Too infrequently used extension method for a frequently used type.
        public static float NormalizeAngle(this float angle)
        {
            while (angle > 360)
                angle -= 360;

            while (angle < 0)
                angle += 360;

            return angle;
        }

        // Leads to performance degradation and design disruption.
        public static void DestroyComponentImmediateIfExists<T>(this Component component) where T : Component
        {
            T otherComponent = component.GetComponent<T>();

            if (otherComponent != null)
                Object.DestroyImmediate(otherComponent);
        }

        // Leads to performance degradation and design disruption.
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            T otherComponent = component.GetComponent<T>();

            if (otherComponent == null)
                otherComponent = component.gameObject.AddComponent<T>();

            return otherComponent;
        }

        // Leads to design disruption.
        public static Transform GetNestedChild(this Transform transform, params int[] indexesPath)
        {
            Transform child = transform.transform;

            foreach (int index in indexesPath)
                child = child.GetChild(index);

            return child;
        }
    }
}