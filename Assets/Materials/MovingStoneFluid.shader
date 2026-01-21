Shader "Custom/URP/ProceduralStoneFluidStable"
{
    Properties
    {
        _StoneColor ("Stone Color", Color) = (0.45,0.45,0.45,1)

        _LiquidColorA ("Liquid Dark Green", Color) = (0.1,0.3,0.15,1)
        _LiquidColorB ("Liquid Light Green", Color) = (0.3,0.7,0.4,1)

        _StructureScale ("Structure Scale", Float) = 1.2
        _LiquidNoiseScale ("Liquid Noise Scale", Float) = 2.5

        _StructureSpeed ("Structure Speed", Float) = 0.05
        _LiquidSpeed ("Liquid Speed", Float) = 0.6

        _StoneThreshold ("Stone Threshold", Range(0,1)) = 0.58
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
            };

            float4 _StoneColor;
            float4 _LiquidColorA;
            float4 _LiquidColorB;

            float _StructureScale;
            float _LiquidNoiseScale;
            float _StructureSpeed;
            float _LiquidSpeed;
            float _StoneThreshold;

            // ---------------- HASH ----------------
            float hash(float3 p)
            {
                return frac(sin(dot(p, float3(127.1,311.7,74.7))) * 43758.5453);
            }

            // ---------------- 3D NOISE ----------------
            float noise3D(float3 p)
            {
                float3 i = floor(p);
                float3 f = frac(p);
                float3 u = f * f * (3.0 - 2.0 * f);

                return lerp(
                    lerp(
                        lerp(hash(i + float3(0,0,0)), hash(i + float3(1,0,0)), u.x),
                        lerp(hash(i + float3(0,1,0)), hash(i + float3(1,1,0)), u.x),
                        u.y
                    ),
                    lerp(
                        lerp(hash(i + float3(0,0,1)), hash(i + float3(1,0,1)), u.x),
                        lerp(hash(i + float3(0,1,1)), hash(i + float3(1,1,1)), u.x),
                        u.y
                    ),
                    u.z
                );
            }

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS   = TransformObjectToWorldNormal(IN.normalOS);
                OUT.positionCS = TransformWorldToHClip(OUT.positionWS);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // ---------- STRUCTURE (QUASI FIXE) ----------
                float3 structurePos =
                    IN.positionWS * _StructureScale +
                    _Time.y * _StructureSpeed;

                float structureNoise = noise3D(structurePos);

                float stoneMask =
                    smoothstep(_StoneThreshold - 0.02, _StoneThreshold + 0.02, structureNoise);

                // ---------- LIQUID COLOR NOISE (ANIMÉ) ----------
                float3 liquidPos =
                    IN.positionWS * _LiquidNoiseScale +
                    float3(0.0, _Time.y * _LiquidSpeed, 0.0);

                float liquidNoise = noise3D(liquidPos);

                float3 liquidColor =
                    lerp(_LiquidColorA.rgb, _LiquidColorB.rgb, liquidNoise);

                // ---------- FINAL COLOR ----------
                float3 baseColor =
                    lerp(liquidColor, _StoneColor.rgb, stoneMask);

                // ---------- LIGHTING ----------
                Light light = GetMainLight();
                float NdotL = saturate(dot(normalize(IN.normalWS), light.direction));

                float3 finalColor = baseColor * (0.35 + 0.65 * NdotL);

                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
}
