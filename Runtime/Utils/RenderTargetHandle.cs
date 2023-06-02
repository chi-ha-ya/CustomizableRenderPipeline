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
        static RenderTargetHandle _BackBuffer;
        public static RenderTargetHandle BackBuffer { get { if (_BackBuffer == null) _BackBuffer = new RenderTargetHandle() { Name = string.Empty, id = -1, rt = null, active = true }; return _BackBuffer; } }
        const int blurPyramidCount = 16;
        static RenderTargetHandle[] _blurPyramid;
        public static RenderTargetHandle[] blurPyramid
        {
            get
            {
                if (_blurPyramid == null)
                {
                    _blurPyramid = new RenderTargetHandle[blurPyramidCount];
                    for (int i = 0; i < blurPyramidCount; ++i) _blurPyramid[i] = new RenderTargetHandle($"_BlurPyramid{i}");
                }
                return _blurPyramid;
            }
        }
        public int id;
        public string Name;
        public bool active;
        public RenderTexture rt;
        static HashSet<RenderTargetHandle> _ActiveTemporaryRT;
        public static HashSet<RenderTargetHandle> ActiveTemporaryRT
        {
            get
            {
                if (_ActiveTemporaryRT == null) _ActiveTemporaryRT = new HashSet<RenderTargetHandle>();
                return _ActiveTemporaryRT;
            }
        }
        static HashSet<RenderTargetHandle> _ActiveRenderTexture;
        public static HashSet<RenderTargetHandle> ActiveRenderTexture
        {
            get
            {
                if (_ActiveRenderTexture == null) _ActiveRenderTexture = new HashSet<RenderTargetHandle>();
                return _ActiveRenderTexture;
            }
        }
        /// <summary>
        /// forbidden!
        /// </summary>
        private RenderTargetHandle() { }

        /// <summary>
        /// TemporaryRT will be released automatically on current frame end
        /// pls call this func on process Init().
        /// </summary>
        /// <param name="Name"></param>
        public RenderTargetHandle(string Name)
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
        /// RenderTexture will be released automatically on current pipeline destroy
        /// pls call this func on process Init().
        /// </summary>
        public RenderTargetHandle(RenderTexture texture)
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(texture.name)) Debug.LogError("the texture must have a name while create RenderTargetHandle by it!");
#endif
            this.Name = texture.name;
            id = Shader.PropertyToID(texture.name);//this id can not be used to get Identifier...
            rt = texture;
            active = true;
            ActiveRenderTexture.Add(this);
        }

        #region GetTemporaryRT helper functions
        public RenderTargetHandle GetTemporaryRT(CommandBuffer command, RenderTextureDescriptor descriptor, FilterMode filter = FilterMode.Bilinear)
        {
            if (rt) return this;
            if (!active)
            {
                command.GetTemporaryRT(id, descriptor, filter);
                ActiveTemporaryRT.Add(this);
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
                ActiveTemporaryRT.Add(this);
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
                ActiveTemporaryRT.Add(this);
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
                ActiveTemporaryRT.Add(this);
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
                ActiveTemporaryRT.Add(this);
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
                ActiveTemporaryRT.Remove(this);
            }
            this.active = false;
        }
        public void ReleaseRenderTexture()
        {
            if (!rt) return;
            if (active)
            {
                RenderTexture.ReleaseTemporary(rt);
                ActiveRenderTexture.Remove(this);
            }
            this.active = false;
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
        public static void ReleaseAllTemporaryRT(CommandBuffer command)
        {
            foreach (var handle in ActiveTemporaryRT)
            {
                command.ReleaseTemporaryRT(handle.id);
                handle.active = false;
            }
            ActiveTemporaryRT.Clear();
        }
        public static void ReleaseAllRenderTexture()
        {
            foreach (var handle in ActiveRenderTexture)
            {
                RenderTexture.ReleaseTemporary(handle.rt);
                handle.active = false;
            }
            ActiveRenderTexture.Clear();
        }
        // public override bool Equals(object obj)
        // {
        //     if (ReferenceEquals(null, obj)) return false;
        //     return obj is RenderTargetHandle && Equals((RenderTargetHandle)obj);
        // }
        // public override int GetHashCode()
        // {
        //     return id;
        // }
        // public static bool operator ==(RenderTargetHandle c1, RenderTargetHandle c2)
        // {
        //     return c1.Equals(c2);
        // }
        // public static bool operator !=(RenderTargetHandle c1, RenderTargetHandle c2)
        // {
        //     return !c1.Equals(c2);
        // }
    }
}