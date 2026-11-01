Shader "Shader Graphs/CloudShader"
{
    Properties
    {
 
        _LitColor ("Sun Side Colour", Color) = (1, 0.97, 0.92, 1)
        _ShadeColor ("Shadow Side Colour", Color) = (0.70, 0.66, 0.85, 1)
        _Wrap ("Light Wrap", Range(0, 1)) = 0.6
        _AmbientStrength ("Sky Light", Range(0, 1)) = 0.3

        _RimColor ("Rim Colour", Color) = (1, 0.85, 0.65, 1)
        _RimStrength ("Rim Strength", Range(0, 2)) = 0.6
        _RimPower ("Rim Width", Range(0.5, 8)) = 3
        _BacklightBoost ("Facing Sun Boost", Range(0, 1)) = 0.7

        _Opacity ("Opacity", Range(0, 1)) = 0.95
        _EdgeSoftness ("Edge Softness", Range(0.01, 1)) = 0.35
        [NoScaleOffset] _NoiseTex ("Breakup Noise", 2D) = "grey" {}
        _EdgeBreakup ("Edge Breakup", Range(0, 1)) = 0.35
        _BreakupScale ("Breakup Scale", Float) = 0.05
        _DepthFade ("Rock Fade Distance", Float) = 4
        _CameraFade ("Camera Fade Distance", Float) = 8

        _BillowStrength ("Billow Strength", Float) = 0.4
        _BillowSpeed ("Billow Speed", Float) = 0.3
        _BillowScale ("Billow Scale", Float) = 0.15
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True" }

        Pass
        {
            Name "Cloud"
            Tags { "LightMode" = "UniversalForward" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            CBUFFER_START(UnityPerMaterial)
                half4 _LitColor, _ShadeColor, _RimColor;
                float _Wrap, _AmbientStrength, _RimStrength, _RimPower, _BacklightBoost;
                float _Opacity, _EdgeSoftness, _EdgeBreakup, _BreakupScale, _DepthFade, _CameraFade;
                float _BillowStrength, _BillowSpeed, _BillowScale;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
                float fogFactor : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

                float3 p = positionWS * _BillowScale;
                float t = _Time.y * _BillowSpeed;
                float billow = sin(p.x + t + sin(p.z * 1.3 + t * 0.7)) * sin(p.z * 0.9 - t * 0.8 + sin(p.y * 1.7));
                positionWS += normalWS * billow * _BillowStrength;

                output.positionWS = positionWS;
                output.normalWS = normalWS;
                output.positionCS = TransformWorldToHClip(positionWS);
                output.screenPos = ComputeScreenPos(output.positionCS);
                output.fogFactor = ComputeFogFactor(output.positionCS.z);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                float3 n = normalize(input.normalWS);
                float3 v = normalize(GetWorldSpaceViewDir(input.positionWS));
                Light sun = GetMainLight();

                float wrap = saturate((dot(n, sun.direction) + _Wrap) / (1 + _Wrap));
                half3 col = lerp(_ShadeColor.rgb, _LitColor.rgb * sun.color, wrap);
                col += SampleSH(n) * _AmbientStrength;
                float facing = saturate(dot(n, v));
                float towardSun = pow(saturate(dot(-v, sun.direction)), 4);
                float rim = pow(1 - facing, _RimPower) * lerp(1 - _BacklightBoost, 1, towardSun);
                col += _RimColor.rgb * sun.color * rim * _RimStrength;

                float noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, input.positionWS.xz * _BreakupScale + input.positionWS.y * _BreakupScale * 0.5).b;
                float alpha = saturate((facing - (1 - noise) * _EdgeBreakup) / _EdgeSoftness);

                float2 screenUV = input.screenPos.xy / input.screenPos.w;
                float sceneDepth = LinearEyeDepth(SampleSceneDepth(screenUV), _ZBufferParams);
                float surfaceDepth = input.screenPos.w;
                alpha *= saturate((sceneDepth - surfaceDepth) / max(_DepthFade, 0.001));
                alpha *= saturate(surfaceDepth / max(_CameraFade, 0.001) - 0.2);

                col = MixFog(col, input.fogFactor);
                return half4(col, alpha * _Opacity);
            }
            ENDHLSL
        }
    }
    Fallback Off
}
