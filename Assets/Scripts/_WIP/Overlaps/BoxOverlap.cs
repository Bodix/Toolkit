using System;
using System.Collections.Generic;
using System.Linq;
using Evolutex.Evolunity.Utilities.Gizmos;
using UnityEngine;

namespace AI.Overlaps
{
    public class BoxOverlap : MonoBehaviour
    {
        public Transform Origin;
        public Vector3 HalfExtents = Vector3.one;
        public LayerMask Layers = Physics.AllLayers;
        
        private readonly Collider[] _collidersBuffer = new Collider[512];
        private bool _wasExecuted;
        
        private Transform GetOrigin => Origin ? Origin : transform;
        
        public int Execute(out IEnumerable<Collider> colliders)
        {
            int collidersCount = Physics.OverlapBoxNonAlloc(
                GetOrigin.transform.position, HalfExtents,
                _collidersBuffer, GetOrigin.rotation, Layers);
            colliders = _collidersBuffer.Take(collidersCount).Where(x => x != null);

            _wasExecuted = true;

            return collidersCount;
        }

        private void Update()
        {
            Execute(out IEnumerable<Collider> colliders);
        }

        private void OnDrawGizmos()
        {
            if (!_wasExecuted)
                Gizmos.DrawCube(GetOrigin.transform.position, HalfExtents);
            else
            {
                using (new GizmosColorScope(Color.green))
                {
                    Gizmos.DrawCube(GetOrigin.transform.position, HalfExtents);
            
                    _wasExecuted = false;
                }
            }
        }
    }
}