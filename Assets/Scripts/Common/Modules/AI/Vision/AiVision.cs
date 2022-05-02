using System;
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
        public ConicalOverlap Overlap;
        public Transform Eyes;
        public bool AutoCheck;
        [ShowIf(nameof(AutoCheck))]
        public float Interval = 1;

        protected IEnumerable<Collider> _collidersInRange;

        private Coroutine _checkCoroutine;

        private void OnValidate()
        {
            Overlap.SetPoseTransform(Eyes);
        }

        private void OnEnable()
        {
            _checkCoroutine = StartCoroutine(CheckCoroutine());
        }

        private void OnDisable()
        {
            StopCoroutine(_checkCoroutine);
        }

        private IEnumerator CheckCoroutine()
        {
            while (enabled)
            {
                yield return new WaitForSeconds(Interval);

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