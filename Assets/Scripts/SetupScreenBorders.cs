/*
 * Copyright (C) 2020 by Evolutex - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Bogdan Nikolaev <bodix321@gmail.com>
*/

using UnityEngine;
using Evolunity.Attributes;

public class SetupScreenBorders : MonoBehaviour
{
    [SerializeField, Layer] private int layer = 0;
    [SerializeField] private new Camera camera;
    [SerializeField] private float thickness = 1f;
        
    private BoxCollider2D topBorder;
    private BoxCollider2D bottomBorder;
    private BoxCollider2D leftBorder;
    private BoxCollider2D rightBorder;

    private void Awake()
    {
        if (!camera)
            camera = Camera.main;
                
        Create();
        SetPositions();
        SetSizes();
    }

    private void Create()
    {
        topBorder = new GameObject("Top Border").AddComponent<BoxCollider2D>();
        bottomBorder = new GameObject("Bottom Border").AddComponent<BoxCollider2D>();
        leftBorder = new GameObject("Left Border").AddComponent<BoxCollider2D>();
        rightBorder = new GameObject("Right Border").AddComponent<BoxCollider2D>();
            
        topBorder.transform.SetParent(transform);
        bottomBorder.transform.SetParent(transform);
        leftBorder.transform.SetParent(transform);
        rightBorder.transform.SetParent(transform);

        topBorder.gameObject.layer = layer;
        bottomBorder.gameObject.layer = layer;
        leftBorder.gameObject.layer = layer;
        rightBorder.gameObject.layer = layer;
    }
        
    private void SetPositions()
    {
        Vector3 horizontalBordersPosition = camera.ViewportToWorldPoint(
            new Vector3(
                0.5f, 
                1f + thickness / camera.orthographicSize / 4,
                -camera.transform.position.z));
        Vector3 verticalBordersPosition = camera.ViewportToWorldPoint(
            new Vector3(
                1f + thickness / camera.orthographicSize / (4 * camera.aspect),
                0.5f, 
                -camera.transform.position.z));

        topBorder.transform.position = horizontalBordersPosition;
        bottomBorder.transform.position = -horizontalBordersPosition;
        leftBorder.transform.position = verticalBordersPosition;
        rightBorder.transform.position = -verticalBordersPosition;
    }

    private void SetSizes()
    {
        Vector2 width = camera.ViewportToWorldPoint(Vector3.right) - camera.ViewportToWorldPoint(Vector3.zero);
        Vector2 height = camera.ViewportToWorldPoint(Vector3.up) - camera.ViewportToWorldPoint(Vector3.zero);
            
        topBorder.size = new Vector2(width.magnitude, thickness);
        bottomBorder.size = new Vector2(width.magnitude, thickness);
        leftBorder.size = new Vector2(thickness, height.magnitude);
        rightBorder.size = new Vector2(thickness, height.magnitude);
    }
}