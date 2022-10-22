using System;
using NaughtyAttributes;
using UnityEngine;

namespace _WIP.Utilities
{
    public class OutOfZoneHandler : MonoBehaviour
    {
        [SerializeField]
        private Timer _timer;

        public float Timeout = 20f;

        public event Action ExitZone;
        public event Action EnterZone;
        public event Action TimeoutEvent;

        [ShowNativeProperty]
        public bool IsOutOfZone { get; private set; }

        private void Awake()
        {
            _timer.Completed += () =>
            {
                ToggleOutOfZone(false);

                TimeoutEvent.Invoke();
            };
        }

        public void HandleZoneExit()
        {
            if (!IsOutOfZone)
            {
                ToggleOutOfZone(true);

                ExitZone.Invoke();
            }
        }

        public void HandleZoneEnter()
        {
            if (IsOutOfZone)
            {
                ToggleOutOfZone(false);

                EnterZone.Invoke();
            }
        }

        public void ToggleOutOfZone(bool isOutOfZone)
        {
            IsOutOfZone = isOutOfZone;

            if (isOutOfZone)
                _timer.Start(Timeout);
            else
                _timer.Stop();
        }
    }
}