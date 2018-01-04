Shader "Ocean/OceanShader"
{
	Properties
	{
		_Color("MainColor", Color) = (1, 1, 1, 1)
		_MainTex ("Texture", 2D) = "white" {}
		_WaveLength("WaveLength", float) = 2
		_Amplitude("Amplitude", float) = 1
		_Speed("Speed", float) = 1
		 _Direction("Direction", Vector) = (1,1,1,1) 
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#define pi 3.14159265
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};
		


			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float _WaveLength;
			float _Amplitude;
			float _Speed;
			float4 _Direction;
			// static const float pi = 3.14;
			static const float freq = 2 * pi / _WaveLength;
			static const float phase = _Speed * freq;


			v2f vert (appdata v)
			{
				v2f o;
				
				// float freq = 2 * pi / _WaveLength;
				// float phase = _Speed * freq;
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				v.vertex.y += _Amplitude * sin(dot(_Direction.xz, worldPos.xz) * freq  + _Time.w * phase );

				// o.vertex.y += sin(worldPos.x + _Time.w);
				
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				// fixed4 col = _Color;
				return col;
			}
			ENDCG
		}
	}
}
