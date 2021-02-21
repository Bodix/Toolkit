Shader "Custom/Unlit/Normals"
{
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
                float3 vertex : POSITION;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                
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