Shader "Custom/PlanetaryHeatMesh"
{
    Properties {
        _MainTex    ("Texture",       2D        ) = "white" {}
		_Height("Height", Range(0,1)) = 0.5
		_Width("Width", Range(0,1)) = 0.5
		_Attenuation("Attenuation", Range(0,1)) = 0.5
		_Offset("Offset", Range(0,1)) = 0.5
		_MinVelocity("MinVelocity", Range(0,100)) = 0.5

		_Ambient("Ambient", Range(0,1)) = 1
		_AmbientSaturation("Ambient Saturation", Range(0,1)) = 1
		_FaceNoise("Face Noise", Range(0,1)) = 0.1
        _Center("Center", Vector) = (0,0,0)
		
        _Diffraction("Diffraction", Vector) = (0,0,0,0)
        _Color("Color", Color) = (0,0,0,0)
        _BottomColor("Bottom Color", Color) = (0,0,0,0)

		_WindFrequency("WindFrequency", Range(0,100)) = 1
		_WindInfluence("WindInfluence", Range(0,1)) = 1
		_WindGust("WindGust", Range(0,1)) = 1

		_AbsorbanceExponent("AbsorbanceExponent", Range(0,20)) = 1
        _AbsorbanceOffset("AbsorbanceOffset", Range(0,100)) = 1
		

	}
	SubShader
	{
		

		

		// Forward rendering base (main directional light) pass.
		Pass
		{
			Name "FORWARD"

			ZWrite Off
			Cull Off
			Blend SrcAlpha One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// Want regular shader variants for ForwardBase pass, but don't care about
			// lightmaps, dynamic GI etc. Just shadows/no-shadows
			#pragma multi_compile_fwdbase nolightmap nodynlightmap novertexlight
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			

			struct v2f
			{
				float4 pos : SV_POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD3;
                float normalFactor : TEXCOORD1;
                float velocityAmount : TEXCOORD0;
                
                
			};

            float3 GetFaceNormal(float3 position) {
				float3 dx = ddx(position);
				float3 dy = ddy(position);
				return normalize(cross(dy, dx));
			}

            sampler2D _MainTex;
			float4    _MainTex_ST;
            float _Height;
            float _Width;
			float _Attenuation;
			float _Offset;
			float _MinVelocity;

            float _Ambient;
            float _AmbientSaturation;
            float _FaceNoise;
			float3    _LightCenter;
            float4	  _Diffraction;
            float4	  _Color;
            float4	  _BottomColor;
			float _WindFrequency;
			float _WindInfluence;
			float _WindGust;
			float _AbsorbanceExponent;
        	float _AbsorbanceOffset;

			float3 _Velocity;
			float _Mask;
			

			v2f vert (appdata_full v)
			{
				v2f o;
                o.normal = mul(unity_ObjectToWorld, v.normal);
                o.uv  = TRANSFORM_TEX(v.texcoord, _MainTex);
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex);

				float speed = 50;
				float detail = 10;

				float noise1 = (3.0 + sin(v.vertex.x*detail+_Time.y*speed) + sin(v.vertex.y*detail+_Time.y*speed) + sin(v.vertex.z*detail+_Time.y*speed))/6.0;
				float3 noise = float3(sin(v.vertex.x*detail+_Time.y*speed), sin(v.vertex.y*detail+_Time.y*speed), sin(v.vertex.z*detail+_Time.y*speed));
				

				float3 veolcity_dir = _Velocity;//v.tangent.xyz;//normalize(worldPos - _WorldSpaceCameraPos);

				float3 right = normalize(cross(veolcity_dir, o.normal));
				float3 surface = normalize(cross(right, o.normal));
				float3 up = o.normal*(1-_Attenuation) + surface*_Attenuation;


				float normalFactor = dot(o.normal, normalize(veolcity_dir));
				o.normalFactor = normalFactor;

				up = up*(max(0, length(veolcity_dir)-_MinVelocity));

				o.velocityAmount = length(veolcity_dir);

				float normalFactor2 = pow(normalFactor, 2);

				up = up*max(0.1,2*(1-normalFactor2));

				up = up*noise1;
				//right = right*noise1;

				right *= 1-o.uv.y;
				
				worldPos = worldPos + (right*_Width*(o.uv.x-0.5f) + up*_Height*(o.uv.y-0.0f))*_Mask;
				worldPos += o.normal*_Offset;


				

				o.pos = UnityWorldToClipPos(worldPos+noise*0.0);
 
				
				

				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				clip(i.normalFactor);
				
				


				clip(i.velocityAmount-_MinVelocity);
				
				float4 color = lerp(_BottomColor, _Color, i.uv.y);
				color.a = i.uv.y;
				return color;
			}

			ENDCG

		}

		

		
	}
}