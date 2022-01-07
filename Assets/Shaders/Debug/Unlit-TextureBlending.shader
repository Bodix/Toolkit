Shader "Unlit/Texture Blending"
{
    Properties
    {
        _FirstColor ("First Color", Color) = (1, 1, 1, 1)
        _SecondColor ("Second Color", Color) = (1, 1, 1, 1)
        _FirstTex ("First Texture", 2D) = "" { }
        _SecondTex ("Second Texture", 2D) = "" { }
        [KeywordEnum(Average, Darken, Multiply, Color Burn, Linear Burn)] _Blend ("Blend Mode", Float) = 2
        _NoiseTex ("Noise Texture", 2D) = "" { }
        _NoiseIntensity("Noise Intensity", Range(0, 2)) = 1
        _Speed("Speed", Float) = 1
        _Directions("Directions", Vector) = (1, 1, -1, -1)
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _BLEND_AVERAGE _BLEND_DARKEN _BLEND_MULTIPLY _BLEND_COLOR_BURN _BLEND_LINEAR_BURN
            #include "BlendModes.cginc"
            #include "UnityCG.cginc"
            
            #if defined(_BLEND_AVERAGE)
            #define BLEND_FUNC(col1, col2) half4(Average(col1, col2), 1)
            #elif defined(_BLEND_DARKEN)
            #define BLEND_FUNC(col1, col2) half4(Darken(col1, col2), 1)
            #elif defined(_BLEND_MULTIPLY)
            #define BLEND_FUNC(col1, col2) half4(Multiply(col1, col2), 1)
            #elif defined(_BLEND_COLOR_BURN)
            #define BLEND_FUNC(col1, col2) half4(ColorBurn(col1, col2), 1)
            #elif defined(_BLEND_LINEAR_BURN)
            #define BLEND_FUNC(col1, col2) half4(LinearBurn(col1, col2), 1)
            #endif

            // appdata_img
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv1 : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float2 uvNoise : TEXCOORD2;
            };

            half4 _FirstColor;
            half4 _SecondColor;
            sampler2D _FirstTex;
            sampler2D _SecondTex;
            sampler2D _NoiseTex;
            float4 _FirstTex_ST;
            float4 _SecondTex_ST;
            float4 _NoiseTex_ST;
            float _NoiseIntensity;
            float _Speed;
            float4 _Directions;

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv1 = TRANSFORM_TEX(v.uv, _FirstTex);
                o.uv2 = TRANSFORM_TEX(v.uv, _SecondTex);
                o.uvNoise = TRANSFORM_TEX(v.uv, _NoiseTex);

                return o;
            }

            half4 noise(float2 uv)
            {
                return tex2D(_NoiseTex, uv) * _NoiseIntensity;
            }

            fixed4 frag(v2f i) : SV_TARGET
            {
                float time = _Time.x * _Speed;
                half4 colorNoise = noise(i.uvNoise);
                half4 color1 = tex2D(_FirstTex, i.uv1 + colorNoise + time * _Directions.xy) * _FirstColor;
                half4 color2 = tex2D(_SecondTex, i.uv2 + colorNoise + time * _Directions.zw) * _SecondColor;

                return BLEND_FUNC(color1, color2);
            }
            ENDCG
        }
    }
}