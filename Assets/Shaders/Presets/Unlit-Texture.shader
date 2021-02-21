// Unlit texture shader.
// - no lighting
// - no lightmap support

Shader "Custom/Unlit/Texture"
{
    Properties
    {
        // Use [NoScaleOffset] only for simplified version.
        [NoScaleOffset] _MainTex ("Texture", 2D) = "" { }
    }
    SubShader
    {
        // Simplified version with using UnityCG.cginc. 
        // Doesn't support texture tiling and offset.
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            
            fixed4 frag(v2f_img i): SV_TARGET
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }

        // Pass
        // {
        //     CGPROGRAM
        //     #pragma vertex vert
        //     #pragma fragment frag
            
        //     #include "UnityCG.cginc"
            
        //     struct appdata
        //     {
        //         float4 vertex: POSITION;
        //         float2 uv: TEXCOORD0;
        //     };
            
        //     struct v2f
        //     {
        //         float4 vertex: SV_POSITION;
        //         float2 uv: TEXCOORD0;
        //     };
            
        //     sampler2D _MainTex;
        //     float4 _MainTex_ST;
            
        //     v2f vert(appdata v)
        //     {
        //         v2f o;
                
        //         o.vertex = UnityObjectToClipPos(v.vertex);
        //         o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
        //         return o;
        //     }
            
        //     fixed4 frag(v2f i): SV_TARGET
        //     {
        //         return tex2D(_MainTex, i.uv);
        //     }
        //     ENDCG
        // }
    }
}