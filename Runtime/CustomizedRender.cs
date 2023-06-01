using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomizablePipeline
{

    public sealed class CustomizedRender : ScriptableObject
    {
        public bool Initialized { get; private set; }
        public List<CustomizedProcess> Processes = new List<CustomizedProcess>();
        internal CommandBuffer cmd;
        private void OnEnable()
        {
            OnCreate();
        }
        internal void OnCreate()
        {
            foreach (var process in Processes) process.OnCreate(this);
            // cmd = CommandBufferPool.Get(string.Empty);
        }
        internal void OnBeginFrameRendering(ref ScriptableRenderContext context, CommandBuffer cmd)
        {
            foreach (var process in Processes) if (process.Enable) process.OnFrameBegin(cmd);
        }
        internal void OnBeginCameraRendering(ref ScriptableRenderContext context, ref CameraData cameraData, CommandBuffer cmd)
        {
            foreach (var process in Processes) if (process.Enable) process.OnCameraSetup(cmd, ref cameraData);
        }
        internal void RenderingSingleCamera(ref ScriptableRenderContext context, ref RendererStatus status)
        {

        }
        internal void OnEndCameraRendering(ref ScriptableRenderContext context, ref CameraData cameraData)
        {
            foreach (var process in Processes) if (process.Enable) process.OnCameraCleanup(cmd, ref cameraData);
        }
        internal void OnEndFrameRendering(ref ScriptableRenderContext context, CommandBuffer cmd)
        {
            foreach (var process in Processes) if (process.Enable) process.OnFrameCleanup(cmd);
        }
        public void OnDispose()
        {

        }
    }
}