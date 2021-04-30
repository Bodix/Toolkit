Shader "Open Simplex 2/Noise"
{
    SubShader
    {
        Pass
        {
            GLSLPROGRAM
            #pragma multi_compile BCCNOISE4 BCCNOISE8
            #pragma multi_compile _ THREED
            #pragma multi_compile _ FRACTAL
            #pragma multi_compile _ GRAD_NUMERICAL GRAD_ANALYTICAL
            #include "Noise.glslinc"
            ENDGLSL
        }
    }
}