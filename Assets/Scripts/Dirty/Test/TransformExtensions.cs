using UnityEngine;

namespace Dirty.Test
{
    public static class TransformExtensions
    {
        public static Transform GetNestedChild(this Transform parent, params int[] indexesPath)
        {
            Transform result = parent.transform;
            foreach (int index in indexesPath)
                result = result.GetChild(index);

            return result;
        }
    }
}