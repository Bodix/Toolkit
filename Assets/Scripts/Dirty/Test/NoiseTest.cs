using UnityEngine;

[ExecuteInEditMode]
public class NoiseTest : MonoBehaviour
{
    public enum NoiseType
    {
        ClassicPerlin,
        PeriodicPerlin,
        Simplex,
        FastSimplex,
        SuperSimplex
    }

    private enum GradientType
    {
        None,
        Numerical,
        Analytical
    }

    [SerializeField]
    private NoiseType _noiseType = NoiseType.ClassicPerlin;
    [SerializeField]
    private GradientType _gradientType = GradientType.None;
    [SerializeField]
    private bool _is3D = false;
    [SerializeField]
    private bool _isFractal = false;
    [SerializeField]
    private Shader shader = null;

    private Material material;

    private void Update()
    {
        if (material == null)
        {
            material = new Material(shader);
            material.hideFlags = HideFlags.DontSave;
            GetComponent<Renderer>().material = material;
        }
        else
        {
            material = GetComponent<Renderer>().material;
        }

        material.shaderKeywords = null;

        if (_noiseType == NoiseType.ClassicPerlin)
            material.EnableKeyword("CNOISE");
        else if (_noiseType == NoiseType.PeriodicPerlin)
            material.EnableKeyword("PNOISE");
        else if (_noiseType == NoiseType.Simplex)
            material.EnableKeyword("SNOISE");
        else if (_noiseType == NoiseType.FastSimplex)
            material.EnableKeyword("BCCNOISE4");
        else if (_noiseType == NoiseType.SuperSimplex)
            material.EnableKeyword("BCCNOISE8");

        if (_gradientType == GradientType.Analytical)
            material.EnableKeyword("GRAD_ANALYTICAL");
        else if (_gradientType == GradientType.Numerical)
            material.EnableKeyword("GRAD_NUMERICAL");

        if (_is3D)
            material.EnableKeyword("THREED");

        if (_isFractal)
            material.EnableKeyword("FRACTAL");
    }
}