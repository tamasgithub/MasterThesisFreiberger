// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/DecisionBoundaryShader"
{
    Properties
    {
        _XRange("Ranges", Vector) = (0.0,0.0,0.0,0.0)
        _Color("Color", Color) = (1,0,0,1)
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader {
         Tags { "Queue"="Transparent" "Render"="Transparent" "IgnoreProjector"="True"}
         LOD 200
         
         ZWrite Off
         Blend SrcAlpha OneMinusSrcAlpha
 
         Pass{
             CGPROGRAM
 
             #pragma target 3.0
             #pragma vertex vert
             #pragma fragment frag
 
             #include "UnityCG.cginc"
 
             struct appdata {
                 float4 vertex : POSITION;
                 
             };
 
             struct v2f {
                 float4 vertex : SV_POSITION;
                 float3 worldPos : TEXCOORD0;
             };
 
             uniform float4 _Ranges;
             uniform float4 _Color;
             
 
             v2f vert(appdata v) {
                 v2f o;
 
                 o.worldPos = mul (unity_ObjectToWorld, v.vertex);
                 o.vertex = UnityObjectToClipPos(v.vertex);
 
                 return o;
             }
 
             fixed4 frag(v2f i) : SV_Target {
                 fixed4 col = _Color;
                 col.a = (i.worldPos.x > _Ranges.x && i.worldPos.x < _Ranges.y && i.worldPos.y > _Ranges.z && i.worldPos.y < _Ranges.w) ? 1.0 : 0.0;
                 return col;
             }
 
             ENDCG
         }
 
     
     }
     FallBack "Diffuse"
}
