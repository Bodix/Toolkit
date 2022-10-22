using UnityEngine;
using UnityEngine.UI;

// You can extract all initialization logic to a generic parent class, but you will lose member names of components.
[RequireComponent(typeof(Image), typeof(AspectRatioFitter))]
public class StrongInitializationExample : MonoBehaviour
{
    [HideInInspector]
    // [SerializeField] used to save the reference, which was obtained in the OnValidate() method,
    // between play mode changes (domain reloading).
    [SerializeField]
    private Image image;

    [HideInInspector]
    // [SerializeField] used to save the reference, which was obtained in the OnValidate() method,
    // between play mode changes (domain reloading).
    [SerializeField]
    private AspectRatioFitter aspectRatioFitter;

    private bool IsComponentsInitialized => image && aspectRatioFitter;

    // To initialize components in edit time.
    // It will reduce redundant GetComponent() calls in runtime to improve performance.
    private void OnValidate()
    {
        InitializeComponentsIfRequired();
    }

    private void Awake()
    {
        InitializeComponentsIfRequired();
    }

    // If you want to execute some logic right after instantiating an object,
    // it's better to do it in the Start() method (but not necessarily),
    // because sometimes the object needs some more data to work
    // and at the same time, this data is filled from another class's Awake().
    private void Start()
    {
        // If you execute the same logic in Awake(), it may be executed before the necessary data is filled.
        DoSomethingWithComponents();
    }

    public void DoSomethingWithComponents()
    {
        // Initialization for cases when some logic is used before calling Awake() on uninitialized game objects.
        // This can happen when an object wasn't enabled after instantiating and was not initialized during edit time.
        InitializeComponentsIfRequired();

        // Some logic.
        aspectRatioFitter.aspectRatio = (float)image.sprite.texture.width / image.sprite.texture.height;
    }

    private void InitializeComponentsIfRequired()
    {
        if (!IsComponentsInitialized)
        {
            image = GetComponent<Image>();
            aspectRatioFitter = GetComponent<AspectRatioFitter>();
        }
    }
}