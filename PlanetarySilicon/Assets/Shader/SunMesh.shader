Shader "Custom/SunMesh"
{
    Properties {
        _MainTex    ("Texture",       2D        ) = "white" {}
		_Height("Height", Range(0,5000)) = 0.5
		_Width("Width", Range(0,1000)) = 0.5
		_Attenuation("Attenuation", Range(0,10)) = 0.5
		_Offset("Offset", Range(-0.1,0.1)) = 0.0
		_AddColor("AddColor", Range(0,1)) = 0.0

		_MinVelocity("MinVelocity", Range(0,100)) = 0.5
		_DepthGradientShallow("Depth Gradient Shallow", Color) = (0.325, 0.807, 0.971, 0.725)
		_Darkening("Darkening", Range(0,1)) = 0.0



		_Ambient("Ambient", Range(0,1)) = 1
		_AmbientSaturation("Ambient Saturation", Range(0,1)) = 1
		_FaceNoise("Face Noise", Range(0,5000)) = 0.1
        _Center("Center", Vector) = (0,0,0)
		
        _Diffraction("Diffraction", Vector) = (0,0,0,0)
        _Color("Color", Color) = (0,0,0,0)
        _BottomColor("Bottom Color", Color) = (0,0,0,0)

		_WindFrequency("WindFrequency", Range(0,100)) = 1
		_WindInfluence("WindInfluence", Range(0,1)) = 1
		_WindGust("WindGust", Range(0,50)) = 1

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
                float2 uv : TEXCOORD0;
                float rim : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float scale : TEXCOORD3;
                


                
                
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
			float4 _StarColor;
			float4 _DepthGradientShallow;

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
			float _SunBrightDistance;
			float _AddColor;
			float4 _AddStarColor;

			float3 _Velocity;
			float _Mask;

			float _Darkening;

			float4 correctBrightness(float4 c, float d = 2){
				float m = max(max(c.r,c.g),c.b);
				m = 1-m-d/255.0;
				return float4(c.r+m, c.g+m, c.b+m, 1);
			}
			

			v2f vert (appdata_full v)
			{
				v2f o;
                o.normal = -(mul(unity_ObjectToWorld, v.vertex)-mul(unity_ObjectToWorld, float4(0,0,0,1)))*(1.0+_Offset);
				o.scale = length(o.normal);
                o.uv  = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, float4(0,0,0,1)) + o.normal;

				o.normal = normalize(o.normal);


				float speed = 50;
				float detail = 10;

				float noise1 = (3.0 + sin(o.normal.x*detail+_Time.y*speed) + sin(o.normal.y*detail+_Time.y*speed) + sin(o.normal.z*detail+_Time.y*speed))/6.0;
				float3 noise = float3(sin(o.normal.x*detail+_Time.y*speed), sin(o.normal.y*detail+_Time.y*speed), sin(o.normal.z*detail+_Time.y*speed));
				

				float3 viewDir = normalize(mul((float3x3)unity_CameraToWorld, float3(0,0,1)));
				float3 right = normalize(cross(viewDir, o.normal));
				float3 up = o.normal*_Height + o.normal*((sin(v.vertex*100000.0) + 1) * _FaceNoise);




				float speeddiff = pow((sin(o.worldPos.x+o.worldPos.y+o.worldPos.z)+1)/2, _WindGust)*_WindFrequency;

				up = up * (1-_WindInfluence) + up * _WindInfluence * pow((sin(o.worldPos.x + _Time.y*speeddiff)+1)/2, 50);
				right = right * 0.8 + right * 0.2 * pow((sin(o.worldPos.x + _Time.y*speeddiff)+1)/2, 50);



				
				o.worldPos = o.worldPos + (right*_Width*(o.uv.x-0.5f) + up*(o.uv.y-0.0f));


				

				o.pos = UnityWorldToClipPos(o.worldPos);
				o.rim = saturate(dot(viewDir, -o.normal));
				o.rim = pow(1-o.rim,_Attenuation);
				
				

				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{

				

				float scale = i.scale/5000.0;
				//scale = 1;

				float intensity = 1-i.uv.y;


				float dist01 = 1-saturate(length(i.worldPos-_WorldSpaceCameraPos)/(_SunBrightDistance*1));
				float dist02 = 1-saturate(length(i.worldPos-_WorldSpaceCameraPos)/(_SunBrightDistance)*0.3);

				float darkening = dist02;
				darkening = saturate(darkening-0.4);


				intensity = max(1-darkening, intensity);



				//float4 sun_color = lerp(1, _DepthGradientShallow, max(1-intensity, 0));
				float4 starcolor = correctBrightness(lerp(_StarColor, _AddStarColor, _AddColor));
				float4 sun_color = lerp(1, starcolor, max(1-intensity, 0));

				sun_color = pow(sun_color,200*(1-intensity));

				//sun_color = lerp(sun_color, 1, );
				
				sun_color.a=dist02*_Ambient*saturate(scale);
				return i.rim * sun_color;
			
			}

			ENDCG

		}

		

		
	}
}