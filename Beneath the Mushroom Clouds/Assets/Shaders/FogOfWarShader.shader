//Shader mixes textures of currently visible areas and discovered areas and then assigns appropriate alpha gradient to them
//Based on: 
Shader "Hidden/FogOfWarShader"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _SecondaryTex("Secondary Texture", 2D) = "white" {}
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

    v2f vert(appdata v)
    {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = v.uv;
        UNITY_TRANSFER_FOG(o, o.vertex);
        return o;
    }
    ENDCG
        SubShader
    {
        Tags
        {
            "Queue" = "Transparent+1"
            "RenderType" = "Opaque" 
        }

        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            

            

            //Main texture is a black canvas with discovered areas painted in red (1,0,0,1)
            sampler2D _MainTex;
            //Secondary texture is a black canvas with only the current visible area in green (0,1,0,1)
            sampler2D _SecondaryTex;
            
            float4 _MainTex_TexelSize;
            float4 _SecondaryTex_TexelSize;


            fixed4 frag(v2f i) : SV_Target
            {

                

                fixed4 blurredMainTex;
                fixed4 blurredSecondaryTex;
                


                //Currently averaged pixel
                float3 originalSampleMainTex = tex2D(_MainTex, i.uv);
                float3 originalSampleSecondaryTex = tex2D(_SecondaryTex, i.uv);

                //4 pixels both vertically and horizontally from the current pixel

                fixed4 blurSampleMainTex;
                fixed4 blurSampleSecondaryTex;

                for(int j = 1; j < 5; j++){
                    blurSampleMainTex += tex2D(_MainTex, i.uv + _MainTex_TexelSize.xy * float2(-j, -j)) +
                    tex2D(_MainTex, i.uv + _MainTex_TexelSize.xy * float2(-j, j)) +
                    tex2D(_MainTex, i.uv + _MainTex_TexelSize.xy * float2(j, -j)) +
                    tex2D(_MainTex, i.uv + _MainTex_TexelSize.xy * float2(j, j));

                    blurSampleMainTex += tex2D(_MainTex, i.uv + _MainTex_TexelSize.xy * float2(0, -j)) +
                    tex2D(_MainTex, i.uv + _MainTex_TexelSize.xy * float2(0, j)) +
                    tex2D(_MainTex, i.uv + _MainTex_TexelSize.xy * float2(j, 0)) +
                    tex2D(_MainTex, i.uv + _MainTex_TexelSize.xy * float2(-j, 0));
                    

                    blurSampleSecondaryTex += tex2D(_SecondaryTex, i.uv + _SecondaryTex_TexelSize.xy * float2(-j, -j)) +
                    tex2D(_SecondaryTex, i.uv + _SecondaryTex_TexelSize.xy * float2(-j, j)) +
                    tex2D(_SecondaryTex, i.uv + _SecondaryTex_TexelSize.xy * float2(j, -j)) +
                    tex2D(_SecondaryTex, i.uv + _SecondaryTex_TexelSize.xy * float2(j, j));

                    blurSampleSecondaryTex += tex2D(_SecondaryTex, i.uv + _SecondaryTex_TexelSize.xy * float2(0, -j)) +
                    tex2D(_SecondaryTex, i.uv + _SecondaryTex_TexelSize.xy * float2(0, j)) +
                    tex2D(_SecondaryTex, i.uv + _SecondaryTex_TexelSize.xy * float2(j, 0)) +
                    tex2D(_SecondaryTex, i.uv + _SecondaryTex_TexelSize.xy * float2(-j, 0));


                }

                //Since we're adding values of 32 pixels we need to devide by 32 to avoid high brightness
                blurSampleMainTex /=32;
                blurSampleSecondaryTex /=32;

                
                float brightness = originalSampleMainTex.g; 
                blurredMainTex = fixed4((brightness * originalSampleMainTex + (1-brightness) * blurSampleMainTex), 1);

                brightness = originalSampleSecondaryTex.r;
                blurredSecondaryTex = fixed4((brightness * originalSampleSecondaryTex + (1-brightness) * blurSampleSecondaryTex), 1);

                //Overlay the textures -> causes discovered areas to be red (1,0,0,1) and areas currently seen to be yellow (1,1,0,1)
                fixed4 col = blurredMainTex + blurredSecondaryTex;

                //Use this formula to calculate alpha of the final texture that will be displayed on the FOW Raw Image
                //Examples:
                //Undiscovered pixel (0,0,0,1) -> col.a = 2 - 0 - 0 = 2 (alpha above 1 is just considered 1) -> fully black
                //Discovered pixel (1,0,0,1) -> col.a = 2 - 1.5 - 0 = 0.5 -> faded black (gray areas)
                //Visible pixel (1,1,0,1) -> col.a = 2 - 1.5 - 0.5 = 0 -> no black (fully visible areas)
                col.a = 1.0f - col.r * 0.5f - col.g * 0.5f;
                //The result is just different alpha gradients
                return fixed4(0,0,0,col.a);
            }
            ENDCG
        }

    }
}
