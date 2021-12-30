using System.Collections.Generic;
using System.Linq;
using Dirty.Test;
using Evolutex.Evolunity.Components.Physics;
using UnityEngine;

namespace Dirty.Modules.Physics
{
    [RequireComponent(typeof(BoxOverlap))]
    public class ConicalOverlap : MonoBehaviour
    {
        // public float Angle;
        [SerializeField, HideInInspector]
        private BoxOverlap _boxOverlap;
        [SerializeField]
        private bool GizmosEnabled;

        // private float Angle
        // {
        //     get
        //     {
        //         if (_boxOverlap.HalfExtents.x > _boxOverlap.HalfExtents.y)
        //             return Vec
        //     }
        // }

        private float Angle;

        private void OnValidate()
        {
            _boxOverlap = GetComponent<BoxOverlap>();
        }

        public int Execute(out IEnumerable<Collider> colliders)
        {
            _boxOverlap.Execute(out colliders);
            colliders = colliders.Where(x =>
            {
                Vector3 forwardDir = _boxOverlap.Pose.forward;
                Vector3 colliderDir = (x.transform.position - _boxOverlap.Center).normalized;
                float dot = Vector3.Dot(forwardDir, colliderDir);

                return dot >= Mathf.Cos(Angle / 2);
            });

            return colliders.Count();
        }
        
        public void OnDrawGizmos()
        {
            if (!GizmosEnabled)
                return;

            Gizmos.matrix = Matrix4x4.TRS(_boxOverlap.Center, _boxOverlap.Orientation, Vector3.one);
            // using (new GizmosColorScope(Color.Lerp(GizmosDefaultColor, GizmosExecutedColor,
            //     Mathf.SmoothDamp(_gizmosColorValue, 0, ref _gizmosColorValue, GizmosFadeTime))))
            GizmosExtend.DrawCone(Vector3.zero, Vector3.forward * _boxOverlap.HalfExtents.z, angle: Angle / 2);
        }
        
    }
}