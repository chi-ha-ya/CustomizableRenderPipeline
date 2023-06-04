using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using UnityEngine.Rendering;

namespace CustomizablePipeline
{
    public class EditorCoreUtils
    {
        public static readonly string packagePath = "Packages/com.seed.render-pipelines";
        static Dictionary<ScriptableObject, Editor> EditorPool = new Dictionary<ScriptableObject, Editor>();
        public static Editor GetEditor(ScriptableObject o)
        {
            Editor editor = null;
            if (EditorPool.ContainsKey(o)) editor = EditorPool[o];
            else
            {
                editor = Editor.CreateEditor(o);
                EditorPool.Add(o, editor);
            }
            return editor;
        }
        public static void SetEditor(ScriptableObject o, Editor e)
        {
            if (!EditorPool.ContainsKey(o)) EditorPool.Add(o, e);
            else EditorPool[o] = e;
        }
        public static Editor RemoveEditor(ScriptableObject o)
        {
            Editor editor = null;
            if (EditorPool.ContainsKey(o))
            {
                EditorPool.TryGetValue(o, out editor);
                EditorPool.Remove(o);
            }
            return editor;
        }
        public static void RemoveAllCachedEditor()
        {
            EditorPool.Clear();
        }
        public static void RefreshSceneView()
        {
            EditorWindow.GetWindow<SceneView>().Repaint();
            EditorWindow.GetWindow(Type.GetType("UnityEditor.GameView,UnityEditor")).Repaint();
            SceneView.RepaintAll();
            HandleUtility.Repaint();
        }
        /// <summary>
        /// Destroys a UnityEngine.Object safely.
        /// </summary>
        /// <param name="obj">Object to be destroyed.</param>
        public static void Destroy(UnityEngine.Object obj)
        {
            if (obj != null)
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                    UnityEngine.Object.Destroy(obj);
                else
                    UnityEngine.Object.DestroyImmediate(obj);
#else
                UnityEngine.Object.Destroy(obj);
#endif
            }
        }
        public static void DrawSplitter(bool isBoxed = false)
        {
            var rect = GUILayoutUtility.GetRect(1f, 1f);
            float xMin = rect.xMin;

            // Splitter rect should be full-width
            rect.xMin = 0f;
            rect.width += 4f;

            if (isBoxed)
            {
                rect.xMin = xMin == 7.0 ? 4.0f : EditorGUIUtility.singleLineHeight * 2f;
                rect.width -= EditorGUIUtility.singleLineHeight;
            }

            if (Event.current.type != EventType.Repaint) return;
            EditorGUI.DrawRect(rect, !EditorGUIUtility.isProSkin
                ? new Color(0.6f, 0.6f, 0.6f, 1.333f)
                : new Color(0.12f, 0.12f, 0.12f, 1.333f));
        }

        public static (bool, bool) DrawHeaderToggle(bool foldout, bool toggle, GUIContent label, Action contentGUI)
        {
            var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

            var labelRect = backgroundRect;
            labelRect.xMin += 32f;
            labelRect.xMax -= 20f + 16 + 5;

            var foldoutRect = backgroundRect;
            foldoutRect.y += 1f;
            foldoutRect.width = 13f;
            foldoutRect.height = 13f;

            var toggleRect = backgroundRect;
            toggleRect.x += 16f;
            toggleRect.y += 2f;
            toggleRect.width = 13f;
            toggleRect.height = 13f;

            EditorGUI.LabelField(labelRect, label, EditorStyles.boldLabel);
            foldout = GUI.Toggle(foldoutRect, foldout, GUIContent.none, EditorStyles.foldout);
            toggle = GUI.Toggle(toggleRect, toggle, GUIContent.none, new GUIStyle("ShurikenToggle"));

            if (foldout)
            {
                using (new EditorGUI.DisabledScope(!toggle))
                {
                    EditorGUI.indentLevel++;
                    contentGUI();
                    EditorGUI.indentLevel--;
                }
            }

            var e = Event.current;
            if (e.type == EventType.MouseDown)
            {
                if (labelRect.Contains(e.mousePosition))
                {
                    foldout = !foldout;
                }
            }
            return (foldout, toggle);
        }
        // static TheSeedRenderPipelineAsset _OverDrawRender;
        // public static TheSeedRenderPipelineAsset OverDrawRender
        // {
        //     get
        //     {
        //         if (_OverDrawRender == null)
        //         {
        //             _OverDrawRender = ScriptableObject.CreateInstance<TheSeedRenderPipelineAsset>();
        //             _OverDrawRender.Renderers.Add(ScriptableObject.CreateInstance<ScriptableRenderer>());
        //             _OverDrawRender.DefaultRendererIndex = 0;
        //             var opaque = ScriptableObject.CreateInstance<DrawObjectsProcess>();
        //             opaque.Layer = -1;
        //             opaque.ResetCameraRenderMatrice = true;
        //             opaque.ResetCameraRenderTarget = true;
        //             opaque.TargetClearFlag = RenderTargetClearFlags.ColorAndDepth;
        //             opaque.ColorLoadAction = RenderBufferLoadAction.DontCare;
        //             opaque.ColorStoreAction = RenderBufferStoreAction.Store;
        //             opaque.DepthLoadAction = RenderBufferLoadAction.DontCare;
        //             opaque.DepthStoreAction = RenderBufferStoreAction.Store;
        //             opaque.Criteria = SortingCriteria.CommonTransparent;
        //             opaque.Range = RenderQueueType.Opaque;
        //             opaque.Name = "OverDrawOpaqueProcess";
        //             opaque.name = opaque.Name;
        //             opaque.OverrideMaterial = new Material(Shader.Find("Hidden/TheSeedRenderPipeline/Editor/Overdraw"));
        //             opaque.OverrideMaterialPassIndex = 0;
        //             _OverDrawRender.DefaultRenderer.Processes.Add(opaque);

        //             var transparent = ScriptableObject.CreateInstance<DrawObjectsProcess>();
        //             transparent.Layer = -1;
        //             transparent.ResetCameraRenderMatrice = false;
        //             transparent.ResetCameraRenderTarget = false;
        //             transparent.Criteria = SortingCriteria.CommonTransparent;
        //             transparent.Range = RenderQueueType.Transparent;
        //             transparent.Name = "OverDrawTransparentProcess";
        //             transparent.name = opaque.Name;
        //             transparent.OverrideMaterial = opaque.OverrideMaterial;
        //             transparent.OverrideMaterialPassIndex = opaque.OverrideMaterialPassIndex;
        //             _OverDrawRender.DefaultRenderer.Processes.Add(transparent);
        //         }
        //         return _OverDrawRender;
        //     }
        // }
    }
}