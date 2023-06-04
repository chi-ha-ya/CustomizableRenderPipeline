using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomizablePipeline
{

    public class CustomizedRenderPipeline : RenderPipeline
    {
        RenderData m_RenderData;
        public CustomizedRenderPipelineAsset asset;
        public CustomizedRenderPipeline(CustomizedRenderPipelineAsset asset)
        {
            this.asset = asset;
        }
        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            BeginFrameRendering(context, cameras);
            if (cameras != null || cameras.Length > 0)
            {
                var cmd = CommandBufferPool.Get();
                using (new ProfilingScope(cmd, KeywordStrings.CustomizedRP))
                {
                    m_RenderData.Init(context, cameras, this);
                    foreach (var renderer in m_RenderData.ActiveRenders) renderer.Init();

                    foreach (var renderer in m_RenderData.ActiveRenders) renderer.OnBeginFrameRendering();

                    for (int i = 0; i < m_RenderData.Length; ++i) RenderingSingleCamera(m_RenderData[i]);

                    foreach (var renderer in m_RenderData.ActiveRenders) renderer.OnEndFrameRendering();
                    RenderStatus.target.ReleaseAllActiveTemporaryRT(cmd);
                }
                CommandBufferPool.Release(cmd);

                context.Submit();
            }
            EndFrameRendering(context, cameras);
        }

        void RenderingSingleCamera(CameraData data)
        {
            var camera = data.camera;
            var renderer = data.renderer;
            BeginCameraRendering(RenderStatus.context, camera);

            using (new ProfilingScope(null, ProfilingSampler.Get(camera.name)))
            {
                RenderStatus.SetupCameraProperties(data);
                Culling(camera, out RenderStatus.cullingResults);

                renderer.OnBeginCameraRendering();
                renderer.Execute();
#if UNITY_EDITOR
                if (UnityEditor.Handles.ShouldRenderGizmos())
                {
                    // Target.ResetViewProjectionMatrices();
                    RenderStatus.context.DrawWireOverlay(camera);
                    RenderStatus.context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
                    RenderStatus.context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
                }
#endif
                renderer.OnEndCameraRendering();
            }
            EndCameraRendering(RenderStatus.context, camera);
        }
        protected virtual void Culling(Camera camera, out CullingResults results)
        {
            camera.TryGetCullingParameters(false, out var parameters); //todo: 支持AR/XR
            parameters.isOrthographic = camera.orthographic;
            parameters.maximumVisibleLights = LightConstants.MaxVisibleLights;
            parameters.shadowDistance = Mathf.Clamp(LightConstants.shadowDistance, 0f, camera.farClipPlane);
            results = RenderStatus.context.Cull(ref parameters);
        }
        struct RenderData
        {
            public int Length;
            CameraData[] m_CameraDatas;
            public HashSet<CustomizedRender> ActiveRenders;
            public CameraData this[int index] => m_CameraDatas[index];

            public void Init(ScriptableRenderContext context, Camera[] cameras, CustomizedRenderPipeline pipeline)
            {
                Length = cameras.Length;

                Array.Sort(cameras, (camera1, camera2) => { return (int)camera1.depth - (int)camera2.depth; });
                RenderStatus.Init(context, pipeline);

                if (ActiveRenders == null) ActiveRenders = new HashSet<CustomizedRender>();
                else ActiveRenders.Clear();

                if (m_CameraDatas == null || m_CameraDatas.Length < cameras.Length) m_CameraDatas = new CameraData[Length];//与cameras一一对应
                for (int i = 0; i < Length; ++i)
                {
                    TryGetCameraData(pipeline.asset, cameras, i, out m_CameraDatas[i]);
                    ActiveRenders.Add(m_CameraDatas[i].renderer);
                }
            }
            void TryGetCameraData(CustomizedRenderPipelineAsset asset, Camera[] cameras, int idx, out CameraData data)
            {
                var camera = cameras[idx];
                camera.TryGetComponent<CameraSettings>(out var settings);
                data.camera = camera;
                data.cameras = cameras;
                data.settings = settings;
                data.isBaseCamera = (camera.clearFlags <= CameraClearFlags.Color);
                data.isOverlayCamera = !data.isBaseCamera; //suppose that skybox,color as base camera, depth or nothing as overlay camera
                data.isRenderingUI = (camera.cullingMask & KeywordIds.UILayer) != 0;
                data.renderer = asset.GetRenderer(settings == null ? -1 : settings.RendererIndex);
            }
        }
    }
}