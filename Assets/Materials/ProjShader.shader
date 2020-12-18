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
            fixed4 LightColor;

            v2f vert(appdata v)
            {
                //Position data passed explicity from CPU
                v2f o;
                float4 vertexWorldPos = mul(XformMat, v.vertex); //Apply our transform to each vertex. Storing this value to be used elsewhere
                o.vertex = vertexWorldPos; //Set the world position 
                o.vertex = mul(UNITY_MATRIX_VP, o.vertex); //Convert the position to projection space

                o.uv = v.uv; //For textures. Not doing anything in particular for this
                o.vertexWC = vertexWorldPos; 

                float3 p = v.vertex + v.normal; //The vertex normal + vertex position in object space
                p = mul(XformMat, p); //convert this point to world space
                o.normal = normalize(p - o.vertexWC); 

                return o;
            }

            fixed4 ComputeDiffuse(v2f i) {
                float3 l = normalize(LightPosition - i.vertexWC);
                return clamp(dot(i.normal, l), 0, 1);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                if (transparentObj == 0) {
                    //To help support more complex snowman accessories. The container object holding multiple children
                    //has an "invisible" mesh. If we are one of these objects, we can simply toss out the fragment.
                    //NOTE TO SELF: I tried setting the alpha to 0 initially, but it was resulting in a black color for some reason
                    discard;
                }
                
                if (hasTexture == 1) {
                    //If we're using a texture as opposed to a solid color. Attempt to determine how "strong" the light source is
                    //on the fragment
                    fixed4 col = tex2D(_MainTex, i.uv);
                    float diff = ComputeDiffuse(i);
                    return col * LightColor * diff;
                }

                return desiredColor;
            }
            ENDCG
        }
    }
}
