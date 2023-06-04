using UnityEditor;
using UnityEngine;

namespace CustomizablePipeline
{
    internal class PropertyContent
    {
        public GUIContent content;
        public SerializedProperty property;
        public PropertyContent(GUIContent content, SerializedProperty property = null)
        {
            this.content = content;
            this.property = property;
        }
        public void PropertyField()
        {
            EditorGUILayout.PropertyField(property, content);
        }
        public void Slider(float left, float right)
        {
            property.floatValue = EditorGUILayout.Slider(content, property.floatValue, left, right);
        }
        public void IntSlider(int left, int right)
        {
            property.intValue = EditorGUILayout.IntSlider(content, property.intValue, left, right);
        }
    }
}