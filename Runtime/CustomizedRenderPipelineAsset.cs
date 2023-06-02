using System;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace CustomizablePipeline
{
    public class CustomizedRenderPipelineAsset : RenderPipelineAsset
    {

        public bool URPShaderCompatible = true;
        public bool UseScriptableRenderPipelineBatching = true;
        public bool LightsUseLinearIntensity = true;
        public MsaaQuality MsaaQuality = MsaaQuality._4x;
        public int DefaultRendererIndex = 0;
        public List<CustomizedRender> Renderers;
        public CustomizedRender DefaultRenderer => Renderers[DefaultRendererIndex];
        public LightRenderingMode AdditionalLightsRenderingMode = LightRenderingMode.PerPixel;
        public int AdditionalLightsPerObjectLimit = 4;

        protected override RenderPipeline CreatePipeline()
        {
            if (CoreUtils.EnvironmentCheck(this))
            {
                GraphicsSettings.lightsUseLinearIntensity = LightsUseLinearIntensity;
                GraphicsSettings.useScriptableRenderPipelineBatching = UseScriptableRenderPipelineBatching;

                if (URPShaderCompatible) Shader.globalRenderPipeline = "UniversalPipeline";//URP shader always has tag: "RenderPipeline" = "UniversalPipeline"
                else Shader.globalRenderPipeline = string.Empty;
                return new CustomizedRenderPipeline(this);
            }

            return null;
        }
        public CustomizedRender GetRenderer(int index)
        {
            if (index >= 0 && index < Renderers.Count) return Renderers[index];
            else return DefaultRenderer;
        }
    }
}