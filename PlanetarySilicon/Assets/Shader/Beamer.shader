Shader "Custom/Beamer" {
	Properties {
		_MainTex    ("Texture",       2D        ) = "white" {}
		
		_Speed("Speed", Range(0,100)) = 0.0
		
		_StripeWidth("StripeWidth", Range(10,1000)) = 0.0
		_StripeSkip("StripeSkip", Range(0,100)) = 0.0
		
		
		_Color("Color", Color) = (0.325, 0.807, 0.971, 0.725)
		

	}
	SubShader {
		Blend SrcAlpha OneMinusSrcAlpha
		Zwrite Off
		Cull Off
		

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
			float4 _Color;
			float _Speed;
			float _StripeWidth;
			int _StripeSkip;

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
				return o;
			}


			
			
			fixed4 frag (v2f i) : SV_Target {

				float y = i.uv.y + _Time.x*_Speed;

				y = floor(y*_StripeWidth);

				//float t = (sin(_Time.x*_Speed)+1)/2.0;
				
				float stripe = 1-min(1, y%_StripeSkip);

				clip(i.uv.y-0.5);


				return saturate(lerp(_Color*((1-(i.uv.y-0.5)*2)), _Color, stripe));
			}
			ENDCG
		}

		
	}
}