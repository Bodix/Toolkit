// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Noise-Simplex-Frag" {
Properties {
	_Freq ("Frequency", Float) = 1
	_Speed ("Speed", Float) = 1
}

SubShader {
	Pass {
		CGPROGRAM
		#pragma target 2.0
		
		#pragma vertex vert
		#pragma fragment frag
		
		#include "NoiseSimplex.cginc"
		
		struct v2f {
			float4 pos : SV_POSITION;
			float3 srcPos : TEXCOORD0;
            float noise : TEXCOORD1;
		};
		
		uniform float
			_Freq,
			_Speed
		;
		
		v2f vert(float4 objPos : POSITION)
		{
			v2f o;

			o.pos =	UnityObjectToClipPos(objPos);
			
			o.srcPos = mul(unity_ObjectToWorld, objPos).xyz;
			o.srcPos *= _Freq;
			o.srcPos.y += _Time.y * _Speed;
            o.noise = snoise(o.srcPos) / 2 + 0.5f;
			
			return o;
		}
		
		float4 frag(v2f i) : COLOR
		{
			// float ns = snoise(i.srcPos) / 2 + 0.5f;

            float ns = i.noise;

			return float4(ns, ns, ns, 1.0f);
		}
		
		ENDCG
	}
}

}