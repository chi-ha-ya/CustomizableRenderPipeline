using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

namespace CustomizablePipeline
{
    /// <summary>
    /// Handle to manage renderer target, Do Not new this, Use target.Get(...) Instead
    /// </summary>
    public class RenderTargetHandle
    {
        internal static RenderTargetHandle ScreenBuffer = new RenderTargetHandle() { Name = string.Empty, id = -1, rt = null, active = true };

        public string Name;
        public int id;
        public RenderTexture rt;
        public bool active { get; private set; }
        public bool isTemporary => rt == null && id != -1;

        /// <summary>
        /// forbidden!
        /// </summary>
        private RenderTargetHandle() { }

        /// <summary>
        /// Handle to manage renderer target, Do Not new this, Use target.Get(...) Instead,
        /// TemporaryRT will be released automatically on current frame end
        /// </summary>
        /// <param name="Name"></param>
        internal RenderTargetHandle(string Name)
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(Name)) Debug.LogError("RenderTargetHandle must have a name!");
#endif
            this.Name = Name;
            id = Shader.PropertyToID(Name);
            rt = null;
            active = false;
        }
        /// <summary>
        /// Handle to manage renderer target, Do Not new this, Use target.Get(...) Instead,
        /// RenderTexture will be released automatically on current pipeline destroy
        /// </summary>
        internal RenderTargetHandle(RenderTexture texture)
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(texture.name)) Debug.LogError("the texture must have a name while create RenderTargetHandle by it!");
#endif
            this.Name = texture.name;
            id = Shader.PropertyToID(texture.name);//this id can not be used to get Identifier...
            rt = texture;
            active = true;
        }

        #region GetTemporaryRT helper functions
        public RenderTargetHandle GetTemporaryRT(CommandBuffer command, RenderTextureDescriptor descriptor, FilterMode filter = FilterMode.Bilinear)
        {
            if (rt) return this;
            if (!active)
            {
                command.GetTemporaryRT(id, descriptor, filter);
                active = true;
            }
            return this;
        }
        public RenderTargetHandle GetTemporaryRT(CommandBuffer command, RenderTextureDescriptor descriptor, FilterMode filter, float scale)
        {
            if (rt) return this;
            if (!active)
            {
                descriptor.width = (int)(descriptor.width * scale);
                descriptor.height = (int)(descriptor.height * scale);
                command.GetTemporaryRT(id, descriptor, filter);
                active = true;
            }
            return this;
        }
        public RenderTargetHandle GetTemporaryRT(CommandBuffer command, RenderTextureDescriptor descriptor, int width, int height, FilterMode filter = FilterMode.Bilinear)
        {
            if (rt) return this;
            if (!active)
            {
                descriptor.width = width;
                descriptor.height = height;
                command.GetTemporaryRT(id, descriptor, filter);
                active = true;
            }
            return this;
        }
        public RenderTargetHandle GetTemporaryRT(CommandBuffer command, RenderTextureDescriptor descriptor, FilterMode filter, GraphicsFormat overrideGraphicsFormat, float scale = 1f)
        {
            if (rt) return this;
            if (!active)
            {
                descriptor.width = (int)(descriptor.width * scale);
                descriptor.height = (int)(descriptor.height * scale);
                descriptor.graphicsFormat = overrideGraphicsFormat;
                command.GetTemporaryRT(id, descriptor, filter);
                active = true;
            }
            return this;
        }
        public RenderTargetHandle GetTemporaryRT(CommandBuffer command, RenderTextureDescriptor descriptor, FilterMode filter, RenderTextureFormat overrideColorFormat, float scale = 1f)
        {
            if (rt) return this;
            if (!active)
            {
                descriptor.width = (int)(descriptor.width * scale);
                descriptor.height = (int)(descriptor.height * scale);
                descriptor.colorFormat = overrideColorFormat;
                command.GetTemporaryRT(id, descriptor, filter);
                active = true;
            }
            return this;
        }

        #endregion

        public void ReleaseTemporaryRT(CommandBuffer command)
        {
            if (rt) return;
            if (active)
            {
                command.ReleaseTemporaryRT(id);
            }
            active = false;
        }
        public void ReleaseRenderTexture()
        {
            if (!rt) return;
            if (active)
            {
                RenderTexture.ReleaseTemporary(rt);
            }
            active = false;
            rt = null;
        }
        public RenderTargetIdentifier Identifier()
        {
            if (id == -1) return BuiltinRenderTextureType.CameraTarget;
            if (rt) return new RenderTargetIdentifier(rt);
            return new RenderTargetIdentifier(id);
        }
        public bool Equals(RenderTargetHandle other)
        {
            if (rt) return rt == other.rt;
            return id == other.id;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is RenderTargetHandle && Equals((RenderTargetHandle)obj);
        }
        public override int GetHashCode()
        {
            return id;
        }
        public static bool operator ==(RenderTargetHandle lhs, RenderTargetHandle rhs)
        {
            return lhs.Equals(rhs);
        }
        public static bool operator !=(RenderTargetHandle lhs, RenderTargetHandle rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}