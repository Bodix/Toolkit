using System.Collections.Generic;
using Evolutex.Evolunity.Components.Physics;
using NaughtyAttributes;
using UnityEngine;

namespace Dirty.Modules.Physics
{
    // See also:
    // https://gamedev.stackexchange.com/a/171780/132595
    // https://gist.github.com/StagPoint/4d8ca93923f66ad60ce480124c0d5092
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
            Gizmos.matrix = Matrix4x4.TRS(_boxOverlap.Center, _boxOverlap.Rotation, Vector3.one);
            Gizmos.DrawFrustum(Vector3.zero, Fov, Min, _boxOverlap.HalfExtents.z, _boxOverlap.HalfExtents.x / 
            _boxOverlap.HalfExtents.y);
        }
    }
}