Shader "Custom/Builder" {
	Properties {
		_MainTex1    ("Noise 1",       2D        ) = "white" {}
		_MainTex2    ("Noise 2",       2D        ) = "white" {}
		_MainTex3    ("Noise 3",       2D        ) = "white" {}
		_MainTex4    ("Noise 4",       2D        ) = "white" {}
		
		
		_Radius("Radius", Range(0,1000)) = 100
		_Seed("Seed", Range(0,100000)) = 100

		_NoiseDetail1("Noise Detail 1", Range(0,100)) = 1
		_NoiseImpact1("Noise Impact 1", Range(0,10000)) = 10

		_NoiseDetail2("Noise Detail 2", Range(0,100)) = 1
		_NoiseImpact2("Noise Impact 2", Range(0,50)) = 10

		_NoiseDetail3("Noise Detail 3", Range(0,10)) = 1
		_NoiseImpact3("Noise Impact 3", Range(0,200)) = 10

		_NoiseDetail4("Noise Detail 4", Range(0,10)) = 1
		_NoiseImpact4("Noise Impact 4", Range(0,200)) = 10
		
		
		
		
		
		_BlendNormal("Blend Normals", Range(0,1)) = 0.5
		_Ambient("Ambient", Range(0,1)) = 0.0
		_Fog("Fog", Range(0,2)) = 0.0
		_Center("Center", Vector) = (0,0,0)
		_GlobalIntensity("GlobalIntensity", Range(0,10)) = 1

		_Diffraction("Diffraction", Vector) = (0,0,0)
		_DepthGradientShallow("Depth Gradient Shallow", Color) = (0.325, 0.807, 0.971, 0.725)

	}
	SubShader {
		Tags { "RenderType"="Opaque" "LightMode"="ForwardBase" }
		LOD 100

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"
            #include "Lighting.cginc"

			#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            // shadow helper functions and macros
            #include "AutoLight.cginc"

			/*struct appdata {
				float4 vertex : POSITION;
				float2 uv     : TEXCOORD0;
				float3 normal : NORMAL;
				float3 color  : COLOR0;
			};*/

			struct v2f {
				float4 pos   : SV_POSITION;
				float2 uv       : TEXCOORD0;
				SHADOW_COORDS(1)
				float3 normal   : NORMAL;
				float3 worldPos : COLOR0;
			};

			sampler2D _MainTex1;
			float4    _MainTex1_ST;
			sampler2D _MainTex2;
			float4    _MainTex2_ST;
			sampler2D _MainTex3;
			float4    _MainTex3_ST;
			sampler2D _MainTex4;
			float4    _MainTex4_ST;
			float     _BlendNormal;
			float     _Ambient;
			float     _Fog;
			float     _Radius;
			float     _GlobalIntensity;
			float     _NoiseDetail1;
			float     _NoiseImpact1;
			float     _NoiseDetail2;
			float     _NoiseImpact2;
			float     _NoiseDetail3;
			float     _NoiseDetail4;
			float     _NoiseImpact3;
			float     _NoiseImpact4;
			float3    _Center;
			float3	  _Diffraction;
			float4 _DepthGradientShallow;
			float _Seed;


			float fromHp(float3 c) {

				return (c.r+c.g+c.b)/(3);
				//return (c.r * 255*255 + c.g*255 + c.b)/(255*255);
			}

			float sampleTexture3D(float3 position, sampler2D tex, float detail) {

				//normal = ((normal + new Vector3(1.25f,3.50f,5.75f))/detail) * Radius*0.01f;
				
				position = ((normalize(position) + float3(1.25+_Seed,3.50+_Seed,5.75+_Seed))/detail) * _Radius*0.01;

				float c1 = fromHp(tex2Dlod(tex, float4(position.x, position.y, 0, 0)));
				float c2 = fromHp(tex2Dlod(tex, float4(position.x, position.z, 0, 0)));
				float c3 = fromHp(tex2Dlod(tex, float4(position.z, position.y, 0, 0)));



				//return c1;

				return (c1+c2+c3)/3;
			}

			
			v2f vert (appdata_base v) {
				v2f o;

				float offset1 = sampleTexture3D(v.vertex, _MainTex1, _NoiseDetail1);
				float offset2 = sampleTexture3D(v.vertex, _MainTex2, _NoiseDetail2);
				float offset3 = sampleTexture3D(v.vertex, _MainTex3, _NoiseDetail3);
				float offset4 = sampleTexture3D(v.vertex, _MainTex4, _NoiseDetail4);




				//o.pos   = UnityObjectToClipPos(v.vertex + normalize(v.vertex)*offset*_NoiseImpact1);
				o.normal   = UnityObjectToWorldNormal(v.normal);
				//o.uv       = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldPos = unity_ObjectToWorld._m03_m13_m23 + v.vertex * (_Radius + (offset1*_NoiseImpact1 * pow(offset2,_NoiseImpact2))+offset3*_NoiseImpact3+offset4*_NoiseImpact4);
				o.pos = UnityWorldToClipPos(o.worldPos);
				TRANSFER_SHADOW(o)
				return o;
			}

			float3 GetFaceNormal(float3 position) {
				float3 dx = ddx(position);
				float3 dy = ddy(position);
				return normalize(cross(dy, dx));
			}

			float GetLighting(float3 normal) {
				//float intensity = max(-1,dot(normal, _WorldSpaceLightPos0.xyz));		
				//intensity = (intensity + 1)/2;

				float intensity = max(0,dot(normal, _WorldSpaceLightPos0.xyz));		


				return intensity;
			}

			
			
			fixed4 frag (v2f i) : SV_Target {
				fixed4 color = fixed4(1,1,1,1);

				//color = fixed4(color.r-color.r%0.2, color.g-color.g%0.2, color.b-color.b%0.2, 1);


				float3 diff = normalize(i.worldPos-_Center);
				float fog_intensity = 1-saturate(dot(diff, normalize(_WorldSpaceCameraPos.xyz-_Center)));

				//return fog_intensity;

				float global_intensity = (dot(diff, _WorldSpaceLightPos0.xyz) + 1)/2;

				global_intensity = pow(global_intensity, _GlobalIntensity);


				
				float3 light_color = 1-(1-global_intensity)*_Diffraction;

				float3 fogcolor = _DepthGradientShallow * light_color * global_intensity;



				float3 faceNormal  = GetFaceNormal(i.worldPos);
				float3 finalNormal = lerp(i.normal, faceNormal, _BlendNormal);

				fixed shadow = 1;



				float3 fu = ((faceNormal));
				fu = fu*100;
				float f = ((sin(fu.x) + sin(fu.y) + sin(fu.z))+3)/6;
				//color.rgb = f;

				//return color;


				color.rgb *= light_color;

				color.rgb *= ((GetLighting(finalNormal)*shadow)*(1-_Ambient) + _Ambient)*1;
				color.rgb *= global_intensity + f*0.0;

				float4 fog = (1,1,1,1);
				fog.rgb = fogcolor.rgb;

				fog_intensity *= _Fog;
				
				return color*(1-fog_intensity) + fog*fog_intensity;
			}
			ENDCG
		}

		
	}
}