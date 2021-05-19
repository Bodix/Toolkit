Shader "Custom/Unlit/Radial Gradient"
{
    Properties
    {
        _StartColor ("Start Color", Color) = (1, 1, 1, 1)
        _EndColor ("End Color", Color) = (1, 1, 1, 1)
        _Radius ("Radius", Float) = 0.5
        _CenterPosition ("Center Position", Vector) = (0.5, 0.5, 0, 0)
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _StartColor;
            float4 _EndColor;
            float _Radius;
            float4 _CenterPosition;

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv - _CenterPosition;

                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET
            {
                return lerp(_StartColor, _EndColor, length(i.uv) / _Radius);
            }
            ENDCG
        }
    }
}