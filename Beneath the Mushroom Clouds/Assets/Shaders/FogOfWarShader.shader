//Shader mixes textures of currently visible areas and discovered areas and then assigns appropriate alpha gradient to them
//Based on: 
Shader "Hidden/FogOfWarShader"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _SecondaryTex("Secondary Texture", 2D) = "white" {}
    }
        SubShader
    {
        Tags
        {
            "Queue" = "Transparent+1"
        }
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            //Main texture is a black canvas with discovered areas painted in red (1,0,0,1)
            sampler2D _MainTex;
            //Secondary texture is a black canvas with only the current visible area in green (0,1,0,1)
            sampler2D _SecondaryTex;

            fixed4 frag(v2f i) : SV_Target
            {
                //Overlay the textures -> causes discovered areas to be red (1,0,0,1) and areas currently seen to be yellow (1,1,0,1)
                fixed4 col = tex2D(_MainTex, i.uv) + tex2D(_SecondaryTex, i.uv);

                //Use this formula to calculate alpha of the final texture that will be displayed on the FOW Raw Image
                //Examples:
                //Undiscovered pixel (0,0,0,1) -> col.a = 2 - 0 - 0 = 2 (alpha above 1 is just considered 1) -> fully black
                //Discovered pixel (1,0,0,1) -> col.a = 2 - 1.5 - 0 = 0.5 -> faded black (gray areas)
                //Visible pixel (1,1,0,1) -> col.a = 2 - 1.5 - 0.5 = 0 -> no black (fully visible areas)
                col.a = 2.0f - col.r * 1.5f - col.g * 0.5f;
                //The result is just different alpha gradients
                return fixed4(0,0,0,col.a);
            }
            ENDCG
        }

    }
}
