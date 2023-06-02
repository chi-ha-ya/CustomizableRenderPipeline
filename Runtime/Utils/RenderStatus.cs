using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomizablePipeline
{
    public static class RenderStatus
    {
        public static RenderTarget target;
        public static ScriptableRenderContext context;
        public static Camera[] cameras;
        public static CameraData cameraData;
        public static CullingResults cullingResults;
        public static CustomizedRenderPipeline pipeline;

        /// <summary>
        /// submit a command buffer to context
        /// </summary>
        /// <param name="command">command buffer to execute</param>
        /// <param name="clear">true if the command need to be clear after execute</param>
        public static void Commit(CommandBuffer command, bool clear = true)
        {
            context.ExecuteCommandBuffer(command);
            if (clear) command.Clear();
        }
    }
}