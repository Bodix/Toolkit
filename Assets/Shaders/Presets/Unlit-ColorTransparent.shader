// Unlit transparent shader. 
// Simplest possible colored shader with transparency:
// - no texture
// - no lighting

Shader "Custom/Unlit/Color Transparent"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 _Color;

            float4 vert(float4 vertex : POSITION) : SV_POSITION
            {
                return UnityObjectToClipPos(vertex);
            }

            fixed4 frag() : SV_TARGET
            {
                return _Color;
            }
            ENDCG
        }
    }
}