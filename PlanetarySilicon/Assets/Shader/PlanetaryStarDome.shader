// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/PlanetaryStarDome"
{
    Properties {
        
        _MaxDustDepthDistance("MaxDustDepthDistance", Range(0,10)) = 1
        _StarSize("StarSize", Range(0,10)) = 5
        _BloomDistance("BloomDistance", Range(0,8)) = 1
        _BloomAmount("BloomAmount", Range(0,10)) = 0.1
        _BloomAlpha("BloomAlpha", Range(0,1)) = 0.1
        _Saturation("Saturation", Range(0,1)) = 0.1
        _StarMinScale("StarMinScale", Range(0,1)) = 0.1


	}

	SubShader
	{
        Tags
        { 
            "RenderType" = "Transparent" 
            "IgnoreProjector" = "True"
            "Queue" = "Transparent" 
        }

        

		

        Pass
		{
			Name "BLOOM"

            Cull Back
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			


			struct v2f
			{
				float4 pos : SV_POSITION;
                float3 normal : NORMAL;
                float3 worldPos : TEXCOORD1;
                float4 screenPosition : TEXCOORD2;
                float2 uv : TEXCOORD3;
                float4 color: COLOR;
                float bloom : TEXCOORD4;
			};

            
            
            float _StarSize;
            float _VisualizationDistance;
            float3 _FocusStarPosition;
            float _VisualizationScale;
            float _MaxDustDepthDistance;
            float _BloomDistance;
            float _BloomAmount;
            float _BloomAlpha;
            float _Saturation;
            float _FocusStarOffset;
         

        
            #define GALAXY_SCALE 100000000


            float4 correctBrightness(float4 color, float brightness){
                color.rgb += 1-max(color.g, max(color.r, color.b));
                color.rgb *= brightness;
                return color;
            }

            float4 increaseSaturation(float4 color, float factor){
                return float4(pow(color.r,factor), pow(color.g,factor), pow(color.b,factor), color.a);
            }


			v2f vert (appdata_full v)
			{
				v2f o;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.color = v.color;
                o.uv  = v.texcoord;

                float starSize = _StarSize;


                float3 objectCenter = mul(unity_ObjectToWorld, float4(0.0,0.0,0.0,1.0));
                float visScale = pow(10,-_VisualizationScale);


                float3 camWorldPos = _WorldSpaceCameraPos/visScale;

                o.worldPos = v.vertex + objectCenter*GALAXY_SCALE;
                o.worldPos -= (_FocusStarPosition+_FocusStarOffset*10);
                

                float depthDistance = length(o.worldPos - camWorldPos)/GALAXY_SCALE/_BloomDistance;
                float depthDistance01 = saturate(depthDistance);
                o.bloom=depthDistance01;

                starSize = starSize + (o.bloom*_BloomAmount*100*_StarSize);


                float3 camdir = normalize(o.worldPos - camWorldPos);
                starSize *= GALAXY_SCALE * 0.001f;
                float3 right = normalize(cross(camdir, UNITY_MATRIX_IT_MV[1].xyz)) * starSize;
				float3 up = normalize(cross(camdir, right)) * starSize;
                
                float focusMask = saturate(length(o.worldPos)/GALAXY_SCALE*1000); //  FOCUSDISTANCE ?
                
                o.worldPos = o.worldPos + up*(o.uv.y-0.5)*focusMask + right*(o.uv.x-0.5)*focusMask;



                o.worldPos*=visScale;

                float camDistance = length(o.worldPos - _WorldSpaceCameraPos);
                camdir = normalize(o.worldPos - _WorldSpaceCameraPos);
                o.worldPos = _WorldSpaceCameraPos + camdir * max(1, _VisualizationDistance * visScale);


				o.pos = UnityWorldToClipPos(o.worldPos);
                o.screenPosition = ComputeScreenPos(o.pos);
				return o;
			}

            

			fixed4 frag (v2f i) : SV_Target
			{

                
                float uvDistance = length(i.uv-0.5)*2;
                clip(1-uvDistance);
                //return 1;

                

                float density = i.color.a;
                //return density;

                i.color.a = 1;
                //return i.color;

                float4 starColor = correctBrightness(i.color, 1);
                //return starColor;

                starColor = increaseSaturation(starColor, _Saturation*20);

                
                
                float rim = 1-pow(uvDistance,1.0/3);
                starColor.a = rim*_BloomAlpha;


                
                

                return starColor;


           




                


            
                

			}

			ENDCG

		}






        Pass
		{
			Name "STARS"

            Cull Back
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha
            

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			


			struct v2f
			{
				float4 pos : SV_POSITION;
                float3 normal : NORMAL;
                float3 worldPos : TEXCOORD1;
                float4 screenPosition : TEXCOORD2;
                float2 uv : TEXCOORD3;
                float4 color: COLOR;
			};

            
            
            float _StarSize;
            float _VisualizationDistance;
            float3 _FocusStarPosition;
            float _VisualizationScale;
            float _MaxDustDepthDistance;
            float _BloomDistance;
            float _BloomAmount;
            float _BloomAlpha;
            float _Saturation;
            float _StarMinScale;
            float _FocusStarOffset;
         

        
            #define GALAXY_SCALE 100000000


            float4 correctBrightness(float4 color, float brightness){
                color.rgb += 1-max(color.g, max(color.r, color.b));
                color.rgb *= brightness;
                return color;
            }

            float4 increaseSaturation(float4 color, float factor){
                return float4(pow(color.r,factor), pow(color.g,factor), pow(color.b,factor), color.a);
            }


			v2f vert (appdata_full v)
			{
				v2f o;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.color = v.color;
                o.uv  = v.texcoord;

                float starSize = _StarSize;


                float3 objectCenter = mul(unity_ObjectToWorld, float4(0.0,0.0,0.0,1.0));
                float visScale = pow(10,-_VisualizationScale);


                float3 camWorldPos = _WorldSpaceCameraPos/visScale;

                o.worldPos = v.vertex + objectCenter*GALAXY_SCALE;
                o.worldPos -= (_FocusStarPosition+_FocusStarOffset*10);
                

                float depthDistance = length(o.worldPos - camWorldPos)/GALAXY_SCALE/_BloomDistance;
                float depthDistance01 = saturate(depthDistance);
                //o.bloom=depthDistance01;


                float starScaleDepth = length(o.worldPos - camWorldPos)/GALAXY_SCALE/_StarMinScale;
                if(starScaleDepth>1){
                    starSize*=starScaleDepth;
                }


                float3 camdir = normalize(o.worldPos - camWorldPos);
                starSize *= GALAXY_SCALE * 0.001f;
                float3 right = normalize(cross(camdir, UNITY_MATRIX_IT_MV[1].xyz)) * starSize;
				float3 up = normalize(cross(camdir, right)) * starSize;

                float focusMask = saturate(length(o.worldPos)/GALAXY_SCALE*1000); //  FOCUSDISTANCE ?
                
 
                o.worldPos = o.worldPos + up*(o.uv.y-0.5)*focusMask + right*(o.uv.x-0.5)*focusMask;


                

                o.worldPos*=visScale;

                float camDistance = length(o.worldPos - _WorldSpaceCameraPos);
                camdir = normalize(o.worldPos - _WorldSpaceCameraPos);
                o.worldPos = _WorldSpaceCameraPos + camdir * max(1, _VisualizationDistance * visScale);


				o.pos = UnityWorldToClipPos(o.worldPos);
                o.screenPosition = ComputeScreenPos(o.pos);
				return o;
			}

            

			fixed4 frag (v2f i) : SV_Target
			{

                
                float uvDistance = length(i.uv-0.5)*2;
                clip(1-uvDistance);
                //return 1;

               


                

                float density = i.color.a;
                //return density;

                i.color.a = 1;
                //return i.color;

                float4 starColor = correctBrightness(i.color, 1);
                //return starColor;

                starColor = increaseSaturation(starColor, _Saturation*20*pow(uvDistance,3.0));
                
                

                return starColor;


           




                


            
                

			}

			ENDCG

		}
       

		






     
		
	}
}