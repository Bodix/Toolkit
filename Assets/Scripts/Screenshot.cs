// Copyright © 2020 Bogdan Nikolayev <bodix321@gmail.com>
// All Rights Reserved

using System;
using System.IO;
using UnityEngine;

public static class Screenshot
{
    private static Screenshoter screenshoter;
    private static string name = "screenshot";

    public static string Path => Application.persistentDataPath + "/" + name + ".png";

    public static void TakeAndSave(Transform target, ScreenshotSettings settings,
        string name, bool destroyOnComplete = true)
    {
        if (!string.IsNullOrEmpty(name))
            Screenshot.name = name;

        Take(target, settings, Save, destroyOnComplete);
    }

    public static void Take(Transform target, ScreenshotSettings settings,
        Action<byte[]> callback, bool destroyOnComplete = true)
    {
        if (destroyOnComplete)
            callback += bytes => UnityEngine.Object.Destroy(screenshoter);

        if (screenshoter == null)
            screenshoter = Camera.main.gameObject.AddComponent<Screenshoter>();

        screenshoter.TakeScreenshot(target, settings, callback);
    }

    private static void Save(byte[] bytes) => File.WriteAllBytes(Path, bytes);
}

[RequireComponent(typeof(Camera))]
[DisallowMultipleComponent]
public class Screenshoter : MonoBehaviour
{
    private Camera cam;
    private Transform model;
    private ScreenshotSettings settings;
    private Action<byte[]> callback;
    private bool takeScreenshot;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    public void TakeScreenshot(Transform model, ScreenshotSettings settings, Action<byte[]> callback)
    {
        this.model = model;
        this.settings = settings;
        this.callback = callback;

        takeScreenshot = true;
    }

    private void LateUpdate()
    {
        if (takeScreenshot)
        {
            Vector3 modelPosition = model.position;
            Quaternion modelRotation = model.rotation;
            Vector3 modelScale = model.localScale;

            CameraClearFlags clearFlags = cam.clearFlags;
            Color backgroundColor = cam.backgroundColor;
            bool isOrthographic = cam.orthographic;
            float size = isOrthographic ? cam.orthographicSize : cam.fieldOfView;
            int cullingMask = cam.cullingMask;

            SetupModelAndCamera();
            callback?.Invoke(RenderScreen());

            model.position = modelPosition;
            model.rotation = modelRotation;
            model.localScale = modelScale;

            cam.clearFlags = clearFlags;
            cam.backgroundColor = backgroundColor;
            cam.orthographic = isOrthographic;
            if (isOrthographic) cam.orthographicSize = size;
            else cam.fieldOfView = size;
            cam.cullingMask = cullingMask;

            takeScreenshot = false;
        }
    }

    private void SetupModelAndCamera()
    {
        model.position = settings.ModelPosition;
        model.rotation = settings.ModelRotation;
        model.localScale = settings.ModelScale;

        cam.clearFlags = CameraClearFlags.Color;
        cam.backgroundColor = settings.BackgroundColor;
        cam.orthographic = true;
        cam.orthographicSize = settings.CameraSize;
        if (settings.IsIgnoreUI)
            cam.cullingMask = ~(1 << 5);
    }

    private byte[] RenderScreen()
    {
        // Render screen to texture.
        RenderTexture renderTexture = new RenderTexture(settings.Resolution.x, settings.Resolution.y, 24);
        cam.targetTexture = renderTexture;
        cam.Render();

        // Read pixels from texture.
        RenderTexture.active = renderTexture;
        Texture2D screenshot = new Texture2D(
            settings.Resolution.x,
            settings.Resolution.y,
            TextureFormat.RGBA32,
            false);
        screenshot.ReadPixels(new Rect(0, 0, settings.Resolution.x, settings.Resolution.y), 0, 0);

        // Destroy texture.
        cam.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        return screenshot.EncodeToPNG();
    }
}

public class ScreenshotSettings
{
    public Vector2Int Resolution { get; set; } = new Vector2Int(1024, 1024);

    public Vector3 ModelPosition { get; set; } = Vector3.zero;
    public Quaternion ModelRotation { get; set; } = Quaternion.identity;
    public Vector3 ModelScale { get; set; } = Vector3.one;

    public Color BackgroundColor { get; set; } = Color.clear;
    public float CameraSize { get; set; } = 10;
    public bool IsIgnoreUI { get; set; } = true;
}