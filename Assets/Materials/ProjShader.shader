Shader "Unlit/ProjShader"
{
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
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
                float4 vertex : SV_POSITION;
            };

            float4x4 XformMat;
            fixed4 desiredColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = mul(XformMat, v.vertex);
                o.vertex = mul(UNITY_MATRIX_VP, o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return desiredColor;
            }
            ENDCG
        }
    }
}
