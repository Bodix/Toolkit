// https://www.intel.com/content/www/us/en/developer/articles/technical/an-investigation-of-fast-real-time-gpu-based-image-blur-algorithms.html

Shader "Dirty/Blur"
{
    Properties
    {
        [MainTexture] _MainTex ("Texture", 2D) = "" { }
//        _

        // Use [NoScaleOffset] only for simplified version.
        // [NoScaleOffset] [MainTexture] _MainTex ("Texture", 2D) = "" { }
    }
    SubShader
    {
        Blend Off
        
        Tags { "Queue" = "Transparent" "RenderType"="Opaque" }
        
        GrabPass
        {
            "_GrabTex"
        }
        
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
                float4 grabPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            // Use x and y to get tiling.
            // Use z and w to get offset.
            float4 _MainTex_ST;
            sampler2D _GrabTex;
            float4 _GrabTex_TexelSize;

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.grabPos = ComputeGrabScreenPos (o.vertex);

                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET
            {
                // fixed4 grabColor = tex2D(_GrabTex, i.grabPos.xy);
                float4 grabColor = tex2Dproj(_GrabTex, i.grabPos);
                
                float4 col0 = tex2D(_GrabTex, (i.grabPos + float2(-_GrabTex_TexelSize.x * 3, 0)) / i.grabPos.w);
                float4 col1 = tex2D(_GrabTex, (i.grabPos + float2(_GrabTex_TexelSize.x * 3, 0)) / i.grabPos.w);
                float4 col2 = tex2D(_GrabTex, (i.grabPos + float2(0, -_GrabTex_TexelSize.y * 3)) / i.grabPos.w);
                float4 col3 = tex2D(_GrabTex, (i.grabPos + float2(0, _GrabTex_TexelSize.y * 3)) / i.grabPos.w);
                float4 col = (col0 + col1 + col2 + col3) * 0.25;

    //             vec3 col0 = textureOffset(uTex0, vTexCoord, ivec2( -2,  0 ) ).xyz;
    // vec3 col1 = textureOffset(uTex0, vTexCoord, ivec2(  2,  0 ) ).xyz;
    // vec3 col2 = textureOffset(uTex0, vTexCoord, ivec2(  0, -2 ) ).xyz;
    // vec3 col3 = textureOffset(uTex0, vTexCoord, ivec2(  0,  2 ) ).xyz;
    //
    // vec3 col = (col0+col1+col2+col3) * 0.25;
                
                return col;
                // return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}