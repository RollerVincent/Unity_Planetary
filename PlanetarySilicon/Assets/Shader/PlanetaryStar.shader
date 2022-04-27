// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/PlanetaryStar"
{
    Properties {
        
        _StarSize("StarSize", Range(0,1)) = 5
        _Saturation("Saturation", Range(0,1)) = 5


	}

	SubShader
	{
        Tags
        { 
        }

        

		

        Pass
		{
			Name "Main"

                        

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
			};

            
            
            float _StarSize;
            float _VisualizationDistance;
            float _VisualizationScale;
            float4 _StarColor;
            float _Saturation;
            float3 _FocusStarPosition;
            float3 _FocusStarOffset;

         

        
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
                o.uv  = v.texcoord;

                float starSize = _StarSize*GALAXY_SCALE * 0.001f;

                float visScale = pow(10,-_VisualizationScale);

                float3 scaledVertice = ((v.vertex*starSize - _FocusStarOffset*10))*visScale;


                o.worldPos = scaledVertice;//mul(unity_ObjectToWorld, float4(scaledVertice, 1.0)).xyz;

              

				o.pos = UnityWorldToClipPos(o.worldPos);
                o.screenPosition = ComputeScreenPos(o.pos);
				return o;
			}

            

			fixed4 frag (v2f i) : SV_Target
			{
                float4 color = 1;
                color.rgb = _StarColor.rgb;
                color = correctBrightness(color, 1);
                color = increaseSaturation(color, _Saturation*40);
                
                return color;


                
                

                

           




                


            
                

			}

			ENDCG

		}






		






     
		
	}
}