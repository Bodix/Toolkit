// Unlit shader. Simplest possible colored shader.
// - no lighting
// - no lightmap support
// - no texture

Shader "Custom/Unlit/Color"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            
            #pragma fragment frag
            #pragma vertex vert
            
            struct appdata
            {
                float4 vertex: POSITION;
            };
            
            struct v2f
            {
                float4 vertex: SV_POSITION;
            };
            
            float4 _Color;
            
            fixed4 frag(v2f i): SV_Target
            {
                return _Color;
            }
            
            v2f vert(appdata v)
            {
                v2f o;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                return o;
            }
            ENDCG
        }
    }
}
