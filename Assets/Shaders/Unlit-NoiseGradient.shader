// Unlit texture shader.
// - no lighting
// - no lightmap support

Shader "Custom/Unlit/NoiseGradient"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise", 2D) = "" {}
        _Scale ("Scale", float) = 100
        _Speed ("Speed", float) = 1

        // Use [NoScaleOffset] only for simplified version.
        // [NoScaleOffset] _MainTex ("Texture", 2D) = "" { }
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 scaledUV : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;

            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            float4 _NoiseTex_TexelSize;

            fixed _Scale;
            fixed _Speed;

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.scaledUV = o.uv / _Scale;

                return o;
            }

            float2 round_prec(float2 x, float precision)
            {
                return round(x * precision) / precision;
            }

            fixed4 frag(v2f i) : SV_TARGET
            {
                float2 vecToRounded = round_prec(i.scaledUV, 100) - i.scaledUV;
                float vecToRoundedLength = length(vecToRounded);

                // return color;
                return tex2D(_NoiseTex, i.scaledUV * vecToRoundedLength + _Time.x * _Speed);
                // return tex2D(_MainTex, float2(color, color));
            }

            ENDCG
        }
    }
}