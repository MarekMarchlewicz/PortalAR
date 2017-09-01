Shader "Custom/Outline"
{
    Properties 
    {
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline width", Range (0, 0.1)) = .005
    }
 
    SubShader 
    {
        Tags { "RenderType"="Transparent" }
        Pass
        {
            Cull Front
			Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag
           
            uniform float _OutlineWidth;
            uniform float4 _OutlineColor;
 
            struct v2f
            {
                float4 pos : POSITION;
                float4 color : COLOR;
            };
           
            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos (v.vertex);
                float3 norm   = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
                float2 offset = TransformViewToProjection(norm.xy);
                o.pos.xy += offset  * _OutlineWidth;
                o.color = _OutlineColor;
                return o;
            }
           
            half4 frag(v2f i) :COLOR
            {
                return i.color;
            }
                   
            ENDCG
        }
    }
   
    Fallback Off
}