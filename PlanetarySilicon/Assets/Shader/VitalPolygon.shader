Shader "Custom/VitalPolygon"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _GoodColor ("GoodColor", Color) = (1,1,1,1)
        _BadColor ("BadColor", Color) = (1,1,1,1)
        _Segments ("Segments", Range(0,0.5)) = 0


        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float vlength : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            fixed4 _GoodColor;
            fixed4 _BadColor;
            float _Segments;

            float4 correctBrightness(float4 c){
				float m = max(max(c.r,c.g),c.b);
				m = 1-m;
				return float4(c.r+m, c.g+m, c.b+m, 1);
			}

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.vlength = length(v.vertex.xy);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                OUT.color = v.color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {

                float segments = _Segments;
                //float4 finalcolor = IN.texcoord.x*IN.color;
                float4 finalcolor = IN.color;


                finalcolor.a=1;
                //finalcolor = correctBrightness(finalcolor);

                //finalcolor = float4(finalcolor.r-finalcolor.r%segments+segments, finalcolor.g-finalcolor.g%segments+segments, finalcolor.b-finalcolor.b%segments+segments, 1);

                float centerfac = IN.texcoord.x-IN.texcoord.x%segments+segments;

                return lerp(_Color,finalcolor,centerfac);

                return finalcolor;

                /*half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                

                float3 diff = IN.vertex - UnityObjectToClipPos(float3(0,0,0));


                float vitalfactor = (IN.texcoord.x-0.4)/(1-0.4);
                vitalfactor = (vitalfactor-0.5f)*2;
                vitalfactor = sqrt(vitalfactor*vitalfactor);

                float4 vitalcolor = lerp(_BadColor, _GoodColor, 1-vitalfactor);


                float segments = 0.1;

                float4 c = vitalcolor; 


                float4 finalcolor = c;//correctBrightness(c);//float4(c.r-c.r%0.1+0.1, c.g-c.g%0.1+0.1, c.b-c.b%0.1+0.1, 1);
                finalcolor = float4(finalcolor.r-finalcolor.r%segments+segments, finalcolor.g-finalcolor.g%segments+segments, finalcolor.b-finalcolor.b%segments+segments, 1);


                //finalcolor = lerp(_Color, finalcolor, saturate(pow(IN.texcoord.y, 0.5) + 0));


                return finalcolor;

                return color;*/
            }
        ENDCG
        }
    }
}