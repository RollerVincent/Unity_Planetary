Shader "Custom/NavBall"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DarkColor("DarkColor", Color) = (0,0,0,0)
        _BrightColor("BrightColor", Color) = (0,0,0,0)

    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="true" "RenderType"="Transparent"}
        ZWrite Off

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float3 worldDir : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _DarkColor;
            float4 _BrightColor;
            float3 _NavSunDir;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldDir = normalize(UnityObjectToWorldNormal(v.vertex));

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 camUp = UNITY_MATRIX_V[1].xyz;
                float upIntensity = 1-saturate(dot(camUp, i.worldDir));
                float sunIntensity = saturate(dot(_NavSunDir, i.worldDir));

                float4 color = lerp(_BrightColor, _DarkColor, 1-sunIntensity);
                

                color.a = 1-upIntensity;

                


               
                return color;
                return float4(i.worldDir, 1);
                return float4(i.normal, 1);

                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                return col;
            }
            ENDCG
        }
    }
}
