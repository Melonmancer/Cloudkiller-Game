Shader "Shader Graphs/SkyShader"
{
    Properties
    {
        _TopColour ("Top Colour", Color) = (0.36, 0.55, 0.78, 1)
        _HorizonColour ("Horizon Colour", Color) = (0.95, 0.90, 0.75, 1)
        _BottomColour ("Bottom Colour", Color) = (0.72, 0.80, 0.82, 1)
        _SunColour ("Sun Colour", Color) = (1, 0.92, 0.70, 1)
        _HorizonHeight ("Horizon Height", Range(-0.5, 0.5)) = 0
        _HorizonSoftness ("Horizon Softness", Range(0.01, 1)) = 0.5

        _SunSize ("Sun Size", Range(0, 0.01)) = 0.0008
        _GlowFalloff ("Sun Glow Falloff", Range(1, 64)) = 16
        _GlowStrength ("Sun Glow Strength", Range(0, 2)) = 0.35

        _BottomSoftness ("Bottom Softness", Range(0.01, 1)) = 0.3
    }

    SubShader
    {
        Tags { "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" "RenderPipeline" = "UniversalPipeline" }
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _TopColour, _HorizonColour, _BottomColour, _SunColour;
                float _HorizonHeight, _HorizonSoftness, _SunSize, _GlowFalloff, _GlowStrength, _BottomSoftness;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 direction : TEXCOORD0;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.direction = input.positionOS.xyz;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 dir = normalize(input.direction);
                float height = dir.y;

                float sunDot = dot(dir, _MainLightPosition.xyz);
                float glow = pow(saturate(sunDot), _GlowFalloff) * _GlowStrength;
                float disc = smoothstep(1 - _SunSize, 1 - _SunSize * 0.7, sunDot);

                half3 sky = lerp(_HorizonColour.rgb, _TopColour.rgb, smoothstep(_HorizonHeight, _HorizonHeight + _HorizonSoftness, height));
                half3 below = lerp(_HorizonColour.rgb, _BottomColour.rgb, smoothstep(0, _BottomSoftness, -height));

                half3 col = lerp(below, sky, smoothstep(-0.005, 0.005, height));
                col += _SunColour.rgb * (glow + disc * 2);
                return half4(col, 1);
            }
            ENDHLSL
        }
    }
    Fallback Off
}
