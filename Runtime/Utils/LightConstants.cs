using System.Globalization;
using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomizablePipeline
{
    public class LightConstants
    {
        public static readonly int k_MaxCascades = 4; //shader中数组长度定死的
        public static int MaxVisibleLights = 32;
        public static int maxPerObjectLights = 8;
        public static int mainLightIndex = 0;
        // public static NativeArray<VisibleLight> lights;
        public static int additionalLightsCount = 0;
        public static bool mainLightShadowEnable = true;
        public static bool softShadow = true;
        // public static Vector4 shadowBias;
        public static Vector4 lightDirection;
        // public static Vector4 shadowParam;
        public static Vector4 shadowmapSize;
        public static int shadowCascade = 1;
        // public static float shadowStrength = 1f;
        public static float shadowDistance = 30f;//todo: pipeline 使用此参数做culling，更新此参数后下帧起效???
        public static Vector4 mainLightPosition = new Vector4(0, 0, 1, 0);
        public static Color mainLightColor = Color.black;
        public static Matrix4x4[] worldToShadow = new Matrix4x4[k_MaxCascades + 1]; //结尾多填一个无操作矩阵节省shader分支
        public static ShadowSplitData[] splitData = new ShadowSplitData[k_MaxCascades];
        static Vector4[] AdditionalLightPositions = new Vector4[MaxVisibleLights];
        static Vector4[] AdditionalLightColors = new Vector4[MaxVisibleLights];
        static Vector4[] AdditionalLightAttenuations = new Vector4[MaxVisibleLights];
        static Vector4[] AdditionalLightSpotDirections = new Vector4[MaxVisibleLights];
        static Vector4[] AdditionalLightOcclusionProbeChannels = new Vector4[MaxVisibleLights];
        public static void SetupLights(CommandBuffer command, ref CullingResults cullingResults, int cascadeCount, float shadowDistance, bool mainLightShadowEnable, bool softShadow)
        {
            LightConstants.shadowCascade = cascadeCount;
            LightConstants.shadowDistance = shadowDistance;
            LightConstants.mainLightShadowEnable = mainLightShadowEnable;
            LightConstants.softShadow = softShadow;
            LightConstants.SetupMainLight(command, cullingResults.visibleLights);
            LightConstants.SetupAdditionalLight(command, ref cullingResults);
        }

        public static void SetupMainLight(CommandBuffer command, NativeArray<VisibleLight> lights)
        {
            mainLightIndex = GetMainLightIndex(lights);
            if (mainLightIndex == -1) return;//No light in scene
            var mainLight = lights[mainLightIndex];
            if (mainLight.lightType == LightType.Directional)
            {
                lightDirection = -mainLight.localToWorldMatrix.GetColumn(2);
                lightDirection.w = 0f;
                mainLightPosition = lightDirection;
            }
            else
            {
                Vector4 pos = mainLight.localToWorldMatrix.GetColumn(3);
                mainLightPosition = new Vector4(pos.x, pos.y, pos.z, 1.0f);
            }
            mainLightColor = mainLight.finalColor;

            //To make the shadow fading fit into a single MAD instruction:
            //distanceCamToPixel2 * oneOverFadeDist + minusStartFade (single MAD)
            var m_MaxShadowDistance = shadowDistance * shadowDistance;
            var startFade = m_MaxShadowDistance * 0.9f;
            var oneOverFadeDist = 1 / (m_MaxShadowDistance - startFade);
            var minusStartFade = -startFade * oneOverFadeDist;
            var enableSoftShadow = softShadow && mainLight.light.shadows == LightShadows.Soft;
            var shadowParam = new Vector4(mainLight.light.shadowStrength, enableSoftShadow ? 1 : 0, oneOverFadeDist, minusStartFade);

            command.SetGlobalVector(KeywordIds._MainLightShadowParams, shadowParam);
            command.SetGlobalVector(KeywordIds._LightDirectionId, lightDirection);
            command.SetGlobalVector(KeywordIds._MainLightPosition, mainLightPosition);
            command.SetGlobalVector(KeywordIds._MainLightColor, mainLightColor);

            if (mainLightShadowEnable && (mainLight.light.shadows != LightShadows.None)) command.EnableShaderKeyword(KeywordStrings.MainLightShadows);
            else command.DisableShaderKeyword(KeywordStrings.MainLightShadows);

            if (shadowCascade > 1) command.EnableShaderKeyword(KeywordStrings.MainLightShadowCascades);
            else command.DisableShaderKeyword(KeywordStrings.MainLightShadowCascades);

            if (enableSoftShadow) command.EnableShaderKeyword(KeywordStrings.SoftShadows);
            else command.DisableShaderKeyword(KeywordStrings.SoftShadows);
        }
        public static void SetupAdditionalLight(CommandBuffer command, ref CullingResults cullingResults)
        {
            additionalLightsCount = SetupPerObjectLightIndices(command, ref cullingResults);
            var additionalLightsMode = RenderStatus.pipeline.asset.AdditionalLightsRenderingMode;
            var perObjectLightsLimit = RenderStatus.pipeline.asset.AdditionalLightsPerObjectLimit;
            var lights = cullingResults.visibleLights;
            var isShadowMaskEnable = false;
            if (additionalLightsCount > 0)
            {
                for (int i = 0, lightIter = 0; i < lights.Length && i < MaxVisibleLights; i++)
                {
                    var light = lights[i];
                    AdditionalLightPositions[lightIter] = new Vector4(0.0f, 0.0f, 1.0f, 0.0f);
                    AdditionalLightColors[lightIter] = Color.black;
                    AdditionalLightAttenuations[lightIter] = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
                    AdditionalLightSpotDirections[lightIter] = new Vector4(0, 0, 1, 0);
                    AdditionalLightOcclusionProbeChannels[lightIter] = new Vector4(-1.0f, 1.0f, -1.0f, -1.0f);

                    if (mainLightIndex != i)
                    {
                        AdditionalLightColors[lightIter] = light.finalColor;
                        if (light.lightType == LightType.Directional)
                        {
                            Vector4 dir = -light.localToWorldMatrix.GetColumn(2);
                            dir.w = 0;
                            AdditionalLightPositions[lightIter] = dir;
                        }
                        else
                        {
                            Vector4 pos = light.localToWorldMatrix.GetColumn(3);
                            pos.w = 1;
                            AdditionalLightPositions[lightIter] = pos;

                            float lightRangeSqr = light.range * light.range;
                            float fadeStartDistanceSqr = 0.8f * 0.8f * lightRangeSqr;
                            float fadeRangeSqr = (fadeStartDistanceSqr - lightRangeSqr);
                            float oneOverFadeRangeSqr = 1.0f / fadeRangeSqr;
                            float lightRangeSqrOverFadeRangeSqr = -lightRangeSqr / fadeRangeSqr;
                            float oneOverLightRangeSqr = 1.0f / Mathf.Max(0.0001f, light.range * light.range);

                            // On mobile and Nintendo Switch: Use the faster linear smoothing factor (SHADER_HINT_NICE_QUALITY).
                            // On other devices: Use the smoothing factor that matches the GI.
                            AdditionalLightAttenuations[lightIter].x = Application.isMobilePlatform || SystemInfo.graphicsDeviceType == GraphicsDeviceType.Switch ? oneOverFadeRangeSqr : oneOverLightRangeSqr;
                            AdditionalLightAttenuations[lightIter].y = lightRangeSqrOverFadeRangeSqr;
                        }

                        if (light.lightType == LightType.Spot)
                        {
                            Vector4 dir = light.localToWorldMatrix.GetColumn(2);
                            AdditionalLightSpotDirections[lightIter] = new Vector4(-dir.x, -dir.y, -dir.z, 0.0f);
                            // Spot Attenuation with a linear falloff can be defined as
                            // (SdotL - cosOuterAngle) / (cosInnerAngle - cosOuterAngle)
                            // This can be rewritten as
                            // invAngleRange = 1.0 / (cosInnerAngle - cosOuterAngle)
                            // SdotL * invAngleRange + (-cosOuterAngle * invAngleRange)
                            // If we precompute the terms in a MAD instruction
                            float cosOuterAngle = Mathf.Cos(Mathf.Deg2Rad * light.spotAngle * 0.5f);
                            // We neeed to do a null check for particle lights
                            // This should be changed in the future
                            // Particle lights will use an inline function
                            float cosInnerAngle;
                            if (light.light != null)
                                cosInnerAngle = Mathf.Cos(light.light.innerSpotAngle * Mathf.Deg2Rad * 0.5f);
                            else
                                cosInnerAngle = Mathf.Cos((2.0f * Mathf.Atan(Mathf.Tan(light.spotAngle * 0.5f * Mathf.Deg2Rad) * (64.0f - 18.0f) / 64.0f)) * 0.5f);
                            float smoothAngleRange = Mathf.Max(0.001f, cosInnerAngle - cosOuterAngle);
                            float invAngleRange = 1.0f / smoothAngleRange;
                            float add = -cosOuterAngle * invAngleRange;
                            AdditionalLightAttenuations[lightIter].z = invAngleRange;
                            AdditionalLightAttenuations[lightIter].w = add;
                        }

                        // Set the occlusion probe channel.
                        int occlusionProbeChannel = light.light.bakingOutput.occlusionMaskChannel;
                        AdditionalLightOcclusionProbeChannels[lightIter] = Vector4.zero;//10.x版URP默认为0

                        if (light != null
                         && light.light.bakingOutput.lightmapBakeType == LightmapBakeType.Mixed
                         && 0 <= occlusionProbeChannel && occlusionProbeChannel < 4)
                        {
                            AdditionalLightOcclusionProbeChannels[lightIter][occlusionProbeChannel] = 1.0f;
                        }
                        isShadowMaskEnable |=
                        light.light.bakingOutput.lightmapBakeType == LightmapBakeType.Mixed &&
                        light.light.shadows != LightShadows.None &&
                        light.light.bakingOutput.mixedLightingMode == MixedLightingMode.Shadowmask;

                        // If we have baked the light, the occlusion channel is the index we need to sample in 'unity_ProbesOcclusion'
                        // If we have not baked the light, the occlusion channel is -1.
                        // In case there is no occlusion channel is -1, we set it to zero, and then set the second value in the
                        // input to one. We then, in the shader max with the second value for non-occluded lights.
                        // AdditionalLightOcclusionProbeChannels[lightIter].x = occlusionProbeChannel == -1 ? 0f : occlusionProbeChannel;
                        // AdditionalLightOcclusionProbeChannels[lightIter].y = occlusionProbeChannel == -1 ? 1f : 0f;
                        ++lightIter;
                    }
                }
                command.SetGlobalVectorArray(KeywordIds._AdditionalLightsPosition, AdditionalLightPositions);
                command.SetGlobalVectorArray(KeywordIds._AdditionalLightsColor, AdditionalLightColors);
                command.SetGlobalVectorArray(KeywordIds._AdditionalLightsAttenuation, AdditionalLightAttenuations);
                command.SetGlobalVectorArray(KeywordIds._AdditionalLightsSpotDir, AdditionalLightSpotDirections);
                command.SetGlobalVectorArray(KeywordIds._AdditionalLightOcclusionProbeChannel, AdditionalLightOcclusionProbeChannels);
                command.SetGlobalVector(KeywordIds._AdditionalLightsCount, new Vector4(Mathf.Min(perObjectLightsLimit, maxPerObjectLights), 0.0f, 0.0f, 0.0f));
            }
            else command.SetGlobalVector(KeywordIds._AdditionalLightsCount, Vector4.zero);

            if (additionalLightsCount > 0 && additionalLightsMode == LightRenderingMode.PerVertex)
                command.EnableShaderKeyword(KeywordStrings.AdditionalLightsVertex);
            else command.DisableShaderKeyword(KeywordStrings.AdditionalLightsVertex);

            if (additionalLightsCount > 0 && additionalLightsMode == LightRenderingMode.PerPixel)
                command.EnableShaderKeyword(KeywordStrings.AdditionalLightsPixel);
            else command.DisableShaderKeyword(KeywordStrings.AdditionalLightsPixel);

            if (isShadowMaskEnable) command.EnableShaderKeyword(KeywordStrings.SHADOWS_SHADOWMASK);
            else command.DisableShaderKeyword(KeywordStrings.SHADOWS_SHADOWMASK);
        }
        static int SetupPerObjectLightIndices(CommandBuffer cmd, ref CullingResults cullingResults)
        {
            var visibleLights = cullingResults.visibleLights;
            var perObjectLightIndexMap = cullingResults.GetLightIndexMap(Allocator.Temp);
            int globalDirectionalLightsCount = 0;
            int additionalLights = 0;

            // Disable all directional lights from the perobject light indices
            // Pipeline handles main light globally and there's no support for additional directional lights atm.
            for (int i = 0; i < visibleLights.Length; ++i)
            {
                if (additionalLights >= MaxVisibleLights)
                    break;

                VisibleLight light = visibleLights[i];
                if (i == mainLightIndex)
                {
                    perObjectLightIndexMap[i] = -1;
                    ++globalDirectionalLightsCount;
                }
                else
                {
                    perObjectLightIndexMap[i] -= globalDirectionalLightsCount;
                    ++additionalLights;
                }
            }

            // Disable all remaining lights we cannot fit into the global light buffer.
            for (int i = globalDirectionalLightsCount + additionalLights; i < perObjectLightIndexMap.Length; ++i)
                perObjectLightIndexMap[i] = -1;

            cullingResults.SetLightIndexMap(perObjectLightIndexMap);
            perObjectLightIndexMap.Dispose();
            return additionalLights;
        }
        public static void SetupShadow(CommandBuffer cmd, RenderTargetHandle shadowmap)
        {
            cmd.SetGlobalTexture(shadowmap.id, shadowmap.Identifier());
            cmd.SetShadowSamplingMode(shadowmap.Identifier(), ShadowSamplingMode.CompareDepths);

            shadowmapSize = new Vector4(1f / shadowmap.rt.width, 1f / shadowmap.rt.height, shadowmap.rt.width, shadowmap.rt.height);
            cmd.SetGlobalVector(KeywordIds._ShadowmapSize, shadowmapSize);
            cmd.SetGlobalMatrixArray(KeywordIds._MainLightWorldToShadow, worldToShadow);

            cmd.SetGlobalVector(KeywordIds._CascadeShadowSplitSpheres0, splitData[0].cullingSphere);
            cmd.SetGlobalVector(KeywordIds._CascadeShadowSplitSpheres1, splitData[1].cullingSphere);
            cmd.SetGlobalVector(KeywordIds._CascadeShadowSplitSpheres2, splitData[2].cullingSphere);
            cmd.SetGlobalVector(KeywordIds._CascadeShadowSplitSpheres3, splitData[3].cullingSphere);

            cmd.SetGlobalVector(KeywordIds._CascadeShadowSplitSphereRadii, new Vector4(
                splitData[0].cullingSphere.w * splitData[0].cullingSphere.w,
                splitData[1].cullingSphere.w * splitData[1].cullingSphere.w,
                splitData[2].cullingSphere.w * splitData[2].cullingSphere.w,
                splitData[3].cullingSphere.w * splitData[3].cullingSphere.w)
            );

            if (softShadow)
            {
                var invHalfShadowAtlasWidth = shadowmapSize.x * 0.618f;
                var invHalfShadowAtlasHeight = shadowmapSize.y * 0.618f;
                cmd.SetGlobalVector(KeywordIds._ShadowOffset0, new Vector4(-invHalfShadowAtlasWidth, -invHalfShadowAtlasHeight, 0.0f, 0.0f));
                cmd.SetGlobalVector(KeywordIds._ShadowOffset1, new Vector4(invHalfShadowAtlasWidth, -invHalfShadowAtlasHeight, 0.0f, 0.0f));
                cmd.SetGlobalVector(KeywordIds._ShadowOffset2, new Vector4(-invHalfShadowAtlasWidth, invHalfShadowAtlasHeight, 0.0f, 0.0f));
                cmd.SetGlobalVector(KeywordIds._ShadowOffset3, new Vector4(invHalfShadowAtlasWidth, invHalfShadowAtlasHeight, 0.0f, 0.0f));
            }
        }
        // 投射阴影的主光源由rendersetting指定,否则选取最亮的平行光
        public static int GetMainLightIndex(NativeArray<VisibleLight> visibleLights)
        {
            int brightestIdx = -1;//if no light at all, return -1
            float brightestLightIntensity = 0f;
            for (var i = 0; i < visibleLights.Length; i++)
            {
                var visibleLight = visibleLights[i];
                var light = visibleLight.light;
                if (light == RenderSettings.sun) return i;
                if (visibleLight.lightType == LightType.Directional && light.intensity > brightestLightIntensity)
                {
                    brightestIdx = i;
                    brightestLightIntensity = light.intensity;
                }
            }
            return brightestIdx;
        }
        public static Vector4 GetShadowBias(ref VisibleLight shadowLight, ref Matrix4x4 lightProj, int shadowResolution, float shadowDepthBias, float shadowNormalBias)
        {
            float frustumSize;
            if (shadowLight.lightType == LightType.Directional)
            {
                // Frustum size is guaranteed to be a cube as we wrap shadow frustum around a sphere
                frustumSize = 2.0f / lightProj.m00;
            }
            else if (shadowLight.lightType == LightType.Spot)
            {
                // For perspective projections, shadow texel size varies with depth
                // It will only work well if done in receiver side in the pixel shader. Currently UniversalRP
                // do bias on caster side in vertex shader. When we add shader quality tiers we can properly
                // handle this. For now, as a poor approximation we do a constant bias and compute the size of
                // the frustum as if it was orthogonal considering the size at mid point between near and far planes.
                // Depending on how big the light range is, it will be good enough with some tweaks in bias
                frustumSize = Mathf.Tan(shadowLight.spotAngle * 0.5f * Mathf.Deg2Rad) * shadowLight.range;
            }
            else
            {
                Debug.LogWarning("For now Only spot and directional shadow casters are supported!!!");
                frustumSize = 0.0f;
            }

            // depth and normal bias scale is in shadowmap texel size in world space
            float texelSize = frustumSize / shadowResolution;
            float depthBias = -shadowDepthBias * texelSize;
            float normalBias = -shadowNormalBias * texelSize;

            // TODO: depth and normal bias assume sample is no more than 1 texel away from shadowmap
            // This is not true with PCF. Ideally we need to do either
            // cone base bias (based on distance to center sample)
            // or receiver place bias based on derivatives.
            // For now we scale it by the PCF kernel size (5x5)
            const float kernelRadius = 2.5f;
            // const float kernelRadius = 5f;
            depthBias *= kernelRadius;
            normalBias *= kernelRadius;
            return new Vector4(depthBias, normalBias, 0.0f, 0.0f);
        }

        public static Vector4 GetShadowBias(float frustumSize, int shadowResolution, float shadowDepthBias, float shadowNormalBias)
        {
            // depth and normal bias scale is in shadowmap texel size in world space
            float texelSize = frustumSize / shadowResolution;
            float depthBias = -shadowDepthBias * texelSize;
            float normalBias = -shadowNormalBias * texelSize;

            // TODO: depth and normal bias assume sample is no more than 1 texel away from shadowmap
            // This is not true with PCF. Ideally we need to do either
            // cone base bias (based on distance to center sample)
            // or receiver place bias based on derivatives.
            // For now we scale it by the PCF kernel size (5x5)
            const float kernelRadius = 2.5f;
            // const float kernelRadius = 5f;
            depthBias *= kernelRadius;
            normalBias *= kernelRadius;
            return new Vector4(depthBias, normalBias, 0.0f, 0.0f);
        }
    }
}