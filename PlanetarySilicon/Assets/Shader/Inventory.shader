Shader "Custom/Inventory"
{
	Properties {
		
		_Color("Color", Color) = (0,0,0,0)
		_BlendNormal("Blend Normals", Range(0,1)) = 0.5

		
		
     
		

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
				float3 normal : NORMAL;
                float3 worldPos : TEXCOORD1;
				
			};

		
			float4 _Color;
			float _BlendNormal;
           
		   	float3 GetFaceNormal(float3 position) {
				float3 dx = ddx(position);
				float3 dy = ddy(position);
				return normalize(cross(dy, dx));
			}
		

			
			v2f vert (appdata_base v) {
				v2f o;
				o.pos   = UnityObjectToClipPos(v.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;
				return o;
			}


			
			
			fixed4 frag (v2f i) : SV_Target {

				return 0;

				float3 faceNormal  = GetFaceNormal(i.worldPos);
				float3 finalNormal = lerp(i.normal, faceNormal, _BlendNormal);

				float3 ambientLightDirection = normalize(_WorldSpaceCameraPos - i.worldPos);
                float ambientLightIntensity = max(0.0, dot(finalNormal, ambientLightDirection));


				float4 color = _Color;
				color = color * ambientLightIntensity;
				color.a = 1;

				return color;
			}
			ENDCG
		}

		
	}
}