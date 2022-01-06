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
    [RequireComponent(typeof(ARPlanes), typeof(ARRaycastManager))]
    public class ARModelPlacer : MonoBehaviour
    {
        public GameObject ModelPrefab;
        public bool MultiplePlacing = true;
        public bool DisablePlanesAfterPlacing = true;
        public bool DisableTrackingAfterPlacing = false;
        public bool RotateModelToCamera = true;
        public TrackableType TrackableTypes = TrackableType.Planes;
        public HitIndex HitIndex = HitIndex.Last;

        private readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();
        private ARPlanes planes;
        private ARRaycastManager raycastManager;
        private new Camera camera;
        private bool isPlaced;

        public event Action<GameObject> Placed;
        
        private void Awake()
        {
            planes = GetComponent<ARPlanes>();
            raycastManager = GetComponent<ARRaycastManager>();
            camera = Camera.main;
        }

        public bool PlaceModel()
        {
            return PlaceModel(new Vector2(Screen.width / 2f, Screen.height / 2f));
        }

        public bool PlaceModel(Vector2 screenPosition)
        {
            if (!MultiplePlacing && isPlaced)
                return false;

            if (raycastManager.Raycast(screenPosition, hits, TrackableTypes))
            {
                InstantiateModel(hits[GetHitIndex()].pose.position, Quaternion.identity);

                if (DisableTrackingAfterPlacing)
                    planes.ToggleTracking(false);

                if (DisablePlanesAfterPlacing)
                    planes.TogglePlanes(false);

                return isPlaced = true;
            }
            else return false;
        }

        public void InstantiateModel(Vector3 position, Quaternion rotation)
        {
            GameObject model = Instantiate(ModelPrefab, position, rotation);

            if (RotateModelToCamera)
                model.transform.LookAt(new Vector3(
                    camera.transform.position.x,
                    model.transform.position.y,
                    camera.transform.position.z));

            Placed?.Invoke(model);
        }

        private int GetHitIndex()
        {
            switch (HitIndex)
            {
                case HitIndex.First:
                    return 0;
                case HitIndex.Last:
                    return hits.Count > 0 ? hits.Count - 1 : 0;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    public enum HitIndex
    {
        First,
        Last
    }
}
# endif