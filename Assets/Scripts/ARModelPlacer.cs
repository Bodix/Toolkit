/*
 * Copyright (C) 2020 by Evolutex - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Bogdan Nikolaev <bodix321@gmail.com>
*/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Evolunity.Components.Input;

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