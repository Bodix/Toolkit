/*
 * Copyright (C) 2020 by Evolutex - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Bogdan Nikolaev <bodix321@gmail.com>
*/

using System;
using UnityEngine;

public struct Direction
{
    public static Direction Up => new Direction(Vector2.up, vector => vector.y);
    public static Direction Down => new Direction(Vector2.down, vector => -vector.y);
    public static Direction Right => new Direction(Vector2.right, vector => vector.x);
    public static Direction Left => new Direction(Vector2.left, vector => -vector.x);
        
    public readonly Vector2 NormalizedVector;
    public readonly float Angle;
        
    private readonly Func<Vector2, float> axisSelector;

    private Direction(Vector2 vector, Func<Vector2, float> axisSelector)
    {
        NormalizedVector = vector.normalized;
        Angle = Vector2.SignedAngle(Vector2.right, NormalizedVector);
            
        this.axisSelector = axisSelector;
    }
        
    public bool Equals(Direction other)
    {
        return NormalizedVector.Equals(other.NormalizedVector) && Angle.Equals(other.Angle) && Equals(axisSelector, other.axisSelector);
    }

    public override bool Equals(object obj)
    {
        return obj is Direction other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = NormalizedVector.GetHashCode();
            hashCode = (hashCode * 397) ^ Angle.GetHashCode();
            hashCode = (hashCode * 397) ^ (axisSelector != null ? axisSelector.GetHashCode() : 0);
            return hashCode;
        }
    }
        
    public float GetAxis(Vector2 vector)
    {
        return axisSelector(vector);
    }

    public static bool operator ==(Direction direction1, Direction direction2)
    {
        return direction1.Equals(direction2);
    }

    public static bool operator !=(Direction direction1, Direction direction2)
    {
        return !direction1.Equals(direction2);
    }
}