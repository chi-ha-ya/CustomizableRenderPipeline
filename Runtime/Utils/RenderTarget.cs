using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomizablePipeline
{
    public struct RenderTarget
    {
        static Dictionary<string, RenderTargetHandle> TemporaryRTHandles = new Dictionary<string, RenderTargetHandle>();
        static Dictionary<RenderTexture, RenderTargetHandle> RenderTextureHandles = new Dictionary<RenderTexture, RenderTargetHandle>();
        public RenderTextureDescriptor descriptor;
        public RenderTargetHandle ScreenBuffer => RenderTargetHandle.ScreenBuffer;
        public RenderTargetHandle color;
        public RenderTargetHandle depth;

        public RenderTargetHandle GetHandle(string Name)
        {
            TemporaryRTHandles.TryGetValue(Name, out var handle);
            if (handle == null)
            {
                handle = new RenderTargetHandle(Name);
                TemporaryRTHandles.Add(Name, handle);
            }
            return handle;
        }
        public RenderTargetHandle GetHandle(RenderTexture texture)
        {
            RenderTextureHandles.TryGetValue(texture, out var handle);
            if (handle == null)
            {
                handle = new RenderTargetHandle(texture);
                RenderTextureHandles.Add(texture, handle);
            }
            return handle;
        }
        public void Remove(RenderTargetHandle handle)
        {
            if (handle.isTemporary) TemporaryRTHandles.Remove(handle.Name);
            else RenderTextureHandles.Remove(handle.rt);
        }
        internal void ReleaseAllActiveTemporaryRT(CommandBuffer cmd)
        {
            foreach (var rt in TemporaryRTHandles.Values)
            {
                if (rt.active) rt.ReleaseTemporaryRT(cmd);
            }
        }
        internal void ReleaseAllActiveRenderTexture(CommandBuffer cmd)
        {
            foreach (var rt in RenderTextureHandles.Values)
            {
                if (rt.active) rt.ReleaseRenderTexture();
            }
        }
        internal void Clear()
        {
            TemporaryRTHandles.Clear();
            RenderTextureHandles.Clear();
        }
    }
}