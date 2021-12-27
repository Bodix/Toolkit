using System.Collections.Generic;
using System.Linq;
using Dirty.Test;
using UnityEngine;

namespace AI
{
    public class AiVision : MonoBehaviour
    {
        public float Radius = 5;
        [Range(0, 360)]
        public float Angle = 90;
        public Transform Eyes;
        public LayerMask Layers = Physics.AllLayers;

        private readonly Collider[] _collidersBuffer = new Collider[512];

        private Transform Origin => Eyes ? Eyes : transform;

        public int LookSpherical(out IEnumerable<Collider> colliders)
        {
            int collidersCount = Physics.OverlapSphereNonAlloc(
                Origin.transform.position, Radius, _collidersBuffer, Layers);
            colliders = _collidersBuffer.Take(collidersCount).Where(x => x != null);

            return collidersCount;
        }
    }
}