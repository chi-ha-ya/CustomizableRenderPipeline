using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomizablePipeline
{
    public struct RendererStatus
    {
        public CameraData cameraData;
        public CullingResults cullingResults;
        public CustomizedRenderPipeline pipeline;

    }
}