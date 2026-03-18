Shader "Custom/DitherFadeLitSimple"
{
    Properties
    {
        _BaseMap ("Texture", 2D) = "white" {}
        _BaseColor ("Color", Color) = (1,1,1,1)
        _DitherFade ("Dither Fade", Range(0,1)) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalRenderPipeline"
            "Queue"="AlphaTest"
            "RenderType"="Opaque"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            ZWrite On
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            float4 _BaseColor;
            float _DitherFade;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float4 shadowCoord : TEXCOORD3;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;

                o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
                o.positionHCS = TransformWorldToHClip(o.positionWS);
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.uv = v.uv;

                o.shadowCoord = TransformWorldToShadowCoord(o.positionWS);

                return o;
            }

            float Dither4x4(float2 pos)
            {
                int x = (int)fmod(pos.x, 4);
                int y = (int)fmod(pos.y, 4);

                int index = x + y * 4;

                float dither[16] = {
                    0.0, 0.5, 0.125, 0.625,
                    0.75, 0.25, 0.875, 0.375,
                    0.1875, 0.6875, 0.0625, 0.5625,
                    0.9375, 0.4375, 0.8125, 0.3125
                };

                return dither[index];
            }

            half4 frag(Varyings i) : SV_Target
            {
                float dither = Dither4x4(i.positionHCS.xy);

                if (dither > _DitherFade)
                    discard;

                float3 normal = normalize(i.normalWS);

                Light mainLight = GetMainLight(i.shadowCoord);

                float NdotL = saturate(dot(normal, mainLight.direction));

                float lighting = NdotL * mainLight.shadowAttenuation;

                float4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv) * _BaseColor;

                float3 finalColor = baseColor.rgb * lighting;

                return float4(finalColor, baseColor.a);
            }

            ENDHLSL
        }
    }
}
