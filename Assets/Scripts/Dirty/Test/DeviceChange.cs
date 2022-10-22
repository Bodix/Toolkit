using System;
using System.Collections;
using UnityEngine;

namespace Dirty.Test
{
    public class DeviceChange : MonoBehaviour
    {
        public static event Action<Vector2> OnResolutionChange;
        public static event Action<DeviceOrientation> OnOrientationChange;
        public static float CheckDelay = 0.5f; // How long to wait until we check again.

        private static Vector2 resolution; // Current Resolution
        private static DeviceOrientation orientation; // Current Device Orientation
        private static bool isAlive = true; // Keep this script running?

        private void Start()
        {
            StartCoroutine(CheckForChange());
        }

        private IEnumerator CheckForChange()
        {
            resolution = new Vector2(Screen.width, Screen.height);
            orientation = UnityEngine.Input.deviceOrientation;

            while (isAlive)
            {
                // Check for a Resolution Change
                if (resolution.x != Screen.width || resolution.y != Screen.height)
                {
                    resolution = new Vector2(Screen.width, Screen.height);
                    if (OnResolutionChange != null) OnResolutionChange(resolution);
                }

                // Check for an Orientation Change
                switch (UnityEngine.Input.deviceOrientation)
                {
                    case DeviceOrientation.Unknown: // Ignore
                    case DeviceOrientation.FaceUp: // Ignore
                    case DeviceOrientation.FaceDown: // Ignore
                        break;
                    default:
                        if (orientation != UnityEngine.Input.deviceOrientation)
                        {
                            orientation = UnityEngine.Input.deviceOrientation;
                            if (OnOrientationChange != null) OnOrientationChange(orientation);
                        }

                        break;
                }

                yield return new WaitForSeconds(CheckDelay);
            }
        }

        private void OnDestroy()
        {
            isAlive = false;
        }
    }
}