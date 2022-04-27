Shader "Custom/PlanetaryInnerAtmossphere"
{
    Properties {


        _MainTex    ("Texture",       2D        ) = "white" {}

        _Diffraction("Diffraction", Vector) = (0,0,0,0)
        _HeightDiffraction("HeightDiffraction", Vector) = (0,0,0,0)
        _DepthMaxDistance("DepthMaxDistance", Range(0,500)) = 1
        _DepthMinDistance("DepthMinDistance", Range(0,500)) = 1
        _AbsorbanceExponent("AbsorbanceExponent", Range(0,20)) = 1
        _AbsorbanceOffset("AbsorbanceOffset", Range(0,5)) = 1
        _RimExponent("RimExponent", Range(0,20)) = 1
        _RimOffset("RimOffset", Range(0,1)) = 1
        _StarDistance("StarDistance", Range(0,10000)) = 1
        _ScreenScale("ScreenScale", Range(0,1000)) = 1


	}

	SubShader
	{

        Tags {"Queue"="Transparent" "IgnoreProjector"="true" "RenderType"="Transparent"}
        ZWrite Off	
        ZTest Always	
        Cull Off



        Pass
		{
			Name "BASE"
			Tags { "LightMode" = "ForwardBase" }
            Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
            #include "UnityLightingCommon.cginc"


			struct v2f
			{
				float4 pos : SV_POSITION;
                float3 normal : NORMAL;
                float3 worldPos : TEXCOORD0;
                float4 screenPosition : TEXCOORD1;
                float3 objectWorldPos : TEXCOORD2;
                float3 objectRight : TEXCOORD3;
                float3 objectUp : TEXCOORD4;
                float2 uv       : TEXCOORD5;

			};

            
            sampler2D _CameraDepthTexture;
            sampler2D _MainTex;
			float4    _MainTex_ST;
            
		    float3 _LightCenter;
            float4	  _Diffraction;
            float4 _HeightDiffraction;
            float _DepthMaxDistance;
            float _DepthMinDistance;
            float _AbsorbanceOffset;
            float _AbsorbanceExponent;
            float _RimExponent;
            float _RimOffset;
            float _StarDistance;
            float _PlaceHolderDistance;
            float _ScreenScale;
         

        

           



			v2f vert (appdata_full v)
			{
				v2f o;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.objectRight = UnityObjectToWorldNormal(float3(1.0,0.0,0.0));
                o.objectUp = UnityObjectToWorldNormal(float3(0.0,1.0,0.0));
                o.uv       = TRANSFORM_TEX(v.texcoord, _MainTex);


                float3 forward = mul((float3x3)unity_CameraToWorld, float3(0,0,1));
                float3 right = mul((float3x3)unity_CameraToWorld, float3(1,0,0));
                float3 up = mul((float3x3)unity_CameraToWorld, float3(0,1,0));

                //o.worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;

                float2 centerv = float2((o.uv.x-0.5)*2, (o.uv.y-0.5)*2);
                o.worldPos = _WorldSpaceCameraPos + forward * 1 + up*(centerv.y)*_ScreenScale + right*(centerv.x)*_ScreenScale;

                o.objectWorldPos = mul(unity_ObjectToWorld, float4(float3(0,0,0), 1)).xyz;
                o.pos = UnityWorldToClipPos(o.worldPos);
                o.screenPosition = ComputeScreenPos(o.pos);

                //o.worldPos = _WorldSpaceCameraPos + forward * 0.1 + up*(o.screenPosition.y/_ScreenScale-0.5) + right*(o.screenPosition.x/_ScreenScale-0.5);
                //o.pos = UnityWorldToClipPos(o.worldPos);
                //o.screenPosition = ComputeScreenPos(o.pos);
                
                


                

				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				
               
            
                float existingDepth01 = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosition)).r;
				float existingDepthLinear = LinearEyeDepth(existingDepth01);
				float depthDifference = existingDepthLinear - i.screenPosition.w;
				float DepthDifference01 = saturate((depthDifference-_DepthMinDistance) / (_DepthMaxDistance-_DepthMinDistance));

                float2 centerv = float2((i.uv.x-0.5)*2, (i.uv.y-0.5)*2);
                float uvDist = centerv.x*centerv.x + centerv.y*centerv.y;
                //clip(1-uvDist);

                //return DepthDifference01;


                
                
                float3 DepthWorldPos = i.worldPos - normalize(_WorldSpaceCameraPos - i.worldPos)*min(depthDifference,_DepthMaxDistance);
                float3 ViewWorldPos = i.worldPos - normalize(_WorldSpaceCameraPos - i.worldPos)*depthDifference;              

                float3 tmppos = normalize(DepthWorldPos-i.objectWorldPos);



		        float3 lightDirection = normalize(_LightCenter-i.worldPos);


                float globalIntensity = (dot(tmppos, lightDirection)+1)/2;
                
                float uvx = pow((acos(dot(tmppos, i.objectRight))/3.14159265359),1);
                float uvy = (dot(tmppos, i.objectUp)+1)/2;

                float4 texcolor = tex2D(_MainTex, float2(uvx, uvy));

                //return texcolor;

                globalIntensity = pow(1-globalIntensity, _AbsorbanceExponent);
                globalIntensity = min(1, globalIntensity*_AbsorbanceOffset);
                globalIntensity = 1-globalIntensity;


                float DepthCenterDist = length(DepthWorldPos-i.objectWorldPos);
                float scale = length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)); // scale x axis
                float DepthCenterDist01 = max(0,DepthCenterDist/scale*2-_RimOffset)/(1-_RimOffset);


                float DepthCamCenterDist = length(_WorldSpaceCameraPos-i.objectWorldPos);
                float DepthCamCenterDist01 = max(0,DepthCamCenterDist/scale*2-_RimOffset)/(1-_RimOffset);

                clip(2-DepthCamCenterDist01);

                float distanceAlpha = 1-(saturate((DepthCamCenterDist01-1)*5));

                float3 viewDirection = normalize(_WorldSpaceCameraPos - i.worldPos);
                
                float viewIntensity = saturate(dot(viewDirection, i.normal));
                viewIntensity = pow(viewIntensity, _RimExponent);
                viewIntensity = min(1, viewIntensity*_RimOffset);

                float4 heightDiffraction = _HeightDiffraction*(DepthCenterDist01*1)*_HeightDiffraction.w;
                heightDiffraction.a = 0;
                float4 heightDiffractionColor = 1-heightDiffraction;
                //return heightDiffractionColor;

                float4 lightDiffraction = _Diffraction*(1-globalIntensity)*_Diffraction.w;
                lightDiffraction.a = 0;
                float4 lightDiffractionColor = 1-lightDiffraction;
                //return lightDiffractionColor;

                //return lerp(lightDiffractionColor, heightDiffractionColor, DepthCenterDist01);

                float4 finalLightColor = lerp(lightDiffractionColor, heightDiffractionColor, DepthCenterDist01)*globalIntensity;
                finalLightColor.a=1;
                //return finalLightColor;

                float viewWorldCenterDistance = length(ViewWorldPos - i.objectWorldPos);
                float viewWorldCenterDistance01 = viewWorldCenterDistance/scale*2;
                float viewWorldAtmosDistance = viewWorldCenterDistance01-1;
                //return viewWorldAtmosDistance;

               
                
                float viewWorldAtmosDistance01 = max(0, viewWorldAtmosDistance);


                float transluencyMask = saturate(2*globalIntensity);

                
                if(existingDepthLinear <= _StarDistance){
                    viewWorldAtmosDistance01 = saturate(viewWorldAtmosDistance01*100000);
                    transluencyMask = 1-viewWorldAtmosDistance01;
                }

                transluencyMask = max(1-DepthCenterDist01,transluencyMask);


                

                float4 outColor = finalLightColor*texcolor;




                float placeHolderDistance01 = saturate(length(_WorldSpaceCameraPos - i.objectWorldPos)/_PlaceHolderDistance);
                placeHolderDistance01 = pow(placeHolderDistance01,3);


                float placeHolderEffect = lerp(DepthDifference01, 1, placeHolderDistance01);




               
                transluencyMask = 1-saturate(viewWorldAtmosDistance);

                //transluencyMask = transluencyMask*placeHolderEffect;

                //transluencyMask = max(1-DepthCenterDist01,transluencyMask);


                //return transluencyMask;


               

                transluencyMask = 1-transluencyMask;


                transluencyMask = saturate(((viewWorldAtmosDistance01-100)));

                outColor.a=transluencyMask*DepthDifference01*distanceAlpha*pow(distanceAlpha,1);


                //return transluencyMask;


                return float4(0,0,0,transluencyMask*globalIntensity)*distanceAlpha;

                return outColor;
                

			}

			ENDCG

		}

         Pass
		{
			Name "BASE"
			Tags { "LightMode" = "ForwardBase" }
            Blend SrcAlpha One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
            #include "UnityLightingCommon.cginc"


			struct v2f
			{
				float4 pos : SV_POSITION;
                float3 normal : NORMAL;
                float3 worldPos : TEXCOORD0;
                float4 screenPosition : TEXCOORD1;
                float3 objectWorldPos : TEXCOORD2;
                float3 objectRight : TEXCOORD3;
                float3 objectUp : TEXCOORD4;
                float2 uv       : TEXCOORD5;

			};

            
            sampler2D _CameraDepthTexture;
            sampler2D _MainTex;
			float4    _MainTex_ST;
            
		    float3 _LightCenter;
            float4	  _Diffraction;
            float4 _HeightDiffraction;
            float _DepthMaxDistance;
            float _DepthMinDistance;
            float _AbsorbanceOffset;
            float _AbsorbanceExponent;
            float _RimExponent;
            float _RimOffset;
            float _StarDistance;
            float _PlaceHolderDistance;
            float _ScreenScale;
         

        

           



			v2f vert (appdata_full v)
			{
				v2f o;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.objectRight = UnityObjectToWorldNormal(float3(1.0,0.0,0.0));
                o.objectUp = UnityObjectToWorldNormal(float3(0.0,1.0,0.0));
                o.uv       = TRANSFORM_TEX(v.texcoord, _MainTex);


                float3 forward = mul((float3x3)unity_CameraToWorld, float3(0,0,1));
                float3 right = mul((float3x3)unity_CameraToWorld, float3(1,0,0));
                float3 up = mul((float3x3)unity_CameraToWorld, float3(0,1,0));

                //o.worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;

                float2 centerv = float2((o.uv.x-0.5)*2, (o.uv.y-0.5)*2);
                o.worldPos = _WorldSpaceCameraPos + forward * 1 + up*(centerv.y)*_ScreenScale + right*(centerv.x)*_ScreenScale;

                o.objectWorldPos = mul(unity_ObjectToWorld, float4(float3(0,0,0), 1)).xyz;
                o.pos = UnityWorldToClipPos(o.worldPos);
                o.screenPosition = ComputeScreenPos(o.pos);

                //o.worldPos = _WorldSpaceCameraPos + forward * 0.1 + up*(o.screenPosition.y/_ScreenScale-0.5) + right*(o.screenPosition.x/_ScreenScale-0.5);
                //o.pos = UnityWorldToClipPos(o.worldPos);
                //o.screenPosition = ComputeScreenPos(o.pos);
                
                


                

				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				
               
            
                float existingDepth01 = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosition)).r;
				float existingDepthLinear = LinearEyeDepth(existingDepth01);
				float depthDifference = existingDepthLinear - i.screenPosition.w;
				float DepthDifference01 = saturate((depthDifference-_DepthMinDistance) / (_DepthMaxDistance-_DepthMinDistance));

                float2 centerv = float2((i.uv.x-0.5)*2, (i.uv.y-0.5)*2);
                float uvDist = centerv.x*centerv.x + centerv.y*centerv.y;
                //clip(1-uvDist);

                //return DepthDifference01;


                
                
                float3 DepthWorldPos = i.worldPos - normalize(_WorldSpaceCameraPos - i.worldPos)*min(depthDifference,_DepthMaxDistance);
                float3 ViewWorldPos = i.worldPos - normalize(_WorldSpaceCameraPos - i.worldPos)*depthDifference;              

                float3 tmppos = normalize(DepthWorldPos-i.objectWorldPos);



		        float3 lightDirection = normalize(_LightCenter-i.worldPos);


                float globalIntensity = (dot(tmppos, lightDirection)+1)/2;
                
                float uvx = pow((acos(dot(tmppos, i.objectRight))/3.14159265359),1);
                float uvy = (dot(tmppos, i.objectUp)+1)/2;

                float4 texcolor = tex2D(_MainTex, float2(uvx, uvy));

                //return texcolor;

                globalIntensity = pow(1-globalIntensity, _AbsorbanceExponent);
                globalIntensity = min(1, globalIntensity*_AbsorbanceOffset);
                globalIntensity = 1-globalIntensity;


                float DepthCenterDist = length(DepthWorldPos-i.objectWorldPos);
                float scale = length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)); // scale x axis
                float DepthCenterDist01 = max(0,DepthCenterDist/scale*2-_RimOffset)/(1-_RimOffset);


                float DepthCamCenterDist = length(_WorldSpaceCameraPos-i.objectWorldPos);
                float DepthCamCenterDist01 = max(0,DepthCamCenterDist/scale*2-_RimOffset)/(1-_RimOffset);

                clip(2-DepthCamCenterDist01);

                float distanceAlpha = 1-(saturate((DepthCamCenterDist01-1)*5));

                float3 viewDirection = normalize(_WorldSpaceCameraPos - i.worldPos);
                
                float viewIntensity = saturate(dot(viewDirection, i.normal));
                viewIntensity = pow(viewIntensity, _RimExponent);
                viewIntensity = min(1, viewIntensity*_RimOffset);

                float4 heightDiffraction = _HeightDiffraction*(DepthCenterDist01*1)*_HeightDiffraction.w;
                heightDiffraction.a = 0;
                float4 heightDiffractionColor = 1-heightDiffraction;
                //return heightDiffractionColor;

                float4 lightDiffraction = _Diffraction*(1-globalIntensity)*_Diffraction.w;
                lightDiffraction.a = 0;
                float4 lightDiffractionColor = 1-lightDiffraction;
                //return lightDiffractionColor;

                //return lerp(lightDiffractionColor, heightDiffractionColor, DepthCenterDist01);

                float4 finalLightColor = lerp(lightDiffractionColor, heightDiffractionColor, DepthCenterDist01)*globalIntensity;
                finalLightColor.a=1;
                //return finalLightColor;

                float viewWorldCenterDistance = length(ViewWorldPos - i.objectWorldPos);
                float viewWorldCenterDistance01 = viewWorldCenterDistance/scale*2;
                float viewWorldAtmosDistance = viewWorldCenterDistance01-1;
                //return viewWorldAtmosDistance;

               
                
                float viewWorldAtmosDistance01 = max(0, viewWorldAtmosDistance);


                float transluencyMask = saturate(2*globalIntensity);

                
                if(existingDepthLinear <= _StarDistance){
                    viewWorldAtmosDistance01 = saturate(viewWorldAtmosDistance01*100000);
                    transluencyMask = 1-viewWorldAtmosDistance01;
                }

                transluencyMask = max(1-DepthCenterDist01,transluencyMask);


                

                float4 outColor = finalLightColor*texcolor;




                float placeHolderDistance01 = saturate(length(_WorldSpaceCameraPos - i.objectWorldPos)/_PlaceHolderDistance);
                placeHolderDistance01 = pow(placeHolderDistance01,3);


                float placeHolderEffect = lerp(DepthDifference01, 1, placeHolderDistance01);




               
                transluencyMask = 1-saturate(viewWorldAtmosDistance);

                //transluencyMask = transluencyMask*placeHolderEffect;

                //transluencyMask = max(1-DepthCenterDist01,transluencyMask);


                //return transluencyMask;


               

                transluencyMask = 1-transluencyMask;


                


                transluencyMask = (saturate(viewWorldAtmosDistance01));

                outColor.a=transluencyMask*DepthDifference01*distanceAlpha*pow(distanceAlpha,1);


                //return transluencyMask;


                //return float4(0,0,0,transluencyMask*globalIntensity);

                return outColor;
                

			}

			ENDCG

		}



        Pass
		{
			Name "SKY"
			Tags { "LightMode" = "ForwardBase" }
            Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
            #include "UnityLightingCommon.cginc"


			struct v2f
			{
				float4 pos : SV_POSITION;
                float3 normal : NORMAL;
                float3 worldPos : TEXCOORD0;
                float4 screenPosition : TEXCOORD1;
                float3 objectWorldPos : TEXCOORD2;
                float3 objectRight : TEXCOORD3;
                float3 objectUp : TEXCOORD4;
                float2 uv       : TEXCOORD5;

			};

            
            sampler2D _CameraDepthTexture;
            sampler2D _MainTex;
			float4    _MainTex_ST;
            
		    float3 _LightCenter;
            float4	  _Diffraction;
            float4 _HeightDiffraction;
            float _DepthMaxDistance;
            float _DepthMinDistance;
            float _AbsorbanceOffset;
            float _AbsorbanceExponent;
            float _RimExponent;
            float _RimOffset;
            float _StarDistance;
            float _PlaceHolderDistance;
            float _ScreenScale;
         

        

           



			v2f vert (appdata_full v)
			{
				v2f o;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.objectRight = UnityObjectToWorldNormal(float3(1.0,0.0,0.0));
                o.objectUp = UnityObjectToWorldNormal(float3(0.0,1.0,0.0));
                o.uv       = TRANSFORM_TEX(v.texcoord, _MainTex);


                float3 forward = mul((float3x3)unity_CameraToWorld, float3(0,0,1));
                float3 right = mul((float3x3)unity_CameraToWorld, float3(1,0,0));
                float3 up = mul((float3x3)unity_CameraToWorld, float3(0,1,0));

                //o.worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;

                float2 centerv = float2((o.uv.x-0.5)*2, (o.uv.y-0.5)*2);
                o.worldPos = _WorldSpaceCameraPos + forward * 1 + up*(centerv.y)*_ScreenScale + right*(centerv.x)*_ScreenScale;

                o.objectWorldPos = mul(unity_ObjectToWorld, float4(float3(0,0,0), 1)).xyz;
                o.pos = UnityWorldToClipPos(o.worldPos);
                o.screenPosition = ComputeScreenPos(o.pos);

                //o.worldPos = _WorldSpaceCameraPos + forward * 0.1 + up*(o.screenPosition.y/_ScreenScale-0.5) + right*(o.screenPosition.x/_ScreenScale-0.5);
                //o.pos = UnityWorldToClipPos(o.worldPos);
                //o.screenPosition = ComputeScreenPos(o.pos);
                
                


                

				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				
               
            
                float existingDepth01 = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosition)).r;
				float existingDepthLinear = LinearEyeDepth(existingDepth01);
				float depthDifference = existingDepthLinear - i.screenPosition.w;
				float DepthDifference01 = saturate((depthDifference-_DepthMinDistance) / (_DepthMaxDistance-_DepthMinDistance));

                
                



                float3 DepthWorldPos = i.worldPos - normalize(_WorldSpaceCameraPos - i.worldPos)*min(depthDifference,_DepthMaxDistance);
                float3 ViewWorldPos = i.worldPos - normalize(_WorldSpaceCameraPos - i.worldPos)*depthDifference;              

                float3 tmppos = normalize(DepthWorldPos-i.objectWorldPos);



		        float3 lightDirection = normalize(_LightCenter-i.worldPos);


                float globalIntensity = (dot(tmppos, lightDirection)+1)/2;
                
                float uvx = pow((acos(dot(tmppos, i.objectRight))/3.14159265359),1);
                float uvy = (dot(tmppos, i.objectUp)+1)/2;

                float4 texcolor = tex2D(_MainTex, float2(uvx, uvy));

                //return texcolor;

                globalIntensity = pow(1-globalIntensity, _AbsorbanceExponent);
                globalIntensity = min(1, globalIntensity*_AbsorbanceOffset);
                globalIntensity = 1-globalIntensity;


                float DepthCenterDist = length(DepthWorldPos-i.objectWorldPos);
                float scale = length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)); // scale x axis
                float DepthCenterDist01 = max(0,DepthCenterDist/scale*2-_RimOffset)/(1-_RimOffset);


                float DepthCamCenterDist = length(_WorldSpaceCameraPos-i.objectWorldPos);
                float DepthCamCenterDist01 = max(0,DepthCamCenterDist/scale*2-_RimOffset)/(1-_RimOffset);

                clip(2-DepthCamCenterDist01);

                float distanceAlpha = 1-(saturate((DepthCamCenterDist01-1)*5));
                

                float3 viewDirection = normalize(_WorldSpaceCameraPos - i.worldPos);
                
                float viewIntensity = saturate(dot(viewDirection, i.normal));
                viewIntensity = pow(viewIntensity, _RimExponent);
                viewIntensity = min(1, viewIntensity*_RimOffset);

                float4 heightDiffraction = _HeightDiffraction*(DepthCenterDist01*1)*_HeightDiffraction.w;
                heightDiffraction.a = 0;
                float4 heightDiffractionColor = 1-heightDiffraction;
                //return heightDiffractionColor;

                float4 lightDiffraction = _Diffraction*(1-globalIntensity)*_Diffraction.w;
                lightDiffraction.a = 0;
                float4 lightDiffractionColor = 1-lightDiffraction;
                //return lightDiffractionColor;

                //return lerp(lightDiffractionColor, heightDiffractionColor, DepthCenterDist01);

                float4 finalLightColor = lerp(lightDiffractionColor, heightDiffractionColor, DepthCenterDist01)*globalIntensity;
                finalLightColor.a=1;
                //return finalLightColor;

                float viewWorldCenterDistance = length(ViewWorldPos - i.objectWorldPos);
                float viewWorldCenterDistance01 = viewWorldCenterDistance/scale*2;
                float viewWorldAtmosDistance = viewWorldCenterDistance01-1;
                //return viewWorldAtmosDistance;

               
                
                float viewWorldAtmosDistance01 = max(0, viewWorldAtmosDistance);


                float transluencyMask = saturate(2*globalIntensity);
                
                if(existingDepthLinear <= _StarDistance){
                    viewWorldAtmosDistance01 = saturate(viewWorldAtmosDistance01*100000);
                    transluencyMask = 1-viewWorldAtmosDistance01;
                }

                transluencyMask = max(1-DepthCenterDist01,transluencyMask);


                

                float4 outColor = finalLightColor*texcolor;




                float placeHolderDistance01 = saturate(length(_WorldSpaceCameraPos - i.objectWorldPos)/_PlaceHolderDistance);
                placeHolderDistance01 = pow(placeHolderDistance01,3);


                float placeHolderEffect = lerp(DepthDifference01, 1, placeHolderDistance01);




               
                transluencyMask = 1-saturate(viewWorldAtmosDistance);

                //transluencyMask = transluencyMask*placeHolderEffect;

                transluencyMask = max(1-DepthCenterDist01,transluencyMask);


                //return transluencyMask;
                //return outColor*transluencyMask;


                //outColor.a = 0.8*placeHolderEffect;

                //outColor.rgb *= texcolor;


                transluencyMask = transluencyMask;
                outColor.a *= transluencyMask*DepthDifference01*pow(distanceAlpha,1);
                return outColor;
                

			}

			ENDCG

		}

		
	}
}