Shader "Custom/BasketStylizedURP"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.55, 0.33, 0.16, 1)
        _DarkColor ("Dark Weave Color", Color) = (0.30, 0.17, 0.08, 1)
        _TopTint ("Top Tint", Color) = (0.72, 0.47, 0.24, 1)
        _BottomTint ("Bottom Tint", Color) = (0.38, 0.22, 0.10, 1)

        _WeaveScale ("Weave Scale", Float) = 18
        _WeaveStrength ("Weave Strength", Range(0,1)) = 0.22
        _WeaveContrast ("Weave Contrast", Range(0.1,8)) = 3.0

        _RimColor ("Rim Color", Color) = (1.0, 0.78, 0.45, 1)
        _RimPower ("Rim Power", Range(0.1,8)) = 3.0
        _RimStrength ("Rim Strength", Range(0,2)) = 0.5

        _HighlightStrength ("Highlight Strength", Range(0,2)) = 0.35
        _AmbientBoost ("Ambient Boost", Range(0,2)) = 0.2
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Geometry"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS  : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
                float2 uv          : TEXCOORD2;
                float fogCoord     : TEXCOORD3;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _DarkColor;
                float4 _TopTint;
                float4 _BottomTint;

                float _WeaveScale;
                float _WeaveStrength;
                float _WeaveContrast;

                float4 _RimColor;
                float _RimPower;
                float _RimStrength;

                float _HighlightStrength;
                float _AmbientBoost;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(IN.normalOS);

                OUT.positionHCS = posInputs.positionCS;
                OUT.positionWS = posInputs.positionWS;
                OUT.normalWS = normalize(normalInputs.normalWS);
                OUT.uv = IN.uv;
                OUT.fogCoord = ComputeFogFactor(posInputs.positionCS.z);

                return OUT;
            }

            float WeavePattern(float2 uv, float scale, float contrast)
            {
                float2 p = uv * scale;

                float vertical = sin(p.x * 6.28318);
                float horizontal = sin(p.y * 6.28318);

                float weave = vertical * horizontal;
                weave = 0.5 + 0.5 * weave;
                weave = pow(saturate(weave), contrast);

                return weave;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 normalWS = normalize(IN.normalWS);
                float3 viewDir = normalize(GetWorldSpaceViewDir(IN.positionWS));

                Light mainLight = GetMainLight();
                float3 lightDir = normalize(mainLight.direction);

                float NdotL = saturate(dot(normalWS, lightDir));
                float lambert = NdotL;

                float weave = WeavePattern(IN.uv, _WeaveScale, _WeaveContrast);

                float3 baseCol = _BaseColor.rgb;
                float3 darkCol = _DarkColor.rgb;

                float3 wovenCol = lerp(baseCol, darkCol, weave * _WeaveStrength);

                float heightMask = saturate(IN.uv.y);
                float3 verticalTint = lerp(_BottomTint.rgb, _TopTint.rgb, heightMask);

                float3 albedo = wovenCol * verticalTint;

                float rim = pow(1.0 - saturate(dot(viewDir, normalWS)), _RimPower) * _RimStrength;

                float3 halfDir = normalize(lightDir + viewDir);
                float spec = pow(saturate(dot(normalWS, halfDir)), 24.0) * _HighlightStrength;

                float3 lighting = albedo * (lambert + _AmbientBoost);
                lighting += _RimColor.rgb * rim;
                lighting += spec;

                float3 finalColor = lighting * mainLight.color;

                finalColor = MixFog(finalColor, IN.fogCoord);

                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}