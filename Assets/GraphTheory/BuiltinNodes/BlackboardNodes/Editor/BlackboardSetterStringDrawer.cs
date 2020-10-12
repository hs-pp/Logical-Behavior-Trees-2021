using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BlackboardSetterString))]
public class BlackboardSetterStringDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 0;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(property.FindPropertyRelative(BlackboardSetterString.SetterCommandVarName), GUIContent.none);
        EditorGUILayout.PropertyField(property.FindPropertyRelative(BlackboardSetterString.NewValueVarName), GUIContent.none);
        EditorGUILayout.EndHorizontal();
        EditorGUI.EndProperty();
    }
}
