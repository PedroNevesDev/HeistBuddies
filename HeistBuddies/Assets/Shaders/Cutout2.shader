Shader "Custom/SeeThroughMultiCutout"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CutoutPosition1 ("Cutout Position 1", Vector) = (0, 0, 0, 0)
        _CutoutSize1 ("Cutout Size 1", Float) = 0.5
        _CutoutPosition2 ("Cutout Position 2", Vector) = (0, 0, 0, 0)
        _CutoutSize2 ("Cutout Size 2", Float) = 0.5
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _CutoutPosition1;
            float _CutoutSize1;
            float4 _CutoutPosition2;
            float _CutoutSize2;
            float _Cutoff;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float4 worldPos : TEXCOORD1;
            };

            // Vertex shader
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            // Fragment shader
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // Distance from the fragment to each cutout position
                float dist1 = distance(i.worldPos.xyz, _CutoutPosition1.xyz);
                float dist2 = distance(i.worldPos.xyz, _CutoutPosition2.xyz);

                // Smooth cutout transition
                float cutoutFactor1 = smoothstep(_CutoutSize1 * 0.9, _CutoutSize1, dist1);
                float cutoutFactor2 = smoothstep(_CutoutSize2 * 0.9, _CutoutSize2, dist2);

                // Combine both cutouts
                float finalCutout = min(cutoutFactor1, cutoutFactor2);

                // Only discard fragments inside the cutout area
                if (finalCutout < _Cutoff)
                {
                    discard;
                }

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
