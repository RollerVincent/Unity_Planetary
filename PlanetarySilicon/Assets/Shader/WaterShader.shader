Shader "Custom/Water" {
	Properties {
		_MainTex    ("Texture",       2D        ) = "white" {}
		_BlendNormal("Blend Normals", Range(0,1)) = 0.5
		_Ambient("Ambient", Range(0,1)) = 0.0
		_Impact("Impact", Range(0,10)) = 0.0
		_Speed("Speed", Range(0,10)) = 0.0
		_Detail("Detail", Range(0,1)) = 0.0
		_Center("Center", Vector) = (0,0,0)
		_GlobalIntensity("GlobalIntensity", Range(0,10)) = 1
		_DepthGradientShallow("Depth Gradient Shallow", Color) = (0.325, 0.807, 0.971, 0.725)
		_WaterColor("WaterColor", Color) = (0.325, 0.807, 0.971, 0.725)
		_Diffraction("Diffraction", Vector) = (0,0,0)
		_DepthMaxDistance("DepthMaxDistance", Range(0,100)) = 0.0
	}
	SubShader {
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off

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
				float4 screenPosition : TEXCOORD2;
				float3 normal   : NORMAL;
				float3 worldPos : COLOR0;
			};

			sampler2D _MainTex;
			sampler2D _CameraDepthTexture;
			float4    _MainTex_ST;
			float     _BlendNormal;
			float     _Ambient;
			float     _Impact;
			float     _Detail;
			float     _Speed;
			float _GlobalIntensity;
			float3    _Center;
			float4 _DepthGradientShallow;
			float4 _WaterColor;
			float3 _Diffraction;
			float _DepthMaxDistance;


			float sampleTexture3D(float3 position, sampler2D tex, float detail) {

				//normal = ((normal + new Vector3(1.25f,3.50f,5.75f))/detail) * Radius*0.01f;
				
				position = ((normalize(position) + float3(1.25,3.50,5.75))/detail);

				float c1 = (tex2Dlod(tex, float4(position.x+_Time.y*_Speed, position.y+_Time.y*_Speed, 0, 0))).r;
				float c2 = (tex2Dlod(tex, float4(position.x+_Time.y*_Speed, position.z+_Time.y*_Speed, 0, 0))).r;
				float c3 = (tex2Dlod(tex, float4(position.z+_Time.y*_Speed, position.y+_Time.y*_Speed, 0, 0))).r;



				//return c1;

				return (c1+c2+c3)/3;
			}
			
			v2f vert (appdata_base v) {
				v2f o;


				float offset1 = sampleTexture3D(v.vertex, _MainTex, _Detail);
				
				o.normal   = UnityObjectToWorldNormal(v.normal);
				o.uv       = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				float3 diff = normalize(o.worldPos-_Center);
				//o.pos   = UnityObjectToClipPos(v.vertex+diff*_Impact*sin(o.worldPos.z*_Detail+_Time.y*_Speed));
				o.worldPos   = o.worldPos+diff*_Impact*offset1;
				o.pos = UnityWorldToClipPos(o.worldPos);
				o.screenPosition = ComputeScreenPos(o.pos);
				//o.normal = normalize

				

				TRANSFER_SHADOW(o)
				return o;
			}

			float3 GetFaceNormal(float3 position) {
				float3 dx = ddx(position);
				float3 dy = ddy(position);
				return normalize(cross(dy, dx));
			}

			float GetLighting(float3 normal) {
				float intensity = max(-1,dot(normal, _WorldSpaceLightPos0.xyz));		
				intensity = (intensity + 1)/2;
				return intensity;
			}
			
			fixed4 frag (v2f i) : SV_Target {
				//fixed4 color = tex2D(_MainTex, i.uv);
				fixed4 color = 1;

				
				float existingDepth01 = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosition)).r;
				float existingDepthLinear = LinearEyeDepth(existingDepth01);
				float depthDifference = existingDepthLinear - i.screenPosition.w;
				float waterDepthDifference01 = saturate(depthDifference / _DepthMaxDistance);

				waterDepthDifference01 = waterDepthDifference01;

				return waterDepthDifference01;



				float3 diff = normalize(i.worldPos-_Center);
				float fog_intensity = 1-saturate(dot(diff, normalize(_WorldSpaceCameraPos.xyz-_Center)));

				float global_intensity = (dot(diff, _WorldSpaceLightPos0.xyz) + 1)/2;
				global_intensity = pow(global_intensity, _GlobalIntensity);


				float3 light_color = 1-(1-global_intensity)*_Diffraction;


				float3 fogcolor = _DepthGradientShallow * light_color * global_intensity;


				float3 faceNormal  = GetFaceNormal(i.worldPos);
				float3 finalNormal = lerp(i.normal, faceNormal, _BlendNormal);

				fixed shadow = SHADOW_ATTENUATION(i);


				color.rgb *= light_color * _DepthGradientShallow;


				float lightIntensity = ((GetLighting(finalNormal)*1)*(1-_Ambient) + _Ambient)*1;

				float3 fu = faceNormal;
				fu = fu*100;
				float f = ((sin(fu.x) + sin(fu.y) + sin(fu.z))+3)/6;

				


				color.rgb *= lightIntensity;
				color.rgb *= global_intensity + f*0.1*global_intensity;

				

				float4 fog = (1,1,1,1);
				fog.rgb = fogcolor.rgb;


				float specularReflection = pow(max(0.0, dot(reflect(-_WorldSpaceLightPos0.xyz, finalNormal), normalize(_WorldSpaceCameraPos.xyz-i.worldPos))), 10);
				specularReflection *= shadow;
				float specularReflection2 = pow(max(0.0, dot(reflect(-_WorldSpaceLightPos0.xyz, finalNormal), normalize(_WorldSpaceCameraPos.xyz-i.worldPos))), 40);
				specularReflection2 *= shadow;

				specularReflection = (specularReflection2)/global_intensity*lightIntensity;

				//return specularReflection;

				color.a = waterDepthDifference01;
				color = color * _WaterColor;

				if(waterDepthDifference01<0.01){
					color = 1;
				}
				

				color.rgb = (color + specularReflection*0.5)*(1-fog_intensity) + fog*fog_intensity;

				
				
				return saturate(color);
			}
			ENDCG
		}

		
	}
}