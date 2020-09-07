using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.Editor
{
    [CustomPropertyDrawer(typeof(AGraphProperties))]
    public class GraphPropertiesPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement visualElement = new VisualElement();
            SerializedProperty currentProperty = property.Copy();
            if (currentProperty.NextVisible(true))
            {
                do
                {
                    PropertyField prop = new PropertyField(currentProperty);
                    prop.Bind(property.serializedObject);
                    visualElement.Add(prop);
                }
                while (currentProperty.NextVisible(false));
            }

            return visualElement;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty currentProperty = property.Copy();
            if (currentProperty.NextVisible(true))
            {
                do
                {
                    EditorGUILayout.PropertyField(currentProperty, true);
                }
                while (currentProperty.NextVisible(false));
            }
        }
    }
}