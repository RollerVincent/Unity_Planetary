// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/CaveVis" {
	Properties {
		_MainTex    ("Texture",       2D        ) = "white" {}
		_Cutoff("Cutoff", Range(0,1)) = 0.5
		_Detail("Detail", Range(0,400)) = 0.0
		_MeanHeight("MeanHeight", Range(0,1000)) = 0.0
		_HeightScale("HeightScale", Range(0,1000)) = 0.0
		_Color("Color", Color) = (0.325, 0.807, 0.971, 0.725)

		
	}
	SubShader {
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass {
			
			Cull Back
			Zwrite Off
			ZTest Always
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			

            #include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"
            #include "Lighting.cginc"

			struct v2f {
				float4 pos   : SV_POSITION;
				float2 uv       : TEXCOORD0;
				float4 screenPosition : TEXCOORD1;
				float3 normal   : NORMAL;
				float3 worldPos : COLOR0;
				float3 objectPos : COLOR1;
			};

			sampler2D _MainTex;
			sampler2D _CameraDepthTexture;
			float4    _MainTex_ST;
			float _MeanHeight;
			float _HeightScale;
			float     _Cutoff;
			float     _Detail;
			float4 _Color;
			
			v2f vert (appdata_base v) {
				v2f o;
				
				o.normal   = UnityObjectToWorldNormal(v.normal);
				o.uv       = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.objectPos = v.vertex*length(unity_ObjectToWorld._m00_m10_m20);
				o.pos   = UnityObjectToClipPos(v.vertex);
				o.screenPosition = ComputeScreenPos(o.pos);

				return o;
				
			}

			
			fixed4 frag (v2f i) : SV_Target {


				

				float existingDepth01 = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosition)).r;
				float existingDepthLinear = LinearEyeDepth(existingDepth01);
				float depthDifference = i.screenPosition.w - existingDepthLinear;
				float waterDepthDifference01 = saturate(depthDifference/100);

				waterDepthDifference01 = 1-waterDepthDifference01;
				
				
				float height = length(i.objectPos);
				float heightDistance = (height*height) - (_MeanHeight*_MeanHeight);
				float heightFactor = 1 + max(heightDistance, -heightDistance)/_HeightScale;

				

				float xy = tex2D(_MainTex, float2(i.objectPos.x, i.objectPos.y)/_Detail);
				float yz = tex2D(_MainTex, float2(i.objectPos.y, i.objectPos.z)/_Detail);
				float xz = tex2D(_MainTex, float2(i.objectPos.x, i.objectPos.z)/_Detail);

				float noise = (xz+yz+xy)/3;


				//float noise = tex2D(_MainTex, float2(i.objectPos.x + i.objectPos.y, i.objectPos.z)/_Detail);

				noise = max(noise-_Cutoff, 0)/(1-_Cutoff);
				noise = noise / heightFactor;

				//clip(noise-_Speed);

				float3 diff = normalize(i.worldPos - mul(unity_ObjectToWorld, float4(0,0,0,1)));

				float3 cam_diff = normalize(_WorldSpaceCameraPos-i.worldPos);

				float rim = saturate(dot(diff, cam_diff));
				//rim = max(rim, -rim);

				rim = saturate(pow(rim,1)*1);

				float4 color = _Color;
				color.rgb *= (noise+(0.1-noise%0.1))*1;
				color.a = rim*0.8;


				return color;//*waterDepthDifference01;


			}
			ENDCG
		}

		
	}
}