using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AspectRatioFitter), typeof(Image))]
public class ImageFitter : MonoBehaviour
{
    private AspectRatioFitter aspectRatioFitter;
    private Image image;

    private void Start()
    {
        aspectRatioFitter = GetComponent<AspectRatioFitter>();
        image = GetComponent<Image>();

        Fit();
    }

    public void Fit()
    {
        aspectRatioFitter.aspectRatio = (float)image.sprite.texture.width / image.sprite.texture.height;
    }
}