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
            // Use for light color.
            // #include "UnityLightingCommon.cginc"

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
                // For the first pass, it will always be a direction of directional light.
                // https://docs.unity3d.com/Manual/SL-UnityShaderVariables.html
                float3 L = _WorldSpaceLightPos0.xyz;
                float diffuseLight = dot(N, L);
                // Use saturate(x) or max(0, x) function if you plan to use the light value somewhere else.
                // float diffuseLight = saturate(dot(N, L));
        
                return color * diffuseLight;
                // Use _LightColor0 to apply light color (required "UnityLightingCommon.cginc" include).
                // return color * diffuseLight * _LightColor0;
            }
            ENDCG
        }
    }
}