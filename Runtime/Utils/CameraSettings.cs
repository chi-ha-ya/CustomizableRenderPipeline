using UnityEngine;

namespace CustomizablePipeline
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("CRP/Components/CameraSettings")]
    public class CameraSettings : MonoBehaviour
    {
        [HideInInspector] public int RendererIndex = -1;// -1 use pipeline default
    }
}