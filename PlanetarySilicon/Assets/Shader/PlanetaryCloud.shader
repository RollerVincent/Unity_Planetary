Shader "Custom/PlanetaryCloud"
{
    Properties {
        _MainTex    ("Texture",       2D        ) = "white" {}
        _MainTex2    ("Texture2",       2D        ) = "white" {}
        _Center("Center", Vector) = (0,0,0)
        _LightCenter("LightCenter", Vector) = (0,0,0)
        _Diffraction("Diffraction", Vector) = (0,0,0,0)
        _HeightDiffraction("HeightDiffraction", Vector) = (0,0,0,0)
        _DepthMaxDistance("DepthMaxDistance", Range(0,500)) = 1
        _DepthMinDistance("DepthMinDistance", Range(0,500)) = 1
        _AbsorbanceExponent("AbsorbanceExponent", Range(0,20)) = 1
        _AbsorbanceOffset("AbsorbanceOffset", Range(0,5)) = 1
        _RimExponent("RimExponent", Range(0,20)) = 1
        _RimOffset("RimOffset", Range(0,1)) = 1
        _Detail("Detail", Range(0,10)) = 1
        _Detail2("Detail2", Range(0,10)) = 1
        _Speed("Speed", Range(0,10)) = 1
        _CutOff("CutOff", Range(0,1)) = 1
        _Impact("Impact", Range(0,100)) = 1
        _Bloom("Bloom", Range(1,10)) = 1
        _Ambient("Ambient", Range(0,1)) = 1
		_AmbientSaturation("Ambient Saturation", Range(0,1)) = 1
		_Alpha("Alpha", Range(0,10)) = 1
		_Segmentation("Segmentation", Range(0,1)) = 1
		_Height("Height", Range(0,1)) = 1
        _Color("Color", Color) = (0,0,0,0)

	}

	SubShader
	{

        
		LOD 200
		




		

        	Pass
		{
			Name "BASE"
			Tags { "LightMode" = "ForwardBase" }
            Zwrite On
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

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
                SHADOW_COORDS(0)
                float3 worldPos : TEXCOORD1;
                float4 screenPosition : TEXCOORD2;
                float noise : TEXCOORD3;
			};

            
            sampler2D _CameraDepthTexture;
            sampler2D _MainTex;
			float4    _MainTex_ST;

            sampler2D _MainTex2;
			float4    _MainTex2_ST;
            
            float3    _Center;
		    float3 _LightCenter;
            float4	  _Diffraction;
            float4 _HeightDiffraction;
            float _DepthMaxDistance;
            float _DepthMinDistance;
            float _AbsorbanceOffset;
            float _AbsorbanceExponent;
            float _RimExponent;
            float _RimOffset;
            float _Detail;
            float _Detail2;
            float _Speed;
            float _CutOff;
            float _Impact;
            float _AmbientSaturation;
            float _Ambient;
            float4 _Color;
            float _Alpha;
            float _Segmentation;
            float _Height;
            float _Bloom;
         

        
            float3 GetFaceNormal(float3 position) {
				float3 dx = ddx(position);
				float3 dy = ddy(position);
				return normalize(cross(dy, dx));
			}

            #define STEPS 128
            #define STEPSIZE 3
           
            float sphereDist(float3 pos, float3 center, float radius){
                return length(pos-center)-radius;
            }

            float3 raymarchHit(float3 worldPos, float3 direction, float scale){

                int fu = 0;
                for(int i=0; i<STEPS; i++){


                    if(sphereDist(worldPos, _Center, scale) > 0){
                        return worldPos;
                    }

                    worldPos += direction*STEPSIZE;
                }

                return 0;
            }




			v2f vert (appdata_full v)
			{
				v2f o;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                

                float xy = tex2Dlod(_MainTex, float4(v.vertex.x/_Detail+_Time.y*_Speed, v.vertex.y/_Detail+_Time.y*_Speed,0,0));
				float yz = tex2Dlod(_MainTex, float4(v.vertex.y/_Detail+_Time.y*_Speed, v.vertex.z/_Detail+_Time.y*_Speed,0,0));
				float xz = tex2Dlod(_MainTex, float4(v.vertex.x/_Detail+_Time.y*_Speed, v.vertex.z/_Detail+_Time.y*_Speed,0,0));

                o.noise = (xz+yz+xy)/3;

                xy = tex2Dlod(_MainTex2, float4(v.vertex.x/_Detail2+_Time.y*_Speed, v.vertex.y/_Detail2+_Time.y*_Speed,0,0));
				yz = tex2Dlod(_MainTex2, float4(v.vertex.y/_Detail2+_Time.y*_Speed, v.vertex.z/_Detail2+_Time.y*_Speed,0,0));
				xz = tex2Dlod(_MainTex2, float4(v.vertex.x/_Detail2+_Time.y*_Speed, v.vertex.z/_Detail2+_Time.y*_Speed,0,0));

                o.noise += (xz+yz+xy)/3;

                o.noise /= 2;

                
                float scale = length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x));

                o.worldPos = _Center + normalize(o.worldPos-_Center) * (scale*_Height + saturate(o.noise-_CutOff) * _Impact);
				o.pos = UnityWorldToClipPos(o.worldPos);
                o.screenPosition = ComputeScreenPos(o.pos);

                TRANSFER_SHADOW(o);



				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{

                
                clip(i.noise-_CutOff);
				
				UNITY_LIGHT_ATTENUATION(atten, i, 0)

                

                float scale = length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x));
                float playerAtmosDistance = length(_WorldSpaceCameraPos-_Center)-scale;


                float3 viewDir = normalize(i.worldPos - _WorldSpaceCameraPos);
                float3 atmosSphereHit;

                if(playerAtmosDistance<0){
                    atmosSphereHit = _WorldSpaceCameraPos;
                }else{
                    atmosSphereHit = raymarchHit(i.worldPos, -viewDir, scale);
                }



                float existingDepth01 = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosition)).r;
				float existingDepthLinear = LinearEyeDepth(existingDepth01);
				float depthDifference = length(i.worldPos-atmosSphereHit);
				float DepthDifference01 = saturate((depthDifference-_DepthMinDistance) / (_DepthMaxDistance-_DepthMinDistance));


                
                
				
                float3 DepthWorldPos = atmosSphereHit + viewDir*min(depthDifference,_DepthMaxDistance);

                
                
                

                //////////

                fixed4 surfaceColor = _Color;

                float3 finalNormal  = GetFaceNormal(i.worldPos);

				float3 lightDirection = normalize(_LightCenter-i.worldPos);

				float globalIntensity = (dot(normalize(DepthWorldPos-_Center), lightDirection)+1)/2;
                globalIntensity = pow(1-globalIntensity, _AbsorbanceExponent);
                globalIntensity = min(1, globalIntensity*_AbsorbanceOffset);
                globalIntensity = 1-globalIntensity;
				



                float DepthCenterDist = length(DepthWorldPos-_Center);
                
                float DepthCenterDist01 = max(0,DepthCenterDist/scale*1-_RimOffset)/(1-_RimOffset);

                float4 heightDiffraction = _HeightDiffraction*(DepthCenterDist01)*_HeightDiffraction.w;
                heightDiffraction.a = 0;
                float4 heightDiffractionColor = 1-heightDiffraction;

                float4 lightDiffraction = _Diffraction*(1-globalIntensity)*_Diffraction.w;
                lightDiffraction.a = 0;
                float4 lightColor = 1-lightDiffraction;
                float4 lightDiffractionColor = 1-lightDiffraction;
                
                float4 finalLightColor = lerp(lightDiffractionColor, heightDiffractionColor, DepthCenterDist01)*globalIntensity;
                finalLightColor.a=1;




                float4 ambientColor = lerp(1,lightColor,_AmbientSaturation);
				ambientColor *= globalIntensity;
				//return ambientColor;

                
                float lightIntensity = atten*max(0.0, dot(finalNormal, lightDirection));
				float ambientIntensity = _Ambient;

                lightIntensity*=_Bloom*globalIntensity;
				
                
                fixed4 color = 1;
                color.rgb = (lightColor * (1-ambientIntensity) * lightIntensity) + (ambientColor * (ambientIntensity));
                color.rgb *= surfaceColor;
				
                color.a = (i.noise-_CutOff)*(1/(1-_CutOff));
                color.a = saturate(color.a*(1+_Alpha));
                color.a = color.a  - color.a%_Segmentation;


				return lerp(saturate(color),finalLightColor, DepthDifference01);








                //////////





                


            
                

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
				float noise : TEXCOORD3;
				V2F_SHADOW_CASTER;
			};
			

			sampler2D _CameraDepthTexture;
            sampler2D _MainTex;
			float4    _MainTex_ST;
            
            float3    _Center;
		    float3 _LightCenter;
            float4	  _Diffraction;
            float4 _HeightDiffraction;
            float _DepthMaxDistance;
            float _DepthMinDistance;
            float _AbsorbanceOffset;
            float _AbsorbanceExponent;
            float _RimExponent;
            float _RimOffset;
            float _Detail;
            float _Speed;
            float _CutOff;
            float _Impact;
            float _AmbientSaturation;
            float _Ambient;
            float4 _Color;
            float _Alpha;
            float _Segmentation;

			v2f vert(appdata_full v)
			{
				v2f o;

                
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
                
                
                float xy = tex2Dlod(_MainTex, float4(v.vertex.x/_Detail+_Time.y*_Speed, v.vertex.y/_Detail+_Time.y*_Speed,0,0));
				float yz = tex2Dlod(_MainTex, float4(v.vertex.y/_Detail+_Time.y*_Speed, v.vertex.z/_Detail+_Time.y*_Speed,0,0));
				float xz = tex2Dlod(_MainTex, float4(v.vertex.x/_Detail+_Time.y*_Speed, v.vertex.z/_Detail+_Time.y*_Speed,0,0));

                o.noise = (xz+yz+xy)/3;

                worldPos = worldPos + normalize(worldPos-_Center) * (o.noise-_CutOff) * _Impact;

				

				v.vertex = mul(unity_WorldToObject, float4(worldPos, 1));

				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				return o;
			}

			float4 frag(v2f i) : COLOR
			{
                clip(-1);
                
				clip(i.noise-_CutOff);

				SHADOW_CASTER_FRAGMENT(i)
			}

			ENDCG
		}


     
		
	}
}