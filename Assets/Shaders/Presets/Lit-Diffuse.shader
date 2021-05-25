// Diffuse (Lambertian Reflectance) lighting shader.
// Used to light matte objects.

Shader "Custom/Dirty/Lit/Diffuse"
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
            #include "AutoLight.cginc"

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
        
                return color * diffuseLight;
            }
            ENDCG
        }
    }
}