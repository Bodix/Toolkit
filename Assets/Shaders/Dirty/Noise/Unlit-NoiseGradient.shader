// Unlit texture shader.
// - no lighting
// - no lightmap support

Shader "Dirty/Noise Gradient"
{
    Properties
    {
        _FirstColor ("First Color", Color) = (1,1,1,1)
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

            fixed4 _FirstColor;

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

            float invLerp(float from, float to, float value){
  return (value - from) / (to - from);
}

            fixed4 frag(v2f i) : SV_TARGET
            {
                float4 colorAtScaledUv = tex2D(_NoiseTex, i.scaledUV);
                return colorAtScaledUv;
                float2 vecToRounded = i.scaledUV - round_prec(i.scaledUV, 100);
                // return float4(vecToRounded*1000, 0, 1);
                float vecToRoundedLength = length(vecToRounded)*200;
                // return float4(vecToRoundedLength, vecToRoundedLength, vecToRoundedLength, 1);
                // float color = invLerp(0, colorAtScaledUv, vecToRoundedLength);
                // float color = (pow(colorAtScaledUv, 10)+vecToRoundedLength)/2;
                // return float4(color, color, color, 1);

                // return color;
                // return tex2D(_NoiseTex, i.scaledUV + vecToRoundedLength + _Time.x * _Speed);
                // return max(tex2D(_NoiseTex, i.scaledUV), vecToRoundedLength);
                // float2 vecToRounded = i.scaledUV - round_prec(i.scaledUV, 100);
                // return float4(vecToRounded*1000, 0, 1);
                // float vecToRoundedLength = length(vecToRounded)*100;
                // return float4(vecToRoundedLength, vecToRoundedLength, 0, 1);

                return _FirstColor * tex2D(_NoiseTex, i.scaledUV + _Time.x * _Speed);

                // return tex2D(_NoiseTex, i.scaledUV + vecToRoundedLength + _Time.x * _Speed);
                // return tex2D(_NoiseTex, i.scaledUV + vecToRoundedLength);
            }

            // fixed4 blur(float4 color, )

            ENDCG
        }
    }
}