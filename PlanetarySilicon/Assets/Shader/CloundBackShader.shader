Shader "Custom/CloudBack" {
	Properties {
		_MainTex    ("Texture",       2D        ) = "white" {}
		_BlendNormal("Blend Normals", Range(0,1)) = 0.5
		_Ambient("Ambient", Range(0,1)) = 0.0
		_Impact("Impact", Range(0,500)) = 0.0
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
			
			Cull Back
			Zwrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			

            #include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"
            #include "Lighting.cginc"

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
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.pos   = UnityObjectToClipPos(v.vertex);

				return o;
			}

			
			fixed4 frag (v2f i) : SV_Target {

				float xy = tex2D(_MainTex, float2(i.worldPos.x, i.worldPos.y)/_Impact+_Time.y*_Speed);
				float yz = tex2D(_MainTex, float2(i.worldPos.y, i.worldPos.z)/_Impact+_Time.y*_Speed);
				float xz = tex2D(_MainTex, float2(i.worldPos.x, i.worldPos.z)/_Impact+_Time.y*_Speed);



				fixed4 color = (xz+yz+xy)/3;//tex2D(_MainTex, i.uv);

				

				color.a = color.r;
				color.a = color.a - color.a % _Detail;
				color.rgb = 1;
				

				

				clip(color.a-0.6);
				
				color.a *= pow(color.a,2) * 1;
				color.a = min(1, color.a);

				//color.a = 0.5;
				
				
			
				

				

				float3 diff = i.normal;
				

				float global_intensity = (dot(diff, _WorldSpaceLightPos0.xyz) + 1)/2;
				global_intensity = pow(global_intensity, _GlobalIntensity);

			

				float3 light_color = 1-(1-global_intensity)*_Diffraction;

				color.rgb *= light_color*global_intensity;

				
				
				return color;
			}
			ENDCG
		}

		
	}
}