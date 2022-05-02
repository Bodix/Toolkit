using UnityEngine;

namespace _Game.Networking.Modes.Drift.Zone
{
    [RequireComponent(typeof(BoxTrigger))]
    public class BoxZone : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        private BoxTrigger _boxTrigger;

        protected BoxTrigger Trigger => _boxTrigger;

        private void OnValidate()
        {
            _boxTrigger = GetComponent<BoxTrigger>();
        }
    }
}