namespace CustomizablePipeline
{
    public enum RenderProcessEvent
    {
        //All Setup() in ExperimentalRenderProcess
        //OnFrameSetup
        BeforeFrameRendering = -10000,
        //OnCameraSetup
        //CustomCulling
        BeforeCameraRendering = -5000,
        BeforeRenderingShadows = -1000,
        RenderingShadows = -500,
        AfterRenderingShadows = -200,
        Default = 0,
        BeforeRenderingPrePasses = 200,
        RenderingPrePasses = 400,
        AfterRenderingPrePasses = 600,
        BeforeRenderingGbuffer = 800,
        RenderingGbuffer = 1000,
        AfterRenderingGbuffer = 1200,
        BeforeRenderingDeferredLights = 1500,
        RenderingDeferredLights = 1600,
        AfterRenderingDeferredLights = 1700,
        BeforeRenderingOpaques = 1800,
        RenderingOpaques = 2000,
        AfterRenderingOpaques = 2200,
        BeforeRenderingTransparentCutout = 2400,
        RenderingTransparentCutout = 2450,
        AfterRenderingTransparentCutout = 2500,
        BeforeRenderingSkybox = 2600,
        RenderingSkybox = 2700,
        AfterRenderingSkybox = 2800,
        BeforeRenderingTransparents = 2900,
        RenderingTransparents = 3000,
        AfterRenderingTransparents = 3200,
        BeforeRenderingPostProcessing = 3600,
        RenderingPostProcessing = 4000,
        AfterRenderingPostProcessing = 4200,
        AfterCameraRendering = 5000,
        //OnCameraCleanup
        AfterFrameRendering = 10000,
        //OnFrameCleanup
    }
}
