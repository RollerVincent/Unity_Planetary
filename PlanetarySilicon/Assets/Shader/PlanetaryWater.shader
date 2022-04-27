
Shader "Custom/PlanetaryWater"
{
    Properties {
        _MainTex    ("Texture",       2D        ) = "white" {}
		_BlendNormal("Blend Normals", Range(0,1)) = 0.5
		_Ambient("Ambient", Range(0,1)) = 1
		_AmbientSaturation("Ambient Saturation", Range(0,1)) = 1
		_FaceNoise("Face Noise", Range(0,1)) = 0.1
        _Diffraction("Diffraction", Vector) = (0,0,0,0)
        
		_AbsorbanceExponent("AbsorbanceExponent", Range(0,20)) = 1
        _AbsorbanceOffset("AbsorbanceOffset", Range(0,100)) = 1
        _AmbientFaces("AmbientFaces", Range(0,1)) = 1
		_Speed("WaterSpeed", Range(0,1)) = 1
        _Detail("WaterDetail", Range(0,2)) = 1
        _Impact("WaterImpact", Range(0,10)) = 1
        _WaterDepthMaxDistance("WaterDepthMaxDistance", Range(0,100)) = 1


		_FlatColor("FlatColor", Color) = (0,0,0,0)
		_DeepColor("DeepColor", Color) = (0,0,0,0)

		_AtmossphereRadius("AtmossphereRadius", Range(0,1000)) = 1
		_Steps("Steps", Range(0,1000)) = 1
		_StepSize("StepSize", Range(0, 10)) = 1
        _DepthMaxDistance("FogDepthMaxDistance", Range(0,1000)) = 1
        _DepthMinDistance("FogDepthMinDistance", Range(0,1000)) = 1
		_ReflectionIntensity("ReflectionIntensity", Range(0,2)) = 1
        _ReflectionShininess("ReflectionShininess", Range(0,100)) = 1


		
		
		
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		

		// Forward rendering base (main directional light) pass.
		Pass
		{
			Name "FORWARDBASE"
			Tags { "LightMode" = "ForwardBase" }
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
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
                float4 objectPos : TEXCOORD2;
                float2 uv : TEXCOORD3;
				float4 tangent : TANGENT;
				float4 screenPosition : TEXCOORD4;
                
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
			float3 _LightCenter;
            float4	  _Diffraction;
            float4	  _Color;
			float _AbsorbanceExponent;
			float _AbsorbanceOffset;
			float _AmbientFaces;
			float _Speed;
			float _Detail;
			float _Impact;
			float _WaterDepthMaxDistance;
			float4 _FlatColor;
			float4 _DeepColor;
			sampler2D _CameraDepthTexture;
			float _AtmossphereRadius;
			int _Steps;
			float _StepSize;
			float _DepthMaxDistance;
			float _DepthMinDistance;
			float _ReflectionIntensity;
        	float _ReflectionShininess;


			float sampleTexture3D(float3 position, sampler2D tex, float detail) {
				
				position = ((normalize(position) + float3(1.25,3.50,5.75))/detail);

				float c1 = (tex2Dlod(tex, float4(position.x+_Time.y*_Speed, position.y+_Time.y*_Speed, 0, 0))).r;
				float c2 = (tex2Dlod(tex, float4(position.x+_Time.y*_Speed, position.z+_Time.y*_Speed, 0, 0))).r;
				float c3 = (tex2Dlod(tex, float4(position.z+_Time.y*_Speed, position.y+_Time.y*_Speed, 0, 0))).r;

				return (c1+c2+c3)/3;
			}

			bool sphereOut (float3 p, float d, float3 c)
			{
				return distance(p, c) > d;
			}

			float3 raymarch (float3 position, float3 direction, float3 center, float3 radius)
			{
				for (int i = 0; i < _Steps; i++)
				{
					if (sphereOut(position, radius, center))
						return position;
			
					position += direction * _StepSize;
				}
				return position; // White
			}



			v2f vert (appdata_full v)
			{
				v2f o;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.uv  = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;

				float3 Center = mul(unity_ObjectToWorld, float4((v.tangent.xyz), 1)).xyz;
				float3 viewDirection = normalize(o.worldPos - _WorldSpaceCameraPos);
				float f = saturate(dot(-viewDirection, normalize(o.worldPos-Center)));
				f = min(f*10000000, 1);

				float offset1 = sampleTexture3D(v.vertex, _MainTex, _Detail);
				v.vertex = v.vertex + normalize(v.vertex) * offset1 * _Impact*f;

				o.pos = UnityObjectToClipPos (v.vertex);
				o.screenPosition = ComputeScreenPos(o.pos);
                o.worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;
                o.objectPos = v.vertex;
				o.tangent = v.tangent;
				TRANSFER_SHADOW(o);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				
				UNITY_LIGHT_ATTENUATION(atten, i, 0)

				float3 Center = mul(unity_ObjectToWorld, float4((i.tangent.xyz), 1)).xyz;



				float existingDepth01 = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosition)).r;
				float existingDepthLinear = LinearEyeDepth(existingDepth01);
				float waterDepthDifference = existingDepthLinear - i.screenPosition.w;
				float waterDepthDifference01 = saturate(0.0 + waterDepthDifference / _WaterDepthMaxDistance);


				fixed4 surfaceColor = lerp(_FlatColor, _DeepColor, waterDepthDifference01);

                float3 faceNormal  = GetFaceNormal(i.worldPos);
				float3 finalNormal = lerp(i.normal, faceNormal, _BlendNormal);

				float3 lightDirection = normalize(_LightCenter-i.worldPos);

				float globalIntensity = (dot(normalize(i.worldPos-Center), lightDirection)+1)/2;
                globalIntensity = pow(1-globalIntensity, _AbsorbanceExponent);
                globalIntensity = min(1, globalIntensity*_AbsorbanceOffset);
                globalIntensity = 1-globalIntensity;
				//return globalIntensity;


                float4 lightDiffraction = _Diffraction*(1-globalIntensity)*_Diffraction.w;
                lightDiffraction.a = 0;
                float4 lightColor = 1-lightDiffraction;



                float4 ambientColor = lerp(1,lightColor,_AmbientSaturation);
				ambientColor *= globalIntensity;
				//return ambientColor;


                float lightIntensity = atten * max(0.0, dot(finalNormal, lightDirection));
				float ambientIntensity = _Ambient;

				float3 ambientLightDirection = normalize(_WorldSpaceCameraPos - i.worldPos);
                float ambientLightIntensity = max(0.0, dot(finalNormal, ambientLightDirection));

				ambientColor *= ambientLightIntensity*(_AmbientFaces) + 1*(1-_AmbientFaces);


                float3 perFaceNormal2 = GetFaceNormal(i.objectPos);
                float perFaceNoise = (sin((perFaceNormal2.x + perFaceNormal2.y + perFaceNormal2.z)*1000)+1)/2*globalIntensity;


				
                
                fixed4 color = 1;
                color.rgb = (lightColor * (1-ambientIntensity) * lightIntensity) + (ambientColor * (ambientIntensity * (1-_FaceNoise) + perFaceNoise*_FaceNoise));
                color *= surfaceColor;


				float3 viewDir = mul((float3x3)unity_CameraToWorld, float3(0,0,-1));
				float specularReflection = pow(max(0.0, dot(reflect(-lightDirection, finalNormal), viewDir)), _ReflectionShininess);
				specularReflection *= lightIntensity*_ReflectionIntensity;

                

				color = lerp(color, lightColor*_ReflectionIntensity, specularReflection);









				// fog


				float3 viewDirection = normalize(i.worldPos - _WorldSpaceCameraPos);
    			float3 atmosspherePosition = raymarch(i.worldPos, -viewDirection, Center, _AtmossphereRadius);

				float centerDist = length(_WorldSpaceCameraPos - Center);
				if(centerDist < _AtmossphereRadius){
					atmosspherePosition = _WorldSpaceCameraPos;
				}

				float depthDifference = length(atmosspherePosition-i.worldPos);
				float DepthDifference01 = saturate((depthDifference-_DepthMinDistance) / (_DepthMaxDistance-_DepthMinDistance));	    

				float4 fogColor = lightColor*globalIntensity;

				return lerp(color, fogColor, DepthDifference01);

			

			}

			ENDCG

		}



		// Forward additive pass (only needed if you care about more lights than 1 directional).
		// Can remove if no point/spot light support needed.
		Pass
		{
			Name "FORWARDADD"
			Tags { "LightMode" = "ForwardAdd" }
			ZWrite Off Blend One One

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
				float2 uv : TEXCOORD3;

				SHADOW_COORDS(1)
			};

            float3 GetFaceNormal(float3 position) {
				float3 dx = ddx(position);
				float3 dy = ddy(position);
				return normalize(cross(dy, dx));
			}

			sampler2D _MainTex;
			float4    _MainTex_ST;
            float _BlendNormal;
            float _FaceNoise;
			float4 _Color;

			v2f vert (appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.uv  = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv.y = 1-o.uv.y;
                o.normal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.objectPos = v.vertex;
				TRANSFER_SHADOW(o); // pass shadow coordinates to pixel shader
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos)
                
				fixed4 surfaceColor = tex2D(_MainTex, i.uv)*_Color;

                float3 faceNormal  = GetFaceNormal(i.worldPos);
				float3 finalNormal = lerp(i.normal, faceNormal, _BlendNormal);


                

                float3 lightDirection = normalize(_WorldSpaceLightPos0 - i.worldPos);

                float lightIntensity = atten * max(0.0, dot(finalNormal, lightDirection));

                float3 perFaceNormal2 = GetFaceNormal(i.objectPos);
                float perFaceNoise = (sin((perFaceNormal2.x + perFaceNormal2.y + perFaceNormal2.z)*1000)+1)/2 * atten;


                return _LightColor0 * surfaceColor * (lightIntensity*(1) - perFaceNoise*_FaceNoise*2*lightIntensity);
			}

			ENDCG
		}

		// Support for casting shadows from this shader. Remove if not needed.
		UsePass "VertexLit/SHADOWCASTER"
	}
}