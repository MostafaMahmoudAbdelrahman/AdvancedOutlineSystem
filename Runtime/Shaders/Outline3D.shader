Shader "AdvancedOutlineSystem/Outline3D"
{
    Properties
    {
        _OutlineColor     ("Outline Color",     Color)  = (1,1,1,1)
        _OutlineThickness ("Outline Thickness", Float)  = 3.0
    }

    SubShader
    {
        Tags
        {
            "RenderType"      = "Opaque"
            "RenderPipeline"  = "UniversalPipeline"
            "Queue"           = "Geometry+1"
        }

        // Inverted-hull: expand along normals, cull front faces
        Pass
        {
            Name "Outline3D"
            Cull Front
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _OutlineColor;
                float  _OutlineThickness;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes IN)
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                Varyings OUT;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                // Expand vertex along object-space normal, then transform
                float3 expandedPos = IN.positionOS.xyz + IN.normalOS * (_OutlineThickness * 0.001);
                OUT.positionHCS = TransformObjectToHClip(expandedPos);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return half4(_OutlineColor.rgb, 1.0);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
