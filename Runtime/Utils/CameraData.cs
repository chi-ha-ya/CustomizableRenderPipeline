using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomizablePipeline
{
    public struct CameraData
    {
        /// <summary>
        /// camera clearFlags is Skybox,Color or SolidColor
        /// </summary>
        public bool isBaseCamera;
        /// <summary>
        /// camera clearFlags is Depth or Nothing
        /// </summary>
        public bool isOverlayCamera;
        /// <summary>
        /// true if camera == cameras[0]
        /// </summary>
        // public bool isMainCamera;
        /// <summary>
        /// true if current camera rendering UI layer
        /// </summary>
        public bool isRenderingUI;
        /// <summary>
        /// 当前相机
        /// </summary>
        public Camera camera;
        /// <summary>
        /// 当前帧渲染的所有相机
        /// </summary>
        // public Camera[] cameras;
        /// <summary>
        /// setting attached on current camera
        /// </summary>
        public CameraSettings settings;
        /// <summary>
        /// renderer used by current camera
        /// </summary>
        public CustomizedRender renderer;
    }
}