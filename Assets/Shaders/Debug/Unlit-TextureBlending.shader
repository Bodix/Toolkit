Shader "Unlit/Texture Blending"
{
    Properties
    {
        _FirstTex ("First Texture", 2D) = "" { }
        _SecondTex ("Second Texture", 2D) = "" { }
        _NoiseTex ("Noise Texture", 2D) = "" { }
        _NoiseIntensity("Noise Intensity", Range(0, 10)) = 1
        _NoiseForce("Noise Force", Float) = 1
        _Speed("Speed", Float) = 1
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            sampler2D _FirstTex;
            sampler2D _SecondTex;
            sampler2D _NoiseTex;
            float4 _FirstTex_ST;
            float4 _SecondTex_ST;
            float4 _NoiseTex_ST;
            float _NoiseIntensity;
            float _NoiseForce;
            float _Speed;

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv1 = TRANSFORM_TEX(v.uv, _FirstTex);
                o.uv2 = TRANSFORM_TEX(v.uv, _SecondTex);
                o.uvNoise = TRANSFORM_TEX(v.uv, _NoiseTex);

                return o;
            }

            float4 noise(float2 uv)
            {
                return tex2D(_NoiseTex, uv) * _NoiseIntensity;
            }

            fixed4 frag(v2f i) : SV_TARGET
            {
                float time = _Time.x * _Speed;
                float4 colorNoise = noise(i.uvNoise);
                float4 color1 = tex2D(_FirstTex, i.uv1 + colorNoise + time);
                float4 color2 = tex2D(_SecondTex, i.uv2 + colorNoise - time);

                return lerp(color1, color2, 0.5);
            }
            ENDCG
        }
    }
}