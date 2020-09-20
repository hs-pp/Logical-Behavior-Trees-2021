﻿using UnityEditor;
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
            int depth = currentProperty.depth + 1;

            if (currentProperty.Next(true))
            {
                do
                {
                    PropertyField prop = new PropertyField(currentProperty);
                    prop.Bind(property.serializedObject);
                    visualElement.Add(prop);
                }
                while (currentProperty.NextVisible(true) && currentProperty.depth == depth);
            }

            return visualElement;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty currentProperty = property.Copy();
            int depth = currentProperty.depth + 1;
            if (currentProperty.Next(true))
            {
                do
                {
                    Debug.Log(currentProperty.propertyPath);
                    EditorGUILayout.PropertyField(currentProperty, true);
                }
                while (currentProperty.NextVisible(true) && currentProperty.depth == depth);
            }

            EditorGUI.EndProperty();
        }
    }
}