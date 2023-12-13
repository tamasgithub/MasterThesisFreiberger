Shader "Custom/NodeShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _BorderColor("Border Color", Color) = (0,0,0,1)
        _BorderWidth("Border Width", Range(0, 0.5)) = 0.1
        _Disabled("Disabled", Range(0,1)) = 0
        _DisabledAlpha("Disabled Alpha", Range(0,1.0)) = 0.5
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

            fixed4 _Color;
            fixed4 _BorderColor;
            float _BorderWidth;
            bool _Disabled;
            bool _DisabledAlpha;

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
                float distFromCenter = distance(i.uv, center);

                // Check if inside the circle
                if (distFromCenter <= 0.5)
                {
                    col = _BorderColor;
                    if (_Disabled == 1) {
                        col.a = _DisabledAlpha;
                    }
                }

                // Check inside the border
                if (distFromCenter < 0.5 - _BorderWidth)
                {
                    col = _Color;
                    if (_Disabled == 1) {
                        col.a = _DisabledAlpha;
                    }
                }

                return col;
            }
            ENDCG
        }
    }
}
