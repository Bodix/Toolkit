// Copyright © 2020 Bogdan Nikolayev <bodix321@gmail.com>
// All Rights Reserved

#if UNITY_2019_OR_NEWER
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace AR
{
    [RequireComponent(typeof(ARPlaneManager))]
    public class ARPlanes : MonoBehaviour
    {
        private ARPlaneManager planeManager;

        public bool AnyPlaneTracked => planeManager.trackables.count > 0;

        private void Awake()
        {
            planeManager = GetComponent<ARPlaneManager>();
            planeManager.planesChanged += ToggleChangedPlanes;
        }
        
        // Used to display component activation checkbox in Inspector.
        private void Start()
        {
        }

        public void ToggleTracking(bool isOn)
        {
            planeManager.enabled = isOn;
        }

        public void TogglePlanes(bool isOn)
        {
            foreach (ARPlane plane in planeManager.trackables)
                plane.gameObject.SetActive(isOn);

            enabled = isOn;
        }

        private void ToggleChangedPlanes(ARPlanesChangedEventArgs args)
        {
            foreach (ARPlane plane in args.added)
                plane.gameObject.SetActive(enabled);

            foreach (ARPlane plane in args.updated.Where(plane => plane.gameObject.activeSelf != enabled))
                plane.gameObject.SetActive(enabled);
        }
    }
}
#endif