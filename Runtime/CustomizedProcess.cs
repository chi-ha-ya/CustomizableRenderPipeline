using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Runtime.CompilerServices;

namespace CustomizablePipeline
{
    public abstract class CustomizedProcess : ScriptableObject
    {
        protected RenderTarget target => RenderStatus.target;
        protected ScriptableRenderContext context => RenderStatus.context;
        protected CustomizedRenderPipeline pipeline => RenderStatus.pipeline;
        protected CameraData cameraData => RenderStatus.cameraData;
        protected CullingResults cullingResults => RenderStatus.cullingResults;

        public bool Enable = true;
        public string Name;
        [field: SerializeField] public long Identifier { get; internal set; }
        /// <summary>
        /// once before all Execute(), whether enabled or not
        /// </summary>
        /// <param name="renderer"></param>
        public virtual void Init(CustomizedRender renderer) { }
        /// <summary>
        /// once pre frame, before rendering
        /// </summary>
        /// <param name="cmd"></param>
        public virtual void OnFrameBegin(CommandBuffer cmd) { }
        /// <summary>
        /// once per camera, if enabled & used by camera
        /// </summary>
        /// <param name="cmd"></param>
        public virtual void OnCameraSetup(CommandBuffer cmd) { }
        /// <summary>
        /// once per camera, if enabled & used by camera
        /// </summary>
        /// <param name="cmd"></param>
        public abstract void Execute(CommandBuffer cmd);
        /// <summary>
        ///  once per camera, if enabled & used by camera
        /// </summary>
        /// <param name="cmd"></param>
        public virtual void OnCameraCleanup(CommandBuffer cmd) { }
        /// <summary>
        /// once pre frame, after rendering
        /// </summary>
        /// <param name="cmd"></param>
        public virtual void OnFrameCleanup(CommandBuffer cmd) { }
        public virtual void Dispose(bool disposing) { }
        void OnDisable() { Dispose(true); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void SetRenderTarget(CommandBuffer cmd, RenderTargetHandle targetHandle)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Commit(CommandBuffer command, bool clear = true)
        {
            RenderStatus.context.ExecuteCommandBuffer(command);
            if (clear) command.Clear();
        }
    }
}