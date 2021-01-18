// Copyright © 2020 Bogdan Nikolayev <bodix321@gmail.com>
// All Rights Reserved

using System;
using UnityEngine;

namespace Obsolete.Attributes
{
    [Obsolete("Use NaughtyAttributes.MinMaxSlider instead.")]
    public class MinMaxAttribute : PropertyAttribute
    {
        public float Max { get; }
        public float Min { get; }

        public MinMaxAttribute(int min, int max)
        {
            Min = min;
            Max = max;
        }
    }
}