Shader "Unlit/VerticalGradientShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // <- Dummy texture
        _TopColor ("Top Color", Color) = (1,1,1,1)
        _BottomColor ("Bottom Color", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Background" }
        LOD 100

        Pass
        {
            ZWrite Off
            Cull Off
            Lighting Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex; // <- Required by UI
            fixed4 _TopColor;
            fixed4 _BottomColor;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = lerp(_BottomColor, _TopColor, i.uv.y);
                color.a = 1.0; // force opaque
                return color;
            }
            ENDCG
        }
    }
}
