using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evolutex.Evolunity.Components.Physics;
using NaughtyAttributes;
using UnityEngine;

namespace AI
{
    // See also:
    // https://www.clonefactor.com/wordpress/public/2481/
    public abstract class AiVision : MonoBehaviour
    {
        [InfoBox("If enabled, it will check for collisions at a given interval")]
        public ConicalOverlap Overlap;
        public Transform Eyes;
        public float TimeInterval = 1;

        protected IEnumerable<Collider> _collidersInRange;

        private Coroutine _checkCoroutine;

        private void OnValidate()
        {
            Overlap.SetPoseTransform(Eyes);
        }

        private void Reset()
        {
            enabled = false;
        }

        private void OnEnable()
        {
            _checkCoroutine = StartCoroutine(CheckCoroutine());
        }

        private void OnDisable()
        {
            if (_checkCoroutine != null)
                StopCoroutine(_checkCoroutine);
        }

        private IEnumerator CheckCoroutine()
        {
            while (enabled)
            {
                yield return new WaitForSeconds(TimeInterval);

                Look(out _collidersInRange);
                OnCollidersOverlap(_collidersInRange);
            }
        }

        public int Look(out IEnumerable<Collider> colliders)
        {
            Overlap.SetPoseTransform(Eyes);
            Overlap.Execute(out colliders);

            return colliders.Count();
        }

        protected abstract void OnCollidersOverlap(IEnumerable<Collider> colliders);
    }
}