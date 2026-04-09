Shader "AdvancedOutlineSystem/ScreenSpaceOutline"
{
    Properties
    {
        _MainTex          ("Source",            2D)    = "white" {}
        _OutlineColor     ("Outline Color",     Color) = (0,0,0,1)
        _OutlineThickness ("Outline Thickness", Float) = 1.0
        _DepthThreshold   ("Depth Threshold",   Float) = 0.01
        _NormalThreshold  ("Normal Threshold",  Float) = 0.4
    }

    SubShader
    {
        Tags
        {
            "RenderType"     = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "ScreenSpaceOutline"
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _MainTex_TexelSize;
                float4 _OutlineColor;
                float  _OutlineThickness;
                float  _DepthThreshold;
                float  _NormalThreshold;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv          = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            // Roberts cross edge detection on depth + normals
            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv    = IN.uv;
                float2 texel = _MainTex_TexelSize.xy * _OutlineThickness;

                float d0 = SampleSceneDepth(uv + float2(-texel.x, -texel.y));
                float d1 = SampleSceneDepth(uv + float2( texel.x,  texel.y));
                float d2 = SampleSceneDepth(uv + float2( texel.x, -texel.y));
                float d3 = SampleSceneDepth(uv + float2(-texel.x,  texel.y));

                float depthEdge = step(_DepthThreshold, abs(d1 - d0) + abs(d3 - d2));

                float3 n0 = SampleSceneNormals(uv + float2(-texel.x, -texel.y));
                float3 n1 = SampleSceneNormals(uv + float2( texel.x,  texel.y));
                float3 n2 = SampleSceneNormals(uv + float2( texel.x, -texel.y));
                float3 n3 = SampleSceneNormals(uv + float2(-texel.x,  texel.y));

                float normalEdge = step(_NormalThreshold,
                    dot(abs(n1 - n0) + abs(n3 - n2), float3(1, 1, 1)));

                float edge  = saturate(depthEdge + normalEdge);
                half4 scene = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                return lerp(scene, _OutlineColor, edge * _OutlineColor.a);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
