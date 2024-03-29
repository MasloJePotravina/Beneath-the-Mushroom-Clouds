//Shader used to blur fog of war
//Based on: https://www.youtube.com/watch?v=-FrdBaY2wIA

Shader "BoxBlurShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
CGINCLUDE
#include "UnityCG.cginc"

struct appdata
    {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
    };

    struct v2f
    {
        float2 uv : TEXCOORD0;
        UNITY_FOG_COORDS(1)
        float4 vertex : SV_POSITION;
    };

    sampler2D _MainTex;
    float4 _MainTex_ST;

    v2f vert(appdata v)
    {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        UNITY_TRANSFER_FOG(o, o.vertex);
        return o;
    }
    ENDCG
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 _MainTex_TexelSize;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                //Currently averaged pixel
                float3 originalSample = tex2D(_MainTex, i.uv);
                //Four diagonal pixels around the averaged pixel
                float3 blurSample =
                    tex2D(_MainTex, i.uv + _MainTex_TexelSize.xy * float2(-1, -1)) +
                    tex2D(_MainTex, i.uv + _MainTex_TexelSize.xy * float2(-1, 1)) +
                    tex2D(_MainTex, i.uv + _MainTex_TexelSize.xy * float2(1, -1)) +
                    tex2D(_MainTex, i.uv + _MainTex_TexelSize.xy * float2(1, 1));

                //Since we're adding values of four pixels we need to devide by four to avoid high brightness
                blurSample = blurSample * 0.25;

                //Magic 1.25 fixes fog of war darkness creeping back and covering map 
                //(numbers lower than 1 cause the problem while numbers over 1.3 cause trippy effects (use 100 for ultimate weirdness))
                float brightness = 1.25*blurSample.r; 
                return fixed4((brightness * originalSample + (1-brightness) * blurSample), 1);
                
            }
            ENDCG
        }
    }
}
