using UnityEngine;

namespace CustomizablePipeline
{
    public partial class KeywordIds
    {
        public static int UILayer = 1 << LayerMask.NameToLayer("UI");
        public static int _SourceTexId = Shader.PropertyToID(KeywordStrings._SourceTex);
        public static int _DepthAttachment = Shader.PropertyToID(KeywordStrings._DepthAttachment);
        public static int _DepthAttachmentMS = Shader.PropertyToID(KeywordStrings._DepthAttachmentMS);
        public static int _ShadowBiasId = Shader.PropertyToID(KeywordStrings._ShadowBias);
        public static int _LightDirectionId = Shader.PropertyToID(KeywordStrings._LightDirection);
        public static int _Scatter = Shader.PropertyToID("_Scatter");
        public static int _MainLightPosition = Shader.PropertyToID("_MainLightPosition");
        public static int _MainLightColor = Shader.PropertyToID("_MainLightColor");
        public static int _AdditionalLightsCount = Shader.PropertyToID("_AdditionalLightsCount");
        public static int _AdditionalLightsPosition = Shader.PropertyToID("_AdditionalLightsPosition");
        public static int _AdditionalLightsColor = Shader.PropertyToID("_AdditionalLightsColor");
        public static int _AdditionalLightsAttenuation = Shader.PropertyToID("_AdditionalLightsAttenuation");
        public static int _AdditionalLightsSpotDir = Shader.PropertyToID("_AdditionalLightsSpotDir");
        public static int _AdditionalLightOcclusionProbeChannel = Shader.PropertyToID("_AdditionalLightsOcclusionProbes");

        public static int _MainLightWorldToShadow = Shader.PropertyToID("_MainLightWorldToShadow");
        public static int _MainLightShadowParams = Shader.PropertyToID("_MainLightShadowParams");
        public static int _CascadeShadowSplitSpheres0 = Shader.PropertyToID("_CascadeShadowSplitSpheres0");
        public static int _CascadeShadowSplitSpheres1 = Shader.PropertyToID("_CascadeShadowSplitSpheres1");
        public static int _CascadeShadowSplitSpheres2 = Shader.PropertyToID("_CascadeShadowSplitSpheres2");
        public static int _CascadeShadowSplitSpheres3 = Shader.PropertyToID("_CascadeShadowSplitSpheres3");
        public static int _CascadeShadowSplitSphereRadii = Shader.PropertyToID("_CascadeShadowSplitSphereRadii");
        public static int _ShadowOffset0 = Shader.PropertyToID("_MainLightShadowOffset0");
        public static int _ShadowOffset1 = Shader.PropertyToID("_MainLightShadowOffset1");
        public static int _ShadowOffset2 = Shader.PropertyToID("_MainLightShadowOffset2");
        public static int _ShadowOffset3 = Shader.PropertyToID("_MainLightShadowOffset3");
        public static int _ShadowmapSize = Shader.PropertyToID("_MainLightShadowmapSize");

        public static int _InvCameraViewProj = Shader.PropertyToID("_InvCameraViewProj");
        public static int _ScreenParams = Shader.PropertyToID("_ScreenParams");
        public static int _ScaledScreenParams = Shader.PropertyToID("_ScaledScreenParams");
        public static int _WorldSpaceCameraPos = Shader.PropertyToID("_WorldSpaceCameraPos");
        public static int _ZBufferParams = Shader.PropertyToID("_ZBufferParams");
        public static int unity_OrthoParams = Shader.PropertyToID("unity_OrthoParams");
        public static int _ProjectionParams = Shader.PropertyToID("_ProjectionParams");

        public static int unity_CameraProjection = Shader.PropertyToID("unity_CameraProjection");
        public static int unity_CameraInvProjection = Shader.PropertyToID("unity_CameraInvProjection");
        public static int unity_WorldToCamera = Shader.PropertyToID("unity_WorldToCamera");
        public static int unity_CameraToWorld = Shader.PropertyToID("unity_CameraToWorld");
        public static int viewMatrix = Shader.PropertyToID("unity_MatrixV");
        public static int glstate_matrix_projection = Shader.PropertyToID("glstate_matrix_projection");
        public static int viewAndProjectionMatrix = Shader.PropertyToID("unity_MatrixVP");

        public static int inverseViewMatrix = Shader.PropertyToID("unity_MatrixInvV");
        public static int inverseProjectionMatrix = Shader.PropertyToID("unity_MatrixInvP");
        public static int inverseViewAndProjectionMatrix = Shader.PropertyToID("unity_MatrixInvVP");

        public static int _DoFParams = Shader.PropertyToID("_DoFParams");
        public static int _UserLut = Shader.PropertyToID("_UserLut");
        public static int _UserLutParams = Shader.PropertyToID("_UserLutParams");
        public static int _InternalLutParams = Shader.PropertyToID("_InternalLutParams");
        public static int _GlassMorphismAmount = Shader.PropertyToID("_GlassMorphismAmount");
        public static int _ScreenDisturbTex = Shader.PropertyToID("_ScreenDisturbTex");
        public static int _ScreenDisturbParams = Shader.PropertyToID("_ScreenDisturbParams");
        public static int _RadialBlurCenter = Shader.PropertyToID("_RadialBlurCenter");
        public static int _RadialBlurParams1 = Shader.PropertyToID("_RadialBlurParams1");
        public static int _RadialBlurParams2 = Shader.PropertyToID("_RadialBlurParams2");
        public static int _InvertAmount = Shader.PropertyToID("_InvertAmount");
        public static int _ChromaAmount = Shader.PropertyToID("_ChromaAmount");
        public static int _FilmGrainTex = Shader.PropertyToID("_FilmGrainTex");
        public static int _GrainColor = Shader.PropertyToID("_GrainColor");
        public static int _GrainParams = Shader.PropertyToID("_GrainParams");
        public static int _GrainTiling = Shader.PropertyToID("_GrainTiling");
        public static int _VignetteColor = Shader.PropertyToID("_VignetteColor");
        public static int _Vignette_Params = Shader.PropertyToID("_Vignette_Params");
        public static int _LutBuilderParameters = Shader.PropertyToID("_Lut_Params");
        public static int _ColorBalance = Shader.PropertyToID("_ColorBalance");
        public static int _ColorFilter = Shader.PropertyToID("_ColorFilter");
        public static int _ChannelMixerRed = Shader.PropertyToID("_ChannelMixerRed");
        public static int _ChannelMixerGreen = Shader.PropertyToID("_ChannelMixerGreen");
        public static int _ChannelMixerBlue = Shader.PropertyToID("_ChannelMixerBlue");
        public static int _HueSatCon = Shader.PropertyToID("_HueSatCon");
        public static int _GrayScaleParams = Shader.PropertyToID("_GrayScaleParams");
        public static int _Lift = Shader.PropertyToID("_Lift");
        public static int _Gamma = Shader.PropertyToID("_Gamma");
        public static int _Gain = Shader.PropertyToID("_Gain");
        public static int _Shadows = Shader.PropertyToID("_Shadows");
        public static int _Midtones = Shader.PropertyToID("_Midtones");
        public static int _Highlights = Shader.PropertyToID("_Highlights");
        public static int _ShaHiLimits = Shader.PropertyToID("_ShaHiLimits");
        public static int _SplitShadows = Shader.PropertyToID("_SplitShadows");
        public static int _SplitHighlights = Shader.PropertyToID("_SplitHighlights");

        public static int _CurveMaster = Shader.PropertyToID("_CurveMaster");
        public static int _CurveRed = Shader.PropertyToID("_CurveRed");
        public static int _CurveGreen = Shader.PropertyToID("_CurveGreen");
        public static int _CurveBlue = Shader.PropertyToID("_CurveBlue");
        public static int _CurveHueVsHue = Shader.PropertyToID("_CurveHueVsHue");
        public static int _CurveHueVsSat = Shader.PropertyToID("_CurveHueVsSat");
        public static int _CurveLumVsSat = Shader.PropertyToID("_CurveLumVsSat");
        public static int _CurveSatVsSat = Shader.PropertyToID("_CurveSatVsSat");
        public static int _Time = Shader.PropertyToID("_Time");
        public static int _SinTime = Shader.PropertyToID("_SinTime");
        public static int _CosTime = Shader.PropertyToID("_CosTime");
        public static int unity_DeltaTime = Shader.PropertyToID("unity_DeltaTime");
        public static int _TimeParameters = Shader.PropertyToID("_TimeParameters");
        public static int _GlossyEnvironmentColor = Shader.PropertyToID("_GlossyEnvironmentColor");
        public static int _SubtractiveShadowColor = Shader.PropertyToID("_SubtractiveShadowColor");

        public static int _PostExposure = Shader.PropertyToID("_PostExposure");
    }
}