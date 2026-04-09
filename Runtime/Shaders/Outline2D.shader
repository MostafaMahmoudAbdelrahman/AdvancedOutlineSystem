Shader "AdvancedOutlineSystem/Outline2D"
{
    Properties
    {
        _MainTex          ("Sprite Texture",    2D)     = "white" {}
        _OutlineColor     ("Outline Color",     Color)  = (1,1,1,1)
        _OutlineThickness ("Outline Thickness", Float)  = 2.0
    }

    SubShader
    {
        Tags
        {
            "RenderType"     = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "Queue"          = "Transparent"
        }

        Pass
        {
            Name "Outline2D"
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _MainTex_TexelSize;
                float4 _OutlineColor;
                float  _OutlineThickness;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes IN)
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                Varyings OUT;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv   = IN.uv;
                float  step = _OutlineThickness * _MainTex_TexelSize.x;

                // Sample alpha of 4 neighbours for edge detection
                float a  = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).a;
                float aU = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(0,  step)).a;
                float aD = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(0, -step)).a;
                float aL = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-step, 0)).a;
                float aR = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2( step, 0)).a;

                float maxNeighbour = max(max(aU, aD), max(aL, aR));
                float outline      = saturate(maxNeighbour - a);

                half4 sprite = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                half4 col    = lerp(sprite, _OutlineColor, outline);
                col.a        = max(sprite.a, outline * _OutlineColor.a);
                return col;
            }
            ENDHLSL
        }
    }

    FallBack Off
}
