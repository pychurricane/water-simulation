Shader "Ocean/OceanShader2"
{
	Properties
	{
		_Tint("Tint", Color) = (1, 1, 1, 1)
		_MainTex ("Albedo", 2D) = "white" {}
		_steepness("Steepness", Range(0.0, 1.0)) = 0.5
		
		[Gamma]_Metallic("Metallic", Range(0, 1)) = 0
		_Smoothness("smoothness", range(0, 1)) = 0.1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{	
			Tags {
				"LightMode" = "ForwardBase"
			}
			// Cull off
			CGPROGRAM
			#pragma target 3.0

			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			// #include "UnityCG.cginc"
			// #include "UnityStandardBRDF.cginc"
			// #include "UnityStandardUtils.cginc"
			#include "UnityPBSLighting.cginc"
			#define PI 3.14159265
			#define G 9.81
			 struct Wave 
			{
				float amplitude;
				float2 direction;
				float speed;
				float waveLength;
			};

		
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;


				float3 normal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			StructuredBuffer<Wave> _waveBuffer;
			int _numWaves;
			float _steepness;

			float4 _Tint;
			float _Metallic;
			float _Smoothness;
			// compute the gerstner offset for one wave
			float3 getGerstnerOffset(Wave wave, float2 posXZ, float time, float steepness) {
				float freq = 2 * PI / wave.waveLength;
				float amp = wave.amplitude;
				// float len = wave.waveLength;
				float stee = steepness / (freq * amp * _numWaves);
				float2 dir = wave.direction;
				float phase = wave.speed * freq;

				float2 xzOffset = stee * amp * dir * cos(dot(dir, posXZ) * freq + phase * time );
				float yOffset = amp * sin(freq * dot(dir, posXZ) + phase * time);

				return float3(xzOffset.x, yOffset, xzOffset.y);
			}

			// Helper funtion to compute the binormal of the offset wave point
			// come from derivation of the Gerstner suface in x-direction
			// for one wave
			float3 computeBinormal(float stee, float2 dir, float WA, float S, float C) {
				float3 B = float3(0, 0, 0);

				B.x = stee * pow(dir.x, 2) * WA * S;
				B.y = dir.x * WA * C;
				B.z = stee * dir.x * dir.y * WA * S;

				return B;
			}
			// compute the tangnet vector of the offset wave point
			// This comes form the derivation fo the Gerstner suface in z-direction
			float3 computeTangent(float stee, float2 dir, float WA, float S, float C) {
				float3 T = float3(0, 0, 0);

				T.x = stee * dir.x * dir.y * WA * S;
				T.y = dir.y * WA * C;
				T.z = stee * pow(dir.y, 2) * WA * S;

				return T;
			}
			// for every single vertex
			v2f vert (appdata v)
			{
				v2f o;

				float3 posOffset = float3(0.0, 0.0, 0.0);
				float3 binormal = float3(0, 0, 0);
				float3 tangent = float3(0, 0, 0);

				// float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				float3 objectPos = v.vertex.xyz;
				for(int i = 0; i < _numWaves; i ++) {
					 Wave wave = _waveBuffer[i];
					// properties
					float freq = 2 * PI / wave.waveLength;
					float amp = wave.amplitude;
					float stee = _steepness / (freq * amp * _numWaves);
					float2 dir = wave.direction;
					float phase = wave.speed * freq;

					float WA = freq * amp;
					float S = sin(freq * dot(dir, objectPos.xz) + phase * _Time.w);
					float C = cos(freq * dot(dir, objectPos.xz) + phase * _Time.w);

					 posOffset += getGerstnerOffset(wave, objectPos.xz, _Time.w, _steepness);
					 binormal += computeBinormal(stee, dir, WA, S, C);
					 tangent += computeTangent(stee, dir, WA, S, C); 

				}
				// fix binormal and tangent
				binormal.x = 1 - binormal.x;
				binormal.z = 0 - binormal.z;

				tangent.x = 0 - tangent.x;
				tangent.z = 1 - tangent.z;
				// displace vertex
				v.vertex.x += posOffset.x;
				v.vertex.z += posOffset.z;
				v.vertex.y += posOffset.y;


				// compute new normal------------is the normal correct in wordspace? because i use wordpos to caculate those parameters
				v.normal =  cross(binormal, tangent);
				// save new tangent
				v.tangent = float4(tangent.xyz, 0.0);

				// v.normal = mul(unity_ObjectToWorld, float4(v.normal, 0));
				// o.normal = normalize(v.normal);

				o.normal = -v.normal;

				// correct from object to world space with normalize
				//o.normal = UnityObjectToWorldNormal(v.normal);

				// Transforms a point from object space to the camera’s clip space in homogeneous coordinates
				//  This is the equivalent of mul(UNITY_MATRIX_MVP, float4(pos, 1.0)), and should be used in its place.
				o.vertex = UnityObjectToClipPos(v.vertex);			// screen pos
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);     // for the world position of vertices
				
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				i.normal = normalize(i.normal);
				float3 lightDir = _WorldSpaceLightPos0.xyz;
				float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);

				float3 lightColor = _LightColor0.rgb;
				float3 albedo = tex2D(_MainTex, i.uv).rgb * _Tint.rgb;

				float3 specularTint;
				float oneMinusReflectivity;
				albedo = DiffuseAndSpecularFromMetallic(
					albedo, _Metallic, specularTint, oneMinusReflectivity
				);

				UnityLight light;
				light.color = lightColor;
				light.dir = lightDir;
				light.ndotl = DotClamped(i.normal, lightDir);

				UnityIndirect indirectLight;
				indirectLight.diffuse = 0;
				indirectLight.specular = 0;


				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				

				return UNITY_BRDF_PBS(
					albedo, specularTint,
					oneMinusReflectivity, _Smoothness,
					i.normal, viewDir,
					light, indirectLight
				);
			}
			ENDCG
		}
	}
}
