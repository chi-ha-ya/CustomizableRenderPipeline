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
            if (cameras == null || cameras.Length <= 0) return;
            Array.Sort(cameras, (camera1, camera2) => { return (int)camera1.depth - (int)camera2.depth; });
            RenderStatus.context = context;
            RenderStatus.cameras = cameras;
            RenderStatus.pipeline = this;

            using (new ProfilingScope(null, KeywordStrings.CustomizedRP))
            {
                BeginFrameRendering(context, cameras);

                m_RenderData.Init(asset, cameras);

                foreach (var renderer in m_RenderData.ActiveRenders) renderer.OnBeginFrameRendering();

                for (int i = 0; i < m_RenderData.Length; ++i) RenderingSingleCamera(m_RenderData[i]);

                foreach (var renderer in m_RenderData.ActiveRenders) renderer.OnEndFrameRendering();

                EndFrameRendering(context, cameras);
            }
            context.Submit();
        }

        void RenderingSingleCamera(CameraData data)
        {
            RenderStatus.cameraData = data;
            var camera = data.camera;
            var renderer = data.renderer;
            BeginCameraRendering(RenderStatus.context, camera);

            using (new ProfilingScope(null, ProfilingSampler.Get(camera.name)))
            {
                renderer.OnBeginCameraRendering();

                SetupCameraProperties(camera);
                Culling(camera);
                renderer.RenderingSingleCamera();

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
        protected virtual void SetupCameraProperties(Camera camera)
        {
#if UNITY_EDITOR
            if (camera.cameraType == CameraType.SceneView)
            {
                ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
            }
            else if (camera.cameraType == CameraType.Preview)
            {
                camera.clearFlags = CameraClearFlags.Color;
                camera.backgroundColor = new Color32(0, 0, 0, 255);
            }
#endif
            // Target.SetupCameraProperties(ref data, Asset.RenderScale);
            RenderStatus.context.SetupCameraProperties(camera);//某些设置仍深埋在c++...
        }
        protected virtual void Culling(Camera camera)
        {
            camera.TryGetCullingParameters(false, out var cullingParameters); //todo: 支持AR/XR
            cullingParameters.isOrthographic = camera.orthographic;
            cullingParameters.maximumVisibleLights = LightConstants.MaxVisibleLights;
            cullingParameters.shadowDistance = Mathf.Clamp(LightConstants.shadowDistance, 0f, camera.farClipPlane);
            RenderStatus.cullingResults = RenderStatus.context.Cull(ref cullingParameters);
        }
        struct RenderData
        {
            public int Length;
            CameraData[] m_CameraDatas;
            public HashSet<CustomizedRender> ActiveRenders;
            public CameraData this[int index] => m_CameraDatas[index];

            public void Init(CustomizedRenderPipelineAsset asset, Camera[] cameras)
            {
                Length = cameras.Length;

                if (ActiveRenders == null) ActiveRenders = new HashSet<CustomizedRender>();
                else ActiveRenders.Clear();

                if (m_CameraDatas == null || m_CameraDatas.Length < cameras.Length) m_CameraDatas = new CameraData[Length];//与cameras一一对应
                for (int i = 0; i < Length; ++i)
                {
                    TryGetCameraData(asset, cameras[i], out m_CameraDatas[i]);
                }
            }
            void TryGetCameraData(CustomizedRenderPipelineAsset asset, Camera camera, out CameraData data)
            {
                camera.TryGetComponent<CameraSettings>(out data.settings);
                data.camera = camera;
                data.renderer = asset.GetRenderer(data.settings == null ? -1 : data.settings.RendererIndex);
                data.isBaseCamera = (camera.clearFlags <= CameraClearFlags.Color);
                data.isOverlayCamera = !data.isBaseCamera; //suppose that skybox,color as base camera, depth or nothing as overlay camera
                data.isRenderingUI = (camera.cullingMask & KeywordIds.UILayer) != 0;
                ActiveRenders.Add(data.renderer);
            }
        }
    }
}