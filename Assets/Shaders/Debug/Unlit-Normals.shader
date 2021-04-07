Shader "Custom/Unlit/Normals"
{
    Properties
    {
        [Toggle(WORLD_SPACE)] _WorldSpace ("World Space", float) = 0
    }

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Local directives for multiple shader program variants were added in Unity 2019.1.
            // Also `#if UNITY_VERSION > 201910` not working with `#pragma` directives.
            // https://forum.unity.com/threads/unity_version-broken-in-2018-2.540272/
            // https://docs.unity3d.com/Manual/SL-MultipleProgramVariants.html
            // 
            // #pragma shader_feature_local WORLD_SPACE
            #pragma shader_feature WORLD_SPACE

            #include "UnityCG.cginc"

            struct appdata
            {
                float3 vertex : POSITION;
                half3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                half3 normal : NORMAL;
            };

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                #ifdef WORLD_SPACE
                o.normal = UnityObjectToWorldNormal(v.normal);
                #else
                o.normal = v.normal;
                #endif

                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET
            {
                fixed4 c = 0;

                c.rgb = i.normal * 0.5 + 0.5;

                return c;
            }
            ENDCG
        }
    }
}