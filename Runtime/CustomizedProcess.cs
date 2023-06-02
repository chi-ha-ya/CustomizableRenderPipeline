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
        public virtual void OnCreate(CustomizedRender renderer) { }
        public virtual void OnFrameBegin(CommandBuffer cmd) { }
        public virtual void OnCameraSetup(CommandBuffer cmd) { }
        public abstract void Execute(CommandBuffer cmd);
        public virtual void OnCameraCleanup(CommandBuffer cmd) { }
        public virtual void OnFrameCleanup(CommandBuffer cmd) { }
        public virtual void Dispose(bool disposing) { }
        void OnDisable()
        {
            Dispose(true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void SetRenderTarget(CommandBuffer cmd)
        {

        }
    }
}