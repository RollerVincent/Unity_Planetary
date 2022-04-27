// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'



Shader "Custom/PlanetaryGlass"
{
    Properties {
        _MainTex    ("Texture",       2D        ) = "white" {}
		_BlendNormal("Blend Normals", Range(0,1)) = 0.5
		_Ambient("Ambient", Range(0,1)) = 1
		_AmbientSaturation("Ambient Saturation", Range(0,1)) = 1
		_FogAmount("Fog Amount", Range(0,10)) = 1
		_FaceNoise("Face Noise", Range(0,1)) = 0.1
        _Center("Center", Vector) = (0,0,0)
        _Diffraction("Diffraction", Vector) = (0,0,0,0)
        _Color("Color", Color) = (0,0,0,0)
	}
	


	SubShader
	{
		//Tags {"Queue"="Geometry"}

		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200

		Pass
		{
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase"}
			ZWrite Off
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
                float4 worldPos : TEXCOORD1;
                float4 objectPos : TEXCOORD2;
                float2 uv : TEXCOORD3;
                
			};

            float3 GetFaceNormal(float3 position) {
				float3 dx = ddx(position);
				float3 dy = ddy(position);
				return normalize(cross(dy, dx));
			}

            sampler2D _MainTex;
			float4    _MainTex_ST;
            float _BlendNormal;
            float _Ambient;
            float _AmbientSaturation;
            float _FaceNoise;
            float _FogAmount;
            float3    _Center;
            float4	  _Diffraction;
			float4 _Color;

			v2f vert (appdata_full v)
			{
				v2f o;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.uv  = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.pos = UnityObjectToClipPos (v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.objectPos = v.vertex;
                float3 planetPos = normalize(o.worldPos-_Center);
                
				o.worldPos.w = (dot(planetPos, _WorldSpaceLightPos0.xyz) + 1)/2;
				TRANSFER_SHADOW(o); // pass shadow coordinates to pixel shader
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				
				UNITY_LIGHT_ATTENUATION(atten, i, 0)
				
				return 1;

                fixed4 surfaceColor = tex2D(_MainTex, i.uv);

                

                float3 faceNormal  = GetFaceNormal(i.worldPos);
				float3 finalNormal = lerp(i.normal, faceNormal, _BlendNormal);

                float globalIntensity = i.worldPos.w;

                float4 lightDiffraction = _Diffraction * (1-globalIntensity);
                float4 lightColor = _LightColor0 - lightDiffraction;

                float4 ambientColor = lerp(1,lightColor,_AmbientSaturation);


                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);

                float3 diff = normalize(i.worldPos-_Center);
				float fogIntensity = 1-saturate(dot(diff, normalize(_WorldSpaceCameraPos.xyz-_Center)));
                fogIntensity = min(1,fogIntensity*_FogAmount);
                
				float3 viewDirection = normalize(i.worldPos - _WorldSpaceCameraPos);
				float viewIntensity = dot(viewDirection, finalNormal);
				viewIntensity = 1 - max(viewIntensity, -viewIntensity);

				


                float lightIntensity = max(0.0, dot(finalNormal, lightDirection));

				lightIntensity *= viewIntensity;

                float ambientIntensity = max(0,globalIntensity-_Ambient);

                float3 perFaceNormal2 = GetFaceNormal(i.objectPos);
                float perFaceNoise = (sin((perFaceNormal2.x + perFaceNormal2.y + perFaceNormal2.z)*1000)+1)/2*globalIntensity;

                //lightIntensity = lightIntensity * (1-ambient) + (ambient * (1-_FaceNoise) + perFaceNoise*_FaceNoise);


                float4 fogColor = lightColor*ambientIntensity;


                
                //float3 diffuseReflection = lightColor * lightIntensity;


                

                fixed4 color = 1;

                color.rgb = (lightColor * (1-ambientIntensity) * lightIntensity) + (ambientColor * (ambientIntensity * (1-_FaceNoise) + perFaceNoise*_FaceNoise));

                color = color*surfaceColor * (1-fogIntensity) + fogColor * fogIntensity;

				return 0.5*color*_Color*atten;

			}

			ENDCG

		}

		Pass
		{
			Name "FORWARDADD"
			Tags { "LightMode" = "ForwardAdd"}
			
			ZWrite Off
			Cull Off
			
			Blend One One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// Include shadowing support for point/spot
			#pragma multi_compile_fwdadd_fullshadows
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
                float3 normal : NORMAL;
				float3 worldPos : TEXCOORD0;
                float4 objectPos : TEXCOORD2;

				SHADOW_COORDS(1)
			};

            float3 GetFaceNormal(float3 position) {
				float3 dx = ddx(position);
				float3 dy = ddy(position);
				return normalize(cross(dy, dx));
			}

            float _BlendNormal;
            float _FaceNoise;
			float4 _Color;

			v2f vert (appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.objectPos = v.vertex;
				TRANSFER_SHADOW(o); // pass shadow coordinates to pixel shader
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos)

                float3 faceNormal  = GetFaceNormal(i.worldPos);
				float3 finalNormal = lerp(i.normal, faceNormal, _BlendNormal);

				float3 viewDirection = normalize(i.worldPos - _WorldSpaceCameraPos);
				float viewIntensity = dot(viewDirection, finalNormal);
				viewIntensity = 1 - max(viewIntensity, -viewIntensity);


                

                float3 lightDirection = normalize(_WorldSpaceLightPos0 - i.worldPos);

                float lightIntensity = max(0.0, dot(finalNormal, lightDirection));

				lightIntensity *= viewIntensity;

                float3 perFaceNormal2 = GetFaceNormal(i.objectPos);
                float perFaceNoise = (sin((perFaceNormal2.x + perFaceNormal2.y + perFaceNormal2.z)*1000)+1)/2 * atten;

				

				float4 color = _LightColor0 * (lightIntensity*(1) - perFaceNoise*_FaceNoise*2);

                return color * atten;
			}

			ENDCG
		}

		// Support for casting shadows from this shader. Remove if not needed.

		Pass {
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile_instancing // allow instanced shadow pass for most of the shaders
			#include "UnityCG.cginc"

			struct v2f {
				V2F_SHADOW_CASTER;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert( appdata_base v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				return o;
			}

			float4 frag( v2f i ) : SV_Target
			{
				SHADOW_CASTER_FRAGMENT(i)
				return 1;
			}
			ENDCG

				}
		
	}


}