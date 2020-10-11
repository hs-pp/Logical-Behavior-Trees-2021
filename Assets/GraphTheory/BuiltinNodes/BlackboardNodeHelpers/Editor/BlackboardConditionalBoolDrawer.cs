using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(BlackboardConditionalBool))]
public class BlackboardConditionalBoolDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 0;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty boolProp = property.FindPropertyRelative("m_comparedValue");

        EditorGUI.BeginProperty(position, label, property);
        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(property.FindPropertyRelative("m_boolComparator"), GUIContent.none);
        GUILayout.Label(" to ");
        boolProp.boolValue = Convert.ToBoolean(EditorGUILayout.Popup(Convert.ToInt32(boolProp.boolValue), new string[] { "False", "True" }));
        GUILayout.EndHorizontal();
        EditorGUI.EndProperty();
    }
}
