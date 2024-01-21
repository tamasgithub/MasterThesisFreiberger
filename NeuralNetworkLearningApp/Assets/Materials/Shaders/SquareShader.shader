Shader "Custom/SquareShader"
{
    Properties
    {
        _InsideColor("Inside Color", Color) = (1,1,1,1)
        _BorderColor("Border Color", Color) = (0,0,0,1)
        _BorderWidth("Border Width", Range(0, 0.5)) = 0.1
    }
        SubShader
    {
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100


        Pass
        {
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            fixed4 _InsideColor;
            fixed4 _BorderColor;
            float _BorderWidth;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = float4(0,0,0,0);

                float2 center = float2(0.5, 0.5);
                float distFromBorder = min(min(min(i.uv.x, i.uv.y), 1 - i.uv.x), 1 - i.uv.y);

                if (distFromBorder <= _BorderWidth)
                {
                    col = _BorderColor;
                } else {
                    col = _InsideColor;
                }
                return col;
            }
            ENDCG
        }
    }
}
