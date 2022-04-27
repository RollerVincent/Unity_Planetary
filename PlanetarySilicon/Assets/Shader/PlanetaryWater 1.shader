Shader "Custom/PlanetaryWaterbu"
{
    Properties {
        _MainTex    ("Texture",       2D        ) = "white" {}
		_BlendNormal("Blend Normals", Range(0,1)) = 0.5
		_Ambient("Ambient", Range(0,1)) = 1
		_AmbientSaturation("Ambient Saturation", Range(0,1)) = 1
		_FogAmount("Fog Amount", Range(0,10)) = 1
		_FaceNoise("Face Noise", Range(0,1)) = 0.1
		
        _Diffraction("Diffraction", Vector) = (0,0,0,0)
        _Color("Color", Color) = (0,0,0,0)

        _WaterDiffuseOpacity("WaterDiffuseOpacity", Range(0,1)) = 1
        _Speed("WaterSpeed", Range(0,1)) = 1
        _Detail("WaterDetail", Range(0,2)) = 1
        _Impact("WaterImpact", Range(0,10)) = 1
        _RimIntensity("WaterRimIntensity", Range(0,5)) = 1
        _DepthMaxDistance("WaterDepthMaxDistance", Range(0,50)) = 1
        _ReflectionIntensity("WaterReflectionIntensity", Range(0,10)) = 1
        _ReflectionShininess("WaterReflectionShininess", Range(0,200)) = 1
		_AbsorbanceExponent("AbsorbanceExponent", Range(0,20)) = 1
        _AbsorbanceOffset("AbsorbanceOffset", Range(0,100)) = 1
		_AmbientFaces("AmbientFaces", Range(0,1)) = 1
		_DeepColor("DeepColor", Color) = (0,0,0,0)
        _FlatColor("FlatColor", Color) = (0,0,0,0)



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
                float2 uv : TEXCOORD4;
                float4 screenPosition : TEXCOORD3;
				float4 tangent : TANGENT;

                
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
		
            float4	  _Diffraction;
            float4	  _Color;
            float _Speed;
            float _Detail;
            float _Impact;
            float _RimIntensity;
            float _DepthMaxDistance;
            //sampler2D _CameraDepthTexture;
            float _ReflectionIntensity;
            float _ReflectionShininess;

			float _AbsorbanceExponent;
        	float _AbsorbanceOffset;

			float3 _LightCenter;
			float _AmbientFaces;

			float4 _DeepColor;
			float4 _FlatColor;

            float sampleTexture3D(float3 position, sampler2D tex, float detail) {

				//normal = ((normal + new Vector3(1.25f,3.50f,5.75f))/detail) * Radius*0.01f;
				
				position = ((normalize(position) + float3(1.25,3.50,5.75))/detail);

				float c1 = (tex2Dlod(tex, float4(position.x+_Time.y*_Speed, position.y+_Time.y*_Speed, 0, 0))).r;
				float c2 = (tex2Dlod(tex, float4(position.x+_Time.y*_Speed, position.z+_Time.y*_Speed, 0, 0))).r;
				float c3 = (tex2Dlod(tex, float4(position.z+_Time.y*_Speed, position.y+_Time.y*_Speed, 0, 0))).r;



				//return c1;

				return (c1+c2+c3)/3;
			}

        

            float _WaterDiffuseOpacity;

			v2f vert (appdata_full v)
			{
				v2f o;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.uv  = TRANSFORM_TEX(v.texcoord, _MainTex);
				
				//float offset1 = sampleTexture3D(v.vertex, _MainTex, _Detail);
				//v.vertex = v.vertex + normalize(v.vertex) * offset1 * _Impact;
                
				o.worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;
                
                

                o.pos = UnityWorldToClipPos(o.worldPos.xyz);
                o.screenPosition = ComputeScreenPos(o.pos);
                
                o.objectPos = v.vertex;
                
				TRANSFER_SHADOW(o); // pass shadow coordinates to pixel shader
				o.tangent = v.tangent;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				return 1;
				UNITY_LIGHT_ATTENUATION(atten, i, 0)

				float3 Center = mul(unity_ObjectToWorld, float4((i.tangent.xyz), 1)).xyz;


                /*float existingDepth01 = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosition)).r;
				float existingDepthLinear = LinearEyeDepth(existingDepth01);
				float depthDifference = existingDepthLinear - i.screenPosition.w;
				float waterDepthDifference01 = saturate(0.0 + depthDifference / _DepthMaxDistance);

				waterDepthDifference01 = waterDepthDifference01;*/


                fixed4 surfaceColor = tex2D(_MainTex, i.uv)*_Color;
                //fixed4 surfaceColor = lerp(_FlatColor, _DeepColor, waterDepthDifference01);

                float3 faceNormal  = GetFaceNormal(i.worldPos);
				float3 finalNormal = lerp(i.normal, faceNormal, _BlendNormal);

				float3 lightDirection = normalize(_LightCenter-i.worldPos);

				float globalIntensity = (dot(normalize(i.worldPos-Center), lightDirection)+1)/2;
                globalIntensity = pow(1-globalIntensity, _AbsorbanceExponent);
                globalIntensity = min(1, globalIntensity*_AbsorbanceOffset);
                globalIntensity = 1-globalIntensity;
				//return float4(globalIntensity, globalIntensity, globalIntensity, 1);


                float4 lightDiffraction = _Diffraction*(1-globalIntensity)*_Diffraction.w;
                lightDiffraction.a = 0;
                float4 lightColor = 1-lightDiffraction;
				//return lightColor;

                float4 ambientColor = lerp(1,lightColor,_AmbientSaturation);
				ambientColor *= globalIntensity;
				//return ambientColor;

                

                
                //float3 diffuseReflection = lightColor * lightIntensity;

                
				


				float lightIntensity = atten * max(0.0, dot(finalNormal, lightDirection));
				float ambientIntensity = _Ambient;

				

				float3 ambientLightDirection = normalize(_WorldSpaceCameraPos - i.worldPos);
                float ambientLightIntensity = max(0.0, dot(finalNormal, ambientLightDirection));

				ambientColor *= ambientLightIntensity*(_AmbientFaces) + 1*(1-_AmbientFaces);


                float3 perFaceNormal2 = GetFaceNormal(i.objectPos);
                float perFaceNoise = (sin((perFaceNormal2.x + perFaceNormal2.y + perFaceNormal2.z)*1000)+1)/2*globalIntensity;


				float3 viewDir = mul((float3x3)unity_CameraToWorld, float3(0,0,-1));
				float specularReflection = pow(max(0.0, dot(reflect(-lightDirection, finalNormal), viewDir)), _ReflectionShininess);
				specularReflection *= lightIntensity*_ReflectionIntensity;
                
                fixed4 color = 1;
                color.rgb = (lightColor * (1-ambientIntensity) * lightIntensity) + (ambientColor * (ambientIntensity * (1-_FaceNoise) + perFaceNoise*_FaceNoise));
                color *= surfaceColor;

				color = lerp(color, lightColor*_ReflectionIntensity, specularReflection);


                return 1;

				return color;
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
            #include "UnityLightingCommon.cginc"


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
            float _WaterDiffuseOpacity;
            float _Speed;
            sampler2D _MainTex;
			float4    _MainTex_ST;
            float _Detail;
            float3    _Center;
            float _Impact;
            float _ReflectionBrightness;
            float _ReflectionIntensity;


            float sampleTexture3D(float3 position, sampler2D tex, float detail) {

				//normal = ((normal + new Vector3(1.25f,3.50f,5.75f))/detail) * Radius*0.01f;
				
				position = ((normalize(position) + float3(1.25,3.50,5.75))/detail);

				float c1 = (tex2Dlod(tex, float4(position.x+_Time.y*_Speed, position.y+_Time.y*_Speed, 0, 0))).r;
				float c2 = (tex2Dlod(tex, float4(position.x+_Time.y*_Speed, position.z+_Time.y*_Speed, 0, 0))).r;
				float c3 = (tex2Dlod(tex, float4(position.z+_Time.y*_Speed, position.y+_Time.y*_Speed, 0, 0))).r;



				//return c1;

				return (c1+c2+c3)/3;
			}



            



			v2f vert (appdata_full v)
			{
				v2f o;
				
                o.normal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                
                float offset1 = sampleTexture3D(v.vertex, _MainTex, _Detail);
                float3 diff = normalize(o.worldPos-_Center);
				o.worldPos.xyz   += diff*_Impact*offset1;

                o.pos = UnityWorldToClipPos(o.worldPos.xyz);
                
                o.objectPos = v.vertex;
				TRANSFER_SHADOW(o); // pass shadow coordinates to pixel shader
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos)

                float3 faceNormal  = GetFaceNormal(i.worldPos);
				float3 finalNormal = lerp(i.normal, faceNormal, _BlendNormal);


                

                float3 lightDirection = normalize(_WorldSpaceLightPos0 - i.worldPos);

                float lightIntensity = atten * max(0.0, dot(finalNormal, lightDirection));

                float3 perFaceNormal2 = GetFaceNormal(i.objectPos);
                float perFaceNoise = (sin((perFaceNormal2.x + perFaceNormal2.y + perFaceNormal2.z)*1000)+1)/2 * atten;

                float specularReflection = pow(max(0.0, dot(reflect(-lightDirection, finalNormal), normalize(_WorldSpaceCameraPos.xyz-i.worldPos))), _ReflectionIntensity);
				specularReflection *= lightIntensity;

                float4 lightColor = _LightColor0;
                lightColor *= _WaterDiffuseOpacity;
                

                float4 o = (lightColor*(1+specularReflection*_ReflectionBrightness)) * (lightIntensity*(1) - perFaceNoise*_FaceNoise*2*lightIntensity);
                //o.a = _WaterDiffuseOpacity;

                o.a = 0;

                return 0;
			}

			ENDCG
		}
		

		
	}
}