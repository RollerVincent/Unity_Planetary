Shader "Custom/Soft Flat Shaded" {
	Properties {
		_MainTex    ("Texture",       2D        ) = "white" {}
		_BlendNormal("Blend Normals", Range(0,1)) = 0.5
		_Ambient("Ambient", Range(0,1)) = 0.0
		_Fog("Fog", Range(0,2)) = 0.0
		_Center("Center", Vector) = (0,0,0)
		_GlobalIntensity("GlobalIntensity", Range(0,10)) = 1

		_Diffraction("Diffraction", Vector) = (0,0,0)
		_DepthGradientShallow("Depth Gradient Shallow", Color) = (0.325, 0.807, 0.971, 0.725)

	}
	SubShader {
		
		LOD 100

		Pass {
			Tags { "RenderType"="Opaque" "LightMode"="ForwardBase" }
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
				float4 color : COLOR0;
				float3 worldPos : COLOR1;
			};

			sampler2D _MainTex;
			float4    _MainTex_ST;
			float     _BlendNormal;
			float     _Ambient;
			float     _Fog;
			float     _GlobalIntensity;
			float3    _Center;
			float3	  _Diffraction;
			float4 _DepthGradientShallow;

			
			v2f vert (appdata_full v) {
				v2f o;
				o.pos   = UnityObjectToClipPos(v.vertex);
				o.normal   = UnityObjectToWorldNormal(v.normal);
				o.uv       = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.color = v.color;
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

				fixed shadow = SHADOW_ATTENUATION(i);

				return shadow*_LightColor0;

				float3 fu = ((faceNormal));
				fu = fu*100;
				float f = ((sin(fu.x) + sin(fu.y) + sin(fu.z))+3)/6;


				fixed4 texcolor = tex2D(_MainTex, i.uv);

				//fixed4 color = i.color*i.color.a + texcolor*(1-i.color.a);
				i.color.a = pow(i.color.a,10);
				fixed4 color = lerp(i.color,texcolor,1-i.color.a);


				

				//color.rgb = f;

				//return color;


				color.rgb *= light_color;

				color.rgb *= ((GetLighting(finalNormal)*shadow)*(1-_Ambient) + _Ambient)*1;
				color.rgb *= global_intensity + f*0.2*global_intensity;

				float4 fog = (1,1,1,1);
				fog.rgb = fogcolor.rgb;

				fog_intensity *= _Fog;
				
				return color*(1-fog_intensity) + fog*fog_intensity;
			}
			ENDCG
		}

		Pass {
			Tags { "RenderType"="Opaque" "LightMode"="ForwardAdd" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"
            #include "Lighting.cginc"

			#pragma multi_compile_fwdadd// nolightmap nodirlightmap nodynlightmap novertexlight
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
				float4 color : COLOR0;
				float3 worldPos : COLOR1;
			};

			sampler2D _MainTex;
			float4    _MainTex_ST;
			float     _BlendNormal;
			float     _Ambient;
			float     _Fog;
			float     _GlobalIntensity;
			float3    _Center;
			float3	  _Diffraction;
			float4 _DepthGradientShallow;

			
			v2f vert (appdata_full v) {
				v2f o;
				o.pos   = UnityObjectToClipPos(v.vertex);
				o.normal   = UnityObjectToWorldNormal(v.normal);
				o.uv       = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.color = v.color;
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

				fixed shadow = SHADOW_ATTENUATION(i);

				return shadow*_LightColor0;

				float3 fu = ((faceNormal));
				fu = fu*100;
				float f = ((sin(fu.x) + sin(fu.y) + sin(fu.z))+3)/6;


				fixed4 texcolor = tex2D(_MainTex, i.uv);

				//fixed4 color = i.color*i.color.a + texcolor*(1-i.color.a);
				i.color.a = pow(i.color.a,10);
				fixed4 color = lerp(i.color,texcolor,1-i.color.a);


				

				//color.rgb = f;

				//return color;


				color.rgb *= light_color;

				color.rgb *= ((GetLighting(finalNormal)*shadow)*(1-_Ambient) + _Ambient)*1;
				color.rgb *= global_intensity + f*0.2*global_intensity;

				float4 fog = (1,1,1,1);
				fog.rgb = fogcolor.rgb;

				fog_intensity *= _Fog;
				
				return color*(1-fog_intensity) + fog*fog_intensity;
			}
			ENDCG
		}


		UsePass "VertexLit/SHADOWCASTER"
	}
}