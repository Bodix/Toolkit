// Specular lighting shader.
// Used to light glossy objects.
// Based on (Blinn-)Phong reflection model.

Shader "Custom/Lit/Specular"
{
    Properties
    {
        [MainTexture] _MainTex ("Texture", 2D) = "" { }
        _Smoothness ("Smoothness", Float) = 1
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
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Smoothness;

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);     

                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET
            {
                fixed4 color = tex2D(_MainTex, i.uv);

                // Normalization is used to normalize normals not equal in length to 1, obtained after interpolation.
                // This fixes the ugly grid effect (for example, сan be seen on a default sphere or capsule mesh).
                float3 N = normalize(i.normal);
                // For the first pass, _WorldSpaceLightPos0 will always be a direction of directional light.
                // Breaks down with other types of light!
                // https://docs.unity3d.com/Manual/SL-UnityShaderVariables.html
                float3 L = _WorldSpaceLightPos0.xyz;
                float3 V = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 R = reflect(-L, N);
                float specularLight = dot(V, R);
                // Use saturate(x) or max(0, x) function if necessary.
                // specularLight = saturate(specularLight);
                specularLight = pow(specularLight, _Smoothness);
                
                // The _LightColor0 already contains the color intensity.
                // The _LightColor0 requires the "UnityLightingCommon.cginc" include.
                return color * specularLight * _LightColor0;
            }
            ENDCG
        }
    }
}