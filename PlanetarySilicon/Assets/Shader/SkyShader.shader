Shader "Custom/Sky" {
	Properties {
		_MainTex    ("Texture",       2D        ) = "white" {}
		_BlendNormal("Blend Normals", Range(0,1)) = 0.5
		_Ambient("Ambient", Range(0,1)) = 0.0
		_Impact("Impact", Range(0,1)) = 0.0
		_Speed("Speed", Range(0,1)) = 0.0
		_Detail("Detail", Range(0,1)) = 0.0
		_GlobalIntensity("GlobalIntensity", Range(0,10)) = 1
		_Center("Center", Vector) = (0,0,0)
		_DepthGradientShallow("Depth Gradient Shallow", Color) = (0.325, 0.807, 0.971, 0.725)
		_Diffraction("Diffraction", Vector) = (0,0,0)
	}
	SubShader {
		Blend SrcAlpha One
		Cull Front

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
				float3 normal   : NORMAL;
				float3 worldPos : COLOR0;
			};

			sampler2D _MainTex;
			float4    _MainTex_ST;
			float     _BlendNormal;
			float     _Ambient;
			float     _Impact;
			float     _Detail;
			float     _Speed;
			float3    _Center;
			float _GlobalIntensity;
			float4 _DepthGradientShallow;
			float3 _Diffraction;
			
			v2f vert (appdata_base v) {
				v2f o;
				
				o.normal   = UnityObjectToWorldNormal(v.normal);
				o.uv       = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.pos   = UnityObjectToClipPos(v.vertex);

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
				fixed4 color = _DepthGradientShallow;

				float3 diff = i.normal;
				float3 cam_diff = normalize(_WorldSpaceCameraPos-i.worldPos);
				float3 cam_2_center = normalize(_WorldSpaceCameraPos-_Center);

				float global_intensity = (dot(diff, _WorldSpaceLightPos0.xyz) + 1)/2;

				global_intensity = pow(global_intensity, _GlobalIntensity);

				float rim = dot(diff, cam_diff);
				rim = max(rim, -rim);
				
				float rim1 = pow(min(1,rim+0.7),10);
				float rim2 = pow(min(1,rim+0.6),50);



				
				//global_intensity = pow(min(1,global_intensity+0.2),3);

				float3 light_color = 1-(1-global_intensity)*_Diffraction;






				color.rgb *= light_color;

			
				color.a *= global_intensity;

				//color.rgb /= (rim1*rim1);

				//color.a *= rim2*rim2;

				//color.rgb *= 4 * (1-(global_intensity/1.5)); // !!!
				
				return color*rim1;
			}
			ENDCG
		}

		
	}
}