Shader "Custom/Sun" {
	Properties {
		_MainTex    ("Texture",       2D        ) = "white" {}
		_BlendNormal("Blend Normals", Range(0,10)) = 0.5
		_Ambient("Ambient", Range(0,1)) = 0.0
		_AddColor("AddColor", Range(0,1)) = 0.0
		_Fog("Fog", Range(0,1)) = 0.0
		_Offset("Offset", Range(0,1)) = 0.0
		_Speed("Speed", Range(0,100)) = 0.0
		_Darkening("Darkening", Range(0,1)) = 0.0
		_Center("Center", Vector) = (0,0,0)
		_DepthGradientShallow("Depth Gradient Shallow", Color) = (0.325, 0.807, 0.971, 0.725)
		_DepthGradientDeep("Depth Gradient Deep", Color) = (0.325, 0.807, 0.971, 0.725)

	}
	SubShader {
		Blend SrcAlpha OneMinusSrcAlpha
		

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

			sampler2D _MainTex;
			float4    _MainTex_ST;
			float     _BlendNormal;
			float     _Ambient;
			float     _Fog;
			float3    _Center;
			float4 _DepthGradientShallow;
			float4 _DepthGradientDeep;
			float4 _StarColor;
			float4 _AddStarColor;
			float _Offset;
			float _Darkening;
			float _SunBrightDistance;
			float _Speed;
			float _AddColor;

			float4 correctBrightness(float4 c, float d = 0){
				float m = max(max(c.r,c.g),c.b);
				m = 1-m-d/255.0;
				return float4(c.r+m, c.g+m, c.b+m, 1);
			}

			
			v2f vert (appdata_base v) {
				v2f o;
				o.pos   = UnityObjectToClipPos(v.vertex);
				o.normal   = UnityObjectToWorldNormal(v.normal);
				o.uv       = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
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
				fixed4 color = tex2D(_MainTex, float2(i.worldPos.x/3+10000+_Time.y*20, i.worldPos.y/3-10000)*_BlendNormal*0.001);
				color += tex2D(_MainTex, float2(i.worldPos.x/2-10000, i.worldPos.z/2+10000-_Time.y*_Speed)*_BlendNormal*0.001);
				color += tex2D(_MainTex, float2(i.worldPos.z/2-10000+_Time.y*_Speed, i.worldPos.y+10000)*_BlendNormal*0.001);
				color = color/3;


				float intensity = color.r;

				

				
				float3 cam_diff = normalize(_WorldSpaceCameraPos-i.worldPos);
				float rim = saturate(dot(cam_diff, i.normal));


				rim = pow(rim, _Fog);


				intensity = saturate((intensity - intensity%_Ambient + _Ambient*2)  +  (1-rim));

				//return intensity;

				float darkening = 1-saturate(length(i.worldPos-_WorldSpaceCameraPos)/_SunBrightDistance);
				darkening = saturate(darkening-0.4);


				

				//darkening = saturate(darkening-0.5);

				


				intensity = max(1-darkening, intensity);

				//return intensity;

				float4 starcolor = correctBrightness(lerp(_StarColor, _AddStarColor, _AddColor));
				//float4 sun_color = lerp(1, _DepthGradientShallow, 1-intensity);
				float4 sun_color = lerp(1, starcolor, 1-intensity);

				sun_color = pow(sun_color,200*(1-intensity));

				


				
				


				

				
				sun_color.a=1;


				

				return sun_color;
			}
			ENDCG
		}

		
	}
}