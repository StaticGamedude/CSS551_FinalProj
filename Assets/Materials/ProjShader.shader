Shader "Unlit/ProjShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float3 vertexWC: TEXCOORD3;
            };

            sampler2D _MainTex;
            float4x4 XformMat;
            fixed4 desiredColor;
            float transparentObj;
            float hasTexture;
            float4 LightPosition;

            v2f vert(appdata v)
            {
                //Position data passed explicity from CPU
                v2f o;
                o.vertex = mul(XformMat, v.vertex);
                o.vertex = mul(UNITY_MATRIX_VP, o.vertex);

                o.uv = v.uv;
                o.vertexWC = mul(UNITY_MATRIX_M, v.vertex); //using o.vertex since the resulting position should be in world space

                float3 p = v.vertex + v.normal;
                p = mul(UNITY_MATRIX_M, p);  // now in WC space
                o.normal = normalize(p - o.vertexWC); // NOTE: this is in the world space!!

                return o;
            }

            fixed4 ComputeDiffuse(v2f i) {
                float3 l = normalize(LightPosition - i.vertexWC);
                return clamp(dot(i.normal, l), 0, 1);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                if (transparentObj == 0) {
                    discard;
                }
                
                if (hasTexture == 1) {
                    fixed4 col = tex2D(_MainTex, i.uv);
                    float diff = ComputeDiffuse(i);
                    return col * diff;
                }

                return desiredColor;
            }
            ENDCG
        }
    }
}
