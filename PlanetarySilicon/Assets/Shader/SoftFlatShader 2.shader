Shader "Custom/Soft Flat Shaded bu" {
	Properties {
		_MainTex    ("Texture",       2D        ) = "white" {}
		_BlendNormal("Blend Normals", Range(0,1)) = 0.5
		_Ambient("Ambient", Range(0,1)) = 0.0
		_Center("Center", Vector) = (0,0,0)
		_Diffraction("Diffraction", Vector) = (0,0,0)
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

			sampler2D _MainTex;
			float4    _MainTex_ST;
			float     _BlendNormal;
			float     _Ambient;
			float3    _Center;
			float3	  _Diffraction;
			
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
				fixed4 color = tex2D(_MainTex, i.uv);


				float3 diff = normalize(i.worldPos-_Center);

				float global_intensity = (dot(diff, _WorldSpaceLightPos0.xyz) + 1)/2;

				
				float3 light_color = 1-(1-global_intensity)*_Diffraction;





				float3 faceNormal  = GetFaceNormal(i.worldPos);
				float3 finalNormal = lerp(i.normal, faceNormal, _BlendNormal);

				fixed shadow = SHADOW_ATTENUATION(i);



				float3 fu = ((faceNormal));
				fu = fu*100;
				float f = ((sin(fu.x) + sin(fu.y) + sin(fu.z))+3)/6;
				//color.rgb = f;

				//return color;


				color.rgb *= light_color;

				color.rgb *= ((GetLighting(finalNormal)*shadow)*(1-_Ambient) + _Ambient)*1;
				color.rgb *= global_intensity + f*0.1;
				
				return color;
			}
			ENDCG
		}

		UsePass "VertexLit/SHADOWCASTER"
	}
}