Shader "Custom/SkyDisc" {
	Properties {
		_MainTex    ("Texture",       2D        ) = "white" {}
		_BlendNormal("Blend Normals", Range(0,1)) = 0.5
		_Ambient("Ambient", Range(0,1)) = 0.0
		_Impact("Impact", Range(0,10000)) = 0.0
		_Speed("Speed", Range(0,1)) = 0.0
		_Detail("Detail", Range(0,1)) = 0.0
		_Center("Center", Vector) = (0,0,0)
		_GlobalIntensity("GlobalIntensity", Range(0,10)) = 1
		_DepthGradientShallow("Depth Gradient Shallow", Color) = (0.325, 0.807, 0.971, 0.725)
		_Diffraction("Diffraction", Vector) = (0,0,0)
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
			float     _GlobalIntensity;
			float4 _DepthGradientShallow;
			float3 _Diffraction;
			
			v2f vert (appdata_base v) {
				v2f o;
				
				o.normal   = UnityObjectToWorldNormal(v.normal);
				o.uv       = TRANSFORM_TEX(v.texcoord, _MainTex);
				
				float3 diff = normalize(_WorldSpaceCameraPos-_Center);
				float3 dx = -normalize(cross(diff, float3(1,0,0)));
				float3 dy = -normalize(cross(diff, dx));

				
				o.worldPos = ((_Center + diff*_Detail) + (o.uv.x-0.5)*dx*_Impact + (o.uv.y-0.5)*dy*_Impact); //mul(unity_ObjectToWorld, v.vertex);

				o.pos   = UnityWorldToClipPos(o.worldPos);

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

				float2 cuv = float2(i.uv.x-0.5, i.uv.y-0.5);

				float global_intensity = (dot(normalize(i.worldPos-_Center), _WorldSpaceLightPos0.xyz) + 1)/2;

				global_intensity = 1*_Detail + global_intensity*(1-_Detail);
				global_intensity = pow(global_intensity, _GlobalIntensity);

				

				float sq_center_dist = sqrt(cuv.x*cuv.x + cuv.y*cuv.y);

				sq_center_dist = max(0,sq_center_dist-0.5)*(1/(1-0.5));

				clip(1-sq_center_dist);

			


				sq_center_dist = 1-max(0,sq_center_dist-0.0);
				sq_center_dist = max(0,sq_center_dist);

				


				color.a = pow(sq_center_dist,5)/1;

				//return color;

				
				

				float3 light_color = 1-(1-global_intensity)*_Diffraction;






				color.rgb *= light_color*global_intensity*(1+_Ambient);

				
				return color;
			}
			ENDCG
		}

		
	}
}