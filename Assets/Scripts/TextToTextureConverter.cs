/*
 * Copyright (C) 2020 by Evolutex - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Bogdan Nikolaev <bodix321@gmail.com>
*/

using UnityEngine;
using UnityEngine.UI;

public static class TextToTextureConverter
{
    private static Settings currentSettings;

    public static Texture2D Convert(string text, Settings settings)
    {
        currentSettings = settings;

        Camera cam = CreateCamera();
        Text textComponent = AddTextToCamera(cam);
        Texture2D texture = new Texture2D((int)currentSettings.textureSize, (int)currentSettings.textureSize);

        textComponent.text = text;
        cam.Render();

        RenderTexture.active = cam.activeTexture;
        texture.ReadPixels(new Rect(0, 0, cam.activeTexture.width, cam.activeTexture.height), 0, 0);
        texture.Apply();

        RenderTexture.active = null;
        cam.activeTexture.Release();

        Object.DestroyImmediate(cam.gameObject);

        return texture;
    }

    public static Texture2D[] Convert(string[] texts, Settings settings)
    {
        currentSettings = settings;

        Camera cam = CreateCamera();
        Text textComponent = AddTextToCamera(cam);
        Texture2D[] textures = new Texture2D[texts.Length];

        for (int i = 0; i < textures.Length; i++)
        {
            textComponent.text = texts[i];
            cam.Render();

            RenderTexture.active = cam.activeTexture;
            textures[i] = new Texture2D((int)currentSettings.textureSize, (int)currentSettings.textureSize);
            textures[i].ReadPixels(new Rect(0, 0, cam.activeTexture.width, cam.activeTexture.height), 0, 0);
            textures[i].Apply();

            RenderTexture.active = null;
            cam.activeTexture.Release();
        }

        Object.DestroyImmediate(cam.gameObject);

        return textures;
    }

    private static Camera CreateCamera()
    {
        Camera cam = new GameObject("Camera").AddComponent<Camera>();
        if (currentSettings.backgroundColor == Color.clear)
            cam.clearFlags = CameraClearFlags.Nothing;
        else
        {
            cam.clearFlags = CameraClearFlags.Color;
            cam.backgroundColor = currentSettings.backgroundColor;
        }
        cam.depth = -100f;
        cam.targetTexture = new RenderTexture((int)currentSettings.textureSize, (int)currentSettings.textureSize, 1);

        return cam;
    }

    private static Text AddTextToCamera(Camera camera)
    {
        Canvas canvas = new GameObject("Canvas").AddComponent<Canvas>();
        canvas.transform.SetParent(camera.transform);
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = camera;

        if (currentSettings.backgroundTexture != null)
        {
            Image image = canvas.gameObject.AddComponent<Image>();
            image.sprite = Sprite.Create(
                currentSettings.backgroundTexture,
                new Rect(0, 0, currentSettings.backgroundTexture.width, currentSettings.backgroundTexture.height),
                new Vector2(currentSettings.backgroundTexture.width / 2, currentSettings.backgroundTexture.height / 2));
        }

        Text text = new GameObject("Text").AddComponent<Text>();
        text.transform.SetParent(canvas.transform);
        text.alignment = TextAnchor.MiddleCenter;
        text.font = currentSettings.font;
        text.color = currentSettings.fontColor;
        text.fontSize = (int)currentSettings.fontSize;

        RectTransform textRect = text.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        textRect.anchoredPosition3D = Vector3.zero;
        textRect.localScale = Vector3.one;
        textRect.ForceUpdateRectTransforms();

        return text;
    }

    public class Settings
    {
        public uint textureSize;
        public Color backgroundColor;
        public Texture2D backgroundTexture;
        public Font font;
        public Color fontColor;
        public uint fontSize;

        public Settings()
        {
            textureSize = 512;
            backgroundColor = Color.clear;
            backgroundTexture = null;
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            fontColor = Color.black;
            fontSize = 300;
        }
    }
}