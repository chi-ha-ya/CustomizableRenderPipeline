using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomizablePipeline
{
    public enum MsaaQuality
    {
        Disabled = 1,
        _2x = 2,
        _4x = 4,
        _8x = 8
    }
    public enum LightRenderingMode
    {
        Disabled = 0,
        PerVertex = 2,
        PerPixel = 1,
    }
    public class CoreUtils
    {
        public static bool EnvironmentCheck(CustomizedRenderPipelineAsset asset)
        {
            //todo
            return true;
        }
    }
}