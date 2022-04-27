Shader "Custom/Unlit"
{
    Properties {
		
		_Color("Color", Color) = (0,0,0,0)

		
		
     
		

	}
	SubShader {
		Blend SrcAlpha OneMinusSrcAlpha
		ZTest Always

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
				
			};

		
			float4 _Color;
           
		

			
			v2f vert (appdata_base v) {
				v2f o;
				o.pos   = UnityObjectToClipPos(v.vertex);
				return o;
			}


			
			
			fixed4 frag (v2f i) : SV_Target {
				return _Color;
			}
			ENDCG
		}

		
	}
}
