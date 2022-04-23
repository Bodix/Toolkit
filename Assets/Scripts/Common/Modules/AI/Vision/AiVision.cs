using System.Collections.Generic;
using System.Linq;
using Evolutex.Evolunity.Utilities.Gizmos;
using UnityEngine;

namespace AI.Vision
{
    // See also:
    // https://www.clonefactor.com/wordpress/public/2481/
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

        public int LookBox(out IEnumerable<Collider> colliders)
        {
            int collidersCount = Physics.OverlapBoxNonAlloc(
                Origin.transform.position, new Vector3(Radius, Radius, Radius),
                _collidersBuffer, Origin.rotation, Layers);
            colliders = _collidersBuffer.Take(collidersCount).Where(x => x != null);

            return collidersCount;
        }

        public int LookConical(out IEnumerable<Collider> colliders)
        {
            LookBox(out colliders);
            colliders = colliders.Where(x =>
            {
                Vector3 forwardDir = Origin.forward;
                Vector3 colliderDir = (x.transform.position - Origin.position).normalized;
                float dot = Vector3.Dot(forwardDir, colliderDir);

                return dot >= Mathf.Cos(Angle / 2);
            });

            return colliders.Count();
        }

        // TODO: Make frustum look.
        // public int LookFrustum(out IEnumerable<Collider> colliders)
        // {
        //     LookSpherical(out colliders);
        //     colliders = colliders.Where(x =>
        //     {
        //         
        //     });
        //
        //     return colliders.Count();
        // }

        public void OnDrawGizmos()
        {
            GizmosExtend.DrawCone(Origin.transform.position, Origin.forward * Radius, angle: Angle / 2);
        }
    }
}