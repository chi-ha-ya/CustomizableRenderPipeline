using UnityEditor;
using UnityEngine;

namespace CustomizablePipeline
{
    public class TrTextContent
    {
        public static GUIContent Select = EditorGUIUtility.TrTextContent("Select");
        public static GUIContent MoveUp = EditorGUIUtility.TrTextContent("Move Up");
        public static GUIContent Reset = EditorGUIUtility.TrTextContent("Reset");
        public static GUIContent Remove = EditorGUIUtility.TrTextContent("Remove");
        public static GUIContent MoveDown = EditorGUIUtility.TrTextContent("Move Down");
        public static GUIContent None = EditorGUIUtility.TrTextContent("");
        public static GUIContent SetDefault = EditorGUIUtility.TrTextContent("SetDefault");
        public static GUIContent Default = EditorGUIUtility.TrTextContent("Default", "管线默认使用的Renderer,注意scene相机将始终使用此renderer渲染");
        public static GUIContent Menu = EditorGUIUtility.TrIconContent("_Menu");
        public static GUIContent Target = EditorGUIUtility.TrTextContent("Target");
        public static GUIContent General = EditorGUIUtility.TrTextContent("General");
        public static GUIContent Lighting = EditorGUIUtility.TrTextContent("Lighting");
        public static GUIContent Shadows = EditorGUIUtility.TrTextContent("Shadows");
        public static GUIContent Enable = EditorGUIUtility.TrTextContent("Enable");
        public static GUIContent Disable = EditorGUIUtility.TrTextContent("Disable");
        public static GUIContent Renderer = EditorGUIUtility.TrTextContent("Renderer");
        public static GUIContent Active = EditorGUIUtility.TrTextContent("Active");
        public static GUIContent Lift = EditorGUIUtility.TrTextContent("Lift");
        public static GUIContent Gamma = EditorGUIUtility.TrTextContent("Gamma");
        public static GUIContent Gain = EditorGUIUtility.TrTextContent("Gain");
        public static GUIContent Offset = EditorGUIUtility.TrTextContent("Offset");
        public static GUIContent DepthOfField = EditorGUIUtility.TrTextContent("DepthOfField");
        public static GUIContent UserLut = EditorGUIUtility.TrTextContent("UserLut");
        public static GUIContent WhiteBalance = EditorGUIUtility.TrTextContent("WhiteBalance");
        public static GUIContent ColorAdjust = EditorGUIUtility.TrTextContent("ColorAdjust");
        public static GUIContent GrayScale = EditorGUIUtility.TrTextContent("GrayScale");
        public static GUIContent LiftGammaGain = EditorGUIUtility.TrTextContent("LiftGammaGain");
        public static GUIContent ShadowsMidtonesHighlights = EditorGUIUtility.TrTextContent("ShadowsMidtonesHighlights");
        public static GUIContent ColorCurves = EditorGUIUtility.TrTextContent("ColorCurves");
        public static GUIContent ChannelMixer = EditorGUIUtility.TrTextContent("ChannelMixer");
        public static GUIContent ChannelMixerRed = EditorGUIUtility.TrTextContent("Red", "Red output channel.");
        public static GUIContent ChannelMixerGreen = EditorGUIUtility.TrTextContent("Green", "Green output channel.");
        public static GUIContent ChannelMixerBlue = EditorGUIUtility.TrTextContent("Blue", "Blue output channel.");
        public static GUIContent ColorAdjustMents = EditorGUIUtility.TrTextContent("ColorAdjustMents");
        public static GUIContent ShadowMidtoneHighlight = EditorGUIUtility.TrTextContent("ShadowMidtoneHighlight");
        public static GUIContent Midtones = EditorGUIUtility.TrTextContent("Midtones");
        public static GUIContent Highlights = EditorGUIUtility.TrTextContent("Highlights");
        public static GUIContent Lut_size = EditorGUIUtility.TrTextContent("Lookup Table Size", "For Insurance, You can only change this in code");
        public static GUIContent ProcessName = EditorGUIUtility.TrTextContent("Process Name", "Name That You Want To Show In Frame Debugger");
        public static GUIContent Glassmorphism = EditorGUIUtility.TrTextContent("Glassmorphism");
        public static GUIContent RadialBlur = EditorGUIUtility.TrTextContent("RadialBlur");
        public static GUIContent ScreenDisturb = EditorGUIUtility.TrTextContent("ScreenDisturb");
        public static GUIContent InvertColor = EditorGUIUtility.TrTextContent("InvertColor");
        public static GUIContent ChromaticAberration = EditorGUIUtility.TrTextContent("ChromaticAberration");
        public static GUIContent FilmGrain = EditorGUIUtility.TrTextContent("FilmGrain");
        public static GUIContent Vignette = EditorGUIUtility.TrTextContent("Vignette");
        public static GUIContent Bloom = EditorGUIUtility.TrTextContent("Bloom");
    }
}