using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Rendering;

namespace CustomizablePipeline
{
    [CustomEditor(typeof(CustomizedRenderPipelineAsset), true)]
    public class CustomizedRenderPipelineAssetEditor : Editor
    {
        CustomizedRenderPipelineAsset asset;
        RenderPipelineAsset other = null;
        bool active;
        bool m_TargetSettingsFoldout = false;
        bool m_GeneralSettingsFoldout = false;
        bool m_LightingSettingsFoldout = false;
        // bool m_ShadowSettingsFoldout = false;
        // bool m_ProcessSettingsFoldout = false;
        // SerializedProperty m_isDirty;//a trick to rebuild pipeline by change this value...
        PropertyContent m_RenderTargetMode;
        // PropertyContent m_SV_TargetFormats;
        // PropertyContent m_URPShaderCompatible;
        // PropertyContent m_UseCustomizedRenderPipelineBatching;
        // PropertyContent m_LightsUseLinearIntensity;
        // // PropertyContent m_DepthBufferBits;
        // PropertyContent m_Msaa;
        // PropertyContent m_Shaders;
        // PropertyContent m_RenderScale;
        // PropertyContent m_AdditionalLightsRenderingMode;
        // PropertyContent m_AdditionalLightsPerObjectLimit;
        PropertyContent m_Renderers;
        SerializedProperty m_DefaultRendererIndex;
        void OnEnable()
        {
            asset = target as CustomizedRenderPipelineAsset;
            active = asset == GraphicsSettings.renderPipelineAsset;
            // if (!active) other = GraphicsSettings.renderPipelineAsset;

            //     // m_isDirty = serializedObject.FindProperty(nameof(CustomizedRenderPipelineAsset.isDirty));
            //     InitProp(ref m_URPShaderCompatible, nameof(CustomizedRenderPipelineAsset.URPShaderCompatible), "是否兼容URP的shader");
            //     InitProp(ref m_UseCustomizedRenderPipelineBatching, nameof(CustomizedRenderPipelineAsset.UseCustomizedRenderPipelineBatching), "Enable/Disable SRP batcher");
            //     InitProp(ref m_LightsUseLinearIntensity, nameof(CustomizedRenderPipelineAsset.LightsUseLinearIntensity), "Lights Use Linear Intensity");
            //     InitProp(ref m_RenderTargetMode, nameof(CustomizedRenderPipelineAsset.TargetMode), "渲染模式设置");
            //     InitProp(ref m_SV_TargetFormats, nameof(CustomizedRenderPipelineAsset.SV_TargetFormats), "渲染模式设置");
            //     // InitProp(ref m_DepthBufferBits, nameof(CustomizedRenderPipelineAsset.DepthBufferBits), "深度/模板 缓冲位数");

            //     InitProp(ref m_Msaa, nameof(CustomizedRenderPipelineAsset.MsaaQuality), "MSAA QualityFindProperty");
            //     InitProp(ref m_Shaders, nameof(CustomizedRenderPipelineAsset.Shaders), "这些shader将随此Asset一起打包");
            //     InitProp(ref m_RenderScale, nameof(CustomizedRenderPipelineAsset.RenderScale), "渲染分辨率缩放");
            //     InitProp(ref m_AdditionalLightsRenderingMode, nameof(CustomizedRenderPipelineAsset.AdditionalLightsRenderingMode), "");
            //     InitProp(ref m_AdditionalLightsPerObjectLimit, nameof(CustomizedRenderPipelineAsset.AdditionalLightsPerObjectLimit), "Limit of additional lights. These lights are sorted and culled per-object.");
            InitProp(ref m_Renderers, nameof(CustomizedRenderPipelineAsset.Renderers), "Renderers");
            m_DefaultRendererIndex = serializedObject.FindProperty(nameof(CustomizedRenderPipelineAsset.DefaultRendererIndex));
            //     // EditorCoreUtils.SetEditor(asset, this);
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            DrawBasicSettings();
            //     DrawGeneralSettings();
            //     DrawTargetSettings();
            //     DrawLightingSettings();
            //     DrawResourceSettings();
            DrawRenderSettings();
            if (EditorGUI.EndChangeCheck()) HasChanged();
        }
        void InitProp(ref PropertyContent prop, string findName, string tooltip = null, string showName = null)
        {
            if (prop == null)
            {
                var content = EditorGUIUtility.TrTextContent(string.IsNullOrEmpty(showName) ? findName : showName, tooltip);
                var property = serializedObject.FindProperty(findName);
                prop = new PropertyContent(content, property);
            }
        }
        private void HasChanged()
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
            // AssetDatabase.SaveAssets();
            // AssetDatabase.Refresh();
        }
        // private void DrawResourceSettings()
        // {
        //     m_Shaders.PropertyField();
        // }
        void DrawRenderer(int index, ref SerializedProperty property)
        {
            var renderer = property.objectReferenceValue;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(index.ToString(), EditorStyles.boldLabel, GUILayout.MaxWidth(20));
            EditorGUILayout.ObjectField((SerializedProperty)property, TrTextContent.None);

            GUILayout.FlexibleSpace();

            bool isDefault = index == m_DefaultRendererIndex.intValue;
            var color = GUI.backgroundColor;
            ColorUtility.TryParseHtmlString("#66CCFF", out var TianYiLan);//【【洛天依MMD】 .--  ..  ..-.  .】 https://www.bilibili.com/video/BV1hB4y1k7c8?share_source=copy_web&vd_source=897cef876071534f1ef0f9058219905f
            GUI.backgroundColor = isDefault ? TianYiLan : color;
            if (GUILayout.Button(isDefault ? TrTextContent.Default : TrTextContent.SetDefault, GUILayout.MaxWidth(80)))
            {
                m_DefaultRendererIndex.intValue = index;
                HasChanged();
            }
            GUI.backgroundColor = color;

            if (GUILayout.Button(TrTextContent.Menu, GUILayout.MaxWidth(20)))
            {
                var menu = new GenericMenu();
                if (index == 0)
                    menu.AddDisabledItem(TrTextContent.MoveUp);
                else
                    menu.AddItem(TrTextContent.MoveUp, false, () => MoveComponent(index, -1));

                if (index == m_Renderers.property.arraySize - 1)
                    menu.AddDisabledItem(TrTextContent.MoveDown);
                else
                    menu.AddItem(TrTextContent.MoveDown, false, () => MoveComponent(index, 1));

                menu.AddSeparator(string.Empty);
                if (m_Renderers.property.arraySize == 1)
                    menu.AddDisabledItem(TrTextContent.Remove);
                else
                    menu.AddItem(TrTextContent.Remove, false, () => RemoveComponent(index));
                menu.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
                // menu.ShowAsContext();

                // if (renderer) Selection.SetActiveObjectWithContext(renderer, null);
                // else EditorGUIUtility.ShowObjectPicker<CustomizedRenderer>(null, false, null, index);
            }
            // // If object selector chose an object, assign it to the correct CustomizedRendererData slot.
            // if (Event.current.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == index)
            // {
            //     property.objectReferenceValue = EditorGUIUtility.GetObjectPickerObject();
            // }
            EditorGUILayout.EndHorizontal();
        }
        void MoveComponent(int id, int offset)
        {
            Undo.SetCurrentGroupName("Move Renderer");
            var dest = id + offset;
            m_Renderers.property.MoveArrayElement(id, dest);
            var defaultIdx = m_DefaultRendererIndex.intValue;
            if (defaultIdx == dest) m_DefaultRendererIndex.intValue = id;
            if (defaultIdx == id) m_DefaultRendererIndex.intValue = dest;

            HasChanged();
        }
        void RemoveComponent(int id)
        {
            var property = m_Renderers.property.GetArrayElementAtIndex(id);
            var component = property.objectReferenceValue;
            property.objectReferenceValue = null;

            Undo.SetCurrentGroupName(component == null ? "Remove Renderer" : $"Remove {component.name}");

            // remove the array index itself from the list
            m_Renderers.property.DeleteArrayElementAtIndex(id);
            var defaultIdx = m_DefaultRendererIndex.intValue;
            if (id < defaultIdx) m_DefaultRendererIndex.intValue = defaultIdx - 1;

            HasChanged();
        }
        void DrawRenderSettings()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(TrTextContent.Renderer, EditorStyles.boldLabel);
            // EditorGUILayout.Space();

            if (m_Renderers.property.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No Renderer added", MessageType.Info);
            }
            else
            {
                //Draw List
                EditorCoreUtils.DrawSplitter();
                for (int i = 0; i < m_Renderers.property.arraySize; i++)
                {
                    SerializedProperty property = m_Renderers.property.GetArrayElementAtIndex(i);
                    DrawRenderer(i, ref property);
                    EditorCoreUtils.DrawSplitter();
                }
            }
            EditorGUILayout.Space();

            //Add renderer
            if (GUILayout.Button("Add Renderer", EditorStyles.miniButton))
            {
                serializedObject.Update();
                if (m_Renderers.property.arraySize == 0)
                {
                    var str = AssetDatabase.GetAssetPath(asset);
                    // Load or create a new one
                    var path = EditorUtility.SaveFilePanel("Create New Customized Renderer", Path.GetDirectoryName(str), "CustomizablePipelineRenderer", "asset");

                    if (!string.IsNullOrEmpty(path))
                    {
                        CustomizedRender renderer = null;
                        if (path.Contains("Assets")) path = path.Substring(path.LastIndexOf("Assets"));
                        else if (path.Contains("Packages")) path = path.Substring(path.LastIndexOf("Packages"));
                        if (File.Exists(path))
                        {
                            renderer = AssetDatabase.LoadAssetAtPath<CustomizedRender>(path);
                        }
                        else
                        {
                            renderer = ScriptableObject.CreateInstance<CustomizedRender>();
                            AssetDatabase.CreateAsset(renderer, path);
                        }
                        if (renderer)
                        {
                            m_Renderers.property.InsertArrayElementAtIndex(0);
                            var prop = m_Renderers.property.GetArrayElementAtIndex(0);
                            prop.objectReferenceValue = renderer;
                            // asset.Renderers.Add(renderer);
                            // serializedObject.ApplyModifiedProperties();
                            HasChanged();
                        }
                    }
                }
                else
                {
                    var index = m_Renderers.property.arraySize;
                    m_Renderers.property.InsertArrayElementAtIndex(index);
                }
            }
            // if (false)
            // {
            //     if (asset.DefaultRenderer)
            //     {
            //         var editor = EditorCoreUtils.GetEditor(asset.DefaultRenderer);
            //         editor.OnInspectorGUI();//显示renderer面板
            //     }
            // }
        }
        // void DrawTargetSettings()
        // {
        //     m_TargetSettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_TargetSettingsFoldout, TrTextContent.Target);
        //     if (m_TargetSettingsFoldout)
        //     {
        //         EditorGUI.indentLevel++;
        //         EditorGUILayout.Space();

        //         var changed = false;
        //         var mode = (RenderTargetMode)EditorGUILayout.EnumPopup(m_RenderTargetMode.content, asset.TargetMode);
        //         if (mode != asset.TargetMode)
        //         {
        //             asset.TargetMode = mode;
        //             changed = true;
        //         }
        //         var n = (int)mode;
        //         var formats = asset.SV_TargetFormats;
        //         var c = formats.Count;
        //         if (n != c)
        //         {
        //             if (n < c) formats.RemoveRange(n - 1, c - n);
        //             else for (int i = c; i < n; ++i) formats.Add(RenderTextureFormat.RGB111110Float);
        //             changed = true;
        //         }
        //         for (int i = 0; i < n; ++i)
        //         {
        //             var format = (RenderTextureFormat)EditorGUILayout.EnumPopup($"SV_Target{i}_Format", formats[i]);
        //             if (format != formats[i])
        //             {
        //                 formats[i] = format;
        //                 changed = true;
        //             }
        //         }
        //         if (changed) { serializedObject.Update(); }

        //         EditorGUI.BeginChangeCheck();
        //         // m_DepthBufferBits.PropertyField();
        //         m_RenderScale.Slider(0.5f, 4f);
        //         m_Msaa.PropertyField();

        //         if (EditorGUI.EndChangeCheck()) HasChanged();

        //         EditorGUI.indentLevel--;
        //         EditorGUILayout.Space();
        //     }
        //     EditorGUILayout.EndFoldoutHeaderGroup();
        // }
        // void DrawGeneralSettings()
        // {
        //     m_GeneralSettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_GeneralSettingsFoldout, TrTextContent.General);
        //     if (m_GeneralSettingsFoldout)
        //     {
        //         EditorGUI.indentLevel++;
        //         EditorGUILayout.Space();
        //         m_URPShaderCompatible.PropertyField();
        //         m_UseCustomizedRenderPipelineBatching.PropertyField();
        //         m_LightsUseLinearIntensity.PropertyField();

        //         // m_Msaa.PropertyField();
        //         // m_GammaSpaceUI.PropertyField();
        //         // m_ForceReversedZBuffer.PropertyField();
        //         // m_ColorGradingDontAffectParticleLayer.PropertyField();

        //         EditorGUI.indentLevel--;
        //         EditorGUILayout.Space();
        //     }
        //     EditorGUILayout.EndFoldoutHeaderGroup();
        // }
        // void DrawLightingSettings()
        // {
        //     m_LightingSettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_LightingSettingsFoldout, TrTextContent.Lighting);
        //     if (m_LightingSettingsFoldout)
        //     {
        //         EditorGUI.indentLevel++;
        //         EditorGUILayout.Space();

        //         m_AdditionalLightsRenderingMode.PropertyField();
        //         var disableFlg = m_AdditionalLightsRenderingMode.property.intValue == (int)LightRenderingMode.Disabled;
        //         EditorGUI.BeginDisabledGroup(disableFlg);
        //         m_AdditionalLightsPerObjectLimit.IntSlider(0, 8);
        //         EditorGUI.EndDisabledGroup();

        //         EditorGUI.indentLevel--;
        //         EditorGUILayout.Space();
        //     }
        //     EditorGUILayout.EndFoldoutHeaderGroup();
        // }
        // List<string> shaderIds;
        // string GraphicsSettingsPath;
        void DrawBasicSettings()
        {
            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                var activeAsset = GraphicsSettings.renderPipelineAsset;

                active = activeAsset == asset;
                var text = active ? TrTextContent.Disable : TrTextContent.Enable;
                var toggle = GUILayout.Toggle(active, text, EditorStyles.toolbarButton);
                if (toggle != active)
                {
                    if (toggle)
                    {
                        if (!active) other = GraphicsSettings.renderPipelineAsset;
                        GraphicsSettings.renderPipelineAsset = asset;
                    }
                    else
                    {
                        GraphicsSettings.renderPipelineAsset = other;
                    }
                    // EditorCoreUtils.RemoveAllCachedEditor();
                }
            }
            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Button("Take a Screen Capture", EditorStyles.toolbarButton))
                {
                    var path = EditorUtility.SaveFilePanel("Save Screen Capture As PNG", "Assets", $"ScreenCapture{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}", "png");
                    if (!string.IsNullOrEmpty(path))
                    {
                        // ScreenCapture.CaptureScreenshot(path);
                        // var w = CustomizablePipeline.Instance.Target.descriptor.width;
                        // var h = CustomizablePipeline.Instance.Target.descriptor.height;
                        // Texture2D capture = new Texture2D(w, h, TextureFormat.RGB24, 0, true);
                        // capture.ReadPixels(new Rect(0, 0, w, h), 0, 0);
                        // var png = capture.EncodeToPNG();
                        // System.IO.File.WriteAllBytes(path, png);
                        // AssetDatabase.Refresh();
                    }
                }
            }
        }
        void OnDisable()
        {
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }

        // [MenuItem("Assets/Create/Rendering/CRP/CustomizedProcess")]
        // internal static void CreateNewCustomizedProcess()
        // {
        //     string templatePath = AssetDatabase.GUIDToAssetPath("0df9b854bc7d831489ff81d5c968aeb0");
        //     ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewCustomizedProcess.cs");
        // }

        // [MenuItem("Assets/Create/Rendering/CRP/CustomizedProcessEditor")]
        // internal static void CreateNewCustomizedProcessEditor()
        // {
        //     string templatePath = AssetDatabase.GUIDToAssetPath("1bc775ecab03a5d47b355864aca0814d");
        //     ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewCustomizedProcessEditor.cs");
        // }

        // [MenuItem("Assets/Create/Rendering/CRP/Shader/Lit")]
        // internal static void CreateNewLitShader()
        // {
        //     string templatePath = AssetDatabase.GUIDToAssetPath("bf1a30ee33a4d85479cd87bfa19b40d9");
        //     ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewLitShader.shader");
        // }

        // [MenuItem("Assets/Create/Rendering/CRP/Shader/UnLit")]
        // internal static void CreateNewUnLitShader()
        // {
        //     string templatePath = AssetDatabase.GUIDToAssetPath("868fb0ed438f2aa448630cb18ed6e796");
        //     ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewUnLitShader.shader");
        // }

        // [MenuItem("Assets/Create/Rendering/CRP/Shader/PostProcess")]
        // internal static void CreateNewPostProcessShader()
        // {
        //     string templatePath = AssetDatabase.GUIDToAssetPath("ca64bc7a6a440074c861ec725f5cd5fd");
        //     ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewPostProcessShader.shader");
        // }

        [MenuItem("Assets/Create/Rendering/CRP/CustomizedRenderer")]
        internal static void CreateNewCustomizedRenderer()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, CreateInstance<CreateCustomPipelineRenderer>(), "NewCustomizedRenderer.asset", null, null);
        }

        class CreateCustomPipelineRenderer : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                var instance = CreateInstance<CustomizedRender>();
                AssetDatabase.CreateAsset(instance, pathName);
            }
        }

        [MenuItem("Assets/Create/Rendering/CRP/RenderPipelineAsset")]
        internal static void CreatePipelineAsset()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, CreateInstance<CreateCustomPipelineAsset>(), "CustomizedPipelineAsset.asset", null, null);
        }

        class CreateCustomPipelineAsset : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                var instance = CreateInstance<CustomizedRenderPipelineAsset>();
                AssetDatabase.CreateAsset(instance, pathName);
            }
        }

        // [MenuItem("Graphic/PinCurrentPipelineAsset", false, -99999)]
        // public static void PinCurrentPipelineAsset()
        // {
        //     var asset = GraphicsSettings.currentRenderPipeline;
        //     UnityEditor.EditorUtility.FocusProjectWindow();
        //     EditorGUIUtility.PingObject(asset);
        //     // UnityEditor.Selection.activeObject = asset;
        // }
    }
}