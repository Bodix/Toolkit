// Diffuse lighting shader.
// Used to light matte objects.
// Based on Lambertian reflectance.

Shader "Custom/Lit/Diffuse"
{
    Properties
    {
        [MainTexture] _MainTex ("Texture", 2D) = "" { }
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            // Used for light color (_LightColor0).
            #include "UnityLightingCommon.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = UnityObjectToWorldNormal(v.normal);

                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET
            {
                fixed4 color = tex2D(_MainTex, i.uv);

                float3 N = i.normal;
                // For the first pass, _WorldSpaceLightPos0 will always be a direction of directional light.
                // Breaks down with other types of light!
                // https://docs.unity3d.com/Manual/SL-UnityShaderVariables.html
                float3 L = _WorldSpaceLightPos0.xyz;
                float diffuseLight = dot(N, L);
                // Use saturate(x) or max(0, x) function if necessary.
                // diffuseLight = saturate(diffuseLight);
                
                // The _LightColor0 already contains the color intensity.
                // The _LightColor0 requires the "UnityLightingCommon.cginc" include.
                return color * diffuseLight * _LightColor0;
            }
            ENDCG
        }
    }
}