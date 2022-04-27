Shader "Custom/PlanetarySky"
{
    Properties {
        _MainTex    ("Texture",       2D        ) = "white" {}
		_BlendNormal("Blend Normals", Range(0,1)) = 0.5
		_Ambient("Ambient", Range(0,1)) = 1
		_AmbientSaturation("Ambient Saturation", Range(0,1)) = 1
		_FogAmount("Fog Amount", Range(0,10)) = 1
		_FaceNoise("Face Noise", Range(0,1)) = 0.1
        _Center("Center", Vector) = (0,0,0)
        _LightCenter("LightCenter", Vector) = (0,0,0)
		
        _Diffraction("Diffraction", Vector) = (0,0,0,0)
        _Color("Color", Color) = (0,0,0,0)
        _Radius("Radius", Range(0,1000)) = 1
        _Density("Density", Range(0,10)) = 1
        _Opacity("Opacity", Range(0,1)) = 1
        _DepthMaxDistance("DepthMaxDistance", Range(0,500)) = 1


       

	}
	SubShader
	{
        Tags {"Queue"="Transparent" "IgnoreProjector"="true" "RenderType"="Transparent"}
        ZWrite Off
		//Blend SrcAlpha One
		Blend SrcAlpha DstAlpha

        Cull Front
		

		// Forward rendering base (main directional light) pass.
		Pass
		{
			Name "FORWARDBASE"
			Tags { "LightMode" = "ForwardBase" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// Want regular shader variants for ForwardBase pass, but don't care about
			// lightmaps, dynamic GI etc. Just shadows/no-shadows
			#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
            #include "UnityLightingCommon.cginc"


			struct v2f
			{
				float4 pos : SV_POSITION;
                float3 normal : NORMAL;
				SHADOW_COORDS(0) // shadow parameters to pass from vertex
                float4 worldPos : TEXCOORD1;
                float4 objectPos : TEXCOORD2;
                float2 uv : TEXCOORD4;
                float4 screenPosition : TEXCOORD3;


                
			};

            float3 GetFaceNormal(float3 position) {
				float3 dx = ddx(position);
				float3 dy = ddy(position);
				return normalize(cross(dy, dx));
			}

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
			float4    _MainTex_ST;
            float _BlendNormal;
            float _Ambient;
            float _AmbientSaturation;
            float _FaceNoise;
            float _FogAmount;
            float3    _Center;
		    float3 _LightCenter;
            float4	  _Diffraction;
            float4	  _Color;
            float _Radius;
            float _Density;
            float _DepthMaxDistance;
            float _Opacity;
         

        

            



			v2f vert (appdata_full v)
			{
				v2f o;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.uv  = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv.y = 1-o.uv.y;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);

                

				
				//o.worldPos = float4((_Center + (o.uv.x-0.5)*dx*_Radius + (o.uv.y-0.5)*dy*_Radius), 0);
             
            //    o.worldPos += float4((o.uv.x*dx*_Radius), 0);
            //    o.worldPos += float4((o.uv.y*dy*_Radius), 0);

                
                //float3 wp = _Center + (o.uv.x)*dx*_Radius + (o.uv.y)*dy*_Radius;
                

                o.pos = UnityWorldToClipPos(o.worldPos);
                o.screenPosition = ComputeScreenPos(o.pos);
                
                o.objectPos = v.vertex;
                float3 planetPos = normalize(o.worldPos-_Center);
                
				o.worldPos.w = (dot(planetPos, _WorldSpaceLightPos0.xyz) + 1)/2;
				TRANSFER_SHADOW(o); // pass shadow coordinates to pixel shader
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				
				UNITY_LIGHT_ATTENUATION(atten, i, 0)

            



              

                //float3 faceNormal  = GetFaceNormal(i.worldPos);
				//float3 finalNormal = lerp(i.normal, faceNormal, _BlendNormal);

		        float3 lightDirection = normalize(_LightCenter-i.worldPos);

                //float globalIntensity = (dot(normalize(i.worldPos-_Center), lightDirection)+1)/2;
                float globalIntensity = 1 - max(0,dot(normalize(i.worldPos-_Center), lightDirection)*-1);

                float4 c = 1;
                c.rgb = globalIntensity;

                globalIntensity = pow(globalIntensity,3);


                
                float3 viewDirection = normalize(_WorldSpaceCameraPos - i.worldPos);
                
                float viewIntensity = saturate(dot(-viewDirection, i.normal));


                float4 lightDiffraction = _Diffraction * (1-globalIntensity);

                float4 lightColor = _LightColor0*_Color - lightDiffraction;
                float4 ambientColor = lerp(1,lightColor,_AmbientSaturation);

                

                
                

                

                float lightIntensity = atten * max(0.0, dot(i.normal, lightDirection));

                float ambientIntensity = max(0,globalIntensity-_Ambient);

                float4 fogColor = lightColor;

            

                fogColor.rgb *= globalIntensity;
                //fogColor.rgb *= ambientIntensity;

                //fogColor.a = 0.1;


                //fogColor.a *= min(1,viewIntensity*2);

                fogColor.a=1;
                //fogColor.a = max(0.9,fogColor.a);

                return fogColor;

			}

			ENDCG

		}

		
		

		
	}
}