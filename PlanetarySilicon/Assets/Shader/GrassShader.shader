Shader "Custom/PlanetaryGrass"
{
    Properties {
        _MainTex    ("Texture",       2D        ) = "white" {}
		_Height("Height", Range(0,10)) = 0.5

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
		Tags { "RenderType"="Opaque" }
		LOD 200

		

		// Forward rendering base (main directional light) pass.
		Pass
		{
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }

			ZWrite On
			Cull Off		

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
				SHADOW_COORDS(0) // shadow parameters to pass from vertex
                float3 worldPos : TEXCOORD1;
                float3 objectPos : TEXCOORD2;
                float2 uv : TEXCOORD3;
                float3 center : TANGENT;
                
			};

            float3 GetFaceNormal(float3 position) {
				float3 dx = ddx(position);
				float3 dy = ddy(position);
				return normalize(cross(dy, dx));
			}

            sampler2D _MainTex;
			float4    _MainTex_ST;
            float _Height;
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
			

			v2f vert (appdata_full v)
			{
				v2f o;
                o.normal = mul(unity_ObjectToWorld, v.normal);
                o.uv  = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				

				float3 Center = mul(unity_ObjectToWorld, float4((v.tangent.xyz), 1)).xyz;
				o.center = Center;

				float3 camdir = normalize(o.worldPos - _WorldSpaceCameraPos);
				float3 center_dir = normalize(o.worldPos - Center);

				float3 right = normalize(cross(camdir, center_dir));
				float3 up = center_dir*_Height;
				float3 forward = normalize(cross(right, up));

				float wx = sin(_Time.x * _WindFrequency + o.worldPos.x * _WindGust)*_WindInfluence;
				float wz = sin(_Time.x * _WindFrequency + o.worldPos.z * _WindGust*2)*_WindInfluence;
				
				o.worldPos = o.worldPos + up*o.uv.y + (wx*right*o.uv.y) + (wz*forward*o.uv.y);
				o.pos = UnityWorldToClipPos(o.worldPos);
 
                o.objectPos = mul(unity_WorldToObject, o.worldPos);

				TRANSFER_SHADOW(o); // pass shadow coordinates to pixel shader
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				UNITY_LIGHT_ATTENUATION(atten, i, 0)


				float3 Center = i.center;

			
                fixed4 surfaceColor = tex2D(_MainTex, i.uv);
				clip(surfaceColor.r-0.5);
				surfaceColor = lerp(_BottomColor, _Color, i.uv.y);


				float3 lightDirection = normalize(_LightCenter-i.worldPos);

				
				float globalIntensity = (dot(normalize(i.worldPos-Center), lightDirection)+1)/2;
                globalIntensity = pow(1-globalIntensity, _AbsorbanceExponent);
                globalIntensity = min(1, globalIntensity*_AbsorbanceOffset);
                globalIntensity = 1-globalIntensity;
				//return globalIntensity;
				


                float4 lightDiffraction = _Diffraction*(1-globalIntensity)*_Diffraction.w;
                lightDiffraction.a = 0;
                float4 lightColor = 1-lightDiffraction;
				//return lightColor;


                float4 ambientColor = lerp(1,lightColor,_AmbientSaturation);
				ambientColor *= globalIntensity;
				//return ambientColor;


                float lightIntensity = atten * max(0.0, dot(i.normal, lightDirection));
				float ambientIntensity = _Ambient;


                float3 perFaceNormal2 = GetFaceNormal(i.objectPos);
                float perFaceNoise = (sin((perFaceNormal2.x + perFaceNormal2.y + perFaceNormal2.z)*1000)+1)/2*globalIntensity;


                fixed4 color = 1;
                color.rgb = (lightColor * (1-ambientIntensity) * lightIntensity) + (ambientColor * (ambientIntensity * (1-_FaceNoise) + perFaceNoise*_FaceNoise));
                color.rgb *= surfaceColor;
				color.a = 1;

				return color;
			}

			ENDCG

		}

		

		Pass
		{
			Name "FORWARDADD"
			Tags { "LightMode" = "ForwardAdd" }
			
			ZWrite Off 
			Blend One One
			Cull Off		

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			

			#pragma multi_compile_fwdadd_fullshadows
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			

			struct v2f
			{
				float4 pos : SV_POSITION;
                float3 normal : NORMAL;
				SHADOW_COORDS(0)
                float3 worldPos : TEXCOORD1;
                float3 objectPos : TEXCOORD2;
                float2 uv : TEXCOORD3;
                
			};

            float3 GetFaceNormal(float3 position) {
				float3 dx = ddx(position);
				float3 dy = ddy(position);
				return normalize(cross(dy, dx));
			}

            sampler2D _MainTex;
			float4    _MainTex_ST;
            float _Height;
            float _Ambient;
            float _AmbientSaturation;
            float _FaceNoise;
            float4	  _Diffraction;
            float4	  _Color;
            float4	  _BottomColor;
			float _WindFrequency;
			float _WindInfluence;
			float _WindGust;
			

			v2f vert (appdata_full v)
			{
				v2f o;
                o.normal = mul(unity_ObjectToWorld, v.normal);
                o.uv  = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);

				float3 Center = mul(unity_ObjectToWorld, float4((v.tangent.xyz), 1)).xyz;

				float3 camdir = normalize(o.worldPos - _WorldSpaceCameraPos);
				float3 center_dir = normalize(o.worldPos - Center);

				float3 right = normalize(cross(camdir, center_dir));
				float3 up = center_dir*_Height;
				float3 forward = normalize(cross(right, up));

				float wx = sin(_Time.x * _WindFrequency + o.worldPos.x * _WindGust)*_WindInfluence;
				float wz = sin(_Time.x * _WindFrequency + o.worldPos.z * _WindGust*2)*_WindInfluence;
				
				o.worldPos = o.worldPos + up*o.uv.y + (wx*right*o.uv.y) + (wz*forward*o.uv.y);
				o.pos = UnityWorldToClipPos(o.worldPos);
 
                o.objectPos = mul(unity_WorldToObject, o.worldPos);

				TRANSFER_SHADOW(o); // pass shadow coordinates to pixel shader
				return o;

			}

			fixed4 frag (v2f i) : SV_Target
			{
				
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos)
				


                fixed4 surfaceColor = tex2D(_MainTex, i.uv);
				clip(surfaceColor.r-0.5);
				surfaceColor = lerp(_BottomColor, _Color, i.uv.y);


				float3 lightDirection = normalize(_WorldSpaceLightPos0 - i.worldPos);

                float lightIntensity = atten * max(0.0, dot(i.normal, lightDirection));

                float3 perFaceNormal2 = GetFaceNormal(i.objectPos);
                float perFaceNoise = (sin((perFaceNormal2.x + perFaceNormal2.y + perFaceNormal2.z)*1000)+1)/2 * atten;


				fixed4 color = 1;
                color.rgb = _LightColor0 * lightIntensity - perFaceNoise*_FaceNoise;
                color.rgb *= surfaceColor;
				color.a = 1;

				return color;


		
			}

			ENDCG

		}


		
        Pass
    	{
			Tags{ "LightMode" = "ShadowCaster"  }
			Name "ShadowCaster"

			
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			struct v2f
			{
				float2 uv : TEXCOORD3;
				V2F_SHADOW_CASTER;
			};
			

			sampler2D _MainTex;
			float4    _MainTex_ST;
			float3 _Center;
			float _Height;
			float _WindFrequency;
			float _WindInfluence;
			float _WindGust;

			v2f vert(appdata_full v)
			{
				v2f o;
				o.uv  = TRANSFORM_TEX(v.texcoord, _MainTex);


				float3 worldPos = mul(unity_ObjectToWorld, v.vertex);

				float3 camdir = normalize(worldPos - _WorldSpaceCameraPos);

				float3 Center = mul(unity_ObjectToWorld, float4((v.tangent.xyz), 1)).xyz;

				float3 center_dir = normalize(worldPos - Center);


				float3 right = normalize(cross(camdir, center_dir));
				float3 up = center_dir*_Height;
				float3 forward = normalize(cross(right, up));

				float wx = sin(_Time.x * _WindFrequency + worldPos.x * _WindGust)*_WindInfluence;
				float wz = sin(_Time.x * _WindFrequency + worldPos.z * _WindGust*2)*_WindInfluence;

				v.vertex = mul(unity_WorldToObject, float4(worldPos + up*o.uv.y + (wx*right*o.uv.y) + (wz*forward*o.uv.y), 1));

				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				return o;
			}

			float4 frag(v2f i) : COLOR
			{

				fixed4 color = tex2D(_MainTex, i.uv);
				clip(color.r-0.5);

				SHADOW_CASTER_FRAGMENT(i)
			}

			ENDCG
		}
	}
}