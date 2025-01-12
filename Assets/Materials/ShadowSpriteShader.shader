Shader "Custom/VisibleShadowShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {} // Sprite texture
        _ShadowColor ("Shadow Color", Color) = (0, 0, 0, 0.3) // Default shadow color
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Properties
            sampler2D _MainTex;
            float4 _ShadowColor;

            // Vertex Input
            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            // Vertex to Fragment Output
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the sprite texture
                fixed4 texColor = tex2D(_MainTex, i.uv);

                // Apply shadow color to the texture
                fixed4 shadowColor = _ShadowColor;

                // Combine texture with shadow color for a translucent effect
                return texColor * shadowColor;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Unlit"
}
