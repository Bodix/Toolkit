Shader "Custom/Unlit/Distance"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "" { }
        _VisibleDist ("Visible Distance", Float) = 10
        _InvisibleDist ("Invisible Distance", Float) = 10
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
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
                half distToCam : TEXCOORD1;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            half _VisibleDist;
            half _InvisibleDist;
            
            v2f vert(appdata v)
            {
                v2f o;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.distToCam = length(ObjSpaceViewDir(v.vertex));
                
                return o;
            }
            
            fixed4 frag(v2f i) : SV_TARGET
            {
                fixed4 c = tex2D(_MainTex, i.uv);

                c.a = clamp((i.distToCam - _InvisibleDist) / (_VisibleDist - _InvisibleDist), 0, 1);

                return c;
            }
            ENDCG
        }
    }
}