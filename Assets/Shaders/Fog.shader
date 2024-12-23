Shader "Limbo/Fog"
{
    Properties
    {
        _MaxDistance("Max Distance", Float) = 100
        _StepSize("Step Size", Range(0.1, 20)) = 1
        _DensityMultiplier("Density Multiplier", Range(0, 10)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline"}
        //LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag
            
             
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            float _MaxDistance;
            float _StepSize;
            float _DensityMultiplier;

            float get_density()
            {
                return _DensityMultiplier;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float depth = SampleSceneDepth(IN.texcoord);
                float worldPos = ComputeWorldSpacePosition(IN.texcoord, depth, UNITY_MATRIX_I_VP);

                float entryPoint = _WorldSpaceCameraPos;
                float viewDir = worldPos - _WorldSpaceCameraPos;
                float viewLength = length(viewDir);
                float rayDir = normalize(viewDir);

                float distLimit = min(viewLength, _MaxDistance);
                float distTravelled = 0;
                float transmittance = 0;

                while (distTravelled < distLimit)
                {
                    float density = get_density();
                    if (density > 0)
                    {
                        transmittance += density * _StepSize;
                    }

                    distTravelled += density * _StepSize;
                }
                
                return transmittance;
            }

            


            
            ENDHLSL
        }
    }
}
