// Copyright © 2020 Bogdan Nikolayev <bodix321@gmail.com>
// All Rights Reserved

using System;
using System.Collections.Generic;
using Evolutex.Evolunity.Components;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARPlaneManager), typeof(ARRaycastManager))]
public class ARModelPlacer : MonoBehaviour
{
    [SerializeField]
    private bool multiplePlacing = true;
    [SerializeField]
    private InputReader inputReader = null;
    [SerializeField]
    private GameObject modelPrefab;

    private ARPlaneManager planeManager;
    private ARRaycastManager raycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private bool isPlaced;

    public event Action<GameObject> Placed;

    private void Awake()
    {
        inputReader.Click += PlaceModel;

        planeManager = GetComponent<ARPlaneManager>();
        raycastManager = GetComponent<ARRaycastManager>();
    }

    public void SetModel(GameObject model)
    {
        modelPrefab = model;
    }

    private void PlaceModel(Vector2 screenPosition)
    {
        if (!multiplePlacing && isPlaced)
        {
            inputReader.Click -= PlaceModel;

            return;
        }

        if (raycastManager.Raycast(screenPosition, hits, TrackableType.Planes))
        {
            GameObject model = Instantiate(modelPrefab, hits[0].pose.position, Quaternion.identity);
            Placed?.Invoke(model);

            foreach (ARPlane plane in planeManager.trackables)
                plane.gameObject.SetActive(false);
            planeManager.enabled = false;

            isPlaced = true;
        }
    }
}