using System;
using UnityEngine;

namespace _Game.Networking.Modes.Drift.Zone
{
    [RequireComponent(typeof(BoxCollider))]
    public class BoxTrigger : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        private BoxCollider _boxCollider;

        public event Action<Collider> TriggerEnter;
        public event Action<Collider> TriggerStay;
        public event Action<Collider> TriggerExit;

        private void OnValidate()
        {
            _boxCollider = GetComponent<BoxCollider>();
            _boxCollider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            TriggerEnter?.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            TriggerStay?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            TriggerExit?.Invoke(other);
        }
    }
}