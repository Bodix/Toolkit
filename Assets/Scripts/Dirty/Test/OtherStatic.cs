using System;
using UnityEngine;

namespace Dirty.Test
{
    public static class OtherStatic
    {
        /// <summary>
        /// Round float value to nearest approximation with added offset.
        /// </summary>
        /// <param name="value">Value which will be rounded.</param>
        /// <param name="approximation">Sampling rate for rounding.</param>
        /// <param name="offset">Offset for result.</param>
        /// <returns>Returns rounded float value with added offset.</returns>
        public static float RoundTo(this float value, float approximation, float offset = 0)
        {
            if (approximation <= 0)
                throw new ArgumentException($"Could not round the value, because the approximation ({approximation}) " +
                    "is less than or equal to zero!");
            value -= offset;
            value /= approximation;
            value = Mathf.Round(value);
            return value * approximation + offset;
        }

        public static Vector3 RoundTo(this Vector3 value, Vector3 approximation, Vector3 offset = default)
        {
            value.x = value.x.RoundTo(approximation.x, offset.x);
            value.y = value.y.RoundTo(approximation.y, offset.y);
            value.z = value.z.RoundTo(approximation.z, offset.z);
            return value;
        }

        public static Vector3 ClampMin(this Vector3 value, Vector3 min)
        {
            if (value.x < min.x)
                value.x = min.x;
            if (value.y < min.y)
                value.y = min.y;
            if (value.z < min.z)
                value.z = min.z;
            return value;
        }

        public static Vector3Int[] GetNearPositions(this Vector3Int cell)
        {
            Vector3Int back = cell + new Vector3Int(0, 0, -1);
            Vector3Int forward = cell + new Vector3Int(0, 0, 1);
            Vector3Int left = cell - Vector3Int.right;
            Vector3Int right = cell + Vector3Int.right;
            Vector3Int up = cell + Vector3Int.up;
            Vector3Int down = cell - Vector3Int.up;
            return new[] { back, forward, left, right, up, down, cell };
        }
    }
}