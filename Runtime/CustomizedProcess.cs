using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Runtime.CompilerServices;

namespace CustomizablePipeline
{
    public abstract class CustomizedProcess : ScriptableObject
    {
        public bool Enable = true;
        public string Name;
        [field: SerializeField] public long Identifier { get; internal set; }
        CommandBuffer _cmd;
        // protected CommandBuffer cmd { get { if (_cmd == null) _cmd = CommandBufferPool.Get(string.Empty); return _cmd; } }
        protected CommandBuffer cmd;
        public virtual void OnCreate(CustomizedRender renderer) { }
        public virtual void OnFrameBegin(CommandBuffer cmd) { }
        public virtual void OnCameraSetup(CommandBuffer cmd, ref CameraData cameraData) { }
        public abstract void Execute(ref ScriptableRenderContext context, ref RendererStatus status);
        public virtual void OnCameraCleanup(CommandBuffer cmd, ref CameraData cameraData) { }
        public virtual void OnFrameCleanup(CommandBuffer cmd) { }
        public virtual void OnDispose()
        {
            if (cmd != null)
            {
                CommandBufferPool.Release(cmd);
            }
            cmd = null;
        }
        internal void SetupAndExecute(ref ScriptableRenderContext context, ref RendererStatus status)
        {
            if (Enable)
            {
                using (new ProfilingScope(Name))
                {
                    if (cmd == null) cmd = CommandBufferPool.Get(string.Empty);
                    Execute(ref context, ref status); //主要命令在此填充
                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();
                }
            }
        }
        void OnDisable()
        {
            OnDispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void CommitCommandBuffer(ref ScriptableRenderContext context, bool clear = true)
        {
            context.ExecuteCommandBuffer(cmd);
            if (clear) cmd.Clear();
        }
    }
}