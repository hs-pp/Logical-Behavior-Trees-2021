using System;
using UnityEditor;
using UnityEngine;

namespace Logical.BuiltInNodes
{
    [CustomPropertyDrawer(typeof(BlackboardConditionalBool))]
    public class BlackboardConditionalBoolDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(property.FindPropertyRelative(BlackboardConditionalBool.ComparatorVarName), GUIContent.none);
            GUILayout.Label(" to ");
            SerializedProperty boolProp = property.FindPropertyRelative(BlackboardConditionalBool.ComparedValVarName);
            boolProp.boolValue = Convert.ToBoolean(EditorGUILayout.Popup(Convert.ToInt32(boolProp.boolValue), new string[] { "False", "True" }));
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndProperty();
        }
    }
}