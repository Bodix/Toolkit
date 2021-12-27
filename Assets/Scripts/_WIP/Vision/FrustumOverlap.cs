using System;
using System.Collections.Generic;
using System.Linq;
using Dirty.Test;
using Evolutex.Evolunity.Components.Physics;
using NaughtyAttributes;
using UnityEngine;

namespace AI
{
    public class FrustumOverlap : MonoBehaviour
    {
        [SerializeField]
        public float Fov = 90;
        public float Min = 1;
        public float Max = 2;
        public float Aspect = 1;
        [SerializeField]
        private BoxOverlap _boxOverlap;

        [Button]
        private void Execute()
        {
            _boxOverlap.Execute(out IEnumerable<Collider> colliders);
            
            
        }

        public void Execute(out IEnumerable<Collider> colliders)
        {
            _boxOverlap.Execute(out colliders);
            
            // colliders = colliders.Where()
        }

        private void OnDrawGizmos()
        {
            Gizmos.matrix = Matrix4x4.TRS(_boxOverlap.Center, _boxOverlap.Orientation, Vector3.one);
            Gizmos.DrawFrustum(Vector3.zero, Fov, Min, _boxOverlap.HalfExtents.z, _boxOverlap.HalfExtents.x / 
            _boxOverlap.HalfExtents.y);
        }
    }
}