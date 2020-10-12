using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BlackboardSetterBool))]
public class BlackboardSetterBoolDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 0;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(property.FindPropertyRelative(BlackboardSetterBool.SetterCommandVarName), GUIContent.none);
        if ((BlackboardSetterBool.BoolSetterCommand)property.FindPropertyRelative(BlackboardSetterBool.SetterCommandVarName).intValue != BlackboardSetterBool.BoolSetterCommand.Toggle)
        {
            SerializedProperty boolProp = property.FindPropertyRelative(BlackboardSetterBool.NewValueVarName);
            boolProp.boolValue = Convert.ToBoolean(EditorGUILayout.Popup(Convert.ToInt32(boolProp.boolValue), new string[] { "False", "True" }));
        }
        EditorGUILayout.EndHorizontal();
        EditorGUI.EndProperty();
    }
}
