// Copyright © 2020 Bogdan Nikolayev <bodix321@gmail.com>
// All Rights Reserved

#if UNITY_2019_OR_NEWER
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace AR
{
    [RequireComponent(typeof(ARRaycastManager))]
    public class ARRaycaster : MonoBehaviour
    {
        public TrackableType TrackableTypes = TrackableType.All;

        private readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();
        private ARRaycastManager raycastManager;

        public event Action<List<ARRaycastHit>> RaycastHit;
        public event Action RaycastMiss;

        private void Awake()
        {
            raycastManager = GetComponent<ARRaycastManager>();
        }

        private void Update()
        {
            if (RaycastFromCenter())
                RaycastHit?.Invoke(hits);
            else
                RaycastMiss?.Invoke();
        }

        private bool RaycastFromCenter()
        {
            return raycastManager.Raycast(new Vector2(Screen.width / 2f, Screen.height / 2f), hits, TrackableTypes);
        }
    }
}
#endif