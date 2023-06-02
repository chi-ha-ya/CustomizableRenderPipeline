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
        private void OnEnable()
        {
            OnCreate();
        }
        internal void OnCreate()
        {
            foreach (var process in Processes) process.OnCreate(this);
        }
        internal void OnBeginFrameRendering()
        {
            var cmd = CommandBufferPool.Get(string.Empty);

            foreach (var process in Processes) if (process.Enable) process.OnFrameBegin(cmd);
            RenderStatus.Commit(cmd);

            CommandBufferPool.Release(cmd);
        }
        internal void OnBeginCameraRendering()
        {
            var cmd = CommandBufferPool.Get(string.Empty);

            foreach (var process in Processes) if (process.Enable) process.OnCameraSetup(cmd);
            RenderStatus.Commit(cmd);

            CommandBufferPool.Release(cmd);
        }
        internal void RenderingSingleCamera()
        {
            foreach (var process in Processes)
            {
                if (process.Enable)
                {
                    var cmd = CommandBufferPool.Get(string.Empty);
                    using (new ProfilingScope(cmd, process.Name))
                    {
                        process.Execute(cmd);
                    }
                    CommandBufferPool.Release(cmd);
                }
            }
        }
        internal void OnEndCameraRendering()
        {
            var cmd = CommandBufferPool.Get(string.Empty);

            foreach (var process in Processes) if (process.Enable) process.OnCameraCleanup(cmd);
            RenderStatus.Commit(cmd);

            CommandBufferPool.Release(cmd);
        }
        internal void OnEndFrameRendering()
        {
            var cmd = CommandBufferPool.Get(string.Empty);

            foreach (var process in Processes) if (process.Enable) process.OnFrameCleanup(cmd);
            RenderStatus.Commit(cmd);

            CommandBufferPool.Release(cmd);
        }
        internal void Dispose(bool disposing) { }
        void OnDisable()
        {
            Dispose(true);
        }
    }
}