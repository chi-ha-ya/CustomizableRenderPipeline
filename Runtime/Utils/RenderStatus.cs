using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Runtime.CompilerServices;

namespace CustomizablePipeline
{
    internal struct RenderStatus
    {
        public static RenderTarget target;
        public static ScriptableRenderContext context;
        public static CameraData cameraData;
        public static CullingResults cullingResults;
        public static CustomizedRenderPipeline pipeline;

        internal static void Init(ScriptableRenderContext context, CustomizedRenderPipeline pipeline)
        {
            RenderStatus.context = context;
            RenderStatus.pipeline = pipeline;
        }
        internal static void SetupCameraProperties(CameraData data)
        {
            RenderStatus.cameraData = data;
            var camera = data.camera;
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
            context.SetupCameraProperties(camera);//某些设置仍深埋在c++...
        }

        /// <summary>
        /// submit a command buffer to context
        /// </summary>
        /// <param name="command">command buffer to execute</param>
        /// <param name="clear">true if the command need to be clear after execute</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Commit(CommandBuffer command, bool clear = true)
        {
            context.ExecuteCommandBuffer(command);
            if (clear) command.Clear();
        }
    }
}